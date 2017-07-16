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
        transform.position = new Vector3(1.0f, 0.0f, 1.0f);
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
}
