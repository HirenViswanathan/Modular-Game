using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player_Sword : MonoBehaviour
{
    //player vars
    private CharacterController controller;
    private Camera cam;
    private float gravity = 20.0f;
    private Vector3 moveVector = Vector3.zero;
    private bool grounded = false;
    private float counter;
    public int health;
    //public Text healthText;
    //public GameObject gameOver;
    public Text knifeTxt;
    public float movtSpeed;
    public float jumpSpeed;

    //sword vars
    public GameObject sword;

    private bool holdingSword;
    private Vector3 swordPos;
    private Quaternion swordRot;
    private float stuckAngleThreshold;

    //Throwing knives vars
    private const int maxKnifeCnt = 10;
    private int knifeCnt;
    private const float refillTIme = 1.5f;
    private float refill = refillTIme;
    


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        movtSpeed = 15.0f;
        jumpSpeed = 10.0f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        knifeCnt = maxKnifeCnt;
        holdingSword = true;

        swordPos = sword.transform.localPosition;
        swordRot = sword.transform.localRotation;

        knifeTxt.text = string.Format("Knives: {0}", knifeCnt);

        stuckAngleThreshold = 30f;
    }

    // Update is called once per frame
    void Update()
    {
        //melee
        if (Input.GetMouseButtonDown(0)){
            Ray forwardRay = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(forwardRay, out hit, 2.5f))
            {
                Debug.DrawRay(forwardRay.origin, forwardRay.direction, Color.red);
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    hit.collider.gameObject.GetComponent<TargetHit>().reduceHealth(15f, gameObject);
                }
            }
        }
        //zoom?
        if (Input.GetMouseButtonDown(1)){

        }
        //throwing knives
        if (Input.GetButtonDown("Alternate")){
            if (knifeCnt > 0){
                Ray forwardRay = new Ray(cam.transform.position, cam.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(forwardRay, out hit))
                {
                    //Debug.DrawRay(forwardRay.origin, forwardRay.direction, Color.red);
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        hit.collider.gameObject.GetComponent<TargetHit>().reduceHealth(10f, gameObject);
                    }
                }

                knifeCnt--;
                knifeTxt.text = string.Format("Knives: {0}", knifeCnt);
            }
        }
        //throw and teleport to sword
        if (Input.GetButtonDown("Ability")){
            //throw sword
            if (holdingSword){
                sword.GetComponent<Sword>().ThrowSword(cam.transform.forward);

                holdingSword = false;
            }
            //teleport to sword
            else{
                //check if sword is stuck in something
                if (sword.GetComponent<Sword>().getStuck()){
                    //get angle between stuck surface and up vector
                    GameObject stuckSurf = sword.GetComponent<Sword>().GetStuckSurface();
                    if (Vector3.Angle(stuckSurf.transform.up, Vector3.up) > stuckAngleThreshold){
                        //surface to teleport to and stick
                        Vector3 telePos = GetTeleportPosition(0);
                        Teleport(telePos);
                    }
                    else{
                        //teleport next to sword and pull it out
                        Vector3 telePos = GetTeleportPosition(1);
                        Teleport(telePos);
                    }
                }
                else{
                    transform.position = sword.transform.position;
                    //sword.GetComponent<Sword>().catchSword(transform);

                    //AOE attack on landing if attack button pressed? if from past certain height?
                }
                holdingSword = true;
                sword.GetComponent<Sword>().catchSword(transform);
                sword.transform.localPosition = swordPos;
                sword.transform.localRotation = swordRot;
            }
        }

        //Refilling throwing knives
        if (knifeCnt < maxKnifeCnt){
            refill -= Time.deltaTime;
        }

        if (refill <= 0){
            refill = refillTIme;
            knifeCnt++;
            knifeTxt.text = string.Format("Knives: {0}", knifeCnt);
        }
    }

    void FixedUpdate(){
        //Movement
        if (grounded)
        {
            moveVector = new Vector3();
            moveVector += transform.forward * Input.GetAxis("Vertical") * movtSpeed;
            moveVector += transform.right * Input.GetAxis("Horizontal") * movtSpeed;

            if (Input.GetButton("Jump"))
            {
                moveVector.y = jumpSpeed;
            }
        }

        moveVector.y -= (gravity * Time.deltaTime);
        CollisionFlags flags = controller.Move(moveVector * Time.deltaTime);
        grounded = (flags & CollisionFlags.Below) != 0;
    }

    //FIX THIS - Maybe have a variable store the base of the sword so the character can "stand" on it when they teleport? or use a formula to calculate it when necessary
    private Vector3 GetTeleportPosition(int mode){
        Vector3 teleportPos = new Vector3();
        switch(mode){
            case 0://not floor
                GameObject stuckSurf = sword.GetComponent<Sword>().GetStuckSurface();
                //teleportPos = sword.transform.position + (Vector3.up * 0.75f) + (stuckSurf.transform.up);
                teleportPos = sword.transform.position + (stuckSurf.transform.up * 0.5f) + (Vector3.up);
                break;
            case 1://floor
                stuckSurf = sword.GetComponent<Sword>().GetStuckSurface();
                teleportPos = sword.transform.position + stuckSurf.transform.up;
                break;
            default://error ig
                break;
        }
        return teleportPos;
    }

    private void Teleport(Vector3 telePos){
        transform.position = telePos;
    }
}
