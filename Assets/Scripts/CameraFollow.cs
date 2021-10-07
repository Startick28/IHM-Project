using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 currentLevelPosition;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Level"))
        {
            Camera.main.transform.position = new Vector3(col.transform.position.x, col.transform.position.y, -10f);
            Camera.main.orthographicSize = col.gameObject.GetComponent<Level>().cameraSize;
            gameObject.GetComponent<PlayerControllerT>().SetCurrentLevelSpawnPoint(col.gameObject.GetComponent<Level>().spawnPoint.position);
        }
    }
}
