using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{
    public float speedH = 4.0f;
    public float speedV = 4.0f;
    public Transform player;

    private float pitch = 0.0f;

    void Update()
    {
        float yaw = speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        player.Rotate(Vector3.up * yaw);
    }
}
