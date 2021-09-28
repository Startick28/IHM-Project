using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseControls : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool jump = Input.GetButton("Jump");
        bool dash = Input.GetButton("Dash");

        if (jump) Debug.Log("Jump");
        if (dash) Debug.Log("Dash"); 

        //float Y = Input.GetAxis("Y");
        //float B = Input.GetAxis("B");

        //transform.position = new Vector3(horizontal * circleRadius, vertical * circleRadius, 0f);

        /* spriteRenderer.color = Color.white;
        if (A > 0.1) spriteRenderer.color = Color.green;
        if (B > 0.1) spriteRenderer.color = Color.red;
        if (X > 0.1) spriteRenderer.color = Color.blue;
        if (Y > 0.1) spriteRenderer.color = Color.yellow; */
    }
}
