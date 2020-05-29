using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public int tankType;
    public bool isProtected;
    public bool canMove;
    public Transform position1;
    public Transform position2;
    public GameObject weaponPrefab;
    public GameObject towerBone;
    public GameObject tankBase;
    public GameObject leftTrack;
    public GameObject rightTrack;
    public GameObject gun;
    public Material normalMat;

    private Vector3 impulse;
    private GameObject player;
    private int health;
    private NavMeshAgent nmAgent;
    private float noticeRadius;
    private float stayRadius;
    private float waterLevel;
    public bool updateSpin;
    private const float reloadTime = 5.0f;
    private float reload = reloadTime - 0.1f;

    void Start()
    {
        nmAgent = GetComponent<NavMeshAgent>();
        nmAgent.speed = 5.0f;
        player = GameObject.FindGameObjectWithTag("Player");
        noticeRadius = 60.0f;
        stayRadius = 10.0f;
        impulse = Vector3.zero;
        waterLevel = 0.0f;

        health = 2;

        if (canMove) nmAgent.SetDestination(position1.position);

        updateSpin = false;

        if (tankType != 0)
        {
            isProtected = true;
        }
    }
    
    void Update()
    {
        //Kill tank
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }

        if (gameObject.activeSelf == false)
        {
            Destroy(gameObject);
        }
        else
        {
            //Remove protection from water turret
            if (tankType == 1 && waterLevel >= 40.0f)
            {
                Debug.Log("Protection gone");
                isProtected = false;
                ChangeColor();
            }
            else if (tankType == 2 && waterLevel >= 80.0f)
            {
                Debug.Log("Protection gone");
                isProtected = false;
                ChangeColor();
            }

            //Movement
            Vector3 path = (transform.position - player.transform.position);
            if (path.magnitude > stayRadius)
            {
                if (nmAgent.isStopped) nmAgent.isStopped = false;

                if (path.magnitude <= noticeRadius)
                {
                    updateSpin = true;

                    if (canMove) nmAgent.SetDestination(-path + (path.normalized * stayRadius));

                    if (reload == reloadTime)
                    {
                        FireWeapon();
                        reload -= Time.deltaTime;
                    }
                }
                else
                {
                    updateSpin = false;

                    //Sentry between two positions
                    if (transform.position.x == position1.position.x && transform.position.z == position1.position.z)
                    {
                        if (canMove) nmAgent.SetDestination(position2.position);
                    }
                    else if (transform.position.x == position2.position.x && transform.position.z == position2.position.z)
                    {
                        if (canMove) nmAgent.SetDestination(position1.position);
                    }
                }
            }
            else if (path.magnitude <= stayRadius && path.magnitude >= stayRadius - 1.5f)
            {
                nmAgent.isStopped = true;
                updateSpin = true;

                if (reload == reloadTime)
                {
                    FireWeapon();
                    reload -= Time.deltaTime;
                }
            }
            else
            {
                if (nmAgent.isStopped) nmAgent.isStopped = false;
                if (canMove) nmAgent.SetDestination(path.normalized * stayRadius);
            }
        }

        //Reloading
        if (reload <= 0.0f)
        {
            reload = reloadTime;
        }
        else if (reload != reloadTime)
        {
            reload -= Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        if (updateSpin)//need to fix this - rotation is weird on the tower bone
        {
            //Vector3 target = new Vector3(player.transform.position.x - towerBone.transform.position.x, towerBone.transform.position.y, player.transform.position.z - towerBone.transform.position.z);
            Vector3 target = new Vector3(towerBone.transform.position.y, towerBone.transform.position.z, towerBone.transform.position.x);
            Quaternion targetRot = Quaternion.LookRotation(target);
            float min = Mathf.Min(Time.deltaTime, 1);
            towerBone.transform.rotation = Quaternion.Lerp(towerBone.transform.rotation, targetRot, min);

            //towerBone.transform.rotation = Quaternion.Euler(Vector3.Angle(towerBone.transform.forward, towerBone.transform.forward - new Vector3(0, player.transform.position.y)), Vector3.Angle(towerBone.transform.forward, new Vector3(player.transform.position.x - towerBone.transform.position.x, towerBone.transform.position.y, player.transform.position.z - towerBone.transform.position.z)), 0);
            //towerBone.transform.rotation = Quaternion.FromToRotation(towerBone.transform.position, player.transform.position);
        }
    }

    public void reduceHealth(int amt)
    {
        if (!isProtected)
        {
            health -= amt;
        }
    }

    public void setAgentStop(bool val)
    {
        nmAgent.isStopped = val;
    }

    public void AddWater(float amt)
    {
        this.waterLevel += amt;
    }

    private void FireWeapon()
    {
        GameObject inst_ball = Instantiate(weaponPrefab, gun.transform.position, transform.rotation);
        inst_ball.GetComponent<AIRocket>().dir = (player.transform.position - gun.transform.position).normalized;
    }

    private void ChangeColor()
    {
        tankBase.GetComponent<Renderer>().material = normalMat;
        leftTrack.GetComponent<Renderer>().material = normalMat;
        rightTrack.GetComponent<Renderer>().material = normalMat;
    }
}
