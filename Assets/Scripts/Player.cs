using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Scanner SightScanner;

    public float MoveSpeed;

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
        Vector3 movement = new Vector3(
            Input.GetAxis("Horizontal"), 
            0.0f, 
            Input.GetAxis("Vertical"));

        movement *= MoveSpeed * Time.deltaTime;

        transform.Translate(movement);
    }
}
