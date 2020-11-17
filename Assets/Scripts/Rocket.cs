using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public GameObject explosion;
    public float speed = 30.0f;

    public Vector3 dir;
    private bool hitPlayer = false;

    private float radius = 7.0f;

    private void Start()
    {
        transform.Rotate(new Vector3(90.0f, 0, 0));
    }

    private void Update()
    {
        if (gameObject.activeSelf == false)
        {
            Debug.Log("inactive");
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        transform.position += (dir * Time.deltaTime * speed);
    }

    void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosion, transform.position, transform.rotation);

        Collider[] colls = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider hit in colls)
        {
            CharacterController cc = hit.GetComponent<CharacterController>();
            
            if (cc != null)
            {
                if (cc.gameObject.CompareTag("Player") && !hitPlayer)
                {
                    hit.gameObject.GetComponent<Player_Tank>().SetRocketHitParams(transform.position);
                    hitPlayer = true;
                }
                //if (cc.gameObject.CompareTag("Enemy"))
                //{
                //    if (!Physics.Linecast(collision.transform.position, cc.transform.position))
                //    {
                //        cc.gameObject.GetComponent<AIManager>().reduceHealth(1);

                //        if (cc.gameObject.GetComponent<AIManager>().tankType == 2)
                //        {
                //            cc.gameObject.GetComponent<AIManager>().isProtected = false;
                //        }
                //    }
                //}
            }

            if (hit.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Hit Target");
                float health = (collision.transform.position - hit.transform.position).magnitude * 0.5f;
                hit.gameObject.GetComponent<TargetHit>().reduceHealth(health, GameObject.Find("Main Player"));
                Debug.Log(health);
                //Maybe add Linecast to check if anything is blocking it
            }
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<AIManager>().reduceHealth(1);

            if (other.gameObject.GetComponent<AIManager>().tankType == 2)
            {
                other.gameObject.GetComponent<AIManager>().isProtected = false;
            }

            gameObject.SetActive(false);
        }
    }
}
