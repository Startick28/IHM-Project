using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParameterSettings : MonoBehaviour
{
    public enum JumpParameterMode
    {
        GRAVITY_LOCKED,
        INPUT_SPEED_LOCKED,
        MAX_HEIGHT_LOCKED
    };
    [SerializeField] [Range(-30f, -0.1f)] private float gravity = -10f;
    [SerializeField] [Range(0.01f, 20f)] private float jumpInputSpeed = 5f;
    [SerializeField] [Range(0.5f, 10f)] private float jumpMaxHeight = 3f;

    
    [SerializeField] private JumpParameterMode jumpParameterMode = JumpParameterMode.GRAVITY_LOCKED;

    
    // Update is called once per frame
    void OnValidate ()
    {
        if (jumpParameterMode == JumpParameterMode.GRAVITY_LOCKED)
        {
            gravity = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpMaxHeight), -30f, -0.1f);

            jumpInputSpeed = Mathf.Clamp(Mathf.Sqrt( -2*jumpMaxHeight*gravity ), 0.01f, 20f);
            jumpMaxHeight = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*gravity), 0.5f, 10f);
        }
        if (jumpParameterMode == JumpParameterMode.INPUT_SPEED_LOCKED)
        {
            jumpInputSpeed = Mathf.Clamp(Mathf.Sqrt( -2*jumpMaxHeight*gravity ), 0.01f, 20f);

            jumpMaxHeight = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*gravity), 0.5f, 10f);
            gravity = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpMaxHeight), -30f, -0.1f);
        }
        if (jumpParameterMode == JumpParameterMode.MAX_HEIGHT_LOCKED)
        { 
            jumpMaxHeight = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*gravity), 0.5f, 10f);
            
            gravity = Mathf.Clamp(-jumpInputSpeed * jumpInputSpeed / (2*jumpMaxHeight), -30f, -0.1f);
            jumpInputSpeed = Mathf.Clamp(Mathf.Sqrt( -2*jumpMaxHeight*gravity ), 0.01f, 20f);
            
        }
    }
}
