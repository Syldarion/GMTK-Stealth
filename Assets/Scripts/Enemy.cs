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
    public float AlertWaitTime;
    public float SightRadius;

    public float QuestionResetTime;
    public bool Questioning;
    public bool Alerted;
    
    public bool Sighted;
    public float SightedDuration;

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
        CheckSight();
        if (Sighted)
            SightedDuration += Time.deltaTime;
        else
            SightedDuration = 0.0f;
        if(SightedDuration > 0.5f)
        {
            LevelManager.Instance.FailLevel();
            SightedDuration = 0.0f;
        }
    }

    public void CheckSight()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, SightRadius);
        foreach(Collider col in colliders)
        {
            if(col.tag == "Player")
            {
                RaycastHit hit;
                Physics.Raycast(
                    transform.position,
                    (col.transform.position - transform.position).normalized,
                    out hit,
                    SightRadius);
                Sighted = hit.collider == col && !col.GetComponent<Player>().Hiding;
            }
        }
    }

    public void GeneratePatrolPath(int startRoom, int endRoom)
    {
        Vector3 start = FloorGenerator.Instance.GetPointInRoom(startRoom);
        Vector3 end = FloorGenerator.Instance.GetPointInRoom(endRoom);
        start.y = 0.25f;
        end.y = 0.25f;
        patrolPath = Pathfinder.Instance.FindPath(start, end);
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

    public void Question()
    {
        Questioning = true;
        StartCoroutine(QuestionTimer());
    }

    IEnumerator QuestionTimer()
    {
        float timer = 0.0f;
        while(timer < QuestionResetTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Questioning = false;
    }

    public void Alert(Vector3 alertPos)
    {
        Questioning = false;
        Alerted = true;

        StopAllCoroutines();
        GenerateAlertPath(alertPos);
        StartAlertPath();
    }

    public void EndAlert()
    {
        Alerted = false;
        Questioning = false;

        StopAllCoroutines();
        GenerateReturnPath();
        StartReturnPath();
    }

    public void GenerateAlertPath(Vector3 alertPos)
    {
        Vector3 start = patrolPath[currentPatrolStep];
        start.y = 0.25f;
        alertPos.y = 0.25f;
        alertedPath = Pathfinder.Instance.FindPath(start, alertPos);
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
        returnPath = Pathfinder.Instance.FindPath(start, end);
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
            Vector3 start = transform.position;
            Vector3 next_point = patrolPath[currentPatrolStep];

            float progress = 0.0f;

            while(progress < 1.0f)
            {
                transform.position = Vector3.Lerp(
                    start,
                    next_point,
                    progress);
                
                progress += MoveSpeed * Time.deltaTime;
                yield return null;
            }
            
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
            Vector3 start = transform.position;
            Vector3 next_point = alertedPath[currentAlertStep];

            float progress = 0.0f;

            while (progress < 1.0f)
            {
                transform.position = Vector3.Lerp(
                    start,
                    next_point,
                    progress);

                progress += MoveSpeed * Time.deltaTime;
                yield return null;
            }
            
            currentAlertStep++;

            if (currentAlertStep == alertedPath.Count)
            {
                currentAlertStep--;
                yield return new WaitForSeconds(AlertWaitTime);
                EndAlert();
            }

            yield return null;
        }
    }

    private IEnumerator MoveAlongReturnPath()
    {
        while (currentState == BehaviorState.Returning)
        {
            Vector3 start = transform.position;
            Vector3 next_point = returnPath[currentReturnStep];

            float progress = 0.0f;

            while (progress < 1.0f)
            {
                transform.position = Vector3.Lerp(
                    start,
                    next_point,
                    progress);

                progress += MoveSpeed * Time.deltaTime;
                yield return null;
            }
            
            currentReturnStep++;

            if (currentReturnStep == returnPath.Count)
            {
                StartPatrolPath();
            }

            yield return null;
        }
    }
}
