using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingVertical : MonoBehaviour
{

    [SerializeField] private float Distance;
    [SerializeField] private float speed;

    private Vector3 InitialPosition;
    private int Direction;
    private readonly int UP = 0;
    private readonly int DOWN = 1;
    

    // Start is called before the first frame update
    void Start()
    {
        InitialPosition = transform.position;
        Direction = UP;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Distance: " + Vector3.Distance(transform.position, InitialPosition));
        if( (transform.position.y - InitialPosition.y)  >=  Distance)
        {
            transform.position = new Vector3(transform.position.x, InitialPosition.y + Distance, transform.position.z); //so over time small differences in start and end position won't add up
            Direction = DOWN;
        }
        if( (transform.position.y - InitialPosition.y) <= 0 & Direction == DOWN)
        {
            transform.position = new Vector3(transform.position.x, InitialPosition.y, transform.position.z);//InitialPosition;//so over time small differences in start and end position won't add up
            Direction = UP;
        }

        //Debug.Log("up: " + transform.up);
        if(Direction == UP)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        if (Direction == DOWN)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }

    }
}
