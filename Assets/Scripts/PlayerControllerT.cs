using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerT : MonoBehaviour
{
    [SerializeField] private Transform[] leftCasts;
    [SerializeField] private Transform[] rightCasts;
    [SerializeField] private Transform[] topCasts;
    [SerializeField] private Transform[] downCasts;

    [SerializeField] private float baseMovementSpeed = 2f;              /* horizontal speed on the ground */
    [SerializeField] private float accelerationDuration = 0.5f;         /* time it takes to reach base movement speed */
    [SerializeField] private float baseDecelerationDuration = 0.1f;     /* time it takes to reach to change direction */
    [SerializeField] private float groundDecelerationDuration = 0.1f;   /* time it takes to stop on the ground */
    [SerializeField] private float airDecelerationDuration = 0.5f;      /* time it takes to stop in the air */
    [SerializeField] private float verticalAirFriction = 0f;            /* vertical slowdown factor when in the air */
    [SerializeField] private float horizontalAirFriction = 0f;          /* horizontal slowdown factor when in the air */
    [SerializeField] private float horizontalGroundFriction = 0f;       /* slowdown factor on the ground */
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
    [SerializeField] private JumpParameterMode jumpParameterMode = JumpParameterMode.INPUT_SPEED_LOCKED; /* parameter you cannot change in the editor */
    [SerializeField] [Range(-50f, -0.1f)] private float jumpGravity = -50f;     /* gravity during the jump ascension */
    [SerializeField] [Range(0.01f, 40f)] private float jumpInputSpeed = 18f;    /* initial vertical speed when jumping */
    [SerializeField] [Range(0.5f, 10f)] private float jumpMaxHeight = 3.5f;     /* maximal reachable height while jumping */
    [SerializeField] [Range(-200f, -0.1f)] private float baseGravity = -800f;   /* gravity when falling or when the player stops pushing the jump button */

    [SerializeField] private float dashVelocity = 50f;  /* Dash speed */
    [SerializeField] private float dashDuration = 0.1f;  /* Dash duration */

    private enum SpecialState
    {
        STANDARD,
        DASHING,
    }
    private SpecialState currentPlayerState = SpecialState.STANDARD;
    private bool facingRight;

    private float currentVelocityX = 0f;
    private float currentVelocityY = 0f;
    private bool againstLeftWall = false;
    private bool againstRightWall = false;
    private bool againstRoof = false;
    private bool grounded = false;                  /* grounded is true when the player is actually on the floor */
    private float currentGravity = 0f;
    private float baseAccelerationSpeed;
    private float baseDecelerationSpeed; 
    private float groundDecelerationSpeed;
    private float airDecelerationSpeed;
    private float smoothingHorizontalVelocity = 0f;
    private float currentVerticalFriction = 0f;
    private float currentHorizontalFriction = 0f;
    private bool canDoubleJump = false;
    private float lefthorizontalControlLock = 0f;
    private float righthorizontalControlLock = 0f;
    private float dashTimer = 0f;
    private bool canDash = false;

    void Start()
    {
        baseAccelerationSpeed = baseMovementSpeed / accelerationDuration;
        baseDecelerationSpeed = baseMovementSpeed / baseDecelerationDuration;
        groundDecelerationSpeed = baseMovementSpeed / groundDecelerationDuration;
        airDecelerationSpeed = baseMovementSpeed / airDecelerationDuration;
        currentVerticalFriction = 0f;
    }

    void Update()
    {

        // RECUPERATION DES INPUTS //

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool jumping = Input.GetButton("Jump");
        bool dashing = Input.GetButton("Dash");

        // DEFINITION DE L ETAT //

        lefthorizontalControlLock -= Time.deltaTime;
        righthorizontalControlLock -= Time.deltaTime;
        if (lefthorizontalControlLock > 0) horizontal = Mathf.Max(0,horizontal);
        if (righthorizontalControlLock > 0) horizontal = Mathf.Min(0,horizontal);

        if (horizontal == 1) facingRight = true;
        else if (horizontal == -1) facingRight = false;

        if ((Input.GetButtonUp("Jump") && !grounded) || currentVelocityY < 0) /* Falling case */
        {
            currentGravity = baseGravity;
        }

        dashTimer -= Time.deltaTime;
        if (dashTimer < 0 && currentPlayerState == SpecialState.DASHING)
        {
            currentPlayerState = SpecialState.STANDARD;
        }

        // DETECTION DES COLLISIONS //

        if (!againstLeftWall && !againstRightWall && !grounded)
        {
            currentVerticalFriction = verticalAirFriction;
        }
        
        /* Left Collisions */
        bool wasAgainstLeftWall = againstLeftWall;
        againstLeftWall = isAgainstLeftWall();
        if (againstLeftWall && !wasAgainstLeftWall)
        {
            currentVelocityX = 0f;
        }

        /* Right Collisions */
        bool wasAgainstRightWall = againstRightWall;
        againstRightWall = isAgainstRightWall();
        if (againstRightWall && !wasAgainstRightWall)
        {
            currentVelocityX = 0f;
        }

        /* Top Collisions*/
        bool wasAgainstRoof = againstRoof;
        againstRoof = isAgainstRoof(); 
        if (againstRoof && !wasAgainstRoof)
        {
            currentVelocityY = 0f;
        }

        /* Collisions au sol */
        bool wasGrounded = grounded;
        grounded = isGrounded();
        if (grounded && !wasGrounded)
        {
            currentGravity = 0f;    /* Arrêt du joueur et de la gravité à l'atterrissage */
            currentVelocityY = 0f; 
            currentHorizontalFriction = horizontalGroundFriction;
            currentVerticalFriction = 0f;
            canDoubleJump = false;
            if (horizontal == 0) currentVelocityX *= landingSlowdownFactor;
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


        /* Déplacements standards */
        if (horizontal > 0 && !againstRightWall)
        {
            if (currentVelocityX < 0) currentVelocityX += (baseDecelerationSpeed + currentHorizontalFriction) * Time.deltaTime;
            else currentVelocityX += (baseAccelerationSpeed - currentHorizontalFriction) * Time.deltaTime;
        }
        else if (horizontal < 0 && !againstLeftWall)
        {
            if (currentVelocityX > 0) currentVelocityX -= (baseDecelerationSpeed + currentHorizontalFriction) * Time.deltaTime;
            else currentVelocityX -= (baseAccelerationSpeed - currentHorizontalFriction) * Time.deltaTime;
        }
        else if (horizontal == 0 && currentPlayerState != SpecialState.DASHING)
        {
            if (grounded) currentVelocityX = Mathf.SmoothDamp(currentVelocityX, 0f, ref smoothingHorizontalVelocity, baseMovementSpeed / (groundDecelerationSpeed + currentHorizontalFriction));
            else currentVelocityX = Mathf.SmoothDamp(currentVelocityX, 0f, ref smoothingHorizontalVelocity, baseMovementSpeed / (airDecelerationSpeed + currentHorizontalFriction));
        }
        if (currentPlayerState != SpecialState.DASHING) currentVelocityX = Mathf.Clamp(currentVelocityX, -baseMovementSpeed, baseMovementSpeed);
        

        // Gestion des déplacements verticaux
        currentVelocityY += (currentGravity - currentVerticalFriction * (currentVelocityY > 0 ? 1 : -1) ) * Time.deltaTime;

        if (againstLeftWall || againstRightWall) /* Limited velocity against walls */
        {
            currentVelocityY = Mathf.Max(currentVelocityY, -limitVelocityAgainstWall);
            if (currentVelocityY > 0) currentVerticalFriction = wallFriction;
            else currentVerticalFriction = 0;
        }
        if (currentPlayerState == SpecialState.DASHING) currentVelocityY = Mathf.Max(currentVelocityY, 0); /* Cannot fall during dash */

        if (Input.GetButtonDown("Jump") && againstLeftWall && !grounded) /* Left Wall Jump */
        {
            currentGravity = jumpGravity;
            currentVelocityY = jumpInputSpeed;
            currentVelocityX = baseMovementSpeed;
            currentHorizontalFriction = horizontalAirFriction;
            lefthorizontalControlLock = lockAfterWallJump;
        }
        if (Input.GetButtonDown("Jump") && againstRightWall && !grounded) /* Right Wall Jump */
        {
            currentGravity = jumpGravity;
            currentVelocityY = jumpInputSpeed;
            currentVelocityX = -baseMovementSpeed;
            currentHorizontalFriction = horizontalAirFriction;
            righthorizontalControlLock = lockAfterWallJump;
        }
        if (Input.GetButtonDown("Jump") && !grounded && canDoubleJump && !againstLeftWall && !againstRightWall) /* Double Jump */
        {
            currentGravity = jumpGravity;
            currentVelocityY = jumpInputSpeed;
            grounded = false;
            currentHorizontalFriction = horizontalAirFriction;
            canDoubleJump = false;
        }

        if (Input.GetButtonDown("Jump") && grounded) /* Jump */
        {
            currentGravity = jumpGravity;
            currentVelocityY = jumpInputSpeed;
            grounded = false;
            currentHorizontalFriction = horizontalAirFriction;
            canDoubleJump = true;
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
            if (Physics.Raycast(leftCast.position, Vector3.left, out hit, 0.201f, layerMask))
            {
                Debug.DrawRay(leftCast.position, Vector3.left, Color.yellow);
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
            if (Physics.Raycast(rightCast.position, Vector3.right, out hit, 0.201f, layerMask))
            {
                Debug.DrawRay(rightCast.position, Vector3.right, Color.yellow);
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
            if (Physics.Raycast(topCast.position, Vector3.up, out hit, 0.201f, layerMask))
            {
                Debug.DrawRay(topCast.position, Vector3.up, Color.yellow);
                return true;
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
            if (Physics.Raycast(downCast.position, Vector3.down, out hit, 0.201f, layerMask))
            {
                Debug.DrawRay(downCast.position, Vector3.down, Color.yellow);
                return true;
            }
        }
        return false;
    }


    void OnValidate ()
    {
        if (jumpParameterMode == JumpParameterMode.JUMP_GRAVITY_LOCKED)
        {
            jumpGravity = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpMaxHeight), -50f, -0.1f);

            jumpInputSpeed = Mathf.Clamp(Mathf.Sqrt( -2*jumpMaxHeight*jumpGravity ), 0.01f, 40f);
            jumpMaxHeight = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpGravity), 0.5f, 10f);
        }
        if (jumpParameterMode == JumpParameterMode.INPUT_SPEED_LOCKED)
        {
            jumpInputSpeed = Mathf.Clamp(Mathf.Sqrt( -2*jumpMaxHeight*jumpGravity ), 0.01f, 40f);

            jumpMaxHeight = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpGravity), 0.5f, 10f);
            jumpGravity = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpMaxHeight), -50f, -0.1f);
        }
        if (jumpParameterMode == JumpParameterMode.MAX_HEIGHT_LOCKED)
        { 
            jumpMaxHeight = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpGravity), 0.5f, 10f);
            
            jumpGravity = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpMaxHeight), -50f, -0.1f);
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
}
