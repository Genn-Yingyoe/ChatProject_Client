using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChatMoa_DataBaseServer
{
    /*
     * User_Tabel에는 언제나 000000의 User_Id를 갖는 관리자가 포함됨
     * 관리자는 각 채팅방에 포함되며, 채팅방의 헤더 역할을 함
     */
    [DataContract]
    public class User_Table
    {
        [DataMember] public string User_Id { get; set; }
        [DataMember] public string Id { get; set; }
        [DataMember] public string Password { get; set; }
        [DataMember] public int Ps_Question_Index { get; set; }
        [DataMember] public string Ps_Answer { get; set; }
    }

    [DataContract]
    public class User_Info
    {
        [DataMember] public string User_Id { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public string Nickname { get; set; }
        [DataMember] public string Profile_Image_Path { get; set; }
        [DataMember] public List<string> Chat_Room_List { get; set; }
        [DataMember] public List<string> Waiting_Chat_Room_List { get; set; }
    }

    [DataContract]
    public class _User_Id__Inform_Box
    {
        [DataMember] public int Inform_Id { get; set; }
        [DataMember] public string Inform_Kind { get; set; }
        [DataMember] public string Inform_Date { get; set; }
        [DataMember] public string Inform_Str { get; set; }
        [DataMember] public List<string> need_items { get; set; }
        [DataMember] public bool Inform_Checked { get; set; }
    }

    [DataContract]
    public class _User_Id__Friend_List
    {
        [DataMember] public string Friend_Id { get; set; }
        [DataMember] public string Nickname { get; set; }
    }

    [DataContract]
    public class _User_Id__Setting_Info
    {
        [DataMember] public string Info_Id { get; set; }
        [DataMember] public string Info_Str { get; set; }
    }

    [DataContract]
    public class Chat_Room_List
    {
        [DataMember] public string Room_Id { get; set; }
        [DataMember] public int Users_Num { get; set; }
    }

    [DataContract]
    public class Chat_Room__Room_Id__Info
    {
        [DataMember] public string User_Id { get; set; }
        [DataMember] public int Read_Msg_Num { get; set; }
        [DataMember] public string Read_Last_Date { get; set; }
        [DataMember] public string Sche_List { get; set; }
        [DataMember] public bool invite_state { get; set; }
    }

    [DataContract]
    public class Chat_Room__Room_Id___Date_
    {
        [DataMember] public int Msg_Id { get; set; }
        [DataMember] public string User_Id { get; set; }
        [DataMember] public int Msg_Kind { get; set; }
        [DataMember] public string Date { get; set; }
        [DataMember] public string Msg_Str { get; set; }
    }

    [DataContract]
    public class _User_Id__Scheduler
    {
        [DataMember] public int Sche_Id { get; set; }
        [DataMember] public string Category { get; set; }
        [DataMember] public string Begin_Date { get; set; }
        [DataMember] public string Finish_Date { get; set; }
        [DataMember] public string Sche_Str { get; set; }
        [DataMember] public string Daily { get; set; }
        [DataMember] public string Weekly { get; set; }
        [DataMember] public string Monthly { get; set; }
        [DataMember] public string Yearly { get; set; }

    }

    [DataContract]
    public class Chat_Room__Room_Id__Scheduler
    {
        [DataMember] public int Sche_Id { get; set; }
        [DataMember] public string User_Id { get; set; }
        [DataMember] public string Category { get; set; }
        [DataMember] public string Begin_Date { get; set; }
        [DataMember] public string Finish_Date { get; set; }
        [DataMember] public string Sche_Str { get; set; }
        [DataMember] public string Daily { get; set; }
        [DataMember] public string Weekly { get; set; }
        [DataMember] public string Monthly { get; set; }
        [DataMember] public string Yearly { get; set; }

    }

    // Request Data parsing method
    // byte[0] == opcode                                | kind of request
    // byte[1 .. 6] == User_id(6)                       | User_Id that sent the request
    // byte[7 .. 10] == receive num of datas(num)       | receive num of data
    // byte[11 .. (10+num*4)] == receive lengths        | receive lengths of data
    // byte[(10+num*4)+1 .. ] == datas                 
}