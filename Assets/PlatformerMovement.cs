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

    [Header("Internal Variables")]
    public float xMoveInput;
    public float desiredXVelocity;
    public bool jumpInput;
    public float gravMultiplier;
    public float jumpBufferTimer;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newGravity = new Vector2(0, (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex));
        uRigidbody2D.gravityScale = (newGravity.y / Physics2D.gravity.y) * gravMultiplier;


        xMoveInput = Input.GetAxis("Horizontal");
        desiredXVelocity = xMoveInput * maxSpeed;

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
                Debug.Log("Turning");
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
        uRigidbody2D.velocity = newVelocity;

        if (jumpInput)
        {
            
            if (IsOnGround())
            {
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
    }

    public bool IsOnGround()
    { 
        RaycastHit2D raycastHit = Physics2D.BoxCast(uBoxCollider.bounds.center, 
            uBoxCollider.bounds.size - new Vector3(0.1f, 0f, 0f), 0f, Vector2.down, uGroundCheckExtraHeight, uPlatformLayerMask);
        return raycastHit.collider != null;
    }
}