using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class NPCManager : MonoBehaviour
{
    [SerializeField] private float NPC_MoveSpeed = 15f;
    [SerializeField] private float NPC_RotateSpeed = 10f;
    [SerializeField] private float NPC_SpawnCheckDelay;
    [SerializeField] private float minIPSToSpawnNPC;
    [SerializeField] private float maxIPSToSpawnNPC;
    [SerializeField] private List<GameObject> NPC_Prefabs;
    [SerializeField] private Transform NPC_Routes;

    private WaitForSeconds delay;
    private GameManager gameManager;

    public void StartGame()
    {
        gameManager = GetComponent<GameManager>();
        delay = new WaitForSeconds(NPC_SpawnCheckDelay);
        StartCoroutine(CheckSpawnConditions());
    }

    public IEnumerator CheckSpawnConditions()
    {
        while (true)
        {
            yield return delay;

            if (gameManager.playerData.iridium_PerSecond >= minIPSToSpawnNPC)
            {
                float spawnChance = (float)(gameManager.playerData.iridium_PerSecond - minIPSToSpawnNPC) / (maxIPSToSpawnNPC - minIPSToSpawnNPC);
                spawnChance = Mathf.Clamp(spawnChance, 0.25f, 0.80f);

                if(Random.Range(0f,1f) < spawnChance)
                {
                    SpawnNPC();
                }
            }
        }
    }

    [ContextMenu("Spawn NPC")]
    public void SpawnNPC()
    {
        int npcChoice = Random.Range(0, NPC_Prefabs.Count);
        int routeChoice = Random.Range(0, NPC_Routes.childCount);

        List<Transform> finalRoute = new List<Transform>();

        for(int i = 0; i< NPC_Routes.GetChild(routeChoice).childCount;i++)
        {
            finalRoute.Add(NPC_Routes.GetChild(routeChoice).GetChild(i));
        }

        GameObject spawnedNPC = Instantiate(NPC_Prefabs[npcChoice]);
        spawnedNPC.transform.position = NPC_Routes.GetChild(routeChoice).position;
        spawnedNPC.GetComponent<NPCMovement>().moveSpeed = NPC_MoveSpeed;
        spawnedNPC.GetComponent<NPCMovement>().rotationSpeed = NPC_RotateSpeed;
        spawnedNPC.GetComponent<NPCMovement>().StartMove(finalRoute);
    }
}
