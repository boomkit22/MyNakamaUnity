using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    TMP_InputField _IF_Id;
    TMP_InputField _IF_Password;
    TMP_InputField _IF_PasswordCheck;
    Button _BT_Login;

    void Start()
    {
        _IF_Id = gameObject.transform.Find("IF_Id").GetComponent<TMP_InputField>();
        _IF_Password = gameObject.transform.Find("IF_Password").GetComponent<TMP_InputField>();
        _IF_PasswordCheck = gameObject.transform.Find("IF_PasswordCheck").GetComponent<TMP_InputField>();
    }

    void Update()
    {
        
    }

    async public void OnRegisterButtonClick()
    {
        

        string email = _IF_Id.text;
        string password = _IF_Password.text;
        string passwordCheck = _IF_PasswordCheck.text;

        try
        {
            Manager.Nakama.Session = await Manager.Nakama.Client.AuthenticateEmailAsync(email, password, create: true);
        }
        catch (Exception ex)
        {
            Debug.Log("Exception Message: " + ex.Message);
        }

        if (Manager.Nakama.Session != null)
        {
            Debug.Log("Register Succeed");
            UnityEngine.Object.Destroy(gameObject);
        }
        else
        {
            Debug.Log("Register Failed");
        }

    }

    public void OnCancelButtonClick()
    {
        UnityEngine.Object.Destroy(gameObject);
    }

}
