using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform cameraTransform;

    private Vector3 currentLevelPosition;

    void Awake() => cameraTransform = GameObject.Find("MainCamera").GetComponent<Transform>();

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Level"))
        {
            cameraTransform.position = new Vector3(col.transform.position.x, col.transform.position.y, -10f);
            Camera.main.orthographicSize = col.gameObject.GetComponent<Level>().cameraSize;
            gameObject.GetComponent<PlayerControllerTSafe>().SetCurrentLevelSpawnPoint(col.gameObject.GetComponent<Level>().spawnPoint.position);
        }
    }
}
