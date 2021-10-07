using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private Collider platformCollider;
    
    void Start()
    {
        platformCollider = GetComponent<Collider>();
        platformCollider.enabled = false;
    }

    void Update()
    {
        if (GameManager.instance.GetPlayerY() - 0.4f >= transform.position.y + transform.localScale.y /2f && !GameManager.instance.isPlayerGoingDown())
        {
            platformCollider.enabled = true;
        }
        else platformCollider.enabled = false;
    }
}
