using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingInLavaBlocks : MonoBehaviour
{

    private float speed = 2f;
    private bool MoveLeft = false;
    private bool MoveRight = false;
    [SerializeField] private GameObject Boss;
    private bool HitGround = false;

    private bool DisplayedDialogue = false;
    public GameObject BrotherDialogue;
    public GameObject TheEndText;
    public GameObject Prince;

    private bool StopMovement = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (StopMovement)
        {
            return;
        }

        if (Boss.GetComponent<EnemyDamage>().BossDead == true & !HitGround)
        {
            MoveLeft = true; //dont set to true when floating blocks hit the main ground
        }

        if (MoveLeft)
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        }
        if (MoveRight & !MoveLeft & Prince.GetComponent<Player>().TextUIOnScreen == false)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        if (!DisplayedDialogue)
        {
            if(transform.position.z - GameObject.Find("End(1)").transform.position.z <= 3)
            {
                BrotherDialogue.SetActive(true);
                DisplayedDialogue = true;
                Prince.GetComponent<Player>().TextUIOnScreen = true;
            }
        }
        if(MoveRight & (transform.position.z - GameObject.Find("End(1)").transform.position.z >= 9))
        {
            TheEndText.SetActive(true);
        }
        if (MoveRight & (transform.position.z - GameObject.Find("End(1)").transform.position.z >= 25))
        {
            StopMovement = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(other.name);
        if (other.gameObject.name.Contains("End")) //1 of the objects needs a rigidbody so I added rigidbody to the end blocks(Is Kinematic true so no physics affect them)
        {
            HitGround = true;
            MoveLeft = false;
        }
        if (other.gameObject.name.Equals("Prince"))
        {
            MoveRight = true;
            other.gameObject.transform.parent.transform.SetParent(transform);
            Prince.GetComponent<Player>().GameFinished = true;
        }
    }
}
