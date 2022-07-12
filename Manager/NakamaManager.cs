using Nakama;
using Nakama.TinyJson;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NakamaManager
{
    protected Nakama.IClient _client;
    protected Nakama.ISession _session;
    protected Nakama.ISocket _socket;
    protected RPC _rpc = new RPC();
    //protected RPC _rpc;
    protected string _matchId;
    protected string _playerEmail;
    protected GameObject _player;
    protected GameObject _playerPrefab;
    protected GameObject _remotePlayerPrefab;
    protected Nakama.IMatch _match;

    protected string _playerSessionId;
    public UnityMainThreadDispatcher _mainThread;
    public string PlayerSessionId { get { return _playerSessionId; } set { _playerSessionId = value; } }
    public Action ChatAction = null;
    public Action<String, Vector3> SpawnAction = null;
    public Action<IApiNotification> NotiAction = null;

    public string PlayerEmail { get { return _playerEmail; } set { _playerEmail = value; } }
    public Nakama.IClient Client { get { return _client; } }
    public Nakama.ISession Session { get { return _session; } set { _session = value; } }
    public Nakama.ISocket Socket { get { return _socket; } set { _socket = value; } }
    public string MatchId { get { return _matchId; } set { _matchId = value; } }
    public RPC RPC { get { return _rpc; } }

    public Dictionary<string, GameObject> _players = new Dictionary<string, GameObject>();
    public GameObject Player { get { return _player; } set { _player = value; } }
    public GameObject PlayerPrefab { get { return _playerPrefab; } set { _playerPrefab = value; } }
    public GameObject RemotePlayerPrefab { get { return _remotePlayerPrefab; } set { _remotePlayerPrefab = value; } }
    public Nakama.IMatch Match { get { return _match; } set { _match = value; } }

    //public Nakama.ISession _serverSession;
    //protected Nakama.ISocket _serverSocket;
    //public Nakama.ISession ServerSession { get { return _serverSession; } set { _serverSession = value; } }
    //public Nakama.ISocket ServerSocket { get { return _serverSocket; } set { _serverSocket = value; } }
    public void Init()
    {
        _client = new Nakama.Client("http", "127.0.0.1", 7350, "defaultkey");
        //_client = new Nakama.Client("http", "34.64.99.190", 6350, "defaultkey");

        _client.Timeout = 10;

        PlayerPrefab = Resources.Load<GameObject>("Prefabs/unityChan");
        RemotePlayerPrefab = Resources.Load<GameObject>("Prefabs/remoteUnityChan");
        SpawnAction += SpawnOther;

        if (_mainThread == null)
        {
            _mainThread = UnityMainThreadDispatcher.Instance();
        }

        //_rpc = new RPC();
        //CreateMatch();
    }

    //public async void CreateMatch()
    //{
    //    ServerSocket = _client.NewSocket();
    //    ServerSession = await Client.AuthenticateEmailAsync("server@gmail.com", "password", create: false);

    //    await ServerSocket.ConnectAsync(Session, true, connectTimeout: 10);
    //    Manager.Nakama.MatchId = await Manager.Nakama.RPC.MatchCreate();
    //}

    public async void ConnectSocket()
    {
        //_socket.ReceivedMatchState += OnMatchStateReceived;
        //_socket.ReceivedMatchPresence += OnMatchPresence;
        _socket = _client.NewSocket();
        _socket.ReceivedMatchState += ms => _mainThread.Enqueue(() => OnMatchStateReceived(ms));
        _socket.ReceivedMatchPresence += mp => _mainThread.Enqueue(() => OnMatchPresence(mp));
        _socket.ReceivedNotification += nf => _mainThread.Enqueue(() => OnNotiReceived(nf));
        await _socket.ConnectAsync(Session, true);

        //_socket.ReceivedNotification += notification => {

        //    var inGameNoti = JsonConvert.DeserializeObject<InGameNoti>(notification.Subject);
        //    Debug.Log("content? : " + notification.Content);
        //    Debug.Log("message : " + inGameNoti.Message);
        //    //const int rewardCode = 100;
        //    //switch (notification.Code)
        //    //{
        //    //    case rewardCode:
        //    //        Debug.LogFormat("Congratulations, you won the tournament!\n{0}\n{1}", notification.Subject, notification.Content);
        //    //        break;
        //    //    default:
        //    //        Debug.LogFormat("Other notification: {0}:{1}\n{2}", notification.Code, notification.Subject, notification.Content);
        //    //        break;
        //    //}
        //};
    }

    private void OnNotiReceived(IApiNotification notification)
    {
        if(NotiAction != null)
        {
            NotiAction.Invoke(notification);
        }
    }


    async private void OnMatchStateReceived(IMatchState matchState)
    {
        var stateJson = Encoding.UTF8.GetString(matchState.State);

        switch (matchState.OpCode)
        {
            case OpCodes.Idle:
                {
                    var positionState = JsonParser.FromJson<PositionState>(stateJson);
                    string SenderSessionId = positionState.SenderSessionId;
                    if (SenderSessionId == _playerSessionId)
                    {
                        return;
                    }
                    _players[SenderSessionId].GetComponent<RemotePlayerController>().State = RemotePlayerState.Idle;
                }
                break;
            case OpCodes.Position:
                {
                    var positionState = JsonParser.FromJson<PositionState>(stateJson);
                    string SenderSessionId = positionState.SenderSessionId;

                    if (SenderSessionId == _playerSessionId)
                    {
                        return;

                    }
                    Debug.LogWarning("Receive Other Player Moving");
                    if(_players.ContainsKey(SenderSessionId))
                    {
                        Debug.LogWarning($"x : {positionState._X} y : {positionState._Y} z : {positionState._Z} rotate y :{positionState._Rotate_Y}");
                        Debug.LogWarning("Other Player moved");
                        //_players[SenderSessionId].transform.position = new Vector3(positionState._X, positionState._Y, positionState._Z);
                        _players[SenderSessionId].transform.position = Vector3.Lerp(_players[SenderSessionId].transform.position, new Vector3(positionState._X, positionState._Y, positionState._Z), Time.deltaTime * 10);
                        _players[SenderSessionId].transform.rotation = Quaternion.Euler(0, positionState._Rotate_Y, 0);
                    }
                    //Debug.Log(positionState._X);

                    //if(matchState.UserPresence == null)
                    //{
                    //    return;
                    //}
                    //if (matchState.UserPresence.SessionId == _playerSessionId)
                    //{
                    //    Debug.Log("나야~");
                    //        return;
                    //}

                    //Debug.LogWarning("Received Match State Position");


                    //// Update the GameObject associated with that player
                    //if (_players.ContainsKey(matchState.UserPresence.SessionId))
                    //{
                    //    Debug.Log("여기 가능?");
                    //    // Here we would normally do something like smoothly interpolate to the new position, but for this example let's just set the position directly.
                    //    _players[matchState.UserPresence.SessionId].transform.position = new Vector3(positionState._X, positionState._Y, positionState._Z);
                    //}
                }
                break;

            case OpCodes.Moving:
                {
                    var positionState = JsonParser.FromJson<PositionState>(stateJson);
                    string SenderSessionId = positionState.SenderSessionId;

                    if (SenderSessionId == _playerSessionId)
                    {
                        return;
                    }
                    _players[SenderSessionId].GetComponent<RemotePlayerController>().State = RemotePlayerState.Moving;
                }
                break;

            case OpCodes.Jumping:
                {
                    var positionState = JsonParser.FromJson<PositionState>(stateJson);
                    string SenderSessionId = positionState.SenderSessionId;

                    if (SenderSessionId == _playerSessionId)
                    {
                        return;
                    }
                    _players[SenderSessionId].GetComponent<RemotePlayerController>().State = RemotePlayerState.Jumping;
                }
                break;

            case OpCodes.Join:
                {
                    JoinState joinstate = JsonParser.FromJson<JoinState>(stateJson);
                    string SenderSessionId = joinstate.SenderSessionId;
                    if (SenderSessionId == _playerSessionId)
                    {
                        return;
                    }

                    Debug.LogWarning("Received Match Join");

                    if (!_players.ContainsKey(SenderSessionId))
                    {
                        Vector3 position = new Vector3(joinstate._X, joinstate._Y, joinstate._Z);
                        SpawnAction.Invoke(SenderSessionId, position);
                        JoinState state = new JoinState(_playerSessionId, Player.transform.position);
                        await Manager.Nakama.Socket.SendMatchStateAsync(_matchId, OpCodes.Join, Nakama.TinyJson.JsonWriter.ToJson(state));
                        //_players.Add(SenderSessionId, _remotePlayerPrefab);
                        //GameObject go = UnityEngine.Object.Instantiate(RemotePlayer);
                        //go.transform.position = new Vector3(joinstate._X, joinstate._Y, joinstate._Z);
                    }

                    //foreach (var presence in Match.Presences)
                    //{
                    //    Debug.Log("Match Presence SessionId : " + presence.SessionId);
                    //    Debug.Log("Match Presence UserId : " + presence.UserId);
                    //    Debug.Log("Match Presence Username : " + presence.Username);


                    //    if (!_players.ContainsKey(presence.SessionId))
                    //    {
                    //        _players.Add(presence.SessionId, _remotePlayerPrefab);
                    //        GameObject go = UnityEngine.Object.Instantiate(RemotePlayer);
                    //        var stateJson = Encoding.UTF8.GetString(matchState.State);
                    //        var positionState = JsonParser.FromJson<PositionState>(stateJson);
                    //        go.transform.position = new Vector3(positionState._X, positionState._Y, positionState._Z);
                    //    }

                    //}
                }
                break;

            case OpCodes.Chat:
                {
                    if (ChatAction != null)
                        ChatAction.Invoke();
                }
                break;
        }
     }

    // UnityMainThreadDispathcer 사용하기 전에 이렇게 해보았는데
    // 이래도 안됬음 
    // 문제 : mainThread가 아닌 다른 Thread에서는 Instantiate를 할 수없음
    // 해결범 : UnityMainThreadDispathcer를 사용하여 mainThread로 Instantatite
    private void SpawnOther(string senderSessionId, Vector3 initPosition)
    {
        GameObject go = UnityEngine.Object.Instantiate(RemotePlayerPrefab);
        _players.Add(senderSessionId, go);
        go.transform.position = initPosition;
    }

    private void OnMatchPresence(IMatchPresenceEvent e)
    {
        if (e.Joins.Count() > 0)
        {
            Debug.LogWarning("OnMatchPresence() User(s) Join the game");
        }

        if (e.Leaves.Count() > 0)
        {
            Debug.LogWarning($"OnMatchPresence() User(s) left the game");
        }
    }


    public async Task JoinMatch(string matchId)
    {
        //ConnectSocket();
        Match = await _socket.JoinMatchAsync(matchId);
        _playerSessionId = Match.Self.SessionId;
        JoinState state = new JoinState(_playerSessionId, 0, 0, 0);
        await Manager.Nakama.Socket.SendMatchStateAsync(_matchId, OpCodes.Join, Nakama.TinyJson.JsonWriter.ToJson(state));
        //_matchId = Match.Id;
        //Debug.Log("Match Id : " + Match.Id);
        //Debug.Log("Match Label : " + Match.Label);
        //foreach (var presence in Match.Presences)
        //{
        //    Debug.Log("Match Presence SessionId : " + presence.SessionId);
        //    Debug.Log("Match Presence UserId : " + presence.UserId);
        //    Debug.Log("Match Presence Username : " + presence.Username);

        //    if (!_players.ContainsKey(presence.SessionId))
        //        _players.Add(presence.SessionId, _remotePlayerPrefab);
        //}
        //Debug.Log("Match Self : " + Match.Self);
        //Debug.Log("Match Size : " + Match.Size);

        //foreach (KeyValuePair<string, GameObject> player in _players)
        //{
        //    UnityEngine.Object.Instantiate(player.Value);
        //}
    }

    public async void GameStart()
    {
        SceneManager.LoadScene("Game");
        await Manager.Nakama.RPC.InitializeChat();
        await Manager.Nakama.JoinMatch(Manager.Nakama.MatchId);
        if (Manager.Nakama.PlayerPrefab == null)
            Debug.LogWarning("Load unitychan failed");
        Manager.Nakama.Player = UnityEngine.Object.Instantiate(Manager.Nakama.PlayerPrefab);
    }
}

