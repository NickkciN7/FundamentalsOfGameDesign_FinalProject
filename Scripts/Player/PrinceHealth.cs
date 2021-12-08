using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinceHealth : MonoBehaviour
{
    //private GameObject enemy;
    //private Animator p_animator;
    public int maxHealth = 100;
    public int currentHealth;
    private bool allowHealthDecrease = true;

    //private Animator animator;

    public Bar healthBar;


    private bool HitFromLeft = false;
    private bool HitFromRight = false;
    private Rigidbody rigidBodyComponent;
    Vector3 PushRight = new Vector3(0, 1, 0.8f);
    Vector3 PushLeft = new Vector3(0, 1, -0.8f);


    //[SerializeField] private Material HurtMaterial;
    [SerializeField] private HurtMaterialChange HMatChange;
    private bool ChangeToCommonMaterial = false;

    private bool fallen = false;


    private GameObject Boss;
    private bool AllowBossHit = true;

    private AudioSource Audio_SpikeHit;

    private AudioSource Audio_GotHeart;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxValue(maxHealth);
        rigidBodyComponent = GetComponent<Rigidbody>();

        Boss = GameObject.Find("Boss");

        Audio_SpikeHit = transform.Find("Audio").Find("SpikeHitSound").GetComponent<AudioSource>();

        Audio_GotHeart = GameObject.Find("Heart").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y <= -7.5f & !fallen)
        {
            fallen = true;
            if(transform.Find("Main Camera") != null)
            {
                transform.Find("Main Camera").SetParent(null);
            }

            TakeDamage(100);
        }
        if (transform.position.y <= -15f)
        {
            rigidBodyComponent.constraints = RigidbodyConstraints.FreezePosition;
        }
        if (ChangeToCommonMaterial)
        {
            HMatChange.ChangeToCommon(transform.name);
            ChangeToCommonMaterial = false;
        }


        //if (currentHealth == 0)
        //{
        //    Destroy(gameObject);
        //}

        if (Boss.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            //Debug.Log("Allow hurt");
            AllowBossHit = true;
        }
    }

    private void FixedUpdate()
    {
        
        if (HitFromLeft)
        {
            rigidBodyComponent.AddForce(PushRight, ForceMode.VelocityChange);
            HitFromLeft = false;
        }
        if (HitFromRight)
        {
            rigidBodyComponent.AddForce(PushLeft, ForceMode.VelocityChange);
            HitFromRight = false;
        }

    }

    void TakeDamage(int damage)
    {
        HMatChange.ChangeToHurt(transform.name); //change color slightly red for a small amount of time
        StartCoroutine(WaitToChangeMaterial());
        StartCoroutine(WaitToAllowDamage());
        

        currentHealth -= damage;
        if(currentHealth < 0)
        {
            currentHealth = 0;
        }
        healthBar.SetValue(currentHealth);
    }

    void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > 100)
        {
            currentHealth = 100;
        }
        healthBar.SetValue(currentHealth);
    }
    
    IEnumerator WaitToChangeMaterial()
    {
        yield return new WaitForSeconds(0.3f);
        ChangeToCommonMaterial = true;
    }
    IEnumerator WaitToAllowDamage()
    {
        yield return new WaitForSeconds(1f);
        allowHealthDecrease = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentHealth == 0)
        {
            return;
        }
        if(other.gameObject.layer == 13) //heart
        {
            Heart heartScript = other.gameObject.GetComponent<Heart>();
            if (Physics.OverlapSphere(heartScript.GroundCheckTransform.position, 0.1f, heartScript.HeartMask).Length == 0) //didn't touch ground yet
            {
                return;
            }
            
            Destroy(other.gameObject);
            Audio_GotHeart.PlayOneShot(Audio_GotHeart.clip);
            Heal(10);
        }
        if (other.gameObject.layer == 10 | other.gameObject.layer == 11) 
        {
            int damageAmount = 0;
            if(other.gameObject.layer == 10)//Spike Shell Monster 
            {
                Debug.Log(other.name); //showed that Body and Shell would appear here, so make sure it is SpikeLarge to Cause damage
                if (!other.name.Equals("SpikeLarge"))
                {
                    return; //monster not hitting so don't take any damage
                }
                
                damageAmount = 10;
            }
            if (other.gameObject.layer == 11)//Humanoid Monster 
            {
                //Debug.Log(other.transform.parent.transform.parent.transform.parent.transform.parent.transform.parent.transform.parent.transform.parent.transform.parent.transform.parent.name);
                if (!other.name.Equals("Axe") )
                {
                    return; 
                }
                GameObject RootMonsterObject = other.transform.parent.parent.parent.parent.parent.parent.parent.parent.parent.gameObject;
                if (!RootMonsterObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Hit"))
                {
                    return; //monster not hitting so don't take any damage
                }
                if( ( (transform.position.z - RootMonsterObject.transform.position.z > 0) & (RootMonsterObject.transform.forward.z < 0) ) |
                    ( (transform.position.z - RootMonsterObject.transform.position.z < 0) & (RootMonsterObject.transform.forward.z > 0)) )
                {
                    return; //monster hitting in direction, but prince is behind it. Made this to solve issue of receiving damage when axe slightly touches prince 
                            //even though the prince is behind
                }

                damageAmount = 15;
            }

            if ( allowHealthDecrease ) //wasnt hurt in last second
            {
                allowHealthDecrease = false; //will be false for a short amount of time

                if ((transform.position.z - other.transform.position.z) > 0)
                {
                    HitFromLeft = true;
                }
                else
                {
                    HitFromRight = true;
                }

                TakeDamage(damageAmount);
                
            }
            
        }

        if (other.gameObject.layer == 14) //spike ball
        {
            if (Mathf.Abs(other.transform.position.z - transform.position.z) > 1f)
            {
                return;
            }
            transform.Find("Main Camera").SetParent(null);
            rigidBodyComponent.velocity = Vector3.zero;
            if ((transform.position.z - other.transform.position.z) > 0)
            {
                HitFromLeft = true;
            }
            else
            {
                HitFromRight = true;
            }
            Audio_SpikeHit.PlayOneShot(Audio_SpikeHit.clip);
            TakeDamage(100);
        }

        if (other.gameObject.layer == 15) //spike 
        {
            if (Mathf.Abs(other.transform.position.z - transform.position.z) > 1f) //because sword also can cause OnTriggerEnter, ignore if far away
            {
                return;
            }
            Audio_SpikeHit.PlayOneShot(Audio_SpikeHit.clip);
            TakeDamage(100);
        }

        if (other.gameObject.layer == 17) //LavaBlock
        {
            //if ((transform.position.z - other.transform.position.z) > 0)
            //{
              
            //}
            //else
            //{
                
            //}
            if (Mathf.Abs(other.transform.position.z - transform.position.z) > 1f) //because sword also can cause OnTriggerEnter, ignore if far away
            {
                return;
            }
            TakeDamage(10);
        }

        if (other.gameObject.layer == 18) //Boss
        {
            //was preventing prince from being hurt if too close(but need to get close to even hit boss)
            //if (Mathf.Abs(other.transform.position.z - transform.position.z) > 1f) //because sword also can cause OnTriggerEnter, ignore if far away
            //{
            //    return;
            //} 

            if (!other.name.Equals("Trigger")) //even if sword causes ontrigger event, shouldn't be with the Trigger game object attached to the arm, which is what
            {                                  //should actually hurt the prince
                return;
            }

            if (AllowBossHit) //don't let prince get hurt more than once per boss hit
            {
                TakeDamage(20);
                AllowBossHit = false;
            }
            
        }
    }

    
}
