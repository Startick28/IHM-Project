using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteInEditMode]
public class Properties : MonoBehaviour
{

    public enum CircleMode
    {
        A,
        B,
        C
    };
    [Range(0, 10)] public float _a;
    public float _b;
    public float _c;

    
    public CircleMode _mode;
    
    private float A
    {
        get { 
            return _a; 
        }
        set { _a = value;
        }
    }

    private float B
    {
        get { return _b; }
        set { _b = value; }
    }

    private float C
    {
        get { return _c; }
        set { _c = value; }
    }

    
    // Update is called once per frame
    void Update ()
    {
        if (_mode == CircleMode.A)
        {
            A = B/C;

            B = A * C;
            C = B / A;
        }
        if (_mode == CircleMode.B)
        {
            B = A * C;

            C = B / A;
            A = B/C;
        }
        if (_mode == CircleMode.C)
        { 
            C = B / A;
            
            A = B/C;
            B = A * C;
            
        }
    }
} 