using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//카메라가 플레이어를 따라다니게 하고 구현되어 있는 맵까지만 이동하게 하는 스크립트

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Transform map;

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - player.position;
    }

    void Update()
    {
        transform.position = player.position + offset;

        if (transform.position.x < map.position.x - 18.5f)
        {
            transform.position = new Vector3(map.position.x - 18.5f, transform.position.y, transform.position.z);
        }
        if (transform.position.x > map.position.x + 18.5f)
        {
            transform.position = new Vector3(map.position.x + 18.5f, transform.position.y, transform.position.z);
        }
        if (transform.position.y < map.position.y - 18.5f)
        {
            transform.position = new Vector3(transform.position.x, map.position.y - 18.5f, transform.position.z);
        }
        if (transform.position.y > map.position.y + 18.5f)
        {
            transform.position = new Vector3(transform.position.x, map.position.y + 18.5f, transform.position.z);
        }
    }
}