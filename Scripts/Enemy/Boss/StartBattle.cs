using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBattle : MonoBehaviour
{

    
    private bool Begin = false;
    private bool RainObjects = false;
    [SerializeField] private GameObject LavaBlock;
    [SerializeField] private GameObject NewHeart;
    private int HeartGenerationChance = 20;
    private int HeartGenerationWaitTime = 7;
    [SerializeField] private GameObject BossHealth;
    [SerializeField] private GameObject Warning;
    [SerializeField] private GameObject Boss;

    private int NumBlinks = 3;
    private float WaitTime = 3;
    GameObject prince;

    private AudioSource Audio_BossIntro;

    void Start()
    {
        prince = GameObject.Find("prince");

        Audio_BossIntro = GameObject.Find("BossIntro").GetComponent<AudioSource>();
    }

    void Update()
    {

        //if(Boss.transform.position.z - prince.transform.position.z > 8)
        //{
        //    NumBlinks = 1;
        //}
        //else
        //{
        //    NumBlinks = 3;
        //}
        if (Begin)
        {

            
            if (!RainObjects)
            {
                RainObjects = true;
                //GameObject.Find("Canvas(Boss)").SetActive(true); //can't use Find to find inactive objects
                if (Boss.GetComponent<EnemyDamage>().BossDead == false)
                {
                    BossHealth.SetActive(true);
                    Audio_BossIntro.PlayOneShot(Audio_BossIntro.clip);
                }
                    
                Destroy(GameObject.Find("HoldingUpWall"));
                GameObject.Find("Main Camera").transform.SetParent(null);
                StartCoroutine(WaitToRain()); //wait a little before calling GeneratBoulders()
            }
        }
        if(Boss.GetComponent<EnemyDamage>().BossDead == true)
        {
            Warning.SetActive(false);
            
            if (RainObjects == true)
            {
                StartCoroutine(WaitToRemoveHealthBar());
            }
            
            RainObjects = false; 
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("prince"))
        {
            Begin = true;
        }
    }

    IEnumerator GenerateBoulders()
    {
        while (RainObjects)
        {
            bool FallSoon = false;
            NumBlinks = 3;
            WaitTime = 3;
            if (Boss.transform.position.z - prince.transform.position.z > 7)
            {
                NumBlinks = 1;
                WaitTime = 1.5f;
                FallSoon = true;
            }
            

            float zPos = prince.transform.position.z;
            if(zPos < 318.9f) //so lava block doesnt hit wall
            {
                zPos = 318.9f;
            }
            if (FallSoon)
            {
                Instantiate(LavaBlock, new Vector3(0, 20f, zPos), Quaternion.identity);
            }
            Warning.transform.position = new Vector3(Warning.transform.position.x, Warning.transform.position.y, zPos);
            for (int i = 0; i < NumBlinks; i++) //Blink on and off
            {
                Warning.SetActive(true);
                yield return new WaitForSeconds(0.4f);
                Warning.SetActive(false);
                yield return new WaitForSeconds(0.4f);
            }
            if (!FallSoon)
            {
                GameObject lava = Instantiate(LavaBlock, new Vector3(0, 20f, zPos), Quaternion.identity);
                if (Boss.GetComponent<EnemyDamage>().BossDead == true) //destroy last block if boss dead
                {
                    Destroy(lava);
                }

            }

            yield return new WaitForSeconds(WaitTime);//wait for time
        }
        
        
    }
    IEnumerator GenerateHearts()
    {
        while (RainObjects)
        {
            


            float zPos = prince.transform.position.z;
            if (zPos < 318.9f) //so lava block doesnt hit wall
            {
                zPos = 318.9f;
            }

            if (Random.Range(1, 100) <= HeartGenerationChance) //example where HeartGenerationChance = 33, there is ~33% chance the random number from 1 to 100 will be 33 or lesss
            {
                Instantiate(NewHeart, new Vector3(0, 7f, zPos), Quaternion.identity); //make heart where the enemy was, but in the air
            }


            yield return new WaitForSeconds(HeartGenerationWaitTime);//wait for time
        }


    }

    IEnumerator WaitToRain()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(GenerateBoulders());
        StartCoroutine(GenerateHearts());
    }


    IEnumerator WaitToRemoveHealthBar()
    {
        yield return new WaitForSeconds(2);
        BossHealth.SetActive(false);
    }
}
