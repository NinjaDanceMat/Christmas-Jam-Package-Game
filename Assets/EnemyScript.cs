using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;

    public bool goingTowardsStart;
    public float speed;
    // Update is called once per frame
    void Update()
    {
        Vector3 destination;
        if (goingTowardsStart)
        {
            destination = StartPoint.position;
        }
        else
        {
            destination = EndPoint.position;
        }
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);
        if (Vector3.Distance(transform.position,destination)< 0.1f)
        {
            goingTowardsStart = !goingTowardsStart;
        }
    }
}
