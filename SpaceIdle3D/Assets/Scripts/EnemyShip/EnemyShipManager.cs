using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class EnemyShipManager : MonoBehaviour
{
    [Header("Ship Spawn Settings")]
    [SerializeField] private GameObject enemyShipPrefab;
    [SerializeField] private Vector2 spawnDelayRange;
    [SerializeField] private float shipHeight;
    [SerializeField] private float shipDistance;
    [SerializeField] private float selfDestroyDelay;

    [Header("Ship Fall Settings")]
    [SerializeField] private float shipFallSpeed;
    [SerializeField] private Vector2 xTorqueLimit;
    [SerializeField] private Vector2 yTorqueLimit;
    [SerializeField] private Vector2 zTorqueLimit;

    [Header("Ship Reward Settings")]
    [SerializeField] private Vector2 speedRange;
    [SerializeField] private Vector2 iridiumRewardRange;
    [SerializeField] private Vector2 darkElixirRewardRange;
    [SerializeField] private Vector2 cosmiumRewardRange;

    private GameManager gameManager;

    public void WakeUp()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void StartGame()
    {
        StartSpawnTimer();
    }

    public void StartSpawnTimer()
    {
        float spawnDelay = Random.Range(spawnDelayRange.x, spawnDelayRange.y);

        StartCoroutine(SpawnCoroutine(spawnDelay));
    }
    private IEnumerator SpawnCoroutine(float spawnDelay)
    {
        yield return new WaitForSeconds(spawnDelay);

        SpawnShip();
    }

    [ContextMenu("Spawn Ship")]
    public void SpawnShip()
    {
        Vector2 randomPointInCircle = Random.insideUnitCircle.normalized;
        Vector3 spawnPoint = (new Vector3(randomPointInCircle.x, 0, randomPointInCircle.y).normalized * shipDistance) + (Vector3.up * shipHeight);
        float seed = Random.Range(0f, 1f);
        float rewardType = Random.Range(0f, 1f);

        float iridiumReward = 0;
        float darkElixirReward = 0;
        float cosmiumReward = 0;

        float shipSpeed = speedRange.x + ((speedRange.y - speedRange.x) * seed);

        if (rewardType >= 0f && rewardType <= 0.5f)
        {
            iridiumReward = iridiumRewardRange.x + ((iridiumRewardRange.y - iridiumRewardRange.x) * seed);
        }
        else if (rewardType >= 0.5f && rewardType <= 0.8f)
        {
            darkElixirReward = darkElixirRewardRange.x + ((darkElixirRewardRange.y - darkElixirRewardRange.x) * seed);
        }
        else if (rewardType >= 0.8f && rewardType <= 1.0f)
        {
            cosmiumReward = cosmiumRewardRange.x + ((cosmiumRewardRange.y - cosmiumRewardRange.x) * seed);
        }
        else
        {
            Debug.LogError("RewardType float malfunctioning!");
        }

        EnemyShip ship = Instantiate(enemyShipPrefab, spawnPoint, Quaternion.identity).GetComponent<EnemyShip>();
        ship.transform.LookAt(Vector3.zero + (Vector3.up * shipHeight));
        ship.enemyShipManager = this;
        ship.speed = shipSpeed;
        ship.fallSpeed = shipFallSpeed;
        ship.selfDestroyDelay = selfDestroyDelay;

        ship.iridiumReward = (gameManager.playerData.iridium_PerSecond * gameManager.playerData.iridium_PerSecondBoost) * iridiumReward;
        ship.darkElixirReward = darkElixirReward;
        ship.cosmiumReward = cosmiumReward;

        ship.xTorqueLimit = xTorqueLimit;
        ship.yTorqueLimit = yTorqueLimit;
        ship.zTorqueLimit = zTorqueLimit;

        StartSpawnTimer();
    }

    public void ShipDestroyed(double iridiumReward, double darkElixirReward, double cosmiumReward)
    {
        gameManager.playerData.iridium_Current += iridiumReward;
        gameManager.playerData.iridium_Total += iridiumReward;

        gameManager.playerData.darkElixir_Current += darkElixirReward;
        gameManager.playerData.darkElixir_Total += darkElixirReward;

        gameManager.playerData.cosmium_Total += cosmiumReward;
        gameManager.playerData.cosmium_Current += cosmiumReward;
    }
}
