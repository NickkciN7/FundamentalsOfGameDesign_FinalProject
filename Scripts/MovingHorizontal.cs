using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingHorizontal : MonoBehaviour
{
    [SerializeField] private int Distance;
    [SerializeField] private float speed;
    [SerializeField] private char StartDirection;

    private Vector3 LeftMostPosition;
    private int Direction;
    private readonly int LEFT = 0;
    private readonly int RIGHT = 1;


    // Start is called before the first frame update
    void Start()
    {
       
        if(StartDirection == 'L')
        {
            LeftMostPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - Distance);
            Direction = LEFT;
        }
        if (StartDirection == 'R')
        {
            Direction = RIGHT;
            LeftMostPosition = transform.position;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Distance: " + Vector3.Distance(transform.position, InitialPosition));
        if ((transform.position.z - LeftMostPosition.z) >= Distance)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, LeftMostPosition.z + Distance); //so over time small differences in start and end position won't add up
            Direction = LEFT;
        }
        if ((transform.position.z - LeftMostPosition.z) <= 0 & Direction == LEFT)
        {
            transform.position = LeftMostPosition;//so over time small differences in start and end position won't add up
            Direction = RIGHT;
        }

        //Debug.Log("up: " + transform.up);
        if (Direction == RIGHT)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        if (Direction == LEFT)
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        }

    }
}
