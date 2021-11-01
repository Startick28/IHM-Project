using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private List<Transform> listOfCheckpoints;

    [SerializeField] private float speed;
    [SerializeField] private float waitingTime;

    int currentCheckpoint= 0;


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
            for (float time = 0; time < duration; time+=Time.fixedDeltaTime)
            {
                float t = time/duration;
                t= t*t*(3f-2f*t);
                transform.position = Vector3.Lerp(startPosition, endPosition, t);
                yield return new WaitForFixedUpdate();
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

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.transform.SetParent(transform);
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.transform.SetParent(null);
        }
    }

}
