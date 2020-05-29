using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    private GameObject door;
    public bool isOn;

    void Start()
    {
        door = GameObject.FindGameObjectWithTag("SF_Door");
        isOn = false;
    }
    
    void Update()
    {
        if (isOn)
        {
            door.GetComponent<Animation>().Play("open");
            isOn = false;
        }
    }
}
