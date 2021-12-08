using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    public Transform cam;
    

    // LateUpdate is called after update, so makes sure camera has done all its movement first before pointing the ui billboard at it
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
