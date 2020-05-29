using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRocket : MonoBehaviour
{
    public Vector3 dir;

    GameObject player;
    float distance;
    float speed = 15f;
    float radius = 7.0f;
    

    void Start()
    {
        player = GameObject.Find("Main Player");
    }

    private void Update()
    {
        if (gameObject.activeSelf == false) Destroy(gameObject);
    }

    void FixedUpdate()
    {
        transform.position += (dir * Time.deltaTime * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Enemy"))
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider hit in colls)
            {
                CharacterController cc = hit.GetComponent<CharacterController>();
                if (cc != null)
                {
                    if (cc.gameObject.CompareTag("Player"))
                    {
                        cc.gameObject.GetComponent<Player_Tank>().health -= 1;
                        break;
                    }
                }
            }

            gameObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("Environment"))
        {
            gameObject.SetActive(false);
        }
    }

    public void Deactivate()
    {
        this.gameObject.SetActive(false);
    }
}
