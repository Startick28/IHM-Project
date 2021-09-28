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

    void Start()
    {
    }

    void Update()
    {

        // RECUPERATION DES INPUTS

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool jump = Input.GetButton("Jump");
        bool dash = Input.GetButton("Dash");


        // DEFINITION DE L ETAT





        if (jump) Debug.Log("Jump");
        if (dash) Debug.Log("Dash"); 
        



        // REALISATION DU MOUVEMENT

        transform.Translate(currentVelocityX * Time.deltaTime, currentVelocityY * Time.deltaTime, 0);

    }
}
