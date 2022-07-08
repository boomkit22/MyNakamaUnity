using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    Vector3 _delta = new Vector3(0, 5, -3);

    GameObject _player = null;

    public void SetPlayer(GameObject player) { _player = player; }
    void Start()
    {
            
    }

    void LateUpdate()
    {
        if (_player == null)
            return;

        transform.position = _player.transform.position + _delta;

        transform.LookAt(_player.transform);
    }
}
