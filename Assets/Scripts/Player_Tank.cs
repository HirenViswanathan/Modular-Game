using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class Player_Tank : MonoBehaviour
{
    //player vars
    private CharacterController controller;
    private Camera cam;
    private float gravity = 20.0f;
    private Vector3 moveVector = Vector3.zero;
    private bool grounded = false;
    private float counter;
    public int health;
    public Text healthText;
    public Text weaponText;
    public Text info1;
    public Text info2;
    public Text info3;
    public GameObject gameOver;
    public float movtSpeed;
    public float jumpSpeed;
    
    //Rockets
    public GameObject rocketPrefab;
    public Vector3 contPnt;
    public Vector3 expPnt;
    public GameObject rocketObj;
    
    private float rocketPower = 30.0f;
    private float airRes = 0.2f;
    private float groundRes = 0.3f;
    private const float rocketReloadTime = 3.0f;
    private float rocketReload = rocketReloadTime;
    private Vector3 impulse_rocket;
    private Vector3 impulse_airCannon;
    private float rockDelY = 7.0f;
    //private Animator rocketAnim;
    private float altContPnt;

    /*
    //Water Cannon/Turret
    public GameObject turObj;

    private float watCanPower = 10.0f;
    private const float watCapacity = 100.0f;
    private float watLevel = watCapacity;
    private float watCanRed = 0.3f;
    private float watTurRed = 0.7f;
    private bool watCanInUse = false;
    private bool watTurInUse = false;
    private Vector3 impulse_watCan;
    private Vector3 turDir;
    private Animator turAnim;
    */

    //Grappling/Retractable Hook
    public GameObject hookObj;

    private bool gh_isAtt = false;
    private bool rh_isAtt = false;
    private float gh_maxLength = 300.0f;
    private float gh_len;
    private float rh_speed = 1.0f;
    private Vector3 gh_contPnt;
    private float lastVel;
    //private Animator hookAnim;
    LineRenderer rope;

    //Sniper
    public GameObject rifleObj;

    private const float gunReloadTime = 7.0f;
    private float gunReload = gunReloadTime;
    private const float gunCoolTime = 0.2f;
    private float gunCool = gunCoolTime;
    //private Animator gunAnim;
    private int clipSize = 25;
    private int curClip;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        //selectedWeapon = 0;
        movtSpeed = 10.0f;
        jumpSpeed = 7.0f;

        impulse_rocket = Vector3.zero;
        impulse_airCannon = Vector3.zero;
        //impulse_watCan = Vector3.zero;

        //turDir = Vector3.zero;
        gh_contPnt = Vector3.zero;

        health = 5;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //rocketAnim = rocketObj.GetComponent<Animator>();
        //turAnim = turObj.GetComponent<Animator>();
        //hookAnim = hookObj.GetComponent<Animator>();
        //gunAnim = rifleObj.GetComponent<Animator>();

        healthText.text = "Health: 5";

        rope = GetComponent<LineRenderer>();
        lastVel = 0;

        curClip = clipSize;
    }
    
    void Update()
    {
        //Update health
        if (health == 4)
        {
            healthText.text = "Health: 4";
        }
        else if (health == 3)
        {
            healthText.text = "Health: 3";
        }
        else if (health == 2)
        {
            healthText.text = "Health: 2";
        }
        else if (health == 1)
        {
            healthText.text = "Health: 1";
        }
        else if (health <= 0)
        {
            //Game Over screen
            healthText.text = "Health: 0";
            gameOver.SetActive(true);
            counter += Time.deltaTime;
            if (counter >= 5)
            {
                SceneManager.LoadScene(0);
            }
        }
        
        //Gun
        if (Input.GetMouseButton(0))
        {
            /*
            //Water Cannon
            else if (selectedWeapon == 2 && !watTurInUse && watLevel == watCapacity && grounded)
            {
                watCanInUse = true;
                watLevel -= watCanRed;
                turObj.GetComponents<AudioSource>()[0].Play();
            }
            */
            //Shoot Rifle
            if (gunCool == gunCoolTime && curClip > 0)
            {
                curClip -= 1;
                rifleObj.GetComponents<AudioSource>()[0].Play();

                Ray forwardRay = new Ray(cam.transform.position, cam.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(forwardRay, out hit))
                {
                    Debug.DrawRay(forwardRay.origin, forwardRay.direction, Color.red);
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        hit.collider.gameObject.GetComponent<TargetHit>().reduceHealth(10f, gameObject);
                    }
                    else if (hit.collider.gameObject.CompareTag("Switch"))
                    {
                        UpdateInfo("Switch Hit");
                        hit.collider.gameObject.GetComponent<SwitchController>().isOn = true;
                    }
                }
                gunCool -= Time.deltaTime;
            }
        }

        /*
        //turn off Water Cannon
        if (Input.GetMouseButtonUp(0) && watCanInUse)
        {
            watCanInUse = false;
            turObj.GetComponents<AudioSource>()[0].Stop();
        }
        */

        //Alternate use
        if (Input.GetMouseButton(1))
        {
            //Scope zoom in
            if (GetComponentInChildren<Camera>().fieldOfView == 60)
            {
                GetComponentInChildren<Camera>().fieldOfView = 25f;
            }
            /*
            //Rockets - AOE effect and launch
            if (rocketReload == rocketReloadTime && grounded)
            {
                Collider[] hit = Physics.OverlapSphere(transform.position, 5.0f);
                SetAirCannonParams(hit);

                rocketReload -= Time.deltaTime;
            }
            */
            
            /*
            //Water Turret
            else if (selectedWeapon == 2 && !watCanInUse && watLevel >= 0.0f && grounded)
            {
                watTurInUse = true;
                if (!turObj.GetComponents<AudioSource>()[1].isPlaying)
                {
                    turObj.GetComponents<AudioSource>()[1].Play();
                }

                turDir = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
                Ray forwardRay = new Ray(transform.position, cam.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(forwardRay, out hit, 100f))
                {
                    if (hit.collider.gameObject.CompareTag("Enemy") && hit.collider.gameObject.GetComponent<AIManager>().tankType != 0)
                    {
                        hit.collider.gameObject.GetComponent<AIManager>().AddWater(watTurRed);
                    }
                }

                watLevel -= watTurRed;
            }
            */
        }

        //Rockets
        if (Input.GetButtonDown("Rocket")){
            if (rocketReload == rocketReloadTime)
            {
                Ray forwardRay = new Ray(cam.transform.position, cam.transform.forward);
                RaycastHit hit;
                
                GameObject rocket_inst = Instantiate(rocketPrefab, rocketObj.transform.position, transform.rotation);
                if (Physics.Raycast(forwardRay, out hit))
                {
                    rocket_inst.GetComponent<Rocket>().dir = (hit.point - rocketObj.transform.position).normalized;
                }
                else//Shouldn't need this for enclosed levels but let it be there anyway
                {
                    rocket_inst.GetComponent<Rocket>().dir = forwardRay.direction.normalized;
                }
                rocketReload -= Time.deltaTime;
            }
        }

        if (Input.GetButtonDown("Grapple")){
            /*
            //Grappling Hook
            if (!gh_isAtt && !rh_isAtt)
            {
                UpdateInfo("Grappling Hook Attached");
                hookObj.GetComponent<AudioSource>().Play();

                Ray forwardRay = new Ray(cam.transform.position, cam.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(forwardRay, out hit, gh_maxLength))
                {
                    if (hit.collider.gameObject.CompareTag("Environment"))
                    {
                        gh_isAtt = true;
                        
                        rope.enabled = true;
                        rope.SetPositions(new Vector3[] {hookObj.transform.position, hit.point});

                        SetGrapHookHitParams(hit);
                    }
                }
            }
            */

            //Retractable Hook
            if (!gh_isAtt && !rh_isAtt)
            {
                UpdateInfo("Retractable Hook Attached");
                hookObj.GetComponent<AudioSource>().Play();

                Ray forwardRay = new Ray(cam.transform.position, cam.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(forwardRay, out hit, gh_maxLength))
                {
                    rh_isAtt = true;

                    rope.enabled = true;
                    rope.SetPositions(new Vector3[] { hookObj.transform.position, hit.point });

                    SetRetHookHitParams(hit);
                }
            }
        }
        
        //Get out of Scope
        if (Input.GetMouseButtonUp(1))
        {
            if (GetComponentInChildren<Camera>().fieldOfView == 25f)
            {
                GetComponentInChildren<Camera>().fieldOfView = 60f;
            }
        }
        

        //Update rope position
        if (rope.enabled)
        {
            rope.SetPosition(0, hookObj.transform.position);
        }

        /*
        //Turn off Water Cannon/Turret
        if (Input.GetMouseButtonUp(1) && watTurInUse)
        {
            watTurInUse = false;
            turObj.GetComponents<AudioSource>()[1].Stop();
        }
        if (!grounded && watCanInUse)
        {
            watCanInUse = false;
        }
        */

        //Rocket reloading
        if (rocketReload <= 0.0f)
        {
            rocketReload = rocketReloadTime;
            UpdateInfo("Reloaded Rocket!");
        }
        else if (rocketReload != rocketReloadTime)
        {
            rocketReload -= Time.deltaTime;
        }

        /*
        //Refilling Water Cannon
        if (!watCanInUse && !watTurInUse)
        {
            if (watLevel < watCapacity)
            {
                watLevel += 1.0f;
            }
            else if (watLevel > watCapacity)
            {
                watLevel = watCapacity;
                UpdateInfo("Water Cannon filled!");
            }
        }

        if (watLevel <= 0)
        {
            UpdateInfo("Water cannon empty");
            if (turObj.GetComponents<AudioSource>()[1].isPlaying)
            {
                turObj.GetComponents<AudioSource>()[1].Stop();
            }
        }

        if (watCanInUse)
        {
            watLevel -= watCanRed;
        }
        */

        //Detach Grappling Hook
        if (gh_isAtt || rh_isAtt)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                UpdateInfo("Detaching Hook");
                gh_isAtt = false;
                rh_isAtt = false;
                rope.enabled = false;
            }
        }
        if (rh_isAtt && (gh_contPnt - transform.position).magnitude <= 5.0f)
        {
            UpdateInfo("Detaching retractable hook");
            rh_isAtt = false;
            rope.enabled = false;
        }

        //Rifle shot cooldown
        if (gunCool <= 0f){
            gunCool = gunCoolTime;
        }
        else if (gunCool != gunCoolTime){
            gunCool -= Time.deltaTime;
        }

        //Rifle Reloading
        if (curClip == 0){
            gunReload -= Time.deltaTime;
        }
        if (gunReload <= 0f)
        {
            gunReload = gunReloadTime;
            curClip = clipSize;
            rifleObj.GetComponents<AudioSource>()[1].Play();
            UpdateInfo("Sniper reloaded");
        }
    }

    private void FixedUpdate()
    {
        //Normal Player movement
        Vector3 pl_mvt = new Vector3();
        if (grounded)
        {
            moveVector = new Vector3();
            pl_mvt += transform.forward * Input.GetAxis("Vertical") * movtSpeed;
            pl_mvt += transform.right * Input.GetAxis("Horizontal") * movtSpeed;
            //moveVector = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            //moveVector *= movtSpeed;

            if (Input.GetButton("Jump"))
            {
                pl_mvt.y = jumpSpeed;
            }
        }

        //movement based on rockets
        moveVector += impulse_rocket * (0.3f / (transform.position - expPnt).magnitude);
        if (Mathf.Abs(impulse_rocket.x) > 1.0f)
        {
            float impulseDir = impulse_rocket.x > 0 ? 1.0f : -1.0f;
            if (grounded)
            {
                moveVector.x -= (groundRes * impulseDir);
                impulse_rocket.x -= (groundRes * impulseDir);
            }
            else
            {
                moveVector.x -= (airRes * impulseDir);
                impulse_rocket.x -= (airRes * impulseDir);
            }
        }
        else
        {
            impulse_rocket.x = 0;
        }
        if (Mathf.Abs(impulse_rocket.y) > 1.0f)
        {
            moveVector.y -= airRes;
            impulse_rocket.y -= ((gravity * Time.deltaTime) + airRes);
        }
        else
        {
            impulse_rocket.y = 0;
        }
        if (Mathf.Abs(impulse_rocket.z) > 1.0f)
        {
            float impulseDir = impulse_rocket.z > 0 ? 1.0f : -1.0f;
            if (grounded)
            {
                moveVector.z -= (groundRes * impulseDir);
                impulse_rocket.z -= (groundRes * impulseDir);
            }
            else
            {
                moveVector.z -= (airRes * impulseDir);
                impulse_rocket.z -= (airRes * impulseDir);
            }
        }
        else
        {
            impulse_rocket.z = 0;
        }

        //movement based on Air Cannon
        moveVector += impulse_airCannon * (0.2f/(transform.position.y - altContPnt));
        if (Mathf.Abs(impulse_airCannon.y) > 1.0f)
        {
            impulse_airCannon.y -= airRes;
            impulse_airCannon.y -= airRes;
        }
        else
        {
            impulse_airCannon.y = 0;
        }

        /*
        //movement based on Water Cannon
        if (watCanInUse && watLevel > 0)
        {
            impulse_watCan = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized * watCanPower;
            moveVector += impulse_watCan - new Vector3(groundRes, 0, groundRes);
        }
        */
        
        //Grappling Hook
        if (gh_isAtt)
        {
            Debug.DrawLine(transform.position, gh_contPnt, Color.white, 0.5f);
            Vector3 nextPos = transform.position + (moveVector * Time.deltaTime);
            if ((gh_contPnt - nextPos).magnitude > gh_len + 2.0f)
            {
                moveVector += (gh_contPnt - nextPos) + ((nextPos - gh_contPnt).normalized * gh_len);
            }
        }

        //Retractable Hook
        if (rh_isAtt)
        {
            Vector3 newVel = (gh_contPnt - transform.position).normalized * rh_speed;
            //Detach hook if too slow
            if (moveVector.magnitude == 0 || newVel.magnitude < lastVel){
                UpdateInfo("Detaching retractable hook");
                rh_isAtt = false;
                rope.enabled = false;
            }
            else{
                moveVector += newVel;
            }
        }

        //Adding player movement
        moveVector += pl_mvt;
        //gravity and character move
        moveVector.y -= (gravity * Time.deltaTime);
        CollisionFlags flags = controller.Move(moveVector * Time.deltaTime);
        grounded = (flags & CollisionFlags.Below) != 0;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Environment") && !grounded)//fix this if needed
        {
            moveVector = new Vector3();

            impulse_rocket = new Vector3();
            impulse_airCannon = new Vector3();
            rh_isAtt = false;
            rope.enabled = false;
        }
        if (hit.gameObject.CompareTag("Enemy") && !grounded)
        {
            impulse_rocket = new Vector3();
            impulse_airCannon = new Vector3();
            if (rh_isAtt)
            {
                rh_isAtt = false;
                rope.enabled = false;
            }
        }
        if (hit.gameObject.CompareTag("Weapon"))
        {
            Debug.Log("Collide player");
            health -= 1;
            hit.gameObject.GetComponent<AIRocket>().Deactivate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            GameObject.FindGameObjectWithTag("SF_Door").GetComponent<Animation>().Play("close");
        }

        if (other.gameObject.CompareTag("NextLevel"))
        {
            if (SceneManager.GetActiveScene().buildIndex != 3)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene(0);
            }
        }
    }

    public void SetRocketHitParams(Vector3 ePnt)
    {
        expPnt = ePnt;
        contPnt = transform.position;
        impulse_rocket += (contPnt - expPnt).normalized * rocketPower;
        impulse_rocket.y += rockDelY;

        //health -= 1/(contPnt - expPnt).magnitude;
    }

    private void SetAirCannonParams(Collider[] hit)
    {
        impulse_airCannon += new Vector3(0, rocketPower * (1.0f / 2.0f), 0);
        altContPnt = transform.position.y - 0.2f;

        foreach (Collider obj in hit)
        {
            if (obj.gameObject.CompareTag("Enemy") && Mathf.Abs(obj.transform.position.y - transform.position.y) < 1.0f && !Physics.Linecast(transform.position, obj.gameObject.transform.position))
            {
                obj.gameObject.GetComponent<AIManager>().reduceHealth(1);

                if (obj.gameObject.GetComponent<AIManager>().tankType == 2)
                {
                    obj.gameObject.GetComponent<AIManager>().isProtected = false;
                }
            }
        }
    }

    private void SetGrapHookHitParams(RaycastHit hit)
    {
        //will need different effects for enemy?
        gh_contPnt = hit.point;
        gh_len = hit.distance;
    }

    private void SetRetHookHitParams(RaycastHit hit)
    {
        gh_contPnt = hit.point;
    }

    private void UpdateInfo(string message)
    {
        if (info1.text == "")
        {
            info1.text = message;
        }
        else
        {
            if (info2.text == "")
            {
                info2.text = info1.text;
                info1.text = message;
            }
            else
            {
                info3.text = info2.text;
                info2.text = info1.text;
                info1.text = message;
            }
        }
    }
}

