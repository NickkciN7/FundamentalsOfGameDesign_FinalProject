using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    //serializeField and now we can see in unity at script component in Inspector window
    //use speed variable because character was moving a bit slow
    private float speed = 4.5f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform GroundCheckTransform = null;
    [SerializeField] private Transform FrontGroundCheckTransform = null;
    private GameObject sword;
    private bool firstHitDownSwing = false;
    //[SerializeField] private float turnSpeed = 45f; //rotates too slow without this multiplier
    
    private Animator animator;
    private bool jumpKeyWasPressed = false;
    private int JumpsLeft = 2;
    private bool dodgeLeft = false;
    private bool dodgeRight = false;
    private int dodgeState; //made this because when trying to set dodgeLeft and dodgeRight back to false when the animation wasn't Dodge seemed to set them to false sometimes right before actually transitioning to Dodge. So with these states I can make sure Dodge was actually entered first then exited THEN i can reset dodgeLeft and Right to false
        private int NEVERSET = 0;
        private int START = 1;
        private int DONE = 2;
    private Rigidbody rigidBodyComponent;

    public Bar HealthBar;
    public int maxHealth = 100;
    public int currentHealth;
   
    public Bar StaminaBar;
    public int maxStamina = 100;
    public int currentStamina;

    private bool allowStaminaDecrease1 = true; //for sword hit 1
    private bool allowStaminaDecrease2 = true; //for sword hit 2
    private bool allowStaminaDecrease3 = true; //for sword hit 3

    private bool AllowRegeneration = true;
    private bool CanUseStamina = true;


    public GameObject IntroText;
    public GameObject InstructionsText;
    public GameObject BrotherDialogue;
    public bool TextUIOnScreen = true;
    public bool GameFinished = false;

    public GameObject GameOverText;



    private AudioSource Audio_SwordSwing;
    bool Swing1 = true;
    bool Swing2 = true;
    bool Swing3 = true;

    private AudioSource Audio_Jump;

    private AudioSource Audio_Dodge;
    bool AllowDodgeSound = true;

    private AudioSource Audio_Lost;
    bool AllowLostSound = true;
    //const string POSITIVEZ
    //private directionFacing = 
    // Start is called before the first frame update
    void Start()
    {

        currentHealth = maxHealth;
        HealthBar.SetMaxValue(maxHealth);

        currentStamina = maxStamina;
        StaminaBar.SetMaxValue(maxStamina);


        animator = GetComponent<Animator>();//get access to the Animator tied to the player
        rigidBodyComponent = GetComponent<Rigidbody>();
        sword = GameObject.Find("SwordCopy");
        dodgeState = NEVERSET;

        StartCoroutine(RegenerateStamina());



        Audio_SwordSwing = GameObject.Find("SwordSwing").GetComponent<AudioSource>();
        Audio_Jump = GameObject.Find("Jump").GetComponent<AudioSource>();
        Audio_Dodge = GameObject.Find("Dodge").GetComponent<AudioSource>();
        Audio_Lost = GameObject.Find("Lost").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    GameObject.Find("SwordSwing").GetComponent<AudioSource>().Play();
        //}

        if (GameFinished)
        {
            animator.SetFloat("Speed", 0);
            return;
        }
        if (TextUIOnScreen)
        {
            animator.SetFloat("Speed", 0); //so run animation will end if TextUI appears while runnning
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (InstructionsText.activeSelf == true)
                {
                    InstructionsText.SetActive(false);
                    TextUIOnScreen = false;
                }
                if (IntroText.activeSelf == true)
                {
                    IntroText.SetActive(false);
                    InstructionsText.SetActive(true);
                }
                if (BrotherDialogue.activeSelf == true)
                {
                    BrotherDialogue.SetActive(false);
                    TextUIOnScreen = false;
                }
                
            }
            return;
        }


       
        UnityEngine.Collider[] a = Physics.OverlapSphere(GroundCheckTransform.position, 0.1f, playerMask);
        if (a.Length > 0)
        {
            //for(int i = 0; i < a.Length; i++)
            //{
            //    if (a[0].gameObject.name.Equals("Grass"))
            //    {
            //        if (a[0].gameObject.transform.parent.gameObject.GetComponent<MovingVertical>() != null | a[0].gameObject.transform.parent.gameObject.GetComponent<MovingHorizontal>() != null)
            //        {
            //            //Debug.Log("Has movement script");
            //            transform.SetParent(a[0].gameObject.transform.parent.transform); //set parent to moving block so character moves with the block
            //            break;
            //        }
            //        else
            //        {
            //            //Debug.Log("set null");
            //            transform.SetParent(null);
            //        }

            //    }
            //}
            bool IsMoving = false;
            GameObject currentObj = a[0].gameObject;
            while(currentObj.transform.parent != null)
            {
                if (currentObj.gameObject.name.Equals("Moving"))
                {
                    IsMoving = true;
                    break;
                }
                currentObj = currentObj.transform.parent.gameObject;
            }
            if (IsMoving)
            {
                transform.SetParent(a[0].transform);
            }
            else
            {
                transform.SetParent(null);
            }
            //if(a[0].gameObject.layer == 9 & !a[0].gameObject.name.Equals("ground(broken)")) //ground layer
            //{
            //    transform.SetParent(a[0].transform);
            //}
            //else
            //{
            //    transform.SetParent(null);
            //}
        }


        if ( HealthBar.GetComponent<Slider>().value == 0)
        {
            if (AllowLostSound)
            {
                AllowLostSound = false;
                Audio_Lost.PlayOneShot(Audio_Lost.clip);
            }
            

            animator.SetBool("LostGame", true);
            Physics.IgnoreLayerCollision(6, 10, true); //enemies can walk/hop through where prince is
            Physics.IgnoreLayerCollision(6, 11, true);
            GameOverText.SetActive(true);
            return;
        }
        if(currentStamina == 0 & CanUseStamina)
        {
            CanUseStamina = false;
            StartCoroutine(RegenerateColorChange()); 
        }
        if(!CanUseStamina &  currentStamina >= 50) //don't allow hitting or dodging until stamina regenerates to 50 or greater
        {
            StaminaBar.fill.color = StaminaBar.gradient.Evaluate(0f);
            CanUseStamina = true;
        }
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    ChangeHealth(20);
        //}
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    ChangeStamina(20);
        //}

        //if(!duringHit() & !animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge")){

        //}

        //Debug.Log("character z: " + transform.position.z);
        //if(sword.transform.GetChild(0).transform.position.z < .5)
        //{
        //    Debug.Log("character z - sword tip z: " + Mathf.Abs(transform.position.z - sword.transform.GetChild(0).transform.position.z));
        //}

        //sword tip comes up before swinging down. Don't want to hurt enemy on up swing of first hit. So since tip'z global z value will be very close
        //to the main character's z value at some point on the up swing, the absolute value of the difference will be very small(less than 0.05 as I 
        //confirmed with Debug.log.
        //make sure in hit1 animation so that at very beginning from Tpose to idle it won't set firstHitDownSwing or any other time when running or something


        
        if ( Mathf.Abs(transform.position.z - sword.transform.GetChild(0).transform.position.z) < .05 & animator.GetCurrentAnimatorStateInfo(0).IsName("Hit1Begin") )
        {
            //Debug.Log("down bool set");
            firstHitDownSwing = true;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit1and3End") || animator.GetCurrentAnimatorStateInfo(0).IsName("Hit2End"))
        {
            animator.SetBool("HitStart1", false);
            animator.SetBool("HitStart2", false);
            animator.SetBool("HitStart3", false);
            firstHitDownSwing = false;
        }
        if (Input.GetKeyDown(KeyCode.H) & CanUseStamina & !animator.IsInTransition(0))
        {
            
            if (animator.GetBool("HitStart2") == true & animator.GetCurrentAnimatorStateInfo(0).IsName("Hit2Begin"))
            {
                
                animator.SetBool("HitStart3", true);
            }
            if (animator.GetBool("HitStart1") == true & animator.GetCurrentAnimatorStateInfo(0).IsName("Hit1Begin")) //if already in Hit1 and h pressed again then set HitStart2 to true allowing Hit2Begin to start. 
            {
                
                animator.SetBool("HitStart2", true);   //above SetBool below so that won't set 2 to true as soon as 1 is set to true
            }
           
            animator.SetBool("HitStart1", true);
        }
        //UNCOMMENT BELOW TO DECREASE STAMINA FROM SWORD HIT
        SwordStaminaDecrement();
        
        



        //roll dodge
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
        {
            //Physics.IgnoreLayerCollision(6, 7, true); //allow character to dodge through enemies
            if(dodgeState == NEVERSET)
            {
                ChangeStamina(-20);
            }
            dodgeState = START; //Means the Dodge animation has been entered
            //Debug.Log("dodgeLeft: " + dodgeLeft);
            //Debug.Log("dodgeRight: " + dodgeRight);
            Vector3 dir = new Vector3(0, 0, 0);
            if (dodgeLeft)
            {
                dir = Vector3.forward; //NOT LEFT OR RIGHT BELOW. left and right are along x axis. Also translate seems to move character in the forward direction OF THE CHARACTER not globally so both left and right should be Vector3.forward
            }
            if (dodgeRight)
            {
                dir = Vector3.forward;
            }

            if (Physics.OverlapSphere(FrontGroundCheckTransform.position, 0.15f, playerMask).Length == 0) //dont try to dodge into wall
            {
                if (AllowDodgeSound)
                {
                    AllowDodgeSound = false;
                    Audio_Dodge.PlayOneShot(Audio_Dodge.clip);
                }
                
                transform.Translate(dir * Time.deltaTime * 10f); //using translate instead to avoid issues with collision using rigidBodyComponent.AddForce
            }
            
            
        }
        else
        {
            //Physics.IgnoreLayerCollision(6, 7, false);
        }
        if(dodgeState == START & !animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge")) //the Dodge animation has started and ended
        {

            dodgeState = NEVERSET;
            dodgeLeft = false;
            dodgeRight = false;
        }

        if (animator.GetBool("DodgeStart") == true) 
        {
            animator.SetBool("DodgeStart", false);//set back to false on the frame after set to true so that other animations won't transition back to Dodge without click keys below again 
        }
        if (Input.GetKey(KeyCode.J) & !animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge") & !duringHitBegin() & CanUseStamina)
        {
            //Debug.Log("Pressed V");
            if (Input.GetAxis("Horizontal") < 0 & (transform.forward.z > 0))//clicked negative button and character facing forward
            {
                turnPlayer(); //rotate player 180 degrees
            }
            else if (Input.GetAxis("Horizontal") > 0 & (transform.forward.z < 0))//clicked positive button and character facing backward
            {
                turnPlayer();
            }
            
            //player rotated so now translate quick
            if(Input.GetAxis("Horizontal") < 0)
            {
                dodgeLeft = true;
                animator.SetBool("DodgeStart", true);
            }
            else if (Input.GetAxis("Horizontal") > 0)
            {
                dodgeRight = true;
                animator.SetBool("DodgeStart", true);
            }
            if(dodgeLeft | dodgeRight)
            {
                //transform.GetChild(1).GetComponent<MeshCollider>().enabled = false;
                //gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX;
                //gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                
            }

        }
        


        //forward is positive Z, which is why it was important to have character facing that direction
        //Time.deltaTime to make it frame rate independent
        //vertical axis is 'w' and 's' keys by default



        if (!duringHit() & !animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge") & animator.GetBool("DodgeStart") == false) //don't move like below or jump if character is dodging or hitting. Last condition for the 1st and only frame DodgeStart is set. This way the Speed parameter can't be set below which makes the animator go to Run
        {
            var velocity = Input.GetAxis("Horizontal") * transform.forward * speed;
            //when Time.deltaTime was above instead, the "Speed" parameter would be getting really small value, so only have it the line below in Translate
            if (Physics.OverlapSphere(GroundCheckTransform.position, 0.1f, playerMask).Length != 0) //touching ground so reset JumpsLeft
            {
                JumpsLeft = 2;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpKeyWasPressed = true;
            }
            //Debug.Log("x: " + transform.forward.x);
            //does run animation even when turning. need to fix that and instead make it do a turn animation which I need to make
            //were having slight rotations so checking if equal to Vector3.forward no longer works when rigid body added. just check if z is positive
            if (Input.GetAxis("Horizontal") < 0 & (transform.forward.z > 0))//clicked negative button and character facing forward
            {
                turnPlayer(); //rotate player 180 degrees
            }
            else if (Input.GetAxis("Horizontal") > 0 & (transform.forward.z < 0) )//clicked positive button and character facing backward
            {
                turnPlayer();
            }
            else
            {
                if(Physics.OverlapSphere(FrontGroundCheckTransform.position, 0.15f, playerMask).Length == 0) //dont try to run into wall(even though colliders prevent character from going too far without this, still causes issues like being able to run into wall and stay suspended in air)
                {
                    transform.Translate(velocity * Time.deltaTime); //visual studios tip: can drag highlighted code. like cutting and pasting
                }
                    
            }
            //set the "Speed" parameter made in unity to velocity.magnitude(not the float "speed" above)
            //changed .magnitude to .z so that can have different value when moving backwards
            animator.SetFloat("Speed", velocity.z);
        }




        //for sword swing audio
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            Swing1 = true;
            Swing2 = true;
            Swing3 = true;

            AllowDodgeSound = true;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit1Begin") & Swing1)
        {
            Swing1 = false;
            Audio_SwordSwing.PlayOneShot(Audio_SwordSwing.clip);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit2Begin") & Swing2)
        {
            Swing2 = false;
            Audio_SwordSwing.PlayOneShot(Audio_SwordSwing.clip);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit3Begin") & Swing3)
        {
            Swing3 = false;
            Audio_SwordSwing.PlayOneShot(Audio_SwordSwing.clip);
        }
    }

    void FixedUpdate()
    {
        if (jumpKeyWasPressed)
        {
            jumpKeyWasPressed = false;
            //Debug.Log("Overlap Length: " + Physics.OverlapSphere(GroundCheckTransform.position, 0.1f, playerMask).Length);
            //Debug.Log(Physics.OverlapSphere(GroundCheckTransform.position, 0.1f, playerMask).ToString());
            if (Physics.OverlapSphere(GroundCheckTransform.position, 0.1f, playerMask).Length == 0 & JumpsLeft == 0)
            {
                return;
            }
            //if ( Physics.OverlapSphere(GroundCheckTransform.position, 0.1f, playerMask).Length != 0 &&
            //     Physics.OverlapSphere(FrontGroundCheckTransform.position, 0.1f, playerMask).Length != 0 && 
            //     Physics.OverlapSphere(GroundCheckTransform.position, 0.1f, playerMask)[0] == Physics.OverlapSphere(FrontGroundCheckTransform.position, 0.1f, playerMask)[0])
            //{
            //    return;
            //}
            //increase jump for each coin collected

            float jumpPower = 7f;
            if (JumpsLeft == 1)
            {
               // Debug.Log("pressed space again");

                jumpPower = 5f;
     
            }
            transform.SetParent(null);
            rigidBodyComponent.velocity = Vector3.zero;//so velocity from previous jump won't add up for a really big jump, also so that when falling down, the 2nd jump will still 
                                                       //move the character up as if jumping from the ground at that position in air, otherwise the downward velocity cancels out most
                                                       //of the upward velocity added. And I put it here instead because if on a moving platform even the first jump will add to the moving velocity of 
                                                       //the moving platform
            Audio_Jump.PlayOneShot(Audio_Jump.clip);
            rigidBodyComponent.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            JumpsLeft--;
        }

      
    }


    void turnPlayer()
    {
        var camera = GameObject.Find("Main Camera").transform;
        if(camera.parent != null)
        {
            camera.SetParent(null); //temporarily remove character as parent object of camera so line below doesn't rotate camera too
            transform.Rotate(0, 180, 0); //rotate 180 degrees along y axis(y points up)
            camera.SetParent(transform);
        }
        else
        {
            transform.Rotate(0, 180, 0);
        }
    }

    bool duringHit()
    {
        if ( animator.GetCurrentAnimatorStateInfo(0).IsName("Hit1Begin") | animator.GetCurrentAnimatorStateInfo(0).IsName("Hit2Begin") |
             animator.GetCurrentAnimatorStateInfo(0).IsName("Hit3Begin") | animator.GetCurrentAnimatorStateInfo(0).IsName("Hit1and3End") |
             animator.GetCurrentAnimatorStateInfo(0).IsName("Hit2End") )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool duringHitBegin()
    {
        if ( animator.GetCurrentAnimatorStateInfo(0).IsName("Hit1Begin") | animator.GetCurrentAnimatorStateInfo(0).IsName("Hit2Begin") |
             animator.GetCurrentAnimatorStateInfo(0).IsName("Hit3Begin") )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool duringHitEnd()
    {
        if ( animator.GetCurrentAnimatorStateInfo(0).IsName("Hit1and3End") | animator.GetCurrentAnimatorStateInfo(0).IsName("Hit2End") )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isOnDownSwing() //called by enemy script to decide when to allow health to decrease on first swing
    {
        return firstHitDownSwing;
    }

    void ChangeHealth(int amount)
    {
     
        currentHealth += amount;

        if (currentHealth < 0)
        {
            currentStamina = 0;
        }
        if (currentHealth > maxHealth)
        {
            currentStamina = maxHealth;
        }
        HealthBar.SetValue(currentHealth);
    }

    void ChangeStamina(int amount)
    {

        currentStamina += amount;

        if (currentStamina < 0)
        {
            currentStamina = 0;
        }
        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
        StaminaBar.SetValue(currentStamina);
    }

    //code related to decreasing stamina when hitting. Slightly modified code taken from enemy script for sword health damage
    void SwordStaminaDecrement()
    {
        if (!duringHit())
        {
            //as soon as the hit animations are done allow stamina decrease again
            allowStaminaDecrease1 = true;
            allowStaminaDecrease2 = true;
            allowStaminaDecrease3 = true;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit1Begin") & allowStaminaDecrease1 & isOnDownSwing()) //prince is currently hitting with first slice
        {
            allowStaminaDecrease1 = false; //will be false until hit animations are done so stamina can't decrese more than once per hit
            ChangeStamina(-10);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit2Begin") & allowStaminaDecrease2)
        {
            allowStaminaDecrease2 = false;
            ChangeStamina(-10);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit3Begin") & allowStaminaDecrease3)
        {
            allowStaminaDecrease3 = false;
            ChangeStamina(-10);
        }
    }

    

    IEnumerator RegenerateStamina()
    {
        while (AllowRegeneration)
        {
            if (!duringHit() & !animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
            {
                ChangeStamina(1);
            }
            //Debug.Log("in regenerate function");
            
            
            yield return new WaitForSeconds((1.5f/10f));
        }
        

    }

    static private int DARK = 0;
    static private int LIGHT = 1;
    private int COLOR = LIGHT;

    IEnumerator RegenerateColorChange()
    {
        while (!CanUseStamina)
        {
            if (COLOR == DARK)
            {
                Debug.Log("dark");
                
                StaminaBar.fill.color = StaminaBar.gradient.Evaluate(1f);
                COLOR = LIGHT;
            }
            else
            {
                Debug.Log("light");
                
                StaminaBar.fill.color = StaminaBar.gradient.Evaluate(0f);
                COLOR = DARK;
            }
            yield return new WaitForSeconds(0.5f);
        }
        
    }
}


