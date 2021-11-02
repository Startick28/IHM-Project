using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingPlatform : MonoBehaviour
{
    Vector3 startPosition;
    [SerializeField] private float scrollingSpeed;
    
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(scrollingSpeed * Time.deltaTime, scrollingSpeed * Time.deltaTime, 0f);
        if (transform.position.x > 12f || transform.position.y > 6f ) transform.position = startPosition;
    }
}
