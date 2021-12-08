using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject Prince;
    private Animator p_animator;
    public int maxHealth = 100;
    public int currentHealth;
    private bool allowHealthDecrease1 = true; //for sword hit 1
    private bool allowHealthDecrease2 = true; //for sword hit 2
    private bool allowHealthDecrease3 = true; //for sword hit 3
    private bool downSwing; //so health won't decrease on up swing.
    public HealthBar healthBar;
    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        p_animator = Prince.GetComponent<Animator>();
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
            Destroy(gameObject);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
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
                TakeDamage(10);
            }
            if (p_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit2Begin") & allowHealthDecrease2) 
            {
                allowHealthDecrease2 = false; 
                TakeDamage(10);
            }
            if (p_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit3Begin") & allowHealthDecrease3) 
            {
                allowHealthDecrease3 = false; 
                TakeDamage(10);
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





    //[SerializeField] private Material color;
    //[SerializeField] private GameObject PoliceMan;
    //private int health = 3;
    //private bool allowHealthDecrease = true;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if(health == 0)
    //    {
    //        GetComponent<Renderer>().material = color;
    //    }

    //    //in update instead of OnTriggerEnter because I think it won't register another Trigger if character hasn't moved
    //    if (!PoliceMan.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Hit") & !allowHealthDecrease)
    //    {
    //        //as soon as the hit animation is done and allowHealthDecrease == false, allowHealthDecrease = true again
    //        allowHealthDecrease = true;
    //    }
    //}

    //void DecreaseHealth(int amount)
    //{
    //    if(health == 0)
    //    {
    //        return;
    //    }
    //    health -= amount;
    //    Debug.Log("Health: "+ health);
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //seems to identify the sword as PoliceMan.
    //    Debug.Log("Enemy collided with something");
    //    Debug.Log("layer: " + collision.gameObject.layer);
    //    Debug.Log("name: " + collision.gameObject.name);
    //}



}
