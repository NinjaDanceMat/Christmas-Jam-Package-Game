using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public PlatformerMovement movermentScript;
    public float projectileForceFacing;
    public float projectileForceFacingUp;
    public float upwardSpecialForceMultiplier;
    public float upwardNormalForceMultiplier;
    public float sidewaysNormalForceMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (movermentScript.desiredXVelocity > 0)
        {
            projectileForceFacing = 1;
            projectileForceFacingUp = upwardNormalForceMultiplier;
        }
        else if (movermentScript.desiredXVelocity < 0)
        {
            projectileForceFacing = -1;
            projectileForceFacingUp = upwardNormalForceMultiplier;
        }
        else
        {
            //projectileForceFacing = 0;
            //projectileForceFacingUp = upwardSpecialForceMultiplier;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject newProjectile = Instantiate(projectilePrefab);
            newProjectile.transform.position = gameObject.transform.position;
            newProjectile.GetComponent<Rigidbody2D>().AddForce(
                new Vector2(sidewaysNormalForceMultiplier * projectileForceFacing, projectileForceFacingUp), 
                ForceMode2D.Impulse
                );
        }
    }
}
