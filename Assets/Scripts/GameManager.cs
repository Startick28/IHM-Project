using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private PlayerControllerTSafe playerPrefab;

    [SerializeField] private Transform firstSpawnPoint;

    [SerializeField] private Transform playgroundSpawnPoint;

    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    private PlayerControllerTSafe player;

    private float lateStart = 0.1f;

    public bool isGamePaused = false;

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
        UIManager.instance.SetPlayer(player.GetComponent<PlayerControllerTSafe>());
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

    public Vector3 GetLevelPosition(int numLevel)
    {
        return spawnPoints[numLevel].position;
    }

    public Vector3 GetLevelPlaygroundPosition()
    {
        return playgroundSpawnPoint.position;
    }

}
