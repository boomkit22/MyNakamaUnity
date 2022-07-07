using Nakama;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
  

    TMP_InputField _IF_Id;
    TMP_InputField _IF_Password;
    Button _BT_Login;

    void Start()
    {
        _IF_Id = gameObject.transform.Find("IF_Id").GetComponent<TMP_InputField>();
        _IF_Password = gameObject.transform.Find("IF_Password").GetComponent<TMP_InputField>();
    }

    void Update()
    {
        
    }

    async public void OnLoginButtonClick()
    {
        string email = _IF_Id.text;
        string password = _IF_Password.text;

        try
        {
            Manager.Nakama.Session = await Manager.Nakama.Client.AuthenticateEmailAsync(email, password, create: false);
            Manager.Nakama.ConnectSocket();
        }
        catch (Exception ex)
        {
            Debug.Log("Exception Message: " + ex.Message);
        }

        if (Manager.Nakama.Session != null)
        {
            Debug.LogWarning("Login Succeed");
            SceneManager.LoadScene("Game");

            //Chat Test
            SendUserChatDto userChatData = new SendUserChatDto();
            userChatData.Message = "i logined";
            userChatData.UserName = email;

            var res = await Manager.Nakama.RPC.UserChat(userChatData);
            //Debug.Log(res);

            var recentChat = await Manager.Nakama.RPC.LoadRecentChat();
            //Debug.Log(recentChat);
            //

            //Create and Join Match Test
            //Manager.Nakama.MatchId = await Manager.Nakama.RPC.MatchCreate();
            Manager.Nakama.MatchId = "c3308923-fdfe-4144-b14a-74af2e3264bb.nakama";
            //Debug.Log(matchId);

            try
            {
                await Manager.Nakama.JoinMatch(Manager.Nakama.MatchId);

                if (Manager.Nakama.PlayerPrefab == null)
                    Debug.LogWarning("Load unitychan failed");

                Manager.Nakama.Player = Instantiate(Manager.Nakama.PlayerPrefab);

            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Join Match Error  : {ex}");
            
            }
        }
        else
        {
            Debug.Log("Login Failed");
        }
    }

    public void OnRegisterButtonClick()
    {
        GameObject original = Resources.Load<GameObject>($"Prefabs/UI/Panel_Register");
        if(original == null)
        {
            Debug.Log("Panel_Register Load Failed");
            return;
        }
        GameObject go = UnityEngine.Object.Instantiate(original, transform);

        if (go == null)
            Debug.Log("Panel_Register Instantiate Failed");
    }

}
