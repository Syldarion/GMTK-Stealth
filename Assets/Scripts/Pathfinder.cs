using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathTile
{
    public enum Type
    {
        Wall,
        Floor
    }

    public Type TileType;
    public PathTile Parent;
    public int X;
    public int Y;
    public int F;
    public int G;
    public int H;

    public PathTile(Type type, int x, int y)
    {
        TileType = type;
        Parent = null;
        X = x;
        Y = y;
        F = G = H = 0;
    }

    public void SetScore(int g, int h)
    {
        G = g;
        H = h;
        F = G + H;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        PathTile tile = obj as PathTile;
        if (tile == null)
            return false;

        return (X == tile.X && Y == tile.Y);
    }

    public bool Equals(PathTile tile)
    {
        if (tile == null)
            return false;
        return (X == tile.X && Y == tile.Y);
    }

    public override int GetHashCode()
    {
        return X ^ Y;
    }
}

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder Instance;

    private static Vector2[] adjOffsets = new Vector2[]
    {
        new Vector2(-1, -1),
        new Vector2(0, -1),
        new Vector2(1, -1),
        new Vector2(-1, 0),
        new Vector2(1, 0),
        new Vector2(-1, 1),
        new Vector2(0, 1),
        new Vector2(1, 1)
    };

    private PathTile[,] currentLevelTiles;

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

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        List<PathTile> open = new List<PathTile>();
        List<PathTile> closed = new List<PathTile>();

        PathTile[,] pathTiles = new PathTile[
            currentLevelTiles.GetLength(0),
            currentLevelTiles.GetLength(1)];
        System.Array.Copy(currentLevelTiles, pathTiles, currentLevelTiles.Length);

        open.Add(pathTiles[(int)start.x, (int)start.z]);
        open[0].SetScore(
            0,
            (int)Vector2.Distance(start, end));

        PathTile target_tile = pathTiles[(int)end.x, (int)end.z];

        while (!closed.Contains(target_tile) && open.Count > 0)
        {
            PathTile lowest_f = open.First(x => x.F == open.Min(y => y.F));

            open.Remove(lowest_f);
            closed.Add(lowest_f);

            foreach(Vector2 offset in adjOffsets)
            {
                Vector2 index = new Vector2(lowest_f.X, lowest_f.Y) + offset;
                if (index.x < 0 || index.x >= pathTiles.GetLength(0) ||
                    index.y < 0 || index.y >= pathTiles.GetLength(1))
                    continue;

                PathTile tile = pathTiles[(int)index.x, (int)index.y];

                if (closed.Contains(tile) || tile.TileType != PathTile.Type.Floor)
                    continue;

                if (!open.Contains(tile))
                {
                    open.Add(tile);
                    tile.Parent = lowest_f;
                    tile.SetScore(
                        lowest_f.G + 10,
                        (int)Vector2.Distance(new Vector2(tile.X, tile.Y), end));
                }
                else if (lowest_f.G < tile.G)
                {
                    tile.Parent = lowest_f;
                    tile.SetScore(
                        lowest_f.G + 10,
                        (int)Vector2.Distance(new Vector2(tile.X, tile.Y), end));
                }
            }
        }

        List<Vector3> path = new List<Vector3>();

        PathTile current = target_tile;
        while(current.Parent != null)
        {
            path.Add(new Vector3(current.X, start.y, current.Y));
            current = current.Parent;
        }
        path.Reverse();

        return path;
    }

    public void CreatePathTileMap(int[,] tileMap)
    {
        PathTile[,] tiles = new PathTile[tileMap.GetLength(0), tileMap.GetLength(1)];

        for (int x = 0; x < tileMap.GetLength(0); x++)
        {
            for (int y = 0; y < tileMap.GetLength(1); y++)
            {
                switch (tileMap[x, y])
                {
                    case 0:
                        tiles[x, y] = new PathTile(PathTile.Type.Floor, x, y);
                        break;
                    case 1:
                        tiles[x, y] = new PathTile(PathTile.Type.Wall, x, y);
                        break;
                }
            }
        }

        currentLevelTiles = tiles;
    }

    private void ResetScores()
    {
        for(int x = 0; x < currentLevelTiles.GetLength(0); x++)
        {
            for(int y = 0; y < currentLevelTiles.GetLength(1); y++)
            {
                currentLevelTiles[x, y].F = 0;
            }
        }
    }
}
