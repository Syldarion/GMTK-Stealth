using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private Vector3 currentLevelSpawn;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartNewLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            RestartLevel();
    }

    public void StartNewLevel()
    {
        currentLevelSpawn = FloorGenerator.Instance.CreateFloor();
        Pathfinder.Instance.CreatePathTileMap(FloorGenerator.Instance.Tilemap());
        Player.Instance.transform.position = currentLevelSpawn + Vector3.up;
    }

    public void RestartLevel()
    {
        Player.Instance.transform.position = currentLevelSpawn + Vector3.up;
        //remake enemies
    }

    public void CompleteLevel()
    {
        FloorGenerator.Instance.CleanupFloor();

        StartNewLevel();
    }
}
