using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomWallBlockHitGround : MonoBehaviour
{

    private bool CollisionEntered = false;
    private bool StartTimer = false;
    private AudioSource Audio_BlockHitGround;
    // Start is called before the first frame update
    void Start()
    {
        Audio_BlockHitGround = transform.Find("HitGroundSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CollisionEntered) //will keep being made true if collide again so have to use another bool below that will only allow access to inside an if statement
        {                     //ONCE regardless of how many times OnCollisionEnter is called
            if (!StartTimer)
            {
                Audio_BlockHitGround.PlayOneShot(Audio_BlockHitGround.clip);
                StartTimer = true;
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9) //ground
        {
            if (!collision.gameObject.name.Contains("Wall")) //collided with ground and not another wall block
            {
                CollisionEntered = true;
            }
            
        }
    }

}
