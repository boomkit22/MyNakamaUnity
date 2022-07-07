using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    static Manager s_Instance;
    static Manager Instance { get { Init(); return s_Instance; } }

    NakamaManager _nakama = new NakamaManager();
    public static NakamaManager Nakama { get { return Instance._nakama; } }

    void Start()
    {
        Init();
    }

    void Update()
    {
        
    }

    static void Init()
    {
        if (s_Instance == null)
        {
            GameObject go = GameObject.Find("@Manager");
            if ( go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Manager>();
            }
            DontDestroyOnLoad(go);
            s_Instance = go.GetComponent<Manager>();
            s_Instance._nakama.Init();
        }

    }
}
