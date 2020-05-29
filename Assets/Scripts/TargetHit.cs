using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetHit : MonoBehaviour
{
    public float health;
    public TextMesh healthTxt;

    private const float fadeTime = 1f;
    private float curTime = fadeTime;

    // Start is called before the first frame update
    void Start()
    {
        healthTxt.gameObject.SetActive(false);
        health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (curTime <= 0){
            curTime = fadeTime;
            healthTxt.gameObject.SetActive(false);
        }
        else if (curTime != fadeTime){
            curTime -= Time.deltaTime;
        }
    }

    public void reduceHealth(float dmg, GameObject player){
        health -= dmg;
        if (health <= 0){
            gameObject.SetActive(false);
            return;
        }
        healthTxt.text = health.ToString();
        healthTxt.gameObject.SetActive(true);

        healthTxt.transform.LookAt(player.transform);
        healthTxt.transform.Rotate(new Vector3(0, -180, 0));
        curTime -= Time.deltaTime;
    }
}
