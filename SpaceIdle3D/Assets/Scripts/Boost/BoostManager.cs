using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(GameManager))]
public class BoostManager : MonoBehaviour
{
    public List<BoostSO> boostSOs;
    public List<Boost> activeBoosts = new List<Boost>();
    private GameManager gameManager;

    public Boost iridium_LowestTime;
    public Boost darkElixir_LowestTime;

    public void WakeUp()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void StartGame()
    {
        CalculateNonSerializedBoost();

        UpdateLowestTimeBoosts();
    }

    public void CalculateNonSerializedBoost()
    {
        activeBoosts = gameManager.playerData.activeBoosts;

        for (int i = 0; i < activeBoosts.Count; i++)
        {
            BoostSO currentBoostSO = gameManager.BoostManagerRef.GetBoostSO(activeBoosts[i].boost_Name);

            activeBoosts[i].boost_IridiumPerClick = currentBoostSO.boost_IridiumPerClick;
            activeBoosts[i].boost_IridiumPerSecond = currentBoostSO.boost_IridiumPerSecond;
            activeBoosts[i].boost_DarkElixirPerSecond = currentBoostSO.boost_DarkElixirPerSecond;
        }
    }

    public void LoadBoosts(List<Boost> boosts)
    {
        activeBoosts = boosts;

        UpdateLowestTimeBoosts();
    }

    public List<Boost> GetActiveBoosts()
    {
        return activeBoosts;
    }

    public void UpdateLowestTimeBoosts()
    {
        iridium_LowestTime = null;
        darkElixir_LowestTime = null;

        for (int i = 0; i < activeBoosts.Count; i++)
        {
            if (activeBoosts[i].boost_IridiumPerSecond == 1)
            {
                continue;
            }
            else
            {
                if (iridium_LowestTime == null)
                {
                    iridium_LowestTime = activeBoosts[i];
                }
                else
                {
                    if (iridium_LowestTime.boost_TimeRemaining > activeBoosts[i].boost_TimeRemaining)
                    {
                        iridium_LowestTime = activeBoosts[i];
                    }
                }
            }
        }

        for (int i = 0; i < activeBoosts.Count; i++)
        {
            if (activeBoosts[i].boost_DarkElixirPerSecond == 1)
            {
                continue;
            }
            else
            {
                if (darkElixir_LowestTime == null)
                {
                    darkElixir_LowestTime = activeBoosts[i];
                }
                else
                {
                    if (darkElixir_LowestTime.boost_TimeRemaining > activeBoosts[i].boost_TimeRemaining)
                    {
                        darkElixir_LowestTime = activeBoosts[i];
                    }
                }
            }
        }
    }

    public void ProcessBoostTimers()
    {
        for (int i = 0; i < activeBoosts.Count; i++)
        {
            activeBoosts[i].boost_TimeRemaining -= 1 / (float)GameManager.ticksPerSecond;

            if (activeBoosts[i].boost_TimeRemaining <= 0)
            {
                gameManager.AudioManagerRef.Play("BoostFinish");
                activeBoosts.RemoveAt(i);
                gameManager.UpdateResourceSources();
                UpdateLowestTimeBoosts();
            }
        }
    }

    public void AddBoost(BoostSO boostSO)
    {
        Boost boost = Array.Find(activeBoosts.ToArray(), x => x.boost_Name == boostSO.boost_Name);

        gameManager.AudioManagerRef.Play("BoostActivate");
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
            UpdateLowestTimeBoosts();
        }
    }

    public BoostSO GetBoostSO(string boostName)
    {
        BoostSO bSO = Array.Find(boostSOs.ToArray(), x => x.boost_Name == boostName);

        if (bSO == null)
        {
            Debug.LogError($"Boost \"{boostName}\" not found!");
        }

        return bSO;
    }
}
