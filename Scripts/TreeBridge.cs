using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBridge : MonoBehaviour
{
    private GameObject prince;
    private Animator p_animator;
    private Player player;
    private int maxHealth = 4;
    private int currentHealth;
    private int SwordDamage = 1;
    private bool allowHealthDecrease1 = true; //for sword hit 1
    private bool allowHealthDecrease2 = true; //for sword hit 2
    private bool allowHealthDecrease3 = true; //for sword hit 3
    private bool downSwing; //so health won't decrease on up swing.
    
    private bool AllowHit = true;

    private Quaternion StartRotation;
    private Quaternion StopRotation;
    private float RotProg = 0;

    private bool TreeGoneStart = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        prince = GameObject.Find("prince");
        p_animator = prince.GetComponent<Animator>();
        player = prince.GetComponent<Player>();


        StartRotation = transform.rotation;
        StopRotation = Quaternion.Euler(new Vector3(90, 5, transform.rotation.eulerAngles.z));
    }

    // Update is called once per frame
    void Update()
    {


        //in update instead of OnTriggerEnter because I think it won't register another Trigger if character hasn't moved
        if (!duringHit())
        {
            //as soon as the hit animations are done allow health decrease of enemy again
            allowHealthDecrease1 = true;
            allowHealthDecrease2 = true;
            allowHealthDecrease3 = true;
        }
        if (currentHealth == 0)
        {
            AllowHit = false;
            RotProg += Time.deltaTime * 1.5f;
            transform.rotation = Quaternion.Lerp(StartRotation, StopRotation, RotProg);
        }
        if(prince.transform.position.z - transform.position.z > 23 & !TreeGoneStart)
        {
            TreeGoneStart = true;
            StartCoroutine(ShakeAndFall(7));
        }
        if(transform.position.y < -25)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {


    }

    void TakeDamage(int damage)
    {
        //Debug.Log("H: " + currentHealth);
        StartCoroutine(Shake(3));

        currentHealth -= damage;

    }

    IEnumerator Shake(int ShakeNumber)
    {
        for (int i = 0; i < ShakeNumber; i++)
        {
            transform.Translate(Vector3.forward * 0.1f);
            yield return new WaitForSeconds(0.05f);
            transform.Translate(Vector3.back * 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
    }
    IEnumerator ShakeAndFall(int ShakeNumber)
    {
        for (int i = 0; i < ShakeNumber; i++)
        {
            transform.Translate(Vector3.forward * 0.1f);
            yield return new WaitForSeconds(0.05f);
            transform.Translate(Vector3.back * 0.1f);
            yield return new WaitForSeconds(0.05f);
        }

        Destroy(transform.GetChild(0).GetComponent<MeshCollider>());
        gameObject.AddComponent<Rigidbody>();
    }
    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("triggered");
        if (!AllowHit)
        {
            return;
        }
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
