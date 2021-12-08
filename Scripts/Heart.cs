using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public Transform GroundCheckTransform = null;
    public LayerMask HeartMask;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime*30);
        if (Physics.OverlapSphere(GroundCheckTransform.position, 0.1f, HeartMask).Length == 0) //can also add rigid body with drag increased
        {
            transform.Translate(Vector3.down * Time.deltaTime * 0.5f);
        }
   
    }

}
