using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intel : MonoBehaviour, IInteractable
{
    bool interactable;

    void Awake()
    {
        interactable = true;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public bool CanInteract()
    {
        return interactable;
    }

    public void Interact()
    {
        if (!interactable) return;

        LevelManager.Instance.CollectIntel();

        interactable = false;
    }

    public string InteractString()
    {
        return "Collect (Intel)";
    }

    public void Reset()
    {
        interactable = true;
    }
}
