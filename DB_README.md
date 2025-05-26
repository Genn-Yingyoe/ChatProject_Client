# 0. 기본적인 Data 이동 구조

- 1개 이상의 **Client(채팅 프로그램 | User)** 와 1개의 **Server(서버 프로그램 | Server)** 통신 구조
- 모든 Data 이동은 Client에서 시작 → Server에서 전송
- 통신 구조는 예제, DCM에서 2가지로 분류

## 1) 서버 DB에서 Data 전달 받을 필요 없는 경우  
(ex: 회원가입 – 성공/실패 여부만 알면 됨)
- Server로부터 Data가 한 번 전달됨  
- 반환값: `"1"` (성공), `"0"` (실패)

## 2) 서버 DB의 Data를 전달 받아야 하는 경우
- Server에서 0개 이상의 Data 전달  
- 마지막 반환값: #1과 동일 (“1” 또는 “0”)
- Data는 opcode마다 형식이 다름  
- 보통, DB_Data_Type.cs에 정의된 Data 형식 구조 따름  
- Server로부터 데이터는 **직렬화**된 상태로 전달됨  
    - `{}` 내부에 key:value 형태(string 한 줄)  
- 역직렬화 필요 시:  
    - `private T DeSerializeJson<T>(int num, int index);`
- 다시 직렬화할 땐:  
    - `private static string SerializeJson(object obj);`
- 세부 예제는 하단 참고

### Data 무결성
- 서버에서 중간에 문제가 생기면 요청 즉시 취소 및 모든 작업 백업
- 서버에서 `byte[2]{ 0, 0 }` return, TCP 연결 종료

---

# 1. DCM 함수 요약

```csharp
private string user_id
```
- User를 구분하는 기본키. 6자리 숫자(string)
- Login/Logout시 바뀌며, User 직접 변경 불가

```csharp
private Dictionary<int,List<string>> received_data
```
- 서버로부터 받은 Data 저장
- key: 요청별 식별번호, value: 데이터 목록

```csharp
internal async Task<KeyValuePair<bool,(int, List<int>)>> db_request_data(byte opcode, List<string> items)
```
- 서버 요청 함수 (await 사용)
- opcode별 명령 + items(string 리스트)
- 반환값 설명  
    - `bool`: 데이터 수신 여부  
    - `int`: 요청의 식별키  
    - `List<int>`: 받은 데이터의 인덱스

```csharp
private bool Clear_receive_data(int num)
```
- 사용 끝난 Data를 received_data에서 삭제(식별키 num)

```csharp
private T DeSerializeJson<T>(int num, int index)
```
- 직렬화된 데이터를 역직렬화해 형식화된 변수로 반환

```csharp
private static string SerializeJson(object obj)
```
- 객체를 직렬화 후 string으로 반환

---

# 2. opcode 설명

- 1 byte(0~255)
- **[1~31]**: Main System
- **[32~63]**: Chatting/ChatRoom
- **[64~]**: Scheduler

## [Main System]

