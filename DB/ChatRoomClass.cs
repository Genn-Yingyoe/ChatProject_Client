using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;


/*
case 32:        //make chat_room        >> notification으로 채팅방 초대메세지 보내기
case 33:        //invite chat_room      >> notification으로 채팅방 초대메세지 보내기
case 34:        //exit chat_room
case 35:        //write chat
case 36:        //read chat
 */

namespace ChatMoa_DataBaseServer
{
    internal class ChatRoomClass
    {
        internal static async Task<bool> tempname(byte opcode, List<string> items)  
        {
            List<string> path = new List<string>();

            if (opcode == 32)           //make chat_room    |   items = {User_id, Friend_id_1, Friend_id_2, ... , Friend_id_n}
            {
                
            }
            else if (opcode == 33)      //invite chat_room  |   itmes = {User_id, Room_id, Friend_id_n}
            {
                //
            }
            else if (opcode == 34)      //exit chat_room    |   itmes = {User_id, Room_id}
            {
                //Chat_Room_List 인원 수정 + Chat_Room__Room_Id__Info row삭제 + Chat_Room__Room_Id___Date_ 퇴장 Msg 추가(관리자로)
                string today = DateTime.Now.ToString("yyyyMMdd");
            }
            else if (opcode == 35)      //enter chat_room   |   items = {User_id, Room_id}
            {

            }
            else if (opcode == 36)      //write chat        |   itmes = {User_id, Room_id, Chat_msg}
            {

            }
            else if (opcode == 37)      //read chat         |   itmes = {User_id, Room_id}
            {

            }
            else
            {
                // 향후 업데이트로 opcode가 추가된다면 추가로 코딩
            }

            var list = new List<DB_IO.ItemInfo>();

            await DB_IO.SafeBatchAppendAsync(new[] {obj, path });
            return false;
        }

        public async Task<string> Create_Room_Id()      //complete
        {
            string result = "";
            bool exist = true;
            Random rand = new Random();

            while (exist)
            {
                result = rand.Next(0, 1000000).ToString().PadLeft(8, '0');
                exist = await ExistRoomAsync(result);
            }

            return result;
        }

        private static async Task<bool> ExistRoomAsync(string room_id)
        {
            var ser = new DataContractJsonSerializer(typeof(Chat_Room_List));
            bool exist = false;

            using (var reader = new StreamReader(@"\DB\Chat_Room_List.ndjson", Encoding.UTF8))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null && !exist)
                {
                    // NDJSON → 객체 역직렬화
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                    {
                        var row = (Chat_Room_List)ser.ReadObject(ms);
                        if (row.Room_Id == room_id)
                            exist = true;
                    }
                }
            }
            
            return exist;
        }
    }
}
