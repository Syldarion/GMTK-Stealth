using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public Transform AlertCanvas;
    public UIAlert AlertPrefab;

    public FloorEnd EndPiece;
    public Enemy EnemyPrefab;
    public int MinEnemyCount;
    public int MaxEnemyCount;

    private int spawnRoomIndex;
    private int exitRoomIndex;
    private int[] intelIndices;

    private List<Enemy> enemies;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        enemies = new List<Enemy>();

        StartNewLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            RestartLevel();
    }

    public void StartNewLevel()
    {
        FloorGenerator.Instance.CreateFloor();
        Pathfinder.Instance.CreatePathTileMap(FloorGenerator.Instance.Tilemap());

        int room_count = FloorGenerator.Instance.Rooms().Count;

        spawnRoomIndex = Random.Range(0, room_count);
        do
        {
            exitRoomIndex = Random.Range(0, room_count);
        } while (exitRoomIndex == spawnRoomIndex);

        Player.Instance.transform.position =
            FloorGenerator.Instance.GetPointInRoom(spawnRoomIndex) + Vector3.up;
        EndPiece.transform.position =
            FloorGenerator.Instance.GetPointInRoom(exitRoomIndex) + new Vector3(0.0f, 0.25f, 0.0f);
        
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        int enemy_count = Random.Range(MinEnemyCount, MaxEnemyCount + 1);
        int room_count = FloorGenerator.Instance.Rooms().Count;

        for (int i = 0; i < enemy_count; i++)
        {
            Enemy new_enemy = Instantiate(EnemyPrefab);

            int start, end;
            do
            {
                start = Random.Range(0, room_count);
            } while (start == spawnRoomIndex);
            do
            {
                end = Random.Range(0, room_count);
            } while (end == spawnRoomIndex);

            new_enemy.GeneratePatrolPath(start, end);
            new_enemy.MoveToStartOfPatrol();
            new_enemy.StartPatrolPath();

            enemies.Add(new_enemy);

            UIAlert new_alert = Instantiate(AlertPrefab);
            new_alert.transform.SetParent(AlertCanvas, false);
            new_alert.Setup(new_enemy);
        }
    }

    public void PlaceIntel()
    {
        int intel_count = 3;


    }

    public void RestartLevel()
    {
        Player.Instance.transform.position = new Vector3(1.0f, 1.0f, 1.0f);
        
        foreach(Enemy enemy in enemies)
        {
            enemy.StopAllCoroutines();
            enemy.MoveToStartOfPatrol();
            enemy.StartPatrolPath();
        }
    }

    public void CompleteLevel()
    {
        FloorGenerator.Instance.CleanupFloor();

        StartNewLevel();
    }
}
