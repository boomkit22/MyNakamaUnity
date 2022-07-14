using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerSceneController : MonoBehaviour
{
    
    public void OnServerButtonClick()
    {
        Manager.Nakama.Host = "34.64.99.190";
        Manager.Nakama.Port = 6350;

        SceneManager.LoadScene("Login");
        Manager.Nakama.InitClient();
    }



    public void OnLocalButtonClick()
    {
        Manager.Nakama.Host = "127.0.0.1";
        Manager.Nakama.Port = 7350;
        SceneManager.LoadScene("Login");
        Manager.Nakama.InitClient();
    }
}
