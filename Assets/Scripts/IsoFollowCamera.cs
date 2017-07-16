using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoFollowCamera : MonoBehaviour
{
    public static IsoFollowCamera Instance;

    public Transform FollowTarget;
    public float FollowDistance;
    public float FollowHeight;
    public float FollowRotation;
    public float TargetRotation;
    public float RotationSpeed;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
            FollowRotation -= RotationSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.E))
            FollowRotation += RotationSpeed * Time.deltaTime;
        else
        {
            if (Mathf.Approximately(FollowRotation, TargetRotation))
                FollowRotation = TargetRotation;
            else
                FollowRotation = Mathf.Lerp(FollowRotation, TargetRotation, 10.0f * Time.deltaTime);
        }

        Vector3 follow_offset = new Vector3(
            FollowDistance * Mathf.Cos(FollowRotation * Mathf.Deg2Rad),
            FollowHeight,
            FollowDistance * Mathf.Sin(FollowRotation * Mathf.Deg2Rad));

        transform.position = FollowTarget.position + follow_offset;
        transform.LookAt(FollowTarget);
    }
}
