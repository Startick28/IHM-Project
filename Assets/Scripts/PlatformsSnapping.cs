using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformsSnapping : MonoBehaviour
{
    [SerializeField] private bool isThin = false;

    void Update()
    {   float posX;
        float posY;
        if (isThin)
        {
            posX = Mathf.Round(transform.localPosition.x * 4f) / 4f;
            posY = Mathf.Round(transform.localPosition.y * 200f) / 200f;
            //posY = transform.localPosition.y;
        }
        else
        {
            posX = Mathf.Round(transform.localPosition.x * 4f) / 4f;
            posY = Mathf.Round(transform.localPosition.y * 4f) / 4f;
        }
        transform.localPosition = new Vector3(posX, posY, 0f);

        float scaleX;
        float scaleY;
        if (isThin)
        {
            scaleX = Mathf.Round(transform.localScale.x * 2f) /2f;
            scaleY = Mathf.Round(transform.localScale.y * 4f) / 4f;
        }
        else
        {
            scaleX = Mathf.Round(transform.localScale.x * 2f) /2f;
            scaleY = Mathf.Round(transform.localScale.y * 2f) / 2f;
        }
        
        transform.localScale = new Vector3(scaleX, scaleY, 2f);
    }
}
