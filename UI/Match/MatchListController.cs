using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchListController : MonoBehaviour
{

    Button _btn;
    Dictionary<string, Button> _buttonDictionary = new Dictionary<string, Button>();
    // Start is called before the first frame update
    public  void Init()
    {
        _btn = Resources.Load<Button>("Prefabs/UI/BT_MatchStart");
        InitList();
        transform.parent.gameObject.SetActive(false);
    }

    public async void InitList()
    {

        MatchInfoListDto matchInfoList = await Manager.Nakama.RPC.GetMatchList();

        if (matchInfoList.MatchInfoList != null)
        {
            int matchIndex = 0;
            foreach (var matchInfo in matchInfoList.MatchInfoList)
            {
                matchIndex++;
                //Debug.Log($"Match Id : {matchInfo.MatchId}");
                //Debug.Log($"Match Size : {matchInfo.MatchSize}");
                
                Text text = _btn.transform.Find("Text").GetComponent<Text>();
                Text matchIdText = _btn.transform.Find("MatchIdText").GetComponent<Text>();
                text.text = $"Match{matchIndex} ({matchInfo.MatchSize})";
                matchIdText.text = matchInfo.MatchId;
                Button bt = UnityEngine.Object.Instantiate(_btn, transform);
                bt.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        string matchId = Manager.Nakama.MatchId = bt.transform.Find("MatchIdText").GetComponent<Text>().text;
                        Manager.Nakama.GameStart();
                    }
                );
                _buttonDictionary.Add(matchInfo.MatchId, bt);
            }
        }
    }
    
    //Refresh기능 새로 만들어진 매치들과, 현재 인원수를 파악할 수 있어야함
    //map으로 matchId와 Button을 들고 있다가


    async public void Refresh()
    {
        MatchInfoListDto matchInfoList = await Manager.Nakama.RPC.GetMatchList();
        Debug.Log(matchInfoList.MatchInfoList);
        if (matchInfoList.MatchInfoList != null)
        {
            int matchIndex = 0;
            foreach (var matchInfo in matchInfoList.MatchInfoList)
            {
                matchIndex++;
                //Debug.Log($"Match Id : {matchInfo.MatchId}");
                //Debug.Log($"Match Size : {matchInfo.MatchSize}");
                var isContain = _buttonDictionary.ContainsKey(matchInfo.MatchId);
                if (isContain)
                {
                    Button btn;
                    _buttonDictionary.TryGetValue(matchInfo.MatchId, out btn);
                    Text text1 = btn.transform.Find("Text").GetComponent<Text>();
                    Text matchIdText1 = btn.transform.Find("MatchIdText").GetComponent<Text>();
                    text1.text = $"Match{matchIndex} ({matchInfo.MatchSize})";
                    matchIdText1.text = matchInfo.MatchId;
                    continue;
                }

                Text text = _btn.transform.Find("Text").GetComponent<Text>();
                Text matchIdText = _btn.transform.Find("MatchIdText").GetComponent<Text>();
                text.text = $"Match{matchIndex} ({matchInfo.MatchSize})";
                matchIdText.text = matchInfo.MatchId;
                Button bt = UnityEngine.Object.Instantiate(_btn, transform);
                bt.GetComponent<Button>().onClick.AddListener(() =>
                {
                    string matchId = Manager.Nakama.MatchId = bt.transform.Find("MatchIdText").GetComponent<Text>().text;
                    Manager.Nakama.GameStart();
                }
                );
                _buttonDictionary.Add(matchInfo.MatchId, bt);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnMatchButtonClick()
    {
        
    }



}
