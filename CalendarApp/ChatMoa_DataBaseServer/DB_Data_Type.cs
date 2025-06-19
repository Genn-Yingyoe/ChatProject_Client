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
    [DataContract]  //Add
    internal class User_Table
    {
        //length size = 6 | primary key
        [DataMember] internal string User_Id;
        [DataMember] internal string Id;
        [DataMember] internal string Password;
        [DataMember] internal int Ps_Question_Index;
        [DataMember] internal string Ps_Answer;
    }

    [DataContract]  //Add, Edit
    internal class User_Info
    {
        // primary key and foregin key from "User_Table"
        [DataMember] internal string User_Id;
        [DataMember] internal string Name;
        [DataMember] internal string Nickname;
        [DataMember] internal string Profile_Image_Path;
        [DataMember] internal List<string> Chat_Room_List;
        [DataMember] internal List<string> Waiting_Chat_Room_List;
    }

    [DataContract]  //Add, Del
    internal class _User_Id__Inform_Box
    {
        //primary key
        [DataMember] internal int Inform_Id;
        [DataMember] internal string Inform_Kind;
        [DataMember] internal string Inform_Date;
        [DataMember] internal string Inform_Str;
        [DataMember] internal List<string> need_items;
        [DataMember] internal bool Inform_Checked;
    }

    [DataContract]  //Add, Del
    internal class _User_Id__Friend_List
    {
        //primary key and foregin key from "User_Table"
        [DataMember] internal string Friend_Id;
        [DataMember] internal string Nickname;
    }

    [DataContract]  //Edit
    internal class _User_Id__Setting_Info
    {
        //length size = 4 | primary key
        [DataMember] internal string Info_Id;
        [DataMember] internal string Info_Str;
    }

    [DataContract]  //Add, Edit, Del
    internal class Chat_Room_List
    {
        //length size = 8 | primary key
        [DataMember] internal string Room_Id;
        [DataMember] internal int Users_Num;        //invite_state가 true인 멤버만 해당
    }

    [DataContract]  //Add, Edit, Del
    internal class Chat_Room__Room_Id__Info
    {
        //length size = 6 | primary key and foregin key from "User_Table"
        [DataMember] internal string User_Id;
        [DataMember] internal int Read_Msg_Num;
        [DataMember] internal string Read_Last_Date;
        [DataMember] internal string Sche_List; //temp type
        [DataMember] internal bool invite_state;
    }

    [DataContract]  //Add
    internal class Chat_Room__Room_Id___Date_
    {
        //primary key
        [DataMember] internal int Msg_Id;
        //foregin key from "Chat_Room__Room_Id__Info"
        [DataMember] internal string User_Id;
        [DataMember] internal int Msg_Kind;         // 0 == manager chat | 1 == user chat
        [DataMember] internal string Date;
        [DataMember] internal string Msg_Str;
    }

    [DataContract]  //Add, Edit, Del
    internal class _User_Id__Scheduler
    {
        //primary key
        [DataMember] internal int Sche_Id;
        [DataMember] internal string Category;
        [DataMember] internal string Begin_Date;
        [DataMember] internal string Finish_Date;
        [DataMember] internal string Sche_Str;
        [DataMember] internal string Daily;
        [DataMember] internal string Weekly;
        [DataMember] internal string Monthly;
        [DataMember] internal string Yearly;
        [DataMember] internal string Alert_Date;
    }

    [DataContract]  //Add, Edit, Del
    internal class Chat_Room__Room_Id__Scheduler
    {
        //primary key
        [DataMember] internal int Sche_Id;
        //foregin key from "Chat_Room__Room_Id__Info"
        [DataMember] internal string User_Id;
        [DataMember] internal string Category;
        [DataMember] internal string Begin_Date;
        [DataMember] internal string Finish_Date;
        [DataMember] internal string Sche_Str;
        [DataMember] internal string Daily;
        [DataMember] internal string Weekly;
        [DataMember] internal string Monthly;
        [DataMember] internal string Yearly;
        [DataMember] internal string Alert_Date;
    }


    // Request Data parsing method
    // byte[0] == opcode                                | kind of request
    // byte[1 .. 6] == User_id(6)                       | User_Id that sent the request
    // byte[7 .. 10] == receive num of datas(num)       | receive num of data
    // byte[11 .. (10+num*4)] == receive lengths        | receive lengths of data
    // byte[(10+num*4)+1 .. ] == datas                 
}