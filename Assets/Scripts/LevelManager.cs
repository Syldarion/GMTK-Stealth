using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public GameObject SuccessScreen;
    public GameObject FailScreen;

    public Transform AlertCanvas;
    public UIAlert AlertPrefab;

    public FloorEnd EndPiece;
    public Enemy EnemyPrefab;
    public int MinEnemyCount;
    public int MaxEnemyCount;

    public Crate CratePrefab;
    public Table TablePrefab;
    public Intel IntelPrefab;

    public int IntelCollected;
    public int TotalIntel;

    private int roomCount;
    private int spawnRoomIndex;
    private int exitRoomIndex;
    private int[] intelRoomIndices;

    private Vector3 spawnPosition;

    private List<Enemy> enemies;
    private List<Table> tables;
    private List<Intel> intel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        enemies = new List<Enemy>();
        intel = new List<Intel>();

        StartNewLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            RestartLevel();
    }

    public void StartNewLevel()
    {
        Player.Instance.gameObject.SetActive(true);

        FloorGenerator.Instance.CreateFloor();
        roomCount = FloorGenerator.Instance.Rooms().Count;
        Pathfinder.Instance.CreatePathTileMap(FloorGenerator.Instance.Tilemap());

        spawnRoomIndex = Random.Range(0, roomCount);
        do
        {
            exitRoomIndex = Random.Range(0, roomCount);
        } while (exitRoomIndex == spawnRoomIndex);

        spawnPosition = FloorGenerator.Instance.GetPointInRoom(spawnRoomIndex) + Vector3.up;

        Player.Instance.transform.position = spawnPosition;
        EndPiece.transform.position =
            FloorGenerator.Instance.GetPointInRoom(exitRoomIndex) + new Vector3(0.0f, 0.25f, 0.0f);
        EndPiece.gameObject.SetActive(false);
        
        SpawnEnemies();
        PlaceIntel();
        PlaceCrates();
    }

    public void SpawnEnemies()
    {
        int enemy_count = Random.Range(MinEnemyCount, MaxEnemyCount + 1);

        for (int i = 0; i < enemy_count; i++)
        {
            Enemy new_enemy = Instantiate(EnemyPrefab);

            int start, end;
            do
            {
                start = Random.Range(0, roomCount);
            } while (start == spawnRoomIndex);
            do
            {
                end = Random.Range(0, roomCount);
            } while (end == spawnRoomIndex || end == start);

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
        IntelCollected = 0;
        TotalIntel = 3;
        
        intelRoomIndices = new int[TotalIntel];

        for(int i = 0; i < TotalIntel; i++)
        {
            do
            {
                intelRoomIndices[i] = Random.Range(0, roomCount);
            } while (intelRoomIndices[i] == spawnRoomIndex || intelRoomIndices[i] == exitRoomIndex);

            Table new_table = Instantiate(TablePrefab);
            new_table.transform.position =
                FloorGenerator.Instance.GetPointInRoom(intelRoomIndices[i]) +
                new Vector3(0.0f, 0.15f, 0.0f);

            tables.Add(new_table);

            Intel new_intel = Instantiate(IntelPrefab);
            new_intel.transform.position = new_table.transform.position + 
                new Vector3(0.0f, 0.2f, 0.0f);

            intel.Add(new_intel);
        }
    }

    public void PlaceCrates()
    {
        for(int i = 0; i < roomCount; i++)
        {
            if (i == spawnRoomIndex) continue;

            int crate_count = Random.Range(0, 3);

            for(int j = 0; j < crate_count; j++)
            {
                Crate new_crate = Instantiate(CratePrefab);
                new_crate.transform.position =
                    FloorGenerator.Instance.GetPointInRoom(i) +
                    new Vector3(0.0f, 0.25f, 0.0f);
            }
        }
    }

    public void CollectIntel()
    {
        IntelCollected++;
        InfoUI.Instance.IntelInfoText.text =
            string.Format("Intel: {0} / {1}", IntelCollected, TotalIntel);
        if (IntelCollected >= TotalIntel)
        {
            EndPiece.gameObject.SetActive(true);
        }
    }

    public void RestartLevel()
    {
        Player.Instance.gameObject.SetActive(true);

        Player.Instance.transform.position = spawnPosition;
        
        foreach(Enemy enemy in enemies)
        {
            enemy.Questioning = false;
            enemy.Alerted = false;

            enemy.StopAllCoroutines();
            enemy.MoveToStartOfPatrol();
            enemy.StartPatrolPath();
        }

        foreach(Intel intelObj in intel)
        {
            intelObj.Reset();
        }

        IntelCollected = 0;
        InfoUI.Instance.IntelInfoText.text =
            string.Format("Intel: {0} / {1}", IntelCollected, TotalIntel);

        EndPiece.gameObject.SetActive(false);
    }

    public void CompleteLevel()
    {
        FloorGenerator.Instance.CleanupFloor();

        foreach(Enemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        foreach(Table table in tables)
        {
            Destroy(table.gameObject);
        }

        foreach(Intel intelObj in intel)
        {
            Destroy(intelObj.gameObject);
        }

        enemies.Clear();
        tables.Clear();
        intel.Clear();

        IntelCollected = 0;
        InfoUI.Instance.IntelInfoText.text =
            string.Format("Intel: {0} / {1}", IntelCollected, TotalIntel);

        Player.Instance.gameObject.SetActive(false);
        ShowSuccessScreen();
    }

    public void FailLevel()
    {
        Player.Instance.gameObject.SetActive(false);
        ShowFailScreen();
    }

    public void ShowSuccessScreen()
    {
        SuccessScreen.gameObject.SetActive(true);
    }

    public void HideSuccessScreen()
    {
        SuccessScreen.gameObject.SetActive(false);
    }

    public void ShowFailScreen()
    {
        FailScreen.gameObject.SetActive(true);
    }

    public void HideFailScreen()
    {
        FailScreen.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
