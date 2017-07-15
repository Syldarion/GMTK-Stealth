using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Scanner sightScanner;

    void Start()
    {

    }

    void Update()
    {
        sightScanner.ScanOrigin = transform.position;
    }
}
