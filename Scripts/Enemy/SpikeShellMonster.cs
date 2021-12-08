using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeShellMonster : MonoBehaviour
{
    GameObject prince;
    private Animator p_animator;
    Vector3 JumpRight = new Vector3(0, 1, 1);
    Vector3 JumpLeft = new Vector3(0, 1, -1);
    private float jumpPower = 2f;
    private int hops = 3;
    private Rigidbody rigidBodyComponent;
    

    private bool StartJump = false;
    private bool DuringJump = false;
    [SerializeField] private Transform GroundCheckTransform = null;
    [SerializeField] private LayerMask SpikeShellMonsterMask;

    private Animator animator;
    //seems I cant directly use animator.GetCurrentAnimatorStateInfo(0) in coroutines? so instead change this variable in update 
    private bool IsSquishing = false;
    private bool IsShaking = false;

    //for if character is far or close to enemy
    private int STATE;
    private int CLOSE = 0;
    private int FAR = 1;
    private int ATTACK = 2;
    
    //so that when switching from far to close or close to far, it won't do any other jumps in the new state til the final jump in the old state is done. prevents double jumps
    private bool FarCoRoutineDone = true;
    private bool CloseCoRoutineDone = true;

    private AudioSource Audio_Landing;
    bool AllowLandingSound = true;

    private AudioSource Audio_SpikesOut;
    bool AllowSpikeSound = true;

    // Start is called before the first frame update
    void Start()
    {
        

        prince = GameObject.Find("prince");
        p_animator = prince.GetComponent<Animator>();
        rigidBodyComponent = GetComponent<Rigidbody>();

        STATE = FAR;
        StartCoroutine(HopAlongFar());
        FarCoRoutineDone = false;

        animator = GetComponent<Animator>();


        Audio_Landing = transform.Find("Audio").Find("Landing").GetComponent<AudioSource>();
        Audio_SpikesOut = transform.Find("Audio").Find("SpikesOut").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if ( (Mathf.Abs(transform.position.z - prince.transform.position.z) >= 8) | p_animator.GetCurrentAnimatorStateInfo(0).IsName("LossPose")) //far away so just move back and forth a few hops
        {
            STATE = FAR;
            //FarCoRoutineDone = false;
            
        }
        else
        {
            if(Mathf.Abs(transform.position.z - prince.transform.position.z) <= 1.25) //really close
            {
                STATE = ATTACK;
            }
            else
            {
                STATE = CLOSE;
            }
            
            //if(STATE == FAR)
            //{
            //    STATE = CLOSE;
            //    //CloseCoRoutineDone = false;
               
            //}
            
            //FarFromCharacter();
            //CloseToCharacter();
        }

        if (Physics.OverlapSphere(GroundCheckTransform.position, 0.1f, SpikeShellMonsterMask).Length == 0)
        {
            AllowLandingSound = true;
            DuringJump = true;
        }
        if (Physics.OverlapSphere(GroundCheckTransform.position, 0.1f, SpikeShellMonsterMask).Length != 0 & DuringJump) //touching ground after jumping
        {
            DuringJump = false;
            animator.SetBool("StartSquish", true);
            if (AllowLandingSound)
            {
                if (Mathf.Abs(transform.position.z - prince.transform.position.z) <= 20)
                {
                    Audio_Landing.PlayOneShot(Audio_Landing.clip);
                }
                
                AllowLandingSound = false;
            }
            
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Squish"))
        {
            animator.SetBool("StartSquish", false);//so Default pose won't go back to Squish as soon as Squish exits
            IsSquishing = true;
        }
        else
        {
            IsSquishing = false;
        }


        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Shake"))
        {
            animator.SetBool("StartShake", false);//so Default pose won't go back to Shake as soon as Shake exits
            IsShaking = true;
        }
        if (animator.GetAnimatorTransitionInfo(0).IsName("Shake -> 0NormalPose"))
        {
            IsShaking = false;
        }

    }


    private void FixedUpdate()
    {
        if (StartJump)
        {
            if (transform.forward.z < 0)
            {

                rigidBodyComponent.AddForce(JumpLeft * jumpPower, ForceMode.VelocityChange);
            }
            else
            {
                rigidBodyComponent.AddForce(JumpRight * jumpPower, ForceMode.VelocityChange);
            }
            StartJump = false;
        }
        
    }

    //void FarFromCharacter()
    //{
    //    if(hops > 0)
    //    {
    //        if(transform.forward.z < 0)
    //        {
    //            rigidBodyComponent.AddForce(-JumpForward * jumpPower, ForceMode.VelocityChange);
    //        }
    //        else
    //        {
    //            rigidBodyComponent.AddForce(JumpForward * jumpPower, ForceMode.VelocityChange);
    //        }
            
    //        hops--;
    //    }
    //    else
    //    {
    //        transform.Rotate(0, 180, 0);
    //    }
    //}

    //void CloseToCharacter()
    //{
    //    if ((transform.position.z - prince.transform.position.z) > 0)
    //    {
    //        //rigidBodyComponent.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
    //    }
    //}

    IEnumerator HopAlongClose()
    {
        //while (!FarCoRoutineDone)
        //{
        //    yield return new WaitForSeconds(0.2f);
        //}

        while (STATE == CLOSE)
        {
            jumpPower = 3.5f;
            if (IsSquishing)
            {
                yield return new WaitForSeconds(0.2f);
                continue;
                Debug.Log("getanimation");
            }
            else
            {
                if( (transform.position.z - prince.transform.position.z) > 0){
                    if (transform.forward.z > 0)
                    {
                        transform.Rotate(0, 180, 0);
                    }
                }
                else
                {
                    if (transform.forward.z < 0)
                    {
                        transform.Rotate(0, 180, 0);
                    }
                }


                StartJump = true;

                yield return new WaitForSeconds(1.0f);
            }
            
        }
        if(STATE == FAR)
        {
            StartCoroutine(HopAlongFar());
        }
        if (STATE == ATTACK)
        {
            StartCoroutine(Attack());
        }
    }
    IEnumerator HopAlongFar()
    {
        //while (!CloseCoRoutineDone)
        //{
        //    yield return new WaitForSeconds(0.2f);
        //}
        jumpPower = 2f;
        while (STATE == FAR)
        {
            if (IsSquishing)
            {
                yield return new WaitForSeconds(0.2f);
                continue;
                Debug.Log("getanimation");
            }
            else
            {
                if (hops > 0)
                {

                    //Debug.Log("z: " + transform.forward.z);

                    StartJump = true;
                    hops--;
                }
                else
                {
                    transform.Rotate(0, 180, 0);
                    hops = 3;
                }

                yield return new WaitForSeconds(1.5f);
            }

        }

        if (STATE == CLOSE)
        {
            StartCoroutine(HopAlongClose());
        }
        if (STATE == ATTACK)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        while(STATE == ATTACK)
        {
            yield return new WaitForSeconds(1f);
            animator.SetBool("StartShake", true);
            IsShaking = true;
            while (IsShaking)
            {
                yield return new WaitForSeconds(0.2f);
            }
            transform.Find("SpikeLarge").gameObject.SetActive(true);
            transform.Find("SpikeSmall").gameObject.SetActive(false);
            Audio_SpikesOut.PlayOneShot(Audio_SpikesOut.clip);
            yield return new WaitForSeconds(1.5f);
            animator.SetBool("StartShake", true);
            IsShaking = true;
            while (IsShaking)
            {
                yield return new WaitForSeconds(0.2f);
            }
            transform.Find("SpikeSmall").gameObject.SetActive(true);
            transform.Find("SpikeLarge").gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
        }

        if (STATE == CLOSE)
        {
            StartCoroutine(HopAlongClose());
        }
        if (STATE == FAR)
        {
            StartCoroutine(HopAlongFar());
        }

        //FarCoRoutineDone = true;
        //StartCoroutine(HopAlongClose());
    }
}
