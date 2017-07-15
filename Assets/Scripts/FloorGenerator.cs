﻿using System.Collections;
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
        if (Degree >= maxDegree || (Degree > 2 && Random.value >= 0.8f)) return;

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
    public Vector2 FloorSize;
    public static Vector2 MinRoomSize = new Vector2(5, 5);
    public static float MaxSizeRatio = 0.3f;

    public GameObject FloorPrefab;
    public GameObject WallPrefab;

    public int Degree;

    void Start()
    {
        CreateFloor();
    }

    void Update()
    {

    }

    public void CreateFloor()
    {
        BSPRoom root_room = new BSPRoom(Vector2.zero, FloorSize, 0);
        List<BSPRoom> rooms = new List<BSPRoom>();
        root_room.Split(Degree, ref rooms);

        rooms.Add(new BSPRoom(new Vector2(-2, -2), FloorSize + new Vector2(4, 4), 0));

        foreach(BSPRoom room in rooms)
        {
            Vector3 room_start = new Vector3(room.Position.x, 0.0f, room.Position.y);

            for(int x = 0; x < room.Size.x; x++)
            {
                Vector3 top_wall_pos = room_start + new Vector3(x, 0.0f, 0.0f);
                Vector3 bot_wall_pos = room_start + new Vector3(x, 0.0f, room.Size.y - 1);

                GameObject wall = Instantiate(WallPrefab, top_wall_pos, Quaternion.identity);
                wall = Instantiate(WallPrefab, bot_wall_pos, Quaternion.identity);
            }

            for (int y = 1; y < room.Size.y - 1; y++)
            {
                Vector3 left_wall_pos = room_start + new Vector3(0.0f, 0.0f, y);
                Vector3 right_wall_pos = room_start + new Vector3(room.Size.x - 1, 0.0f, y);

                GameObject wall = Instantiate(WallPrefab, left_wall_pos, Quaternion.identity);
                wall = Instantiate(WallPrefab, right_wall_pos, Quaternion.identity);
            }
        }
    }
}
