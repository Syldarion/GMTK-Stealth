using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public Scanner SightScanner;
    public Vector3 HiddenPosition;
    public float MoveSpeed;
    public float InteractRange;
    public bool Hiding;

    private bool scanning;
    private Rigidbody rBody;
    private MeshRenderer mRenderer;
    
    private List<IInteractable> availableInteractables;

    private KeyCode[] interactKeys =
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5
    };

    private string[] interactKeyTexts =
    {
        "1",
        "2",
        "3",
        "4",
        "5"
    };

    private

    void Awake()
    {
        Instance = this;
        rBody = GetComponent<Rigidbody>();
        mRenderer = GetComponent<MeshRenderer>();
        transform.position = new Vector3(1.0f, 0.0f, 1.0f);

        availableInteractables = new List<IInteractable>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (!Hiding)
            Movement();
        SightScanner.ScanOrigin = Hiding ? HiddenPosition : transform.position;

        CheckForInteractables();
        UpdateInteractUI();

        for (int i = 0; i < interactKeys.Length; i++)
        {
            if (Input.GetKeyDown(interactKeys[i]) &&
                availableInteractables.Count > i)
            {
                availableInteractables[i].Interact();
            }
        }
    }

    void CheckForInteractables()
    {
        availableInteractables.Clear();

        Collider[] interactables = Physics.OverlapSphere(
            transform.position, 
            InteractRange, 
            1 << 9);
        foreach(Collider col in interactables)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable.CanInteract())
                availableInteractables.Add(interactable);
        }
    }

    void UpdateInteractUI()
    {
        InteractUI.Instance.ClearOptions();
        for (int i = 0; i < interactKeys.Length && i < availableInteractables.Count; i++)
            InteractUI.Instance.AddInteractable(
                interactKeyTexts[i],
                availableInteractables[i]);
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

    public void Hide(Vector3 hidePos)
    {
        Hiding = true;

        HiddenPosition = hidePos;

        mRenderer.enabled = false;
    }

    public void Reveal()
    {
        Hiding = false;

        mRenderer.enabled = true;
    }
}
