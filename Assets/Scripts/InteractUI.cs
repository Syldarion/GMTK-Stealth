using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractUI : MonoBehaviour
{
    public static InteractUI Instance;

    public InteractOptionUI InteractOptionPrefab;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void ClearOptions()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddInteractable(string interactKey, IInteractable interactable)
    {
        InteractOptionUI new_option = Instantiate(InteractOptionPrefab);
        new_option.transform.SetParent(transform, false);

        new_option.KeyText.text = interactKey;
        new_option.InteractText.text = interactable.InteractString();
    }
}
