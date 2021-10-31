using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerT : MonoBehaviour
{
    [SerializeField] private float baseMovementSpeed = 2f;
    [SerializeField] private float baseAccelerationSpeed = 2f;
    [SerializeField] private float baseDecelerationSpeed = 4f;

    private bool grounded = false;
    private float currentVelocityX = 0f;
    private float currentVelocityY = 0f;

    
    [SerializeField] [Range(-100f, -0.01f)] private float _gravity = -2f;
    private float Gravity 
    { 
        get { return _gravity; } 
        set { _gravity = value; SetJumpInputSpeed(); } 
    } 

    [SerializeField] [Range(0.01f, 500f)] private float _jumpInputSpeed = 2f;
    private float JumpInputSpeed 
    { 
        get { return _jumpInputSpeed; } 
        set { _jumpInputSpeed = value; SetJumpMaxHeight(); } 
    } 

    [SerializeField] [Range(0.1f, 10f)] private float _jumpMaxHeight = 2f;
    private float JumpMaxHeight 
    { 
        get { return _jumpMaxHeight; } 
        set { _jumpMaxHeight = value; SetGravity(); } 
    } 
    // jumpMaxHeight = - jumpInputSpeed ^2 / ( 2 * gravity)
    // jumpInputSpeed = sqrt ( - 2 * jumpMaxHeight * gravity )
    // gravity = - jumpInputSpeed ^2  / ( 2 * jumpMaxHeight )

    

    private void SetJumpInputSpeed()
    {
        _jumpInputSpeed = Mathf.Sqrt( -2*_jumpMaxHeight*_gravity );
        Debug.Log("yo");
    }

    private void SetJumpMaxHeight()
    {
        _jumpMaxHeight = - _jumpInputSpeed * _jumpInputSpeed / (2*_gravity);
    }

    private void SetGravity()
    {
        _gravity = - _jumpInputSpeed * _jumpInputSpeed / (2*_jumpMaxHeight);
    }

    // Variables for animations
    private bool isSpawning = true;

    void Start()
    {
<<<<<<< Updated upstream
=======
        grounded = true;
        currentVelocityY = 0f;
        baseAccelerationSpeed = baseMovementSpeed / accelerationDuration;
        baseDecelerationSpeed = baseMovementSpeed / baseDecelerationDuration;
        groundDecelerationSpeed = baseMovementSpeed / groundDecelerationDuration;
        airDecelerationSpeed = baseMovementSpeed / airDecelerationDuration;
        currentVerticalFriction = 0f;
        gameObject.transform.localScale = new Vector3(0.1f, 0.1f,0.1f);
>>>>>>> Stashed changes
    }

    void Update()
    {
        // Animations

        // Spawn

        if (isSpawning)
        {
            if(gameObject.transform.localScale.x < 1f)
            {
                gameObject.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
            }
            else
            {
                isSpawning = false;
            }
        }

        // RECUPERATION DES INPUTS

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool jump = Input.GetButton("Jump");
        bool dash = Input.GetButton("Dash");

<<<<<<< Updated upstream
=======
        bool sprinting = Input.GetButton("Sprint");
        if (sprinting)
        { 
            ParticleManager.instance.startSprintParticle();
        }

        if (Input.GetButtonDown("Jump")) jumping = true;
>>>>>>> Stashed changes

        // DEFINITION DE L ETAT





        if (jump) Debug.Log("Jump");
        if (dash) Debug.Log("Dash"); 
        
<<<<<<< Updated upstream
=======
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
            ParticleManager.instance.startCollisionParticle();
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
            ParticleManager.instance.startDashParticle();
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

        if (grounded) currentVelocityY = 0f; /* Not clipping through walls guarantee */

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
>>>>>>> Stashed changes



        // REALISATION DU MOUVEMENT

        transform.Translate(currentVelocityX * Time.deltaTime, currentVelocityY * Time.deltaTime, 0);

<<<<<<< Updated upstream
=======
        if (currentVelocityY >= 0 && currentPlayerState != SpecialState.DASHING) spriteTransform.localScale = new Vector3( Mathf.Lerp(0.9f,1f, tmp* tmp), Mathf.Lerp(1.15f,1f, tmp), 1f);
        if (grounded || againstLeftWall || againstRightWall || currentPlayerState == SpecialState.DASHING) spriteTransform.localScale = new Vector3(1f, 1f, 1f); 

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
        // Activation des particules de mort
        ParticleManager.instance.startDeathParticle();
        transform.position = currentSpawnPoint;

        isSpawning = true;
        gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        grounded = false;
        currentVelocityX = 0f;
        currentVelocityY = 0f;
        currentGravity = baseGravity;
>>>>>>> Stashed changes
    }
}
