using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerController : MonoBehaviour
{

    [SerializeField]
    RemotePlayerState _state = RemotePlayerState.Idle;

    bool _playingJumping = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public RemotePlayerState State
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
                case RemotePlayerState.Idle:
                    anim.CrossFade("WAIT", 0.01f);
                    break;
                case RemotePlayerState.Moving:
                    anim.CrossFade("RUN", 0.005f);
                    break;
                case RemotePlayerState.Jumping:
                    anim.CrossFade("JUMP", 0.001f);
                    break;
            }
        }
    }

    public void OnJumpFinish()
    {
        _playingJumping = false;
    }

}

public enum RemotePlayerState
{
    Idle,
    Moving,
    Jumping,

}