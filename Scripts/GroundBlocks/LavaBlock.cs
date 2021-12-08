using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaBlock : MonoBehaviour
{
    private bool CollisionEntered = false;
    private bool StartTimer = false;
    private AudioSource Audio_LavaHitGround;

    // Start is called before the first frame update
    void Start()
    {
        Audio_LavaHitGround  = transform.Find("HitGroundSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CollisionEntered) //will keep being made true if collide again so have to use another bool below that will only allow access to inside an if statement
        {                     //ONCE regardless of how many times OnCollisionEnter is called
            if (!StartTimer)
            {
                Audio_LavaHitGround.PlayOneShot(Audio_LavaHitGround.clip);
                StartTimer = true;
                StartCoroutine(TimeToDestroy());
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 9) //ground
        {
            CollisionEntered = true;
        }
    }

    IEnumerator TimeToDestroy()
    {
        yield return new WaitForSeconds(1);//wait for time
        Destroy(gameObject);
    }

}