//Old Rocket code, Raycasting, Air Cannon Code
//Old rocket physics code
//moveVector += impulse_rocket * (0.3f / (transform.position - expPnt).magnitude);
//if (Mathf.Abs(impulse_rocket.x) > 1.0f)
//{
//    if (grounded)
//    {
//        moveVector.x += impulse_rocket.x - groundRes;
//        impulse_rocket.x -= groundRes;
//    }
//    else
//    {
//        moveVector.x += impulse_rocket.x - airRes;
//        impulse_rocket.x -= airRes;
//    }
//}
//else
//{
//    impulse_rocket.x = 0;
//}
//if (Mathf.Abs(impulse_rocket.y) > 1.0f)
//{
//    moveVector.y += impulse_rocket.y - airRes;
//    impulse_rocket.y -= ((gravity* Time.deltaTime) + airRes);
//}
//else
//{
//    impulse_rocket.y = 0;
//}
//if (Mathf.Abs(impulse_rocket.z) > 1.0f)
//{
//    if (grounded)
//    {
//        moveVector.z += impulse_rocket.z - groundRes;
//        impulse_rocket.z -= groundRes;
//    }
//    else
//    {
//        moveVector.z += impulse_rocket.z - airRes;
//        impulse_rocket.z -= airRes;
//    }
//}
//else
//{
//    impulse_rocket.z = 0;
//}

