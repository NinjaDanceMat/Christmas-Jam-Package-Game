using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;

    public bool goingTowardsStart;
    public float speed;
    public bool dead;

    public int currentHealth;
    public int maxHealth;
    public Animator animationController;
    public void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
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
            if (Vector3.Distance(transform.position, destination) < 0.1f)
            {
                goingTowardsStart = !goingTowardsStart;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!dead)
        {
            if (collision.gameObject.tag == "PlayerProjectile")
            {
                animationController.SetTrigger("Hit");
                currentHealth -= 1;
                if (currentHealth <= 0)
                {
                    dead = true;
                    Destroy(gameObject);
                }
            }
        }
    }

}
