using System.Collections.Generic;
using UnityEngine;

public enum ActionAfterReach
{
    DeleteSelf,
    Animate
}

public class TroopMovement : MonoBehaviour
{
    [HideInInspector] public List<Transform> targets;
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;

    private bool reachedEndPoint;
    private float minimumDistance = 0.5f;
    private int currentTargetIndex = 0;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private ActionAfterReach actionAfterReach;
    private string animationName;

    public void StartMove(List<Transform> route, ActionAfterReach reachAction = ActionAfterReach.DeleteSelf, string afterReachAnimation = "")
    {
        targets = route;
        actionAfterReach = reachAction;
        animationName = afterReachAnimation;
    }

    private void Update()
    {
        if (reachedEndPoint) return;
        if(targets == null || targets.Count == 0) return;

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
