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
        Debug.Log(notification.Subject);
        //Debug.Log(notification.Content.Length);

        //Empty Content Ȯ�ι�� �ٸ��� ��������
        //if (notification.Content == "{}")
        //    Debug.Log("Empty Content");
        
        //nakama server���� map[string]interface{}�� �����ָ� json�������� ���� �� ����
        //var inGameNotiContent = JsonConvert.DeserializeObject<InGameNotiContent>(notification.Content);
        //Debug.Log($"exp {inGameNotiContent.exp}");
        //Debug.Log($"item {inGameNotiContent.item}");
        //Debug.Log($"reward_coins {inGameNotiContent.reward_coins}");

        var inGameNoti = JsonConvert.DeserializeObject<InGameNoti>(notification.Subject);
        _notiText.text = inGameNoti.Message;
    }


}

public class InGameNoti
{
    public string Message { get; set; }
}

public class InGameNotiContent
{
    public int exp { get; set; }
    public string item { get; set; }
    public int reward_coins { get; set; }
}
