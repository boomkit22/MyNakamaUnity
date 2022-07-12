using Nakama;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotiController : MonoBehaviour
{

    [SerializeField]
    TMP_Text _notiText;

    // Start is called before the first frame update
    void Start()
    {
        //Manager.Nakama.NotiAction -= OnNotiReceived;
        Manager.Nakama.NotiAction += OnNotiReceived;

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnNotiReceived(IApiNotification notification)
    {
        var inGameNoti = JsonConvert.DeserializeObject<InGameNoti>(notification.Subject);
        _notiText.text = inGameNoti.Message;
    }


}
