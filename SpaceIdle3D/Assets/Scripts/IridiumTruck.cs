using System.Collections.Generic;
using UnityEngine;

public class IridiumTruck : MonoBehaviour
{
    [HideInInspector] public List<Transform> targets;
    [HideInInspector] public float moveSpeed = 5f;
    [HideInInspector] public float rotationSpeed = 5f;

    private float minimumDistance = 0.5f;
    private int currentTargetIndex = 0;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    public void StartMove(GameManager gameManager)
    {
        targets = gameManager.truckRoute;
        moveSpeed = gameManager.truckMoveSpeed;
        rotationSpeed = gameManager.truckRotationSpeed;
    }

    private void Update()
    {
        targetPosition = targets[currentTargetIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < minimumDistance)
        {
            currentTargetIndex = (currentTargetIndex + 1) % targets.Count;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("BunkerDoor"))
        {
            other.GetComponent<Animator>().Play("BunkerDoorOpen");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name.Equals("BunkerDoor"))
        {
            other.GetComponent<Animator>().Play("BunkerDoorClose");
        }
    }
}
