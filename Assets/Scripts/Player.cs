using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public Scanner SightScanner;

    public float MoveSpeed;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    void Update()
    {
        Movement();
        SightScanner.ScanOrigin = transform.position;

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartPath();
        }
    }

    void Movement()
    {
        Vector3 movement = new Vector3(
            Input.GetAxis("Horizontal"), 
            0.0f, 
            Input.GetAxis("Vertical"));

        movement *= MoveSpeed * Time.deltaTime;

        transform.Translate(movement, Space.World);
    }

    public void StartPath()
    {
        List<Vector3> path = Pathfinder.Instance.FindPath(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(3, 3));
    }
}
