using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(GameManager))]
public class BoostManager : MonoBehaviour
{
    public List<BoostSO> boostSOs;
    public List<Boost> activeBoosts = new List<Boost>();
    private GameManager gameManager;

    public void WakeUp()
    {
        gameManager = GetComponent<GameManager>();
    }

    public List<Boost> GetActiveBoosts()
    {
        return activeBoosts;
    }

    public void LoadBoosts(List<Boost> activeBoosts)
    {
        if (activeBoosts == null)
            Debug.Log("activeBoosts is null");

        this.activeBoosts = activeBoosts;

        gameManager.UpdateResourceSources();
    }

    public void ProcessBoostTimers()
    {
        for (int i = 0; i < activeBoosts.Count; i++)
        {
            activeBoosts[i].boost_TimeRemaining -= 1 / (float)GameManager.ticksPerSecond;

            if (activeBoosts[i].boost_TimeRemaining <= 0)
            {
                activeBoosts.RemoveAt(i);
                gameManager.UpdateResourceSources();
            }
        }
    }

    public void AddBoost(BoostSO boostSO)
    {
        Boost boost = Array.Find(activeBoosts.ToArray(), x => x.boost_Name == boostSO.boost_Name);
        if (boost != null)
        {
            boost.boost_TimeRemaining += boostSO.boost_Duration;
            if (boost.boost_TimeRemaining > boostSO.boost_MaxDuration)
            {
                boost.boost_TimeRemaining = boostSO.boost_MaxDuration;
            }
        }
        else
        {
            boost = new Boost(boostSO);
            activeBoosts.Add(boost);
            gameManager.UpdateResourceSources();
        }
    }

    public BoostSO GetBoostSO(string boostName)
    {
        BoostSO bSO = Array.Find(boostSOs.ToArray(), x => x.boost_Name == boostName);

        if(bSO == null)
        {
            Debug.LogError($"Boost \"{boostName}\" not found!");
        }

        return bSO;
    }
}
