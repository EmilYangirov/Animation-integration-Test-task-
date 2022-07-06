using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Vector3 cameraOffset;
    public float smooth = 8.0f;
    private Vector3 velocity = Vector3.zero;
     

    private void Start()
    {
        GameObject ch = GameObject.FindGameObjectWithTag("Player");
        player = ch.transform;
        cameraOffset = transform.position-player.position;
    }

    private void FixedUpdate()
    {
        TrackPlayer();
    }   
    private void TrackPlayer()
    {        
        Vector3 newPos = player.position + cameraOffset;

        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smooth);
    }
}
