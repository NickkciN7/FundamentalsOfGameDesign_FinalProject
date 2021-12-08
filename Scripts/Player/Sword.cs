using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{

    [SerializeField] private Transform HitCheckTransform = null;
    [SerializeField] private LayerMask swordMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("sword layer: " + gameObject.layer);
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.layer == 7)
    //    {
    //        collision.gameObject.GetComponent<>
    //    }
    //}
}
