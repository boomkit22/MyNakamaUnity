using Nakama.TinyJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 5.0f;

    [SerializeField]
    PlayerState _state = PlayerState.Idle;

    bool _playingJumping = false;

    public enum PlayerState
    {
        Idle,
        Moving,
        Jumping
    }

    public PlayerState State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
            Animator anim = GetComponent<Animator>();
            switch (_state)
            {
                case PlayerState.Idle:
                    anim.CrossFade("WAIT", 0.01f);
                    break;
                case PlayerState.Moving:
                    anim.CrossFade("RUN", 0.005f);
                    break;
                case PlayerState.Jumping:
                    anim.CrossFade("JUMP", 0.001f);
                    break;
            }
        }
    }

    void Init()
    {
        Camera.main.GetComponent<CameraController>().SetPlayer(gameObject);
    }

    void Start()
    {
        Init();
    }

    async void Update()
    {
        if (!Input.anyKey && !_playingJumping)
        {
            if (State != PlayerState.Idle)
            {
                State = PlayerState.Idle;
                PositionState state = new PositionState(Manager.Nakama.PlayerSessionId, transform.position.x, transform.position.y, transform.position.z, transform.rotation.eulerAngles.y);
                await Manager.Nakama.Socket.SendMatchStateAsync(Manager.Nakama.MatchId, OpCodes.Idle, JsonWriter.ToJson(state));
                return;
            }
        }
        KeyboardInput();
    }

    async void KeyboardInput()
    {
        KeyboardMoving();
            
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playingJumping = true;
            State = PlayerState.Jumping;
            PositionState state = new PositionState(Manager.Nakama.PlayerSessionId, transform.position.x, transform.position.y, transform.position.z, transform.rotation.eulerAngles.y);
            await Manager.Nakama.Socket.SendMatchStateAsync(Manager.Nakama.MatchId, OpCodes.Jumping, JsonWriter.ToJson(state));

        }
    }

    void KeyboardMoving()
    {
        float moveDist = _speed * Time.deltaTime;
        Vector3 dir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            dir = Vector3.forward;
            PlayerMove(dir, moveDist);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            dir = Vector3.back;
            PlayerMove(dir, moveDist);

        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            dir = Vector3.left;
            PlayerMove(dir, moveDist);

        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            dir = Vector3.right;
            PlayerMove(dir, moveDist);
        }
    }

     async void PlayerMove(Vector3 dir, float moveDistance)
     {
        float rotate_y = transform.rotation.eulerAngles.y;

        if (!_playingJumping)
        {
            State = PlayerState.Moving;
            PositionState moving = new PositionState(Manager.Nakama.PlayerSessionId, transform.position.x, transform.position.y, transform.position.z, rotate_y);
            await Manager.Nakama.Socket.SendMatchStateAsync(Manager.Nakama.MatchId, OpCodes.Moving, JsonWriter.ToJson(moving));
        }

        transform.position += dir * moveDistance;
        transform.rotation = Quaternion.Slerp(transform.rotation
        , Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        PositionState state = new PositionState ( Manager.Nakama.PlayerSessionId,  transform.position.x, transform.position.y, transform.position.z, rotate_y);
        await Manager.Nakama.Socket.SendMatchStateAsync(Manager.Nakama.MatchId, OpCodes.Position, JsonWriter.ToJson(state));
     }

    public void OnJumpFinish()
    {
        _playingJumping = false;
    }
}