//Forward Raycast
//Ray forwardRay = new Ray(transform.position, cam.transform.forward);
//RaycastHit hit;

//if(Physics.Raycast(forwardRay, out hit, 1000))
//{
//    Debug.DrawLine(forwardRay.origin, hit.point, Color.blue, 10.0f);
//}

//Backward Raycast
//Ray backRay = new Ray(transform.position, cam.transform.forward * -1);
//RaycastHit hit;

//if (Physics.Raycast(backRay, out hit, 1000))
//{
//    Debug.DrawLine(backRay.origin, hit.point, Color.red, 10.0f);
//}

//Air Cannon vars
//private float airCanPower = 25.0f;
//private float airCanDist = 10.0f;
//private float airCanFallOff = 0.5f;
//private const float airCanReloadTime = 1.0f;
//private float airCanReload = airCanReloadTime;
//private Vector3 impulse_airCannon;

//Air Cannon Code
//if (Input.GetKeyDown(KeyCode.Alpha2))
//{
//    selectedWeapon = 1;
//    Debug.Log("Current Weapon: Air Cannon");
//}

//Primary Use
//else if (selectedWeapon == 1 && airCanReload == airCanReloadTime)
//{
//    //Ray for pushing player
//    Ray forwardRay = new Ray(transform.position, cam.transform.forward);
//    RaycastHit hit, objHit;

