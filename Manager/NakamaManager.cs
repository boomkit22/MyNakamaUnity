using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NakamaManager
{
    protected Nakama.IClient _client;
    protected Nakama.ISession _session;
    protected Nakama.ISocket _socket;
    protected RPC _rpc = new RPC();
    protected string _matchId;

    protected GameObject _player;
    protected GameObject _playerPrefab;
    protected GameObject _remotePlayerPrefab;
    protected Nakama.IMatch _match;

    protected string _playerSessionId;
    protected UnityMainThreadDispatcher mainThread;

    public Action<String, Vector3> SpawnAction = null;
    public string PlayerSessionId { get { return _playerSessionId; } set { _playerSessionId = value; } }


    public Nakama.IClient Client { get { return _client; } }
    public Nakama.ISession Session { get { return _session; } set { _session = value; } }
    public Nakama.ISocket Socket { get { return _socket; } }
    public string MatchId { get { return _matchId; } set { _matchId = value; } }
    public RPC RPC { get { return _rpc; } }

    public Dictionary<string, GameObject> _players = new Dictionary<string, GameObject>();

    public GameObject Player { get { return _player; } set { _player = value; } }
    public GameObject PlayerPrefab { get { return _playerPrefab; } set { _playerPrefab = value; } }
    public GameObject RemotePlayerPrefab { get { return _remotePlayerPrefab; } set { _remotePlayerPrefab = value; } }
    public Nakama.IMatch Match { get { return _match; } set { _match = value; } }
    public void Init()
    {
        _client = new Nakama.Client("http", "127.0.0.1", 7350, "defaultkey");
        _client.Timeout = 10;

        _playerPrefab = Resources.Load<GameObject>("Prefabs/unityChan");
        _remotePlayerPrefab = Resources.Load<GameObject>("Prefabs/remoteUnityChan");
        SpawnAction += SpawnOther;

    }

    public async void ConnectSocket()
    {
        _socket = _client.NewSocket();
        //_socket.ReceivedMatchState += OnMatchStateReceived;
        //_socket.ReceivedMatchPresence += OnMatchPresence;

        _socket.ReceivedMatchState += ms => mainThread.Enqueue(() => OnMatchStateReceived(ms));
        _socket.ReceivedMatchPresence += mp => mainThread.Enqueue(() => OnMatchPresence(mp));


        await _socket.ConnectAsync(_session, true, connectTimeout: 10);

        if (mainThread == null)
        {
            mainThread = UnityMainThreadDispatcher.Instance();
        }
    }

    async private void OnMatchStateReceived(IMatchState matchState)
    {
        switch (matchState.OpCode)
        {
            case OpCodes.Position:
                {
                    var stateJson = Encoding.UTF8.GetString(matchState.State);
                    var positionState = JsonParser.FromJson<PositionState>(stateJson);
                    string SenderSessionId = positionState.SenderSessionId;

                    if(SenderSessionId == _playerSessionId)
                    {
                        return;
                    }
                    Debug.LogWarning("Receive Other Player Moving");

                    if(_players.ContainsKey(SenderSessionId))
                    {
                        Debug.LogWarning($"x : {positionState._X} y : {positionState._Y} z : {positionState._Z} rotate y :{positionState._Rotate_Y}");
                        Debug.LogWarning("Other Player moved");
                        _players[SenderSessionId].transform.position = new Vector3(positionState._X, positionState._Y, positionState._Z);
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
            case OpCodes.Join:
                {

                    var stateJson = Encoding.UTF8.GetString(matchState.State);
                    Debug.Log(stateJson);
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
                        await Manager.Nakama.Socket.SendMatchStateAsync(_matchId, OpCodes.Join, JsonWriter.ToJson(state));

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
        }
     }

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
        Match = await _socket.JoinMatchAsync(matchId);


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

        _playerSessionId = Match.Self.SessionId;
        //foreach (KeyValuePair<string, GameObject> player in _players)
        //{
        //    UnityEngine.Object.Instantiate(player.Value);
        //}

        JoinState state = new JoinState(_playerSessionId, 0, 0, 0);
        await Manager.Nakama.Socket.SendMatchStateAsync(_matchId, OpCodes.Join, JsonWriter.ToJson(state));

    }




}
public static class OpCodes
{
    public const long Position = 1;
    public const long Join = 2;
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