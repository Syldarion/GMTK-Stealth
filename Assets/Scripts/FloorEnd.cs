using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorEnd : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
            LevelManager.Instance.CompleteLevel();
    }
}
