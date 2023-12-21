using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerMovement : MonoBehaviour
{
    [Header("Setup Once in Unity")]
    public Rigidbody2D uRigidbody2D;
    public BoxCollider2D uBoxCollider;
    public LayerMask uPlatformLayerMask;
    public float uGroundCheckExtraHeight;
    public SpriteRenderer characterSpriteRenderer;
    public Animator animationController;

    [Header("Internal Variables")]
    public float xMoveInput;
    public float desiredXVelocity;
    public bool jumpInput;
    public float gravMultiplier;
    public float jumpBufferTimer;
    public bool facingRight = true;
    public int currentHealth;
    public float gotHitCooldown;
    public bool dead;

    [Header("Design Variables")]
    public float maxAcceleration;
    public float maxDeceleration;
    public float maxTurnSpeed;
    public float maxAirAcceleration;
    public float maxAirDeceleration;
    public float maxAirTurnSpeed;
    public float maxSpeed;
    public float jumpHeight;
    public float timeToJumpApex;
    public float downwardMovementMultiplier;
    public float jumpBufferTime;
    public int maxHealth;
    public float gotHitCooldownTime;



    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newGravity = new Vector2(0, (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex));
        uRigidbody2D.gravityScale = (newGravity.y / Physics2D.gravity.y) * gravMultiplier;

        if (!dead)
        {
            xMoveInput = Input.GetAxis("Horizontal");
        }
        else
        {
            xMoveInput = 0;
        }
        desiredXVelocity = xMoveInput * maxSpeed;


        if (!dead)
        {
            if (Input.GetButtonDown("Jump"))
            {
                jumpInput = true;
                jumpBufferTimer = jumpBufferTime;
            }
            else if (jumpBufferTimer > 0)
            {
                jumpBufferTimer -= Time.deltaTime;
            }
        }
        else
        {
            jumpInput = false;
        }
    }

    public void FixedUpdate()
    {
        float maxSpeedChange = 0;
        float acceleration = maxAcceleration;
        float deceleration = maxDeceleration;
        float turnSpeed = maxTurnSpeed;

        if (!IsOnGround())
        {
            acceleration = maxAirAcceleration;
            deceleration = maxAirDeceleration;
            turnSpeed = maxAirTurnSpeed;
        }

        if (xMoveInput != 0)
        {
            if (Mathf.Sign(desiredXVelocity) != Mathf.Sign(uRigidbody2D.velocity.x))
            {
                maxSpeedChange = turnSpeed * Time.deltaTime;
               
            }
            else
            {
                
                maxSpeedChange = acceleration * Time.deltaTime;
            }
        }
        else
        {
            maxSpeedChange = deceleration * Time.deltaTime;
        }
        
        Vector2 newVelocity = uRigidbody2D.velocity;
        newVelocity.x = Mathf.MoveTowards(newVelocity.x, desiredXVelocity, maxSpeedChange);
        
        if (newVelocity.x > 0 || newVelocity.x < 0)
        {
            animationController.SetFloat("Movement", 1);
        }
        else
        {
            animationController.SetFloat("Movement", 0);
        }
        
        uRigidbody2D.velocity = newVelocity;

        if (jumpInput)
        {
            
            if (IsOnGround())
            {
                animationController.SetTrigger("Jump");
                jumpInput = false;
                Vector3 newJumpVelocity = uRigidbody2D.velocity;
                float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * uRigidbody2D.gravityScale * jumpHeight);
                if (newJumpVelocity.y > 0f)
                {
                    jumpSpeed = Mathf.Max(jumpSpeed - newJumpVelocity.y, 0f);
                }
                else if (newJumpVelocity.y < 0f)
                {
                    jumpSpeed += Mathf.Abs(uRigidbody2D.velocity.y);
                }
                newJumpVelocity.y += jumpSpeed;
                uRigidbody2D.velocity = newJumpVelocity;
            }
            else
            {
                if (jumpBufferTimer <= 0)
                {
                    jumpInput = false;
                }
                
            }
        }

        if (uRigidbody2D.velocity.y == 0) 
        { 
            gravMultiplier = 1; 
        }
        if (uRigidbody2D.velocity.y < -0.01f) 
        { 
            gravMultiplier = downwardMovementMultiplier; 
        }

        if (desiredXVelocity > 0)
        {
            facingRight = true;
            characterSpriteRenderer.flipX = false;
        }
        else if (desiredXVelocity < 0)
        {
            facingRight = false;
            characterSpriteRenderer.flipX = true;
        }
        if (gotHitCooldown > 0)
        {
            gotHitCooldown -= Time.deltaTime;
        }
    }

    public bool IsOnGround()
    { 
        RaycastHit2D raycastHit = Physics2D.BoxCast(uBoxCollider.bounds.center, 
            uBoxCollider.bounds.size - new Vector3(0.1f, 0f, 0f), 0f, Vector2.down, uGroundCheckExtraHeight, uPlatformLayerMask);
        return raycastHit.collider != null;
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (!dead)
        {
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Deathbox")
            {
                if (gotHitCooldown <= 0)
                {
                    if (collision.gameObject.tag == "Deathbox")
                    {
                        currentHealth = 0;
                    }
                    else
                    {
                        currentHealth -= 1;
                    }
                    gotHitCooldown = gotHitCooldownTime;
                    float xHitForce = 1;
                    if (collision.transform.position.x > transform.position.x)
                    {
                        xHitForce = -1;
                    }
                    uRigidbody2D.AddForce(new Vector2(10 * xHitForce, 10), ForceMode2D.Impulse);
                    if (currentHealth > 0)
                    {
                        animationController.SetTrigger("Hit");
                    }
                    else
                    {
                        animationController.SetTrigger("Death");
                        dead = true;
                    }
                }
            }
        }
    }
}
