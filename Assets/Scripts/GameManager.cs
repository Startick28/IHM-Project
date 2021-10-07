using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private PlayerControllerT playerPrefab;

    [SerializeField] private Transform firstSpawnPoint;

    private PlayerControllerT player;

    private float lateStart = 0.1f;

    void Start()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(this.gameObject);
        }

        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(lateStart);
        player = Instantiate(playerPrefab, firstSpawnPoint.position, Quaternion.identity);
        player.SetCurrentLevelSpawnPoint(firstSpawnPoint.position);
    }

    public float GetPlayerY()
    {
        if (player) return player.transform.position.y;
        else return 0f;
    }

    public bool isPlayerGoingDown()
    {
        if (player) return player.isGoingDown();
        else return false;
    }

}
