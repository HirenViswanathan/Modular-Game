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
    public Text healthText;
    public Text weaponText;
    public Text info1;
    public Text info2;
    public Text info3;
    public GameObject gameOver;
    public float movtSpeed;
    public float jumpSpeed;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        movtSpeed = 20.0f;
        jumpSpeed = 14.0f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate(){
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
}
