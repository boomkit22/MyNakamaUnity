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

    public
    TMP_InputField _Match_Id;

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
        Manager.Nakama.PlayerEmail = email;


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

            await Manager.Nakama.RPC.InitializeChat();
            //var recentChat = await Manager.Nakama.RPC.LoadRecentChat();

            Debug.LogWarning("Login Succeed");

            SceneManager.LoadScene("Game");

            if (_Match_Id.text == "")
            {
                Manager.Nakama.MatchId = await Manager.Nakama.RPC.MatchCreate();
                Debug.Log(Manager.Nakama.MatchId);
            }
            else
            {
                Debug.Log(_Match_Id.text);
                Manager.Nakama.MatchId = _Match_Id.text;
            }

            await Manager.Nakama.JoinMatch(Manager.Nakama.MatchId);

            if (Manager.Nakama.PlayerPrefab == null)
                Debug.LogWarning("Load unitychan failed");
            Manager.Nakama.Player = Instantiate(Manager.Nakama.PlayerPrefab);


        
            //Debug.Log(matchId);



        


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
