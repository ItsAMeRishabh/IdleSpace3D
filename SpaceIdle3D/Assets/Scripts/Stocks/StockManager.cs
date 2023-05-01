using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(GameManager))]
public class StockManager : MonoBehaviour
{
    public string stockName;
    public double stockBaseValue;
    public double stockCurrentValue;
    public Vector2 stockVariance;
    public double stockRefreshTime = 3600.0;

    public long stockMinimumBuy;
    public long nextStep;
    public long megaNextStep;
    public long previousStep;
    public long megaPreviousStep;

    public long stockOwned;
    public long amountToBuy;

    public double totalPrice;
    public double totalPricePlusNext;
    public double totalPricePlusMegaNext;

    private GameManager gameManager;

    public void InitializeStocks()
    {
        gameManager = GetComponent<GameManager>();
        amountToBuy = stockMinimumBuy;

        RefreshStockValue();
    }

    [ContextMenu("Refresh Stock Price")]
    public void RefreshStockValue()
    {
        float currentStockVariance = UnityEngine.Random.Range(stockVariance.x, stockVariance.y);
        stockCurrentValue = currentStockVariance * stockBaseValue;

        RefreshPrices();
    }

    public void RefreshPrices()
    {
        totalPrice = amountToBuy * stockCurrentValue;
        totalPricePlusNext = (amountToBuy + nextStep) * stockCurrentValue;
        totalPricePlusMegaNext = (amountToBuy + megaNextStep) * stockCurrentValue;
    }

    public void BuyStocks()
    {
        if (gameManager.playerData.iridium_Current >= totalPrice)
        {
            stockOwned += amountToBuy;
            gameManager.playerData.iridium_Current -= totalPrice;
        }
        else
        {
            Debug.LogError($"Not Enough to buy {amountToBuy} stocks!");
        }
    }

    public void NextButton()
    {
        if (gameManager.playerData.iridium_Current >= totalPricePlusNext)
        {
            amountToBuy += nextStep;

            RefreshPrices();
        }
        else
        {
            Debug.LogError("Not enough Iridium to buy next");
        }
    }

    public void MegaNextButton()
    {
        if (gameManager.playerData.iridium_Current >= totalPricePlusMegaNext)
        {
            amountToBuy += megaNextStep;

            RefreshPrices();
        }
        else
        {
            Debug.LogError("Not enough Iridium to buy mega next");
        }
    }

    public void PreviousButton()
    {
        amountToBuy -= previousStep;
        amountToBuy = Math.Clamp(amountToBuy, stockMinimumBuy, long.MaxValue);
        RefreshPrices();
    }

    public void MegaPreviousButton()
    {
        amountToBuy -= megaPreviousStep;
        amountToBuy = Math.Clamp(amountToBuy, stockMinimumBuy, long.MaxValue);
        RefreshPrices();
    }
}
