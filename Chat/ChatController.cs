using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : MonoBehaviour
{
    public List<string> chatList = new List<string>();
    public TMP_InputField _input;

    public TMP_Text chattingList;

    public Button sendBtn; 
    // Start is called before the first frame update

    

    public async void OnEndEdit()
    {
        
        if (_input.text.Equals(""))
        {
            Debug.Log("Empty"); 
            return;
        }
        
        SendUserChatDto userChatData = new SendUserChatDto();
        string msg = _input.text;
        userChatData.Message = msg;
        userChatData.UserName = Manager.Nakama.PlayerEmail;

        await Manager.Nakama.RPC.UserChat(userChatData);

        LoadRecentChat();

        await Manager.Nakama.Socket.SendMatchStateAsync(Manager.Nakama.MatchId, OpCodes.Chat, "chat");

        _input.text = "";
    }
    void Start()
    {
        Manager.Nakama.ChatAction += LoadRecentChat;
    }

    async public void LoadRecentChat()
    {
        var recentChat = await Manager.Nakama.RPC.LoadRecentChat();
        RecentUserChatDto[] recentUserChatList = JsonConvert.DeserializeObject<RecentUserChatDto[]>(recentChat);

        string text = "";
        foreach (var chat in recentUserChatList)
        {
            text += chat.UserName + " " + chat.Message + "\n";
        }
        chattingList.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
