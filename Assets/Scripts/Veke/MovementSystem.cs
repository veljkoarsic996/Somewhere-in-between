using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MovementSystem : MonoBehaviour
{
    [Header("Komponente")]
    private CharacterController player;

    [Header("Movement variables")]
    private float horizontalDir, verticalDir;
    [SerializeField] private float speed;
    [SerializeField] private float isWalking;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;
    public float jumpHeight = 3f;

    [Header("Animations")]
    public Animator animController;



    public float turnSmoothTime = 0.1f;
    float turnSmoothVel;

    [Header("Camera")]
    public Transform camera;

    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        player = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalDir = getInput().x;
        verticalDir = getInput().z;
        Move();
        animationControll();

    }

    private void animationControll() {
        animController.SetBool("isGrounded" , isGrounded);
        animController.SetFloat("isWalking" , isWalking);
    }
    public Vector3 getInput() {
        //Movement Variables
        return new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical")).normalized;
    }

    private void Move() {
        isGrounded = Physics.CheckSphere(groundCheck.position ,groundDistance , groundMask);

        isWalking = getInput().magnitude;
        print(isWalking);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (getInput().magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(getInput().x , getInput().z) * Mathf.Rad2Deg + camera.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel , turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f,targetAngle,0f) * Vector3.forward;
            player.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        player.Move(velocity * Time.deltaTime);


        print(isGrounded);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }
}
