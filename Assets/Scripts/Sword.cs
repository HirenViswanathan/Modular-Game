using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private bool isThrown;
    private bool isHeld;
    private Vector3 dir;
    private float speed;
    private float mass;
    private bool isStuck;
    private GameObject stuckSurface;
    private Vector3 forwardDir;
    private GameObject parentPlayer;

    void Start()
    {
        isThrown = false;
        isHeld = true;
        speed = 30f;
        mass = 0.25f;

        parentPlayer = transform.parent.gameObject;
    }

    void FixedUpdate()
    {
        if (isThrown){
            transform.position += (dir * Time.deltaTime * speed);
            dir.y -= (mass * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision){
        if (collision.gameObject.CompareTag("Environment")){
            dir = Vector3.zero;
            isThrown = false;
            isStuck = true;

            stuckSurface = collision.gameObject;

            //FIX THIS - get direction before hitting, set rotation to normal of stuckSurface in this direction
            transform.position = collision.GetContact(0).point + new Vector3(0, 0.6f, 0);
            transform.Rotate(new Vector3(-Vector3.Angle(Vector3.up, stuckSurface.transform.rotation.eulerAngles), 0, 0));
        }
        else if (collision.gameObject.CompareTag("Enemy")){
            //reduce health of hit player
            //but don't stop the sword from going through
            if (isThrown){
                collision.gameObject.GetComponent<TargetHit>().reduceHealth(15f, parentPlayer);
            }
        }
    }

    public void ThrowSword(Vector3 d){
        isThrown = true;
        isHeld = false;
        dir = d.normalized;
        transform.parent = null;
    }

    public void catchSword(Transform par){
        isThrown = false;
        isHeld = true;
        isStuck = false;
        dir = Vector3.zero;
        transform.parent = par;
    }

    public bool getStuck(){
        return isStuck;
    }

    public GameObject GetStuckSurface(){
        return stuckSurface;
    }
}