| opcode | 기능                        | 필요 items | 주의/설명 |
|--------|----------------------------|------------|-----------|
| 1      | register(회원가입)          | id, ps, ps_question_index, ps_question_answer, name(불변), nickname(가변) | 성공 1, 실패 0, 성공 시 본인의 고유 User_Id(6자리) 생성 |
| 2      | login(로그인)               | id, ps     | 성공 시 User_Id, 1 순서대로 두 번 return / 실패 시 0 |
| 3      | change_password(비번 변경)  | id, ps_question_index, ps_question_answer, new_ps | 성공 1 / 실패 0 |
| 4      | change_nickname(닉네임 변경)| new_nickname | 성공 1 / 실패 0 |
| 5      | change_setting(세팅 변경)   | setting_id, val (쌍으로 여러 개 가능) | 성공 1 / 실패 0 |
| 6      | friend_request(친구 요청)   | friend_id(User_Id, 6자리) | 성공 1 / 실패 0, 채팅 대상에게 알림 추가 |
| 7      | friend_delete(친구 삭제)    | friend_id   | 성공 1 / 실패 0, 둘 다 친구목록에서 서로 삭제 |
| 8      | check_notify(알림 확인/수락)| Inform_Id, check_state("1":수락, "0":확인/거절) | 성공 1 / 실패 0  |
| 9      | delete_notify(알림 삭제)    | Inform_Id   | 읽은 알림만 삭제, 성공 1 / 실패 0 |
| 10     | read_friend_list(친구목록)  | 없음(빈 리스트) | 성공 시 Friend_List DB 전체를 직렬화 된 상태로 받음, 실패 0 |
| 11     | read_all_of_notify(알림 전체 읽기) | 없음(빈 리스트) | 성공 시 Inform_Box DB 전체를 직렬화 된 상태로 받음, 실패 0 |
| 12     | read_user_setting(유저 세팅 읽기) | 없음(빈 리스트) | 성공 시 Setting_Info DB 전체를 직렬화 된 상태로 받음, 실패 0 |

---

## [Chat]

| opcode | 기능                       | 필요 items         | 주의/설명 |
|--------|---------------------------|--------------------|-----------|
| 32     | make_chat_room(채팅방 만들기) | Friend_Id 1개 이상      | 성공 시 Room_Id, 1 반환. 실패 0 |
| 33     | invite_chat_room(채팅방 초대) | Room_Id, Friend_Id | 1명씩만 처리, 성공 1, 실패 0 |
| 34     | exist_chat_room(채팅방 나가기) | Room_Id            | 성공 1, 실패 0 |
| 35     | enter_chat_room(채팅방 입장)  | Room_Id            | 성공 1, 실패 0 |
| 36     | write_chat(채팅 쓰기)         | Room_Id, chat_category, chat_str | 관리자 메세지는 category "0" Client 개발자 사용금지, 성공 1, 실패 0 |
| 37     | read_chat(채팅 읽기)          | ~~Room_Id, last_read_msg_id, last_read_msg_date(yyyyMMdd)~~ | *수정 예정* |
| 38     | read_my_chat_room_list(내 채팅방 목록) | 없음(빈 리스트)    | 성공 시 본인이 입장한 Room_Id(string/0개 이상), 1 순서대로 두 번 return, 실패 0 |
| 39     | read_chat_room_users(채팅방 멤버 읽기) | Room_Id | 성공 시 Chat_Room_Info DB 전체를 직렬화 된 상태로 받음, 실패 0 |

---

## [Scheduler]

| opcode | 기능                          | 필요 items | 주의/설명 |
|--------|------------------------------|------------|-----------|
| 64     | user_sche_add(유저 스케쥴 추가)           | Category 등 상세 8개 | 성공 1, 실패 0 |
| 65     | user_sche_edit(유저 스케쥴 수정)         | Sche_Id 포함 전체 9개 | 성공 1, 실패 0 |
| 66     | user_sche_delete(유저 스케쥴 삭제)        | Sche_Id | 성공 1, 실패 0 |
| 67     | read_user_sche(유저 스케쥴 읽기)          | 없음(빈 리스트) | 성공 시 본인의 Scheduler DB 전체를 직렬화 된 상태로 받음, 실패 0 |
| 68     | chatroom_sche_add(채팅방 스케쥴 추가)     | Room_Id + 상세 8개 | 성공 1, 실패 0 |
| 69     | chatroom_sche_edit(채팅방 스케쥴 수정)    | Room_Id, Sche_Id 포함 9개 | 성공 1, 실패 0 |
| 70     | chatroom_sche_delete(채팅방 스케쥴 삭제)  | Room_Id, Sche_Id | 성공 1, 실패 0 |
| 71     | read_chatroom_sche(채팅방 스케쥴 읽기)    | Room_Id | 성공 시 해당 채팅방의 Scheduler DB 전체를 직렬화 된 상태로 받음, 실패 0 |

---

