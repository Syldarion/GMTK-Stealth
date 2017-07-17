using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, IInteractable
{
    private bool interactable;

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
        if (Player.Instance.Hiding)
        {
            Player.Instance.Reveal();
        }
        else
        {
            Player.Instance.Hide(transform.position);
        }
    }

    public string InteractString()
    {
        if (Player.Instance.Hiding)
            return "Exit (Table)";
        else
            return "Hide (Table)";
    }
}
