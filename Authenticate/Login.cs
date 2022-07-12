using Nakama;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    GameObject _matchList;
    GameObject _Panel_Match;

    void Start()
    {
        _IF_Id = gameObject.transform.Find("IF_Id").GetComponent<TMP_InputField>();
        _IF_Password = gameObject.transform.Find("IF_Password").GetComponent<TMP_InputField>();
        _Panel_Match = gameObject.transform.Find("Panel_Match").gameObject;
        _matchList = _Panel_Match.transform.Find("MatchList").gameObject;
        var matchListController = _matchList.GetComponent<MatchListController>();
        matchListController.Init();
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

            _Panel_Match.SetActive(true);



            //if (_Match_Id.text == "")
            //{
            //    Manager.Nakama.MatchId = await Manager.Nakama.RPC.MatchCreate();
            //    Debug.Log(Manager.Nakama.MatchId);
            //}
            //else
            //{
            //    Debug.Log(_Match_Id.text);
            //    Manager.Nakama.MatchId = _Match_Id.text;
            //}

            //Manager.Nakama.GameStart();
        }
        else
        {
            Debug.Log("Login Failed");
        }
    }

    public void OnRegisterButtonClick()
    {
        GameObject original = Resources.Load<GameObject>($"Prefabs/UI/Panel_Register");
        if (original == null)
        {
            Debug.Log("Panel_Register Load Failed");
            return;
        }
        GameObject go = UnityEngine.Object.Instantiate(original, transform);

        if (go == null)
            Debug.Log("Panel_Register Instantiate Failed");
    }

    public async void OnCreateMatchButtonClick()
    {
        Manager.Nakama.MatchId = await Manager.Nakama.RPC.MatchCreate();
        Manager.Nakama.GameStart();
        Debug.Log("Create TAsk finish");
        //var matchListController = _matchList.GetComponent<MatchListController>();
        //Debug.Log("Refresh Start");
        //matchListController.Refresh();

        //Button[] buttonList = _matchList.GetComponentsInChildren<Button>();
        //foreach (var button in buttonList)
        //{
        //    Destroy(button.gameObject);
        //}
        //var matchListController = _matchList.GetComponent<MatchListController>();
        //awaiter.OnCompleted(
        //    () =>
        //    {
        //        matchListController.Refres h();
        //    });

        //await Manager.Nakama.RPC.MatchCreate();
    }

}
