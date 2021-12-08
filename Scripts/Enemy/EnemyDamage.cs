using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private GameObject prince;
    private Animator p_animator;
    public int maxHealth = 100;
    public int currentHealth;
    private int SwordDamage = 10;
    private bool allowHealthDecrease1 = true; //for sword hit 1
    private bool allowHealthDecrease2 = true; //for sword hit 2
    private bool allowHealthDecrease3 = true; //for sword hit 3
    private bool downSwing; //so health won't decrease on up swing.
    public Bar healthBar;
    private Player player;


    private bool HitFromLeft = false;
    private bool HitFromRight = false;
    private Rigidbody rigidBodyComponent;
    Vector3 PushRight = new Vector3(0, 1, 0.8f);
    Vector3 PushLeft = new Vector3(0, 1, -0.8f);


    //[SerializeField] private Material HurtMaterial;
    [SerializeField] private HurtMaterialChange HMatChange;
    private bool ChangeToCommonMaterial = false;


    [SerializeField] private GameObject NewHeart;
    private int HeartGenerationChance = 100;


    public bool BossDead = false;


    private AudioSource Audio_BeatBoss;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxValue(maxHealth);
        if (!gameObject.name.Equals("Boss"))
        {
            transform.Find("Canvas(Enemy)").GetComponent<Billboard>().cam = GameObject.Find("Main Camera").transform;
        }
        
        prince = GameObject.Find("prince");
        p_animator = prince.GetComponent<Animator>();
        //Debug.Log(Prince.transform.position);
        player = prince.GetComponent<Player>();
        rigidBodyComponent = GetComponent<Rigidbody>();

        
        Audio_BeatBoss = GameObject.Find("BeatBoss").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (ChangeToCommonMaterial)
        {
            HMatChange.ChangeToCommon(transform.name);
            ChangeToCommonMaterial = false;
        }
        //in update instead of OnTriggerEnter because I think it won't register another Trigger if character hasn't moved
        if (!duringHit())
        {
            //as soon as the hit animations are done allow health decrease of enemy again
            allowHealthDecrease1 = true;
            allowHealthDecrease2 = true;
            allowHealthDecrease3 = true;
        }
        if (currentHealth <= 0)
        {
            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.transform.position = new Vector3(transform.position.x, transform.position.y+1, transform.position.z);
            if (!gameObject.name.Equals("Boss"))
            {
                if (Random.Range(1, 100) <= HeartGenerationChance) //example where HeartGenerationChance = 33, there is ~33% chance the random number from 1 to 100 will be 33 or lesss
                {
                    Instantiate(NewHeart, new Vector3(0, 2f, transform.position.z), Quaternion.identity); //make heart where the enemy was, but in the air
                }

                Destroy(gameObject);
            }
            else
            {
                if (!BossDead)
                {
                    BossDead = true;
                    GameObject.Find("BossFloor").SetActive(false); //remove floor making boss fall out of view
                    Destroy(transform.Find("Body").GetComponent<MeshCollider>());
                    Destroy(GameObject.Find("BackGroundMusic"));
                    Audio_BeatBoss.PlayOneShot(Audio_BeatBoss.clip);
                    
                }
                if(transform.position.y < -20) //below where can be seen
                {
                    gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        
        if (HitFromLeft)
        {
            if (!gameObject.name.Equals("Boss"))
            {
                rigidBodyComponent.AddForce(PushRight, ForceMode.VelocityChange);
            }
            //rigidBodyComponent.AddForce(PushRight, ForceMode.VelocityChange);
            HitFromLeft = false;
        }
        if (HitFromRight)
        {
            if (!gameObject.name.Equals("Boss"))
            {
                rigidBodyComponent.AddForce(PushLeft, ForceMode.VelocityChange);
            }
            //rigidBodyComponent.AddForce(PushLeft, ForceMode.VelocityChange);
            HitFromRight = false;
        }

    }

    void TakeDamage(int damage)
    {
        HMatChange.ChangeToHurt(transform.name); //change color slightly red for a small amount of time
        StartCoroutine(WaitToChangeMaterial());
        if ((transform.position.z - prince.transform.position.z) > 0)
        {
            HitFromLeft = true;
        }
        else
        {
            HitFromRight = true;
        }

        currentHealth -= damage;

        healthBar.SetValue(currentHealth);
    }

    IEnumerator WaitToChangeMaterial()
    {
        yield return new WaitForSeconds(0.3f);
        ChangeToCommonMaterial = true;
    }
    private void OnTriggerStay(Collider other)
    {
        //correctly identifies the trigger as sword. I guess because sword is the only trigger object in PoliceMan it no 
        //longer takes the parent instead of the child like with collisions before

        //Debug.Log("Enemy triggered by something");
        //Debug.Log("layer: " + other.gameObject.layer);


        if (other.gameObject.layer == 8) //sword touched enemy
        {

            if (p_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit1Begin") & allowHealthDecrease1 & player.isOnDownSwing()) //prince is currently hitting with first slice
            {
                allowHealthDecrease1 = false; //will be false until hit animations are done so health can't decrese more than once per hit
                TakeDamage(SwordDamage);
            }
            if (p_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit2Begin") & allowHealthDecrease2)
            {
                allowHealthDecrease2 = false;
                TakeDamage(SwordDamage);
            }
            if (p_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit3Begin") & allowHealthDecrease3)
            {
                allowHealthDecrease3 = false;
                TakeDamage(SwordDamage);
            }
        }
    }

    bool duringHit()
    {
        if (p_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit1Begin") | p_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit2Begin") |
             p_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit3Begin") | p_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit1and3End") |
             p_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit2End"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
