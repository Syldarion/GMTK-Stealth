using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPRoom
{
    public enum Orientation { Horizontal, Vertical }
    
    public Vector2 Position;
    public Vector2 Size;
    public BSPRoom Left, Right;
    public int Degree;
    public Orientation SplitOrientation;


    public BSPRoom(Vector2 position, Vector2 size, int degree)
    {
        Position = position;
        Size = size;
        Left = null;
        Right = null;
        Degree = degree;
    }

    public void Split(int maxDegree, ref List<BSPRoom> rooms)
    {
        float split_chance = 0.75f + (maxDegree - Degree) * 0.1f;

        if (Degree >= maxDegree || Random.value >= split_chance) return;

        bool can_split_vert = Size.x > 2 * FloorGenerator.MinRoomSize.x + 3;
        bool can_split_hori = Size.y > 2 * FloorGenerator.MinRoomSize.y + 3;

        Vector2 left_start = Vector2.zero;
        Vector2 right_start = Vector2.zero;
        Vector2 left_size = FloorGenerator.MinRoomSize;
        Vector2 right_size = FloorGenerator.MinRoomSize;

        if(can_split_vert && can_split_hori)
        {
            SplitOrientation = (Orientation)Random.Range(0, 2);
        }
        else if (can_split_vert)
        {
            SplitOrientation = Orientation.Vertical;
        }
        else if (can_split_hori)
        {
            SplitOrientation = Orientation.Horizontal;
        }
        else
        {
            return;
        }

        if(SplitOrientation == Orientation.Vertical)
        {
            int split = Mathf.RoundToInt(
                Random.Range(
                    FloorGenerator.MinRoomSize.x + 1, 
                    Size.x - FloorGenerator.MinRoomSize.x - 1));

            left_start = Position;
            left_size = new Vector2(split, Size.y);
            right_start = new Vector2(left_start.x + left_size.x + 1, left_start.y);
            right_size = new Vector2(Size.x - split - 1, Size.y);
        }
        else if(SplitOrientation == Orientation.Horizontal)
        {
            int split = Mathf.RoundToInt(
                Random.Range(
                    FloorGenerator.MinRoomSize.y + 1,
                    Size.y - FloorGenerator.MinRoomSize.y - 1));

            left_start = Position;
            left_size = new Vector2(Size.x, split);
            right_start = new Vector2(left_start.x, left_start.y + left_size.y + 1);
            right_size = new Vector2(Size.x, Size.y - split - 1);
        }

        rooms.Remove(this);

        Left = new BSPRoom(left_start, left_size, Degree + 1);
        Right = new BSPRoom(right_start, right_size, Degree + 1);

        rooms.Add(Left);
        rooms.Add(Right);

        Left.Split(maxDegree, ref rooms);
        Right.Split(maxDegree, ref rooms);
    }
}

public class FloorGenerator : MonoBehaviour
{
    public static FloorGenerator Instance;

    public Vector2 FloorSize;
    public static Vector2 MinRoomSize = new Vector2(5, 5);
    public static float MaxSizeRatio = 0.3f;

    public GameObject FloorPrefab;
    public GameObject WallPrefab;

    public int Degree;

    private int[,] tileMap;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    //returns random corner for spawn
    public Vector3 CreateFloor()
    {
        CreateTiles(CreateTilemap());

        Vector3[] corners = new Vector3[]
        {
            new Vector3(1.0f, 0.0f, 1.0f),
            new Vector3(1.0f, 0.0f, FloorSize.y - 2),
            new Vector3(FloorSize.x - 2, 0.0f, 1.0f),
            new Vector3(FloorSize.x - 2, FloorSize.y - 2)
        };

        return corners[Random.Range(0, 4)];
    }

    public int[,] CreateTilemap()
    {
        BSPRoom root_room = new BSPRoom(Vector2.zero, FloorSize, 0);
        List<BSPRoom> rooms = new List<BSPRoom>();
        root_room.Split(Degree, ref rooms);

        rooms.Add(root_room);

        int[,] tile_map = new int[(int)FloorSize.x, (int)FloorSize.y];

        foreach (BSPRoom room in rooms)
        {
            int top = (int)room.Position.y;
            int bottom = top + (int)room.Size.y - 1;
            int left = (int)room.Position.x;
            int right = left + (int)room.Size.x - 1;

            for (int x = 0; x < room.Size.x; x++)
            {
                tile_map[left + x, top] = 1;
                tile_map[left + x, bottom] = 1;
            }

            for (int y = 1; y < room.Size.y - 1; y++)
            {
                tile_map[left, top + y] = 1;
                tile_map[right, top + y] = 1;
            }
        }

        tileMap = new int[(int)FloorSize.x, (int)FloorSize.y];
        System.Array.Copy(tile_map, tileMap, tile_map.Length);
        return tile_map;
    }

    public void CreateTiles(int[,] tileMap)
    {
        for (int x = 0; x < FloorSize.x; x++)
        {
            for (int y = 0; y < FloorSize.y; y++)
            {
                Vector3 tile_pos = new Vector3(x, 0.0f, y);

                GameObject tile = Instantiate(
                    tileMap[x, y] == 0 ? FloorPrefab : WallPrefab,
                    tile_pos, Quaternion.identity);
                tile.transform.SetParent(transform, true);
            }
        }
    }

    public void CleanupFloor()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public int[,] Tilemap()
    {
        return tileMap;
    }
}
