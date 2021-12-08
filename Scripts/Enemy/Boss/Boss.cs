using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
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

        
    }

    // Update is called once per frame
    void Update()
    {

        if ((Mathf.Abs(transform.position.z - prince.transform.position.z) <= 4) | p_animator.GetCurrentAnimatorStateInfo(0).IsName("LossPose")) //far away so just move back and forth a few hops
        {
            //Debug.Log("attack distance");
            STATE = ATTACK;

        }
        else
        {
            //Debug.Log("far distance");
            STATE = FAR;
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

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("HitLeftArm"))
        {
            animator.SetBool("HitStart", false);
        }
        if (animator.GetAnimatorTransitionInfo(0).IsName("HitLeftArm -> Idle"))
        {
            HitDone = true; //doesn't seem to be a good idea to make transition condition to false during the transition back to idle
        }

    }


    private void FixedUpdate()
    {


    }

   
    void Attack()
    {

        if (!AttackCoRoutineStarted)
        {
            StartCoroutine(AttackingLoop());
            AttackCoRoutineStarted = true; //don't allow another coroutine to start
        }
    }
    IEnumerator AttackingLoop()
    {
        while (STATE == ATTACK)
        {
            animator.SetBool("HitStart", true);
            HitDone = false;

            while (!HitDone) //wait until attack animation is done
            {

                yield return new WaitForSeconds(0.1f);
            }

            if (STATE == ATTACK) //if not in ATTACK state then don't wait and start following prince again
            {
                yield return new WaitForSeconds(2f); //wait a little after attack is done then turn around if needed to face prince and repeat
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }

            
        }

        AttackCoRoutineStarted = false;
    }

    IEnumerator WaitUntil(float time, bool SetMove)
    {
        yield return new WaitForSeconds(time);//wait for time
        AllowMove = SetMove;
        if (SetMove == true)//if true then called this WaitUntil function from idle state. So rotate at end of time then start moving
        {
            transform.Rotate(0, 180, 0);
        }
        WaitCoRoutineStarted = false; //allow another coroutine of WaitUntil to be made
    }
   
}
