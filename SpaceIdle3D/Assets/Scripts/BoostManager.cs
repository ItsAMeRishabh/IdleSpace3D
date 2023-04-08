using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class BoostManager : MonoBehaviour
{
    private GameManager gameManager;

    public void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void ProcessBoostTimers()
    {
        foreach (Boost boost in gameManager.playerData.boosts)
        {
            if (boost.boost_IsActive)
            {
                boost.boost_TimeRemaining -= 1 / (float)GameManager.ticksPerSecond;
                if (boost.boost_TimeRemaining <= 0)
                {
                    boost.boost_IsActive = false;
                }
            }
        }
    }
}
