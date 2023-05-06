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

    [HideInInspector] public float selfDestroyDelay;
    [HideInInspector] public float speed;
    [HideInInspector] public float fallSpeed;
    [HideInInspector] public Vector2 xTorqueLimit;
    [HideInInspector] public Vector2 yTorqueLimit;
    [HideInInspector] public Vector2 zTorqueLimit;

    private Vector2 mousePos;
    public GameObject HitTheGroundParticleSys;
    [SerializeField] private GameObject floatingTextReward;

    private bool hitByPlayer = false;
    private bool firstCollision = false;
    private Vector3 gravity;
    private Rigidbody rb;

    private Coroutine selfDestroyCoroutine;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        selfDestroyCoroutine = StartCoroutine(SelfDestroyCoroutine(selfDestroyDelay));
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
        if (!hitByPlayer)
        {
            float x = Random.Range(xTorqueLimit.x, xTorqueLimit.y);
            float y = Random.Range(yTorqueLimit.x, yTorqueLimit.y);
            float z = Random.Range(zTorqueLimit.x, zTorqueLimit.y);

            Vector3 randomTorque = new Vector3(x, y, z);

            rb.AddTorque(randomTorque, ForceMode.Impulse);

            gravity = fallSpeed * Vector3.down;

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(mousePos);
            hitByPlayer = true;
        }
    }

    private void ShipDestroyed()
    {
        firstCollision = true;
        enemyShipManager.ShipDestroyed(iridiumReward, darkElixirReward, cosmiumReward);
        Debug.Log($"Iridium: {iridiumReward} \nDark Elixir: {darkElixirReward} \nCosmium: {cosmiumReward}");
        GameObject particle = Instantiate(HitTheGroundParticleSys);

        if(iridiumReward > 0)
        {
            ShowReward(NumberFormatter.FormatNumber(iridiumReward, FormattingTypes.Iridium) + " Iridium");
        }
        else if(darkElixirReward > 0)
        {
            ShowReward(darkElixirReward.ToString("F2") + " Dark Elixir");
        }
        else if(cosmiumReward > 0)
        {
            ShowReward(cosmiumReward.ToString("F2") + " Cosmium");
        }
        else
        {
            ShowReward("0");
        }

        //Ship Destory Particles
        particle.transform.position = gameObject.transform.position - new Vector3(0f, 1f, 0f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitByPlayer && !firstCollision)
            ShipDestroyed();
    }

    private void OnDestroy()
    {
        StopCoroutine(selfDestroyCoroutine);
    }

    private IEnumerator SelfDestroyCoroutine(float timer)
    {
        yield return new WaitForSeconds(timer);

        Destroy(this.gameObject);
    }

    void ShowReward(string text)
    {
        if(floatingTextReward)
        {
            FloatingTextDestroy.floatTextDes.EnableObject(text, transform.position);
            //GameObject prefab = Instantiate(floatingTextReward, transform.position, Quaternion.identity);
            //prefab.GetComponentInChildren<TextMesh>().text = text;
        }
    }
}
