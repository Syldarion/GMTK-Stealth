using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour, IInteractable
{
    void Start()
    {

    }

    void Update()
    {

    }

    public bool CanInteract()
    {
        return true;
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
            return "Exit (Crate)";
        else
            return "Hide (Crate)";
    }
}
