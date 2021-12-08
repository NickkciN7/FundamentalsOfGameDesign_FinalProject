using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidMonster : MonoBehaviour
{
    GameObject prince;
    private Animator p_animator;

    private Rigidbody rigidBodyComponent;



    //[SerializeField] private LayerMask HumanoidMonsterMask;

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

    private bool WaitCoRoutineStarted = false;
    private bool AttackCoRoutineStarted = false;
    private bool AllowMove = true;
    private bool HitDone = true;
    private float speed = 2f;


    private AudioSource Audio_AxeSwing;


    // Start is called before the first frame update
    void Start()
    {


        prince = GameObject.Find("prince");
        p_animator = prince.GetComponent<Animator>();

        rigidBodyComponent = GetComponent<Rigidbody>();

        STATE = FAR;
        //StartCoroutine(MoveFar());
        FarCoRoutineDone = false;

        animator = GetComponent<Animator>();

        Audio_AxeSwing = transform.Find("Audio").Find("AxeSwing").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if ( (Mathf.Abs(transform.position.z - prince.transform.position.z) >= 8) | p_animator.GetCurrentAnimatorStateInfo(0).IsName("LossPose") ) //far away so just move back and forth
        {
            if(STATE != FAR)
            {
                AllowMove = true;
            }
            STATE = FAR;
        }
        else
        {
            if (Mathf.Abs(transform.position.z - prince.transform.position.z) <= 1.25) //really close
            {
                STATE = ATTACK;
            }
            else
            {
                if (STATE != CLOSE)
                {
                    AllowMove = false;
                }
                STATE = CLOSE;
            }

        }

        
        if(STATE == FAR)
        {
            MoveFar();
        }
        if(STATE == CLOSE)
        {
            MoveClose();
        }
        if (STATE == ATTACK)
        {
            Attack();
        }
        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("Shake"))
        //{
        //    animator.SetBool("StartShake", false);//so Default pose won't go back to Shake as soon as Shake exits
        //    IsShaking = true;
        //}

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            animator.SetBool("HitStart", false);
        }
        if (animator.GetAnimatorTransitionInfo(0).IsName("Hit -> Idle"))
        {
            HitDone = true; //doesn't seem to be a good idea to make transition condition to false during the transition back to idle
        }

    }


    private void FixedUpdate()
    {
      

    }

    void MoveClose()
    {
        //if(WaitCoRoutineStarted == true | AttackCoRoutineStarted == true) //dont do below until a Coroutine in progress ends
        //{
        //    return;
        //}

        //above is original, uncomment it if this doesn't work
        //I think this solved this issue. Attempts to explain issue and solution:
        //before whenever monster was running far, then suddenly was in close range it would still do the slow run animation but not move
        //MoveFar moved the monster when AllowMove was true, AllowMove was set to false after the WaitUntil coroutine waits. When false, the RunSlowStart animator
        //parameter is finally set to false. Because the state was changed from far to close, the MoveClose method was no longer called and so the RunSlowStart couldn't
        //be set to false, and also the translate couldnt be called. In this MoveClose method, the translate couldnt be used and the RunSlowStart couldnt be set false
        //also because the WaitCoRoutineStarted hadn't been set to false at the end of the WaitUntil Coroutine(called in MoveFar). So instead of waiting for WaitUntil to
        //finish, just go ahead and let this monster start moving fast immediately

        if (AttackCoRoutineStarted == true) 
        {
            return;
        }
        animator.SetBool("RunSlowStart", false);
        animator.SetBool("RunFastStart", true);

        speed = 2.5f;
        
        if ((transform.position.z - prince.transform.position.z) > 0) //monster to right of prince
        {
            if (transform.forward.z > 0) //if to right of prince but facing right(away from prince) turn towards prince
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

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void MoveFar()
    {
        animator.SetBool("RunFastStart", false);
        speed = 1.5f;
        if (AllowMove)
        {
            if (!WaitCoRoutineStarted)
            {
                StartCoroutine(WaitUntil(2.0f, false)); //AllowMove is true until 2.0f seconds are over
                WaitCoRoutineStarted = true; //don't allow another coroutine to start
            }
            //set bool parameter to let slow run animation start
            animator.SetBool("RunSlowStart", true);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("RunSlowStart", false);
            if (!WaitCoRoutineStarted)
            {
                StartCoroutine(WaitUntil(2.0f, true));//do nothing for 2 seconds, then set AllowMove to true so monster will move again
                WaitCoRoutineStarted = true;
            }
            
        }
    }

    void Attack()
    {

        //if (WaitCoRoutineStarted == true) //dont do below until a Coroutine in progress ends. Probably never happens in attack since no coroutine started in MoveClose. Maybe if character fell from higher platform?
        //{
        //    return;
        //}

        animator.SetBool("RunFastStart", false);
        animator.SetBool("RunSlowStart", false);
        
        if (!AttackCoRoutineStarted)
        {
            StartCoroutine(AttackingLoop());
            AttackCoRoutineStarted = true; //don't allow another coroutine to start
        }
    }
    IEnumerator AttackingLoop()
    {
        while(STATE == ATTACK)
        {
            animator.SetBool("HitStart", true);
            HitDone = false;
            StartCoroutine(WaitToPlayAudio());
            while (!HitDone) //wait until attack animation is done
            {
                
                yield return new WaitForSeconds(0.1f);
            }

            if(STATE == ATTACK) //if not in ATTACK state then don't wait and start following prince again
            {
                yield return new WaitForSeconds(2f); //wait a little after attack is done then turn around if needed to face prince and repeat
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
          
            if ((transform.position.z - prince.transform.position.z) > 0) //monster to right of prince
            {
                if (transform.forward.z > 0) //if to right of prince but facing right(away from prince) turn towards prince
                {
                    Debug.Log("rotate in AttackingLoop");
                    transform.Rotate(0, 180, 0);
                }
            }
            else
            {
                if (transform.forward.z < 0)
                {
                    Debug.Log("rotate in AttackingLoop");
                    transform.Rotate(0, 180, 0);
                }
            }
        }

        AttackCoRoutineStarted = false;
    }

    IEnumerator WaitUntil(float time, bool SetMove)
    {
        yield return new WaitForSeconds(time);//wait for time
        AllowMove = SetMove;
        if(SetMove == true & STATE == FAR)//if true then called this WaitUntil function from idle state. So rotate at end of time then start moving
        {
            //STATE must == FAR so this doesn't rotate in middle of attack sequence or when running towards prince in CLOSE state
            //Debug.Log("rotate in WaitUntil"); 
            transform.Rotate(0, 180, 0);
        }
        WaitCoRoutineStarted = false; //allow another coroutine of WaitUntil to be made
    }

    IEnumerator WaitToPlayAudio()
    {
        yield return new WaitForSeconds(1.2f);//wait for time
        Audio_AxeSwing.PlayOneShot(Audio_AxeSwing.clip);
    }



    //IEnumerator MoveClose()
    //{
    //    //while (!FarCoRoutineDone)
    //    //{
    //    //    yield return new WaitForSeconds(0.2f);
    //    //}

    //    while (STATE == CLOSE)
    //    {
    //        jumpPower = 3.5f;
    //        if (IsSquishing)
    //        {
    //            yield return new WaitForSeconds(0.2f);
    //            continue;
    //            Debug.Log("getanimation");
    //        }
    //        else
    //        {
    //            if ((transform.position.z - prince.transform.position.z) > 0)
    //            {
    //                if (transform.forward.z > 0)
    //                {
    //                    transform.Rotate(0, 180, 0);
    //                }
    //            }
    //            else
    //            {
    //                if (transform.forward.z < 0)
    //                {
    //                    transform.Rotate(0, 180, 0);
    //                }
    //            }


    //            StartJump = true;

    //            yield return new WaitForSeconds(1.0f);
    //        }

    //    }
    //    if (STATE == FAR)
    //    {
    //        StartCoroutine(MoveFar());
    //    }
    //    if (STATE == ATTACK)
    //    {
    //        StartCoroutine(Attack());
    //    }
    //}
    //IEnumerator MoveFar()
    //{
    //    //while (!CloseCoRoutineDone)
    //    //{
    //    //    yield return new WaitForSeconds(0.2f);
    //    //}
    //    jumpPower = 2f;
    //    while (STATE == FAR)
    //    {
    //        if (IsSquishing)
    //        {
    //            yield return new WaitForSeconds(0.2f);
    //            continue;
    //            Debug.Log("getanimation");
    //        }
    //        else
    //        {
    //            if (hops > 0)
    //            {

    //                //Debug.Log("z: " + transform.forward.z);

    //                StartJump = true;
    //                hops--;
    //            }
    //            else
    //            {
    //                transform.Rotate(0, 180, 0);
    //                hops = 3;
    //            }

    //            yield return new WaitForSeconds(1.5f);
    //        }

    //    }

    //    if (STATE == CLOSE)
    //    {
    //        StartCoroutine(MoveClose());
    //    }
    //    if (STATE == ATTACK)
    //    {
    //        StartCoroutine(Attack());
    //    }
    //}

    //IEnumerator Attack()
    //{
    //    while (STATE == ATTACK)
    //    {
    //        yield return new WaitForSeconds(1f);
    //        animator.SetBool("StartShake", true);
    //        IsShaking = true;
    //        while (IsShaking)
    //        {
    //            yield return new WaitForSeconds(0.2f);
    //        }
    //        transform.Find("SpikeLarge").gameObject.SetActive(true);
    //        transform.Find("SpikeSmall").gameObject.SetActive(false);
    //        yield return new WaitForSeconds(1.5f);
    //        animator.SetBool("StartShake", true);
    //        IsShaking = true;
    //        while (IsShaking)
    //        {
    //            yield return new WaitForSeconds(0.2f);
    //        }
    //        transform.Find("SpikeSmall").gameObject.SetActive(true);
    //        transform.Find("SpikeLarge").gameObject.SetActive(false);
    //        yield return new WaitForSeconds(1f);
    //    }

    //    if (STATE == CLOSE)
    //    {
    //        StartCoroutine(MoveClose());
    //    }
    //    if (STATE == FAR)
    //    {
    //        StartCoroutine(MoveFar());
    //    }

    //    //FarCoRoutineDone = true;
    //    //StartCoroutine(HopAlongClose());
    //}
}
