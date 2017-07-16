using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float MoveSpeed;

    private List<Vector3> patrolPath;
    private int currentPatrolStep;
    private List<Vector3> alertedPath;
    private int currentAlertStep;
    private List<Vector3> returnPath;
    private int currentReturnStep;
    private Rigidbody rBody;

    public bool patrolling;
    public bool alerted;
    public bool returning;

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
    }

    public void GeneratePatrolPath()
    {
        Vector3 start = FloorGenerator.Instance.GetRandomFloorPosition();
        Vector3 end = FloorGenerator.Instance.GetRandomFloorPosition();
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
        patrolling = true;
        currentPatrolStep = 0;
        StartCoroutine(MoveAlongPatrolPath());
    }

    public void Alert(Vector3 alertPos)
    {
        patrolling = false;
        alerted = true;

        StopAllCoroutines();
        GenerateAlertPath(alertPos);
        StartAlertPath();
    }

    public void EndAlert()
    {
        patrolling = true;
        alerted = false;

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
        alerted = true;
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
        returning = true;
        currentReturnStep = 0;
        StartCoroutine(MoveAlongReturnPath());
    }

    private IEnumerator MoveAlongPatrolPath()
    {
        while (patrolling)
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
        while (alerted)
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
                alerted = false;
                currentAlertStep--;
                EndAlert();
            }

            yield return null;
        }
    }

    private IEnumerator MoveAlongReturnPath()
    {
        while (returning)
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
                returning = false;
                StartPatrolPath();
            }

            yield return null;
        }
    }
}
