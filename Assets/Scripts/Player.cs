using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public Scanner SightScanner;
    public float MoveSpeed;

    private bool scanning;
    private Rigidbody rBody;

    void Awake()
    {
        Instance = this;
        rBody = GetComponent<Rigidbody>();
    }

    void Start()
    {

    }

    void Update()
    {
        Movement();
        SightScanner.ScanOrigin = transform.position;
        
    }

    void Movement()
    {
        if (SightScanner.Scanning) return;

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
            transform.position,
            new Vector3(3, transform.position.y, 3));
        StartCoroutine(MoveAlongPath(path));
    }

    public IEnumerator MoveAlongPath(List<Vector3> path)
    {
        while(path.Count > 0)
        {
            Vector3 movement;
            Vector3 next_point = path[0];

            while(Vector3.Distance(transform.position, next_point) > 0.1f)
            {
                movement = (next_point - transform.position).normalized * MoveSpeed * Time.deltaTime;
                transform.Translate(movement);

                yield return null;
            }

            transform.position = path[0];
            path.RemoveAt(0);

            yield return null;
        }
    }
}
