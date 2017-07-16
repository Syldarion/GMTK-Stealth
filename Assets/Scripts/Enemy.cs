using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum BehaviorState
    {
        Patrolling,
        Alerted,
        Returning
    }

    public float MoveSpeed;
    public float PatrolSpeed;
    public float AlertedSpeed;

    public bool Questioning;
    public bool Alerted;

    private List<Vector3> patrolPath;
    private int currentPatrolStep;
    private List<Vector3> alertedPath;
    private int currentAlertStep;
    private List<Vector3> returnPath;
    private int currentReturnStep;
    private Rigidbody rBody;

    private BehaviorState currentState;

    void Awake()
    {
        rBody = GetComponent<Rigidbody>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            Alert(Player.Instance.transform.position);
        if (Input.GetKeyDown(KeyCode.N))
        {
            Questioning = true;
            Alerted = false;
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            Questioning = false;
            Alerted = true;
        }
        else if (Input.GetKeyDown(KeyCode.Comma))
        {
            Questioning = false;
            Alerted = false;
        }
    }

    public void GeneratePatrolPath(int startRoom, int endRoom)
    {
        Vector3 start = FloorGenerator.Instance.GetPointInRoom(startRoom);
        Vector3 end = FloorGenerator.Instance.GetPointInRoom(endRoom);
        start.y = 0.25f;
        end.y = 0.25f;

        Debug.Log(string.Format("patrol start: {0}, end: {1}", start, end));

        patrolPath = Pathfinder.Instance.FindPath(start, end);

        Debug.Log(patrolPath.Count);
    }

    public void MoveToStartOfPatrol()
    {
        transform.position = patrolPath[0];
    }

    public void StartPatrolPath()
    {
        MoveSpeed = PatrolSpeed;
        currentState = BehaviorState.Patrolling;
        currentPatrolStep = 0;
        StartCoroutine(MoveAlongPatrolPath());
    }

    public void Alert(Vector3 alertPos)
    {
        StopAllCoroutines();
        GenerateAlertPath(alertPos);
        StartAlertPath();
    }

    public void EndAlert()
    {
        StopAllCoroutines();
        GenerateReturnPath();
        StartReturnPath();
    }

    public void GenerateAlertPath(Vector3 alertPos)
    {
        Vector3 start = patrolPath[currentPatrolStep];

        start.y = 0.25f;
        alertPos.y = 0.25f;

        Debug.Log(string.Format("alert start: {0}, end: {1}", start, alertPos));

        alertedPath = Pathfinder.Instance.FindPath(start, alertPos);

        Debug.Log(alertedPath.Count);
    }

    public void StartAlertPath()
    {
        MoveSpeed = AlertedSpeed;
        currentState = BehaviorState.Alerted;
        currentAlertStep = 0;
        StartCoroutine(MoveAlongAlertPath());
    }

    public void GenerateReturnPath()
    {
        Vector3 start = alertedPath[currentAlertStep];
        Vector3 end = patrolPath[0];

        start.y = 0.25f;
        end.y = 0.25f;

        Debug.Log(string.Format("return start: {0}, end: {1}", start, end));

        returnPath = Pathfinder.Instance.FindPath(start, end);

        Debug.Log(returnPath.Count);
    }

    public void StartReturnPath()
    {
        MoveSpeed = PatrolSpeed;
        currentState = BehaviorState.Returning;
        currentReturnStep = 0;
        StartCoroutine(MoveAlongReturnPath());
    }

    private IEnumerator MoveAlongPatrolPath()
    {
        while (currentState == BehaviorState.Patrolling)
        {
            Vector3 movement;
            Vector3 next_point = patrolPath[currentPatrolStep];

            while(Vector3.Distance(transform.position, next_point) > 0.1f)
            {
                movement = (next_point - transform.position).normalized * MoveSpeed * Time.deltaTime;
                transform.Translate(movement);

                yield return null;
            }

            transform.position = next_point;
            currentPatrolStep++;

            if (currentPatrolStep == patrolPath.Count)
            {
                patrolPath.Reverse();
                currentPatrolStep = 0;
            }

            yield return null;
        }
    }

    private IEnumerator MoveAlongAlertPath()
    {
        while (currentState == BehaviorState.Alerted)
        {
            Vector3 movement;
            Vector3 next_point = alertedPath[currentAlertStep];

            while (Vector3.Distance(transform.position, next_point) > 0.1f)
            {
                movement = (next_point - transform.position).normalized * MoveSpeed * Time.deltaTime;
                transform.Translate(movement);

                yield return null;
            }

            transform.position = next_point;
            currentAlertStep++;

            if (currentAlertStep == alertedPath.Count)
            {
                currentAlertStep--;
                EndAlert();
            }

            yield return null;
        }
    }

    private IEnumerator MoveAlongReturnPath()
    {
        while (currentState == BehaviorState.Returning)
        {
            Vector3 movement;
            Vector3 next_point = returnPath[currentReturnStep];

            while (Vector3.Distance(transform.position, next_point) > 0.1f)
            {
                movement = (next_point - transform.position).normalized * MoveSpeed * Time.deltaTime;
                transform.Translate(movement);

                yield return null;
            }

            transform.position = next_point;
            currentReturnStep++;

            if (currentReturnStep == returnPath.Count)
            {
                StartPatrolPath();
            }

            yield return null;
        }
    }
}
