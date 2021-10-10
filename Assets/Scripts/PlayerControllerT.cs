using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerT : MonoBehaviour
{
    [SerializeField] private Transform[] leftCasts;
    [SerializeField] private Transform[] rightCasts;
    [SerializeField] private Transform[] topCasts;
    [SerializeField] private Transform[] downCasts;

    [SerializeField] private float baseMovementSpeed = 10f;              /* horizontal speed on the ground */
    [SerializeField] private float baseSprintingSpeed = 15f;             /* horizontal sprinting speed on the ground */
    [SerializeField] private float accelerationDuration = 0.5f;         /* time it takes to reach base movement speed */
    [SerializeField] private float baseDecelerationDuration = 0.1f;     /* time it takes to change direction */
    [SerializeField] private float groundDecelerationDuration = 0.1f;   /* time it takes to stop on the ground */
    [SerializeField] private float airDecelerationDuration = 0.5f;      /* time it takes to stop in the air */
    [SerializeField] private float verticalAirFriction = 0f;            /* vertical slowdown factor when in the air */
    [SerializeField] private float horizontalAirFriction = 0f;          /* additional horizontal slowdown factor when in the air */
    [SerializeField] private float horizontalGroundFriction = 0f;       /* additional horizontal slowdown factor on the ground */
    [SerializeField] private float wallFriction = 10f;                  /* slowdown factor against walls */
    [SerializeField] private float limitVelocityAgainstWall = 4f;       /* speed limit when falling down against a wall */
    [SerializeField] private float landingSlowdownFactor = 0.3f;        /* reduction of horizontal speed when landing */
    [SerializeField] private float lockAfterWallJump = 0.27f;           /* time during which the player cannot go towards a wall after jumping from it */

    public enum JumpParameterMode
    {
        JUMP_GRAVITY_LOCKED,
        INPUT_SPEED_LOCKED,
        MAX_HEIGHT_LOCKED
    };
    [SerializeField] private JumpParameterMode jumpParameterMode = JumpParameterMode.INPUT_SPEED_LOCKED;    /* parameter you cannot change in the editor */
    [SerializeField] [Range(-100f, -0.1f)] private float jumpGravity = -60f;                                /* gravity during the jump ascension */
    [SerializeField] [Range(0.01f, 40f)] private float jumpInputSpeed = 20f;                                /* initial vertical speed when jumping */
    [SerializeField] [Range(0.5f, 10f)] private float jumpMaxHeight = 3.5f;                                 /* maximal reachable height while jumping */
    [SerializeField] [Range(-200f, -0.1f)] private float jumpCancelGravity = -130f;                         /* gravity when the player stops pushing the jump button */
    [SerializeField] [Range(-200f, -0.1f)] private float baseGravity = -90f;                                /* gravity when falling*/
    [SerializeField] private float coyoteTimeJump = 0.06f;                                                  /* time during which the player can jump after going pass the edge of a platform */
    [SerializeField] private float coyoteTimeWallJump = 0.06f;                                              /* time during which the player can still wallJump after moving away from a wall */

    [SerializeField] private float dashVelocity = 50f;  /* Dash speed */
    [SerializeField] private float dashDuration = 0.08f;  /* Dash duration */

    [SerializeField] private float propellerAcceleration = 100f;

    private enum SpecialState
    {
        STANDARD,
        DASHING,
    }
    private SpecialState currentPlayerState = SpecialState.STANDARD;
    private bool facingRight;

    private float horizontal = 0f;
    private float vertical = 0f;

    private float currentVelocityX = 0f;
    private float currentVelocityY = 0f;
    private bool againstLeftWall = false;
    private bool againstRightWall = false;
    private bool againstRoof = false;
    private bool grounded = false;                  /* grounded is true when the player is actually on the floor */
    private bool jumping = false;
    private float currentGravity = 0f;
    private float currentMaxHorizontalSpeed = 0f;
    private float baseAccelerationSpeed;
    private float baseDecelerationSpeed; 
    private float groundDecelerationSpeed;
    private float airDecelerationSpeed;
    private float smoothingHorizontalVelocity = 0f;
    private float currentVerticalFriction = 0f;
    private float currentHorizontalFriction = 0f;
    private bool canDoubleJump = false;
    private float coyoteTimer = 0f;
    private float leftWallJumpCoyoteTimer = 0f;
    private float rightWallJumpCoyoteTimer = 0f;
    private float lefthorizontalControlLock = 0f;
    private float righthorizontalControlLock = 0f;
    private float dashTimer = 0f;
    private bool canDash = false;

    private bool onPropeller = false;
    private bool propellerLimitBreak = false;

    private Vector3 currentSpawnPoint = new Vector3(-11.5f, 0f, 0f);

    void Start()
    {
        grounded = true;
        currentVelocityY = 0f;
        baseAccelerationSpeed = baseMovementSpeed / accelerationDuration;
        baseDecelerationSpeed = baseMovementSpeed / baseDecelerationDuration;
        groundDecelerationSpeed = baseMovementSpeed / groundDecelerationDuration;
        airDecelerationSpeed = baseMovementSpeed / airDecelerationDuration;
        currentVerticalFriction = 0f;
    }

    void Update()
    {

        // RECUPERATION DES INPUTS //

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        bool sprinting = Input.GetButton("Sprint");
        if (Input.GetButtonDown("Jump")) jumping = true;
        /* bool dashing = Input.GetButton("Dash"); */

        // DEFINITION DE L ETAT //

        lefthorizontalControlLock -= Time.deltaTime;
        righthorizontalControlLock -= Time.deltaTime;
        if (lefthorizontalControlLock > 0) horizontal = Mathf.Max(0f,horizontal);
        if (righthorizontalControlLock > 0) horizontal = Mathf.Min(0f,horizontal);

        //bool wasFacingRight = facingRight;
        if (horizontal == 1) facingRight = true;
        else if (horizontal == -1) facingRight = false;
        //if (wasFacingRight != facingRight) propellerLimitBreak = false;

        if (currentVelocityY < 0) /* Falling case */
        {
            currentGravity = baseGravity;
        }

        if (Input.GetButtonUp("Jump") && !grounded) /* Jump Cancel case */
        {
            currentGravity = jumpCancelGravity;
        }

        dashTimer -= Time.deltaTime;
        if (dashTimer < 0 && currentPlayerState == SpecialState.DASHING)
        {
            currentPlayerState = SpecialState.STANDARD;
        }

        coyoteTimer -= Time.deltaTime;
        leftWallJumpCoyoteTimer -= Time.deltaTime;
        rightWallJumpCoyoteTimer -= Time.deltaTime;

        // DETECTION DES COLLISIONS //

        if (!againstLeftWall && !againstRightWall && !grounded)
        {
            currentVerticalFriction = verticalAirFriction;
        }
        
        /* Left Collisions */
        bool wasAgainstLeftWall = againstLeftWall;
        againstLeftWall = isAgainstLeftWall();
        if (againstLeftWall)
        {
            currentVelocityX = Mathf.Max(currentVelocityX, 0f);
            leftWallJumpCoyoteTimer = coyoteTimeWallJump;
            propellerLimitBreak = false;
        }

        /* Right Collisions */
        bool wasAgainstRightWall = againstRightWall;
        againstRightWall = isAgainstRightWall();
        if (againstRightWall)
        {
            currentVelocityX = Mathf.Min(currentVelocityX, 0f);
            rightWallJumpCoyoteTimer = coyoteTimeWallJump;
            propellerLimitBreak = false;
        }

        /* Top Collisions*/
        bool wasAgainstRoof = againstRoof;
        againstRoof = isAgainstRoof(); 
        if (againstRoof && !wasAgainstRoof)
        {
            currentVelocityY = 0f;
            propellerLimitBreak = false;
        }

        /* Collisions au sol */
        onPropeller = false;
        bool wasGrounded = grounded;
        grounded = isGrounded();
        if (grounded && !wasGrounded)
        {
            currentGravity = 0f;    /* Arrêt du joueur et de la gravité à l'atterrissage */
            currentVelocityY = 0f; 
            currentHorizontalFriction = horizontalGroundFriction;
            currentVerticalFriction = 0f;
            canDoubleJump = false;
            jumping = false;
            if (horizontal == 0) currentVelocityX *= landingSlowdownFactor;
        }
        if (!grounded && wasGrounded && !jumping) /* Falling from a platform */
        {
            currentGravity = baseGravity;
            canDoubleJump = true;
            coyoteTimer = coyoteTimeJump;
            currentHorizontalFriction = horizontalAirFriction;
        }

        // REALISATION DU MOUVEMENT //

        // Gestion des déplacements horizontaux

        /* Dash */
        if (grounded && currentPlayerState == SpecialState.STANDARD) canDash = true;
        if (Input.GetButtonDown("Dash") && !(facingRight && againstRightWall) && !(!facingRight && againstLeftWall) && canDash)
        {
            dashTimer = dashDuration;
            currentPlayerState = SpecialState.DASHING;
            canDash = false;
            currentVelocityX = dashVelocity * (facingRight == true ? 1 : -1);
            lefthorizontalControlLock = dashDuration;
            righthorizontalControlLock = dashDuration;
        }

        /* Horizontal Speed Control */
        if (!propellerLimitBreak)
        {
            if (sprinting) currentMaxHorizontalSpeed = baseSprintingSpeed;   /* Choice of the current maximum speed */
            else currentMaxHorizontalSpeed = baseMovementSpeed;
        }
        
        if (onPropeller) 
        {
            currentVelocityX = (facingRight ? 1 : -1) * propellerAcceleration;
            currentMaxHorizontalSpeed = Mathf.Abs(currentVelocityX);
            if (facingRight) lefthorizontalControlLock = 0.05f;  /* Cannot change direction on propeller */
            else righthorizontalControlLock = 0.05f;
        } 

        /* Déplacements standards */
        if (horizontal > 0 && !againstRightWall)
        {
            if (currentVelocityX < 0) /* Opposite direction */
            {
                currentVelocityX += (baseDecelerationSpeed + currentHorizontalFriction) * Time.deltaTime;
            } 
            else /* Same direciton */
            {
                if (currentVelocityX <= currentMaxHorizontalSpeed)
                {
                    currentVelocityX += (baseAccelerationSpeed - currentHorizontalFriction) * Time.deltaTime;
                    currentVelocityX = Mathf.Clamp(currentVelocityX, 0f, currentMaxHorizontalSpeed);
                }
                else currentVelocityX -= currentHorizontalFriction * Time.deltaTime;
            }
            smoothingHorizontalVelocity = 0;
        }
        else if (horizontal < 0 && !againstLeftWall)
        {
            if (currentVelocityX > 0) /* Opposite direction */
            {
                currentVelocityX -= (baseDecelerationSpeed + currentHorizontalFriction) * Time.deltaTime;
            }
            else /* Same direciton */
            {
                if (currentVelocityX >= -currentMaxHorizontalSpeed)
                {
                    currentVelocityX -= (baseAccelerationSpeed - currentHorizontalFriction) * Time.deltaTime;
                    currentVelocityX = Mathf.Clamp(currentVelocityX, -currentMaxHorizontalSpeed, 0f);
                }
                else 
                currentVelocityX += currentHorizontalFriction * Time.deltaTime;
            }
            smoothingHorizontalVelocity = 0;
        }
        else if (horizontal == 0 && currentPlayerState != SpecialState.DASHING && !againstLeftWall && !againstRightWall)
        {
            if (grounded) currentVelocityX = Mathf.SmoothDamp(currentVelocityX, 0f, ref smoothingHorizontalVelocity, baseMovementSpeed / (groundDecelerationSpeed + currentHorizontalFriction));
            else currentVelocityX = Mathf.SmoothDamp(currentVelocityX, 0f, ref smoothingHorizontalVelocity, baseMovementSpeed / (airDecelerationSpeed + currentHorizontalFriction));
        }

        if (currentPlayerState != SpecialState.DASHING)
        {
            currentVelocityX = Mathf.Clamp(currentVelocityX, -currentMaxHorizontalSpeed, currentMaxHorizontalSpeed);
        }         

        // Gestion des déplacements verticaux
        currentVelocityY += (currentGravity - currentVerticalFriction * (currentVelocityY > 0 ? 1 : -1) ) * Time.deltaTime;

        if (againstLeftWall || againstRightWall) /* Limited velocity against walls */
        {
            currentVelocityY = Mathf.Max(currentVelocityY, -limitVelocityAgainstWall);
            if (currentVelocityY > 0) currentVerticalFriction = wallFriction;
            else currentVerticalFriction = 0;
        }
        if (currentPlayerState == SpecialState.DASHING) currentVelocityY = Mathf.Max(currentVelocityY, 0); /* Cannot fall during dash */

        if (Input.GetButtonDown("Jump") && (againstLeftWall || leftWallJumpCoyoteTimer >= 0) && !grounded) /* Left Wall Jump */
        {
            currentGravity = jumpGravity;
            currentVelocityY = jumpInputSpeed;
            currentVelocityX = baseMovementSpeed;
            currentHorizontalFriction = horizontalAirFriction;
            lefthorizontalControlLock = lockAfterWallJump;
        }
        if (Input.GetButtonDown("Jump") && (againstRightWall || rightWallJumpCoyoteTimer >= 0) && !grounded) /* Right Wall Jump */
        {
            currentGravity = jumpGravity;
            currentVelocityY = jumpInputSpeed;
            currentVelocityX = -baseMovementSpeed;
            currentHorizontalFriction = horizontalAirFriction;
            righthorizontalControlLock = lockAfterWallJump;
        }
        if (Input.GetButtonDown("Jump") && !grounded && canDoubleJump && !againstLeftWall && !againstRightWall && coyoteTimer < 0) /* Double Jump */
        {
            currentGravity = jumpGravity;
            currentVelocityY = jumpInputSpeed;
            currentHorizontalFriction = horizontalAirFriction;
            canDoubleJump = false;
        }

        if (Input.GetButtonDown("Jump") && (grounded || coyoteTimer >= 0)) /* Jump */
        {
            currentGravity = jumpGravity;
            currentVelocityY = jumpInputSpeed;
            currentHorizontalFriction = horizontalAirFriction;
            canDoubleJump = true;
            coyoteTimer = 0;
        }

        transform.Translate(currentVelocityX*Time.deltaTime, currentVelocityY*Time.deltaTime + 0.5f*currentGravity*Time.deltaTime*Time.deltaTime, 0);
    }


    bool isAgainstLeftWall()
    {
        int layerMask = 1 << 6;
        //layerMask = ~layerMask;
        foreach (Transform leftCast in leftCasts)
        {
            RaycastHit hit;
            if (Physics.Raycast(leftCast.position, Vector3.left, out hit, 0.2f, layerMask))
            {
                Debug.DrawRay(leftCast.position, Vector3.left, Color.yellow);
                if (hit.collider.CompareTag("DeathPlatform"))
                {
                    Die();
                }
                return true;
            }
        }
        return false;
    }

    bool isAgainstRightWall()
    {
        int layerMask = 1 << 6;
        //layerMask = ~layerMask;
        foreach (Transform rightCast in rightCasts)
        {
            RaycastHit hit;
            if (Physics.Raycast(rightCast.position, Vector3.right, out hit, 0.2f, layerMask))
            {
                Debug.DrawRay(rightCast.position, Vector3.right, Color.yellow);
                if (hit.collider.CompareTag("DeathPlatform"))
                {
                    Die();
                }
                return true;
            }
        }
        return false;
    }

    bool isAgainstRoof()
    {
        int layerMask = 1 << 6;
        //layerMask = ~layerMask;
        foreach (Transform topCast in topCasts)
        {
            RaycastHit hit;
            if (Physics.Raycast(topCast.position, Vector3.up, out hit, 0.2f, layerMask))
            {
                Debug.DrawRay(topCast.position, Vector3.up, Color.yellow);
                if (hit.collider.CompareTag("DeathPlatform"))
                {
                    Die();
                }
                if (!hit.collider.CompareTag("OneWayWall")) return true;
            }
        }
        return false;
    }

    bool isGrounded()
    {
        int layerMask = 1 << 6;
        //layerMask = ~layerMask;
        foreach (Transform downCast in downCasts)
        {
            RaycastHit hit;
            if (Physics.Raycast(downCast.position, Vector3.down, out hit, 0.5f, layerMask))
            {
                Debug.DrawRay(downCast.position, Vector3.down, Color.yellow);
                if (hit.collider.CompareTag("DeathPlatform"))
                {
                    Die();
                }
                propellerLimitBreak = false;
                if (hit.collider.CompareTag("HorizontalPropeller"))
                {
                    onPropeller = true;
                    propellerLimitBreak = true;
                }
                
                if (!hit.collider.CompareTag("OneWayWall")) return true;
                else if (currentVelocityY <= 0f)
                {
                    hit.collider.enabled = true;
                    return true;
                }
            }
        }
        return false;
    }


    void OnValidate ()
    {
        if (jumpParameterMode == JumpParameterMode.JUMP_GRAVITY_LOCKED)
        {
            jumpGravity = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpMaxHeight), -100f, -0.1f);

            jumpInputSpeed = Mathf.Clamp(Mathf.Sqrt( -2*jumpMaxHeight*jumpGravity ), 0.01f, 40f);
            jumpMaxHeight = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpGravity), 0.5f, 10f);
        }
        if (jumpParameterMode == JumpParameterMode.INPUT_SPEED_LOCKED)
        {
            jumpInputSpeed = Mathf.Clamp(Mathf.Sqrt( -2*jumpMaxHeight*jumpGravity ), 0.01f, 40f);

            jumpMaxHeight = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpGravity), 0.5f, 10f);
            jumpGravity = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpMaxHeight), -100f, -0.1f);
        }
        if (jumpParameterMode == JumpParameterMode.MAX_HEIGHT_LOCKED)
        { 
            jumpMaxHeight = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpGravity), 0.5f, 10f);
            
            jumpGravity = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpMaxHeight), -100f, -0.1f);
            jumpInputSpeed = Mathf.Clamp(Mathf.Sqrt( -2*jumpMaxHeight*jumpGravity ), 0.01f, 40f);
            
        }
        baseAccelerationSpeed = baseMovementSpeed / accelerationDuration;
        baseDecelerationSpeed = baseMovementSpeed / baseDecelerationDuration;
        groundDecelerationSpeed = baseMovementSpeed / groundDecelerationDuration;
        airDecelerationSpeed = baseMovementSpeed / airDecelerationDuration;

        verticalAirFriction = Mathf.Max(verticalAirFriction,0);
        horizontalAirFriction = Mathf.Max(horizontalAirFriction,0);
        wallFriction = Mathf.Max(wallFriction,0);
    }

    public bool isGoingDown()
    {
        return (vertical < 0);
    }

    public void SetCurrentLevelSpawnPoint(Vector3 spawnPosition)
    {
        currentSpawnPoint = spawnPosition;
    }

    public void Die()
    {
        transform.position = currentSpawnPoint;
        grounded = false;
        currentVelocityX = 0f;
        currentVelocityY = 0f;
        currentGravity = baseGravity;
    }
}
