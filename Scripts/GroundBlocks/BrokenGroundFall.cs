using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenGroundFall : MonoBehaviour
{

    private int ShakeNumber = 10;
    private bool ShakeStarted = false;

    private AudioSource Audio_Fall;
    // Start is called before the first frame update
    void Start()
    {
        Audio_Fall = transform.Find("FallingSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }



    private void OnCollisionEnter(Collision collision)
    {
        if (ShakeStarted)
        {
            return;
        }
        if (collision.gameObject.layer == 6) //player touched it
        {
            Audio_Fall.PlayOneShot(Audio_Fall.clip);
            StartCoroutine(ShakeAndFall());
        }
    }

    IEnumerator ShakeAndFall()
    {
        ShakeStarted = true;
        for(int i = 0; i < ShakeNumber; i++)
        {
            transform.Translate(Vector3.forward * 0.1f);
            yield return new WaitForSeconds(0.05f);
            transform.Translate(Vector3.back * 0.1f);
            yield return new WaitForSeconds(0.05f);
        }

        gameObject.AddComponent<Rigidbody>();
    }

}