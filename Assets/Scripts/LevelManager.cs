using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public FloorEnd EndPiece;
    public Enemy EnemyPrefab;
    public int MinEnemyCount;
    public int MaxEnemyCount;

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
        Player.Instance.transform.position = new Vector3(1.0f, 1.0f, 1.0f);

        PlaceEndPoint();
        SpawnEnemies();
    }

    public void PlaceEndPoint()
    {
        int[,] tile_map = FloorGenerator.Instance.Tilemap();
        
        Vector3[] corners = new Vector3[]
        {
            new Vector3(1.0f, 0.25f, tile_map.GetLength(1) - 2),
            new Vector3(tile_map.GetLength(0) - 2, 0.25f, 1.0f),
            new Vector3(tile_map.GetLength(0) - 2, 0.25f, tile_map.GetLength(1) - 2)
        };
        
        EndPiece.transform.position = corners[Random.Range(0, 3)];
    }

    public void SpawnEnemies()
    {
        int enemy_count = Random.Range(MinEnemyCount, MaxEnemyCount + 1);

        for(int i = 0; i < enemy_count; i++)
        {
            Enemy new_enemy = Instantiate(EnemyPrefab);
            new_enemy.GeneratePatrolPath();
            new_enemy.MoveToStartOfPatrol();
            new_enemy.StartPatrolPath();

            enemies.Add(new_enemy);
        }
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