# 3. DB 데이터 타입 예시

## 사용법 요약

```csharp
User_Table test = new User_Table() { User_Id = "123123", Id = "ididid", Password = "pspsps" };
Console.WriteLine(test.User_Id); // 123123

// 직렬화/역직렬화 예시
string dataLine = @"{ ""Friend_Id"":""123123"", ""Nickname"":""Kwangwooni"" }";  //server에서 받아온 Data
_User_Id__Friend_List friend = DCM.DeSerializeJson<_User_Id__Friend_List>(...);
friend.Nickname = "광운이";

Console.WriteLine(my_friend.Nickname);	//광운이
Console.WriteLine(my_friend.Friend_Id);	//123123

```

---

## 형식화된 데이터 타입

### User_Table
```csharp
[DataContract]
internal class User_Table
{
    [DataMember] internal string User_Id;    // length size = 6 | primary key		| "000000"은 관리자 id로 각종 DB처리에 대해서 도움을 줌(Client 유저 또한 이를 활용 가능)
    [DataMember] internal string Id;
    [DataMember] internal string Password;
    [DataMember] internal int Ps_Question_Index;
    [DataMember] internal string Ps_Answer;
}
```
- 모든 User 로그인/패스워드 정보 저장

### User_Info
```csharp
[DataContract]
internal class User_Info
{
    [DataMember] internal string User_Id;     // primary key and foregin key from <User_Id of "User_Table">
    [DataMember] internal string Name;
    [DataMember] internal string Nickname;
    [DataMember] internal string Profile_Image_Path;
    [DataMember] internal List<string> Chat_Room_List;
    [DataMember] internal List<string> Waiting_Chat_Room_List;
}
```
- 모든 User 각종 정보 저장

### _User_Id__Inform_Box
```csharp
[DataContract]
internal class _User_Id__Inform_Box
{
    [DataMember] internal int Inform_Id;           // primary key
    [DataMember] internal string Inform_Kind;      // 현재 지정된 형식은 "friend_request", "invite" (<<채팅방 초대) 뿐이며, Main Client 개발자가 자유롭게 사용하면 된다.
                                                   // 단 형식을 추가할 시, DB 개발자에게 알려줘야지 처리 Logic code를 추가할 수 있다.
    [DataMember] internal string Inform_Date;
    [DataMember] internal string Inform_Str;
    [DataMember] internal List<string> need_items;
    [DataMember] internal bool Inform_Checked;
}
```
- 각 User들의 알림 정보 저장

### _User_Id__Friend_List
```csharp
[DataContract]
internal class _User_Id__Friend_List
{
    [DataMember] internal string Friend_Id;       // primary key and foregin key from <User_Id of "User_Table">
    [DataMember] internal string Nickname;        // User가 친구의 Nickname을 본인만 보이는 다른 Nickname으로 설정해도 무관
}
```
- 각 User들의 친구 정보 저장

### User_Id__Setting_Info
```csharp
[DataContract]
internal class User_Id__Setting_Info
{
    [DataMember] internal string Info_Id;         // primary key
    [DataMember] internal string Info_Str;
}
```
- 각 User들의 Setting(UI에는 현재 Config) 정보 저장

### Chat_Room_List
```csharp
[DataContract]
internal class Chat_Room_List
{
    [DataMember] internal string Room_Id;         // length size = 8 and primary key
    [DataMember] internal int Users_Num;          // invite_state가 true인 멤버만 해당
}
```
-모든 채팅방에 대한 Room_Id와 입장한 유저수를 순서쌍으로 저장

### Chat_Room__Room_Id__Info
```csharp
[DataContract]
internal class Chat_Room__Room_Id__Info
{
    [DataMember] internal string User_Id;         // length size = 6 | primary key and foregin key from <User_Id "User_Table">
    [DataMember] internal int Read_Msg_Num;
    [DataMember] internal string Read_Last_Date;
    [DataMember] internal string Sche_List;       // 해당 정보에 대한 사용은 Scheduler 개발자가 자유롭게 사용하면 된다.
    [DataMember] internal bool invite_state;
}
```
-각 채팅방에 대한 유저들의 정보 저장
-모든 채팅방에 **"000000"의 User_Id를 가지는 관리자**가 포함되어 있음
-관리자는 인원 수에 포함되지 않으며 데이터 처리에 용이하게 하기 위해 추가됨