public static class OpCodes
{
    public const long Position = 1;
    public const long Join = 2;

    public const long Idle = 13;
    public const long Moving = 14;
    public const long Jumping = 15;

    public const long Chat = 20;

}


public class PositionState
{
    string _senderSessionId;
    public float _X;
    public float _Y;
    public float _Z;
    public float _Rotate_Y;

    public string SenderSessionId { get { return _senderSessionId; } set { _senderSessionId = value; } }

    public PositionState(string senderSessionId, float X, float Y, float Z, float Rotate_Y)
    {
        _senderSessionId = senderSessionId;
        _X = X;
        _Y = Y;
        _Z = Z;
        _Rotate_Y = Rotate_Y;
    }
}

public class JoinState
{
    string _senderSessionId;
    public float _X;
    public float _Y;
    public float _Z;

    public string SenderSessionId { get { return _senderSessionId; } set { _senderSessionId = value; } }
    public JoinState(string senderSessionId, float X, float Y, float Z)
    {
        _senderSessionId = senderSessionId;
        _X = X;
        _Y = Y;
        _Z = Z;
    }
    public JoinState(string senderSessionId, Vector3 position)
    {
        _senderSessionId = senderSessionId;
        _X = position.x;
        _Y = position.y;
        _Z = position.z;
    }
}

public class InGameNoti
{
    string _message;

    public string Message { get { return _message; } set { _message = value; } }

    public InGameNoti(string message)
    {
        _message = message;
    }
}