//    if (Physics.Raycast(forwardRay, out hit, airCanDist))
//    {
//        //might have to rework this to check player position better
//        if (hit.collider.gameObject.CompareTag("Environment") && Physics.Raycast(hit.point, hit.normal, out objHit, airCanDist))
//        {
//            if (objHit.collider.gameObject.CompareTag("Player"))
//            {
//                SetAirCannonEnvironHitParams(hit);
//            }
//        }
//    }
//
//    airCanReload -= Time.deltaTime;
//}

//Air Cannon - Alternate Use
//else if (selectedWeapon == 1 && airCanReload == airCanReloadTime)
//{
//    //AOE attack for pushing back enemies all around the character
//    Collider[] hit = Physics.OverlapSphere(transform.position, 5.0f);
//    SetAirCannonEnemyHitParams(hit);

//    airCanReload -= Time.deltaTime;
//}

//Air Cannon Reloading
//if (airCanReload <= 0.0f)
//{
//    airCanReload = airCanReloadTime;
//    Debug.Log("Reloaded Air Cannon!");
//}
//else if (airCanReload != airCanReloadTime)
//{
//    airCanReload -= Time.deltaTime;
//}
//
//private void SetAirCannonEnvironHitParams(RaycastHit hit)
//{
//    //Debug.Log("Hit environment");
//    impulse_airCannon += hit.normal.normalized * airCanPower;
//    //Debug.Log("Impulse: " + impulse_airCannon);
//}

//movement based on Air Cannons
//if (Mathf.Abs(impulse_airCannon.x) > 1.0f)
//{
//    if (grounded)
//    {
//        impulse_airCannon.x -= groundRes;
//        impulse_airCannon.x *= airCanFallOff;
//    }
//    else
//    {
//        impulse_airCannon.x -= airRes;
//        impulse_airCannon.x *= airCanFallOff;
//    }
//}
//else
//{
//    impulse_airCannon.x = 0;
//}
//if (Mathf.Abs(impulse_airCannon.y) > 1.0f)
//{
//    impulse_airCannon.y -= airRes;
//    impulse_airCannon.y *= airCanFallOff;
//}
//else
//{
//    impulse_airCannon.y = 0;
//}
//if (Mathf.Abs(impulse_airCannon.z) > 1.0f)
//{
//    if (grounded)
//    {
//        impulse_airCannon.z -= groundRes;
//        impulse_airCannon.z *= airCanFallOff;
//    }
//    else
//    {
//        impulse_airCannon.z -= airRes;
//        impulse_airCannon.z *= airCanFallOff;
//    }
//}
//else
//{
//    impulse_airCannon.z = 0;
//}
//moveVector += impulse_airCannon* (airCanFallOff);