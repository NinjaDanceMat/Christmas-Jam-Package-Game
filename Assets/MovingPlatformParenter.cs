using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformParenter : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "Player")
        {
            collision.transform.SetParent(transform);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.tag == "Player")
        {
            collision.transform.SetParent(null);
        }
    }
}
