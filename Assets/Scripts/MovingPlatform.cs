using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private List<Transform> listOfCheckpoints;

    [SerializeField] private float speed;
    [SerializeField] private float waitingTime;

    int currentCheckpoint= 0;
    float currentVelocityX;
    float currentVelocityY;
    Vector3 lastPosition;

    void Start()
    {
        StartCoroutine(FollowCheckpoints());
    }

    IEnumerator FollowCheckpoints()
    {
        while (true)
        {
            Vector3 startPosition = listOfCheckpoints[currentCheckpoint].position;
            Vector3 endPosition;
            if (currentCheckpoint + 1 < listOfCheckpoints.Count) endPosition = listOfCheckpoints[currentCheckpoint+1].position;
            else endPosition = listOfCheckpoints[0].position;
            
            float duration = Vector3.Distance(startPosition,endPosition) / speed;
            for (float time = 0; time < duration; time+=Time.deltaTime)
            {
                lastPosition = transform.position;
                float t = time/duration;
                t= t*t*(3f-2f*t);
                transform.position = Vector3.Lerp(startPosition, endPosition, t);
                currentVelocityX = (transform.position.x - lastPosition.x) / Time.deltaTime;
                currentVelocityY = (transform.position.y - lastPosition.y) / Time.deltaTime;
                yield return null;
            }            
            transform.position = endPosition;

            currentCheckpoint+=1;
            if (currentCheckpoint == listOfCheckpoints.Count)
            {
                currentCheckpoint = 0;
            }
            yield return new WaitForSeconds(waitingTime);
        }
    }

    public float GetVelocityX()
    {
        return currentVelocityX;
    }
    public float GetVelocityY()
    {
        return currentVelocityY;
    }
}
