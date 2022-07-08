using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class SendUserChatDto
{
    public string Message { get; set; }
    public string UserName { get; set; }
}

public class RecentUserChatDto
{
    public string Message { get; set; }
    public string UserName { get; set; }
    public string CreatedTime { get; set; }
}

public class RPC
{

    public async Task<string> InitializeChat()
    {
        Debug.Log("Initialize Chat");
        var res = await Manager.Nakama.Client.RpcAsync(Manager.Nakama.Session, "chat_deleted");

        return res.Payload;
    }

    public async Task<string> UserChat(SendUserChatDto userChatData)
    {
        string json = JsonConvert.SerializeObject(userChatData);

        var res = await Manager.Nakama.Client.RpcAsync(Manager.Nakama.Session, "chat_entered", json);

        return res.Payload;
            
        //return res.Payload;
    }

    public async Task<string> LoadRecentChat()
    {
        var res = await Manager.Nakama.Client.RpcAsync(Manager.Nakama.Session, "load_recent_chat");

        RecentUserChatDto[] recentUserChatList = JsonConvert.DeserializeObject<RecentUserChatDto[]>(res.Payload);

        //Debug.Log(recentUserChatList[0].CreatedTime);

        return res.Payload;
    }

    public async Task<string> MatchCreate()
    {
        var res = await Manager.Nakama.Client.RpcAsync(Manager.Nakama.Session, "Match_Create");
        Debug.Log("Create Match Succeed");
        return res.Payload;
    }

}