### Chat_Room__Room_Id___Date
```csharp
[DataContract]
internal class Chat_Room__Room_Id___Date
{
    [DataMember] internal int Msg_Id;             // primary key
    [DataMember] internal string User_Id;         // foregin key from <User_Id of "Chat_Room__Room_Id__Info">
    [DataMember] internal int Msg_Kind;           // 0 == 관리자 메세지	|  1 == user chat	|  추가적인 형식은 Chatting
    [DataMember] internal string Date;
    [DataMember] internal string Msg_Str;
}
```
-각 채팅에 대한 정보를 날짜별로 저장

### _User_Id__Scheduler
```csharp
[DataContract]
internal class _User_Id__Scheduler
{
    [DataMember] internal int Sche_Id;            // primary key
    [DataMember] internal string Category;
    [DataMember] internal string Begin_Date;
    [DataMember] internal string Finish_Date;
    [DataMember] internal string Sche_Str;
    [DataMember] internal string Daily;
    [DataMember] internal string Weekly;
    [DataMember] internal string Monthly;
    [DataMember] internal string Yearly;
}
```
-각 User들의 Schedule 정보 저장
-Daily, Weekly, Monthly, Yearly 등은 사용하기 편하게 string 형태의 Data로 지정
-수정이 필요할시 DB 개발자에게 문의의


### Chat_Room__Room_Id__Scheduler
```csharp
[DataContract]
internal class Chat_Room__Room_Id__Scheduler
{
    [DataMember] internal int Sche_Id;            //primary key
    [DataMember] internal string User_Id;         //foregin key from <User_Id of "Chat_Room__Room_Id__Info">
    [DataMember] internal string Category;
    [DataMember] internal string Begin_Date;
    [DataMember] internal string Finish_Date;
    [DataMember] internal string Sche_Str;
    [DataMember] internal string Daily;
    [DataMember] internal string Weekly;
    [DataMember] internal string Monthly;
    [DataMember] internal string Yearly;
}
```
-각 채팅방들의 Schedule에 대한 정보 저장
---

# 4. DB server 및 Test_Client 사용법

1. DB Server 파일을 하나의 프로젝트 폴더에 정리 (namespace는 각 프로젝트에 맞게 변경)
    - `DB_IO.cs`, `DB_Data_Type.cs`, `ChatRoomClass.cs`, `SchedulerClass.cs`, `Program.cs`
2. Test_Client을 구동할 **별도 프로젝트**를 만들고서, 파일들을 해당 폴더에 정리 
    - `Test.cs`, `Test.cs [디자인]`
3. Visual Studio 2022에서 **ChatMoa_DataBaseServer**로 실행 (F5)
4. Console창에서 DBSETUP이 정상적으로 출력된다면 DB폴더가 자동 생성
5. Visual Studio 2022에서 우측 솔루션 탐색기를 펼치고서, Test_Client 코드가 포함된 Window Forms 프로젝트를 우클릭하여 "디버그>디버깅 없이 시작"을 실행
6. 두 프로그램(서버 콘솔/클라이언트 창) 동시에 구동, 5번까지 정상적으로 실행이 되었다면 두 프로그램(서버 콘솔/클라이언트 창)이 동시에 실행중인 상태
7. Client에서 데이터 입력, 서버로 전송, 결과 MessageBox 확인
8. DB폴더 내 ndjson 파일(메모장 열림)로 변화 확인

> **참고:**  
> Test_Client에서 문자열 입력 시 `'/'`로 분할되어 List<string> 변환 후 서버로 전송됨에 유의
