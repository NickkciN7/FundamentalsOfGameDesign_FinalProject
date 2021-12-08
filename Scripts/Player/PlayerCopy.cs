using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCopy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Player : MonoBehaviour
//{

//    //serializeField and now we can see in unity at script component in Inspector window
//    //use speed variable because character was moving a bit slow
//    [SerializeField] private float speed = 2f;
//    [SerializeField] private LayerMask playerMask;
//    [SerializeField] private Transform GroundCheckTransform = null;
//    //[SerializeField] private float turnSpeed = 45f; //rotates too slow without this multiplier

//    private Animator animator;
//    private bool jumpKeyWasPressed = false;
//    private Rigidbody rigidBodyComponent;
//    //const string POSITIVEZ
//    //private directionFacing = 
//    // Start is called before the first frame update
//    void Start()
//    {
//        animator = GetComponent<Animator>();//get access to the Animator tied to the player
//        rigidBodyComponent = GetComponent<Rigidbody>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //if(startHitAnimation == true)
//        //{
//        //    animator.GetBool("HitStart");
//        //    animator.SetBool("HitStart", false);
//        //    startHitAnimation = false;
//        //}
//        //if (Input.GetKeyDown(KeyCode.Space))
//        //{
//        //    animator.SetBool("HitStart", true);
//        //    startHitAnimation = true;
//        //}
//        if (animator.GetBool("HitStart") == true)
//        {
//            animator.SetBool("HitStart", false); //set HitStart to false frame after Hit animation starts so that after finishing animation it will transition back to idle
//        }
//        if (Input.GetKeyDown(KeyCode.H))
//        {
//            animator.SetBool("HitStart", true);
//        }

//        //forward is positive Z, which is why it was important to have character facing that direction
//        //Time.deltaTime to make it frame rate independent
//        //vertical axis is 'w' and 's' keys by default
//        var velocity = Input.GetAxis("Horizontal") * transform.forward * speed;
//        //when Time.deltaTime was above instead, the "Speed" parameter would be getting really small value, so only have it the line below in Translate

//        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")) //don't move if character is hitting
//        {
//            if (Input.GetKeyDown(KeyCode.Space))
//            {
//                jumpKeyWasPressed = true;
//            }
//            //Debug.Log("x: " + transform.forward.x);
//            //does run animation even when turning. need to fix that and instead make it do a turn animation which I need to make
//            //were having slight rotations so checking if equal to Vector3.forward no longer works when rigid body added. just check if z is positive
//            if (Input.GetAxis("Horizontal") < 0 & (transform.forward.z > 0))//clicked negative button and character facing forward
//            {
//                turnPlayer(); //rotate player 180 degrees
//            }
//            else if (Input.GetAxis("Horizontal") > 0 & (transform.forward.z < 0))//clicked positive button and character facing backward
//            {
//                turnPlayer();
//            }
//            else
//            {
//                transform.Translate(velocity * Time.deltaTime); //visual studios tip: can drag highlighted code. like cutting and pasting
//            }
//            //set the "Speed" parameter made in unity to velocity.magnitude(not the float "speed" above)
//            //changed .magnitude to .z so that can have different value when moving backwards
//            animator.SetFloat("Speed", velocity.z);
//        }



//    }

//    void FixedUpdate()
//    {
//        if (jumpKeyWasPressed)
//        {
//            jumpKeyWasPressed = false;
//            //Debug.Log("Overlap Length: " + Physics.OverlapSphere(GroundCheckTransform.position, 0.1f, playerMask).Length);
//            if (Physics.OverlapSphere(GroundCheckTransform.position, 0.1f, playerMask).Length == 0)
//            {
//                return;
//            }
//            //increase jump for each coin collected
//            float jumpPower = 5f;
//            rigidBodyComponent.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);

//        }
//    }


//    void turnPlayer()
//    {
//        var camera = transform.Find("Main Camera");
//        camera.SetParent(null); //temporarily remove character as parent object of camera so line below doesn't rotate camera too
//        transform.Rotate(0, 180, 0); //rotate 180 degrees along y axis(y points up)
//        camera.SetParent(transform);
//    }

//}
