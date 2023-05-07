using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [HideInInspector] public List<Transform> targets;
    [HideInInspector] public float moveSpeed = 5f;
    [HideInInspector] public float rotationSpeed = 5f;

    private float minimumDistance = 0.5f;
    private int currentTargetIndex = 0;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    public void StartMove(List<Transform> route)
    {
        targets = route;
        currentTargetIndex = 0;
    }

    private void Update()
    {
        targetPosition = targets[currentTargetIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < minimumDistance)
        {
            if (currentTargetIndex == targets.Count - 1)
            {
                Destroy(gameObject);
            }
            currentTargetIndex = (currentTargetIndex + 1) % targets.Count;
        }
    }
}
