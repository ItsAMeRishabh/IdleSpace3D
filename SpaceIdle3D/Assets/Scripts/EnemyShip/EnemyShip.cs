using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyShip : MonoBehaviour
{
    [HideInInspector] public EnemyShipManager enemyShipManager;

    [HideInInspector] public double iridiumReward;
    [HideInInspector] public double darkElixirReward;
    [HideInInspector] public double cosmiumReward;

    [HideInInspector] public float speed;
    [HideInInspector] public float fallSpeed;
    [HideInInspector] public Vector2 xTorqueLimit;
    [HideInInspector] public Vector2 yTorqueLimit;
    [HideInInspector] public Vector2 zTorqueLimit;

    private Vector3 gravity;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.velocity = transform.forward * speed;
        gravity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    private void OnMouseDown()
    {
        if (!DetectClickOnUI.IsPointerOverUIElement())
        {
            if (gravity.Equals(Vector3.zero))
            {
                StartFall();
            }
        }
    }

    private void StartFall()
    {
        float x = Random.Range(xTorqueLimit.x, xTorqueLimit.y);
        float y = Random.Range(yTorqueLimit.x, yTorqueLimit.y);
        float z = Random.Range(zTorqueLimit.x, zTorqueLimit.y);

        Vector3 randomTorque = new Vector3(x, y, z);

        rb.AddTorque(randomTorque, ForceMode.Impulse);

        gravity = fallSpeed * Vector3.down;
    }

    private void ShipDestroyed()
    {
        enemyShipManager.ShipDestroyed(iridiumReward, darkElixirReward, cosmiumReward);
        Debug.Log($"Iridium: {iridiumReward} \nDark Elixir: {darkElixirReward} \nCosmium: {cosmiumReward}");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        ShipDestroyed();
    }
}
