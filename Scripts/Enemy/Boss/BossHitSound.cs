using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitSound : MonoBehaviour
{
    private AudioSource Audio_Hit;
    public GameObject Boss;

    // Start is called before the first frame update
    void Start()
    {
        Audio_Hit = Boss.transform.Find("Audio").Find("HitSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.name.Equals("PlayBossHitSound Block"))
        {
            Audio_Hit.PlayOneShot(Audio_Hit.clip);
        }
    }
}
