using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(GameManager))]
public class StockManager : MonoBehaviour
{
    public List<StockSO> stockSOs;
    public List<Stock> stocks;

    public int selectedStockIndex;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void StartGame()
    {
        List<Stock> playerStocks = gameManager.playerData.ownedStocks;
        foreach (Stock stock in playerStocks)
        {
            StockSO sSO = GetStockSO(stock.stockName);

            stocks.Add(new Stock(sSO, stock));
        }

        foreach (StockSO sSO in stockSOs)
        {
            Stock foundStock = Array.Find(stocks.ToArray(), x => x.stockName == sSO.stockName);

            if (foundStock != null)
            {
                continue;
            }
            else
            {
                stocks.Add(new Stock(sSO));
            }
        }

        selectedStockIndex = 0;
        CheckRefreshStockPrices();
    }

    public void CheckRefreshStockPrices()
    {
        for (int i = 0; i < stocks.Count; i++)
        {
            if (DateTime.Now > ((DateTime)stocks[i].nextRefreshTime))
            {
                DateTime toBeNextRefresh = ((DateTime)stocks[i].nextRefreshTime).AddSeconds(stocks[i].stockRefreshTime);

                while (DateTime.Now < toBeNextRefresh)
                {
                    toBeNextRefresh = toBeNextRefresh.AddSeconds(stocks[i].stockRefreshTime);
                }

                RefreshStockValue(i);
                stocks[i].nextRefreshTime = toBeNextRefresh;
            }
        }
    }


    public void TickCheckRefreshStockPrices()
    {
        for (int i = 0; i < stocks.Count; i++)
        {
            if (DateTime.Now > ((DateTime)stocks[i].nextRefreshTime))
            {
                RefreshStockValue(i);
                stocks[i].nextRefreshTime = DateTime.Now.AddSeconds(stocks[i].stockRefreshTime);
            }
        }
    }

    [ContextMenu("Refresh Stock Price")]
    public void RefreshStockValue(int index = 0)
    {
        float currentStockVariance = UnityEngine.Random.Range(stocks[index].stockVariance.x, stocks[index].stockVariance.y);
        stocks[index].stockCurrentValue = currentStockVariance * stocks[index].stockBaseValue;

        RefreshPrices();
    }

    public void RefreshPrices()
    {
        for (int i = 0; i < stocks.Count; i++)
        {
            stocks[i].totalPrice = stocks[i].amountToBuy * stocks[i].stockCurrentValue;
            stocks[i].totalPricePlusNext = (stocks[i].amountToBuy + stocks[i].nextStep) * stocks[i].stockCurrentValue;
            stocks[i].totalPricePlusMegaNext = (stocks[i].amountToBuy + stocks[i].megaNextStep) * stocks[i].stockCurrentValue;
        }
    }

    public void BuyStocks()
    {
        if (gameManager.playerData.iridium_Current >= stocks[selectedStockIndex].totalPrice)
        {
            stocks[selectedStockIndex].stockOwned += stocks[selectedStockIndex].amountToBuy;
            gameManager.playerData.iridium_Current -= stocks[selectedStockIndex].totalPrice;
        }
        else
        {
            Debug.LogError($"Not Enough to buy {stocks[selectedStockIndex].amountToBuy} stocks!");
        }
    }

    public void NextButton()
    {
        if (gameManager.playerData.iridium_Current >= stocks[selectedStockIndex].totalPricePlusNext)
        {
            stocks[selectedStockIndex].amountToBuy += stocks[selectedStockIndex].nextStep;

            RefreshPrices();
        }
        else
        {
            Debug.LogError("Not enough Iridium to buy next");
        }
    }

    public void MegaNextButton()
    {
        if (gameManager.playerData.iridium_Current >= stocks[selectedStockIndex].totalPricePlusMegaNext)
        {
            stocks[selectedStockIndex].amountToBuy += stocks[selectedStockIndex].megaNextStep;

            RefreshPrices();
        }
        else
        {
            Debug.LogError("Not enough Iridium to buy mega next");
        }
    }

    public void PreviousButton()
    {
        stocks[selectedStockIndex].amountToBuy -= stocks[selectedStockIndex].previousStep;
        stocks[selectedStockIndex].amountToBuy = Math.Clamp(stocks[selectedStockIndex].amountToBuy, stocks[selectedStockIndex].stockMinimumBuy, long.MaxValue);
        RefreshPrices();
    }

    public void MegaPreviousButton()
    {
        stocks[selectedStockIndex].amountToBuy -= stocks[selectedStockIndex].megaPreviousStep;
        stocks[selectedStockIndex].amountToBuy = Math.Clamp(stocks[selectedStockIndex].amountToBuy, stocks[selectedStockIndex].stockMinimumBuy, long.MaxValue);
        RefreshPrices();
    }

    public StockSO GetStockSO(string sName)
    {
        StockSO sSO = Array.Find(stockSOs.ToArray(), x => x.stockName == sName);

        if (sSO == null)
        {
            Debug.LogError($"Stock {sName} not found!");
        }

        return sSO;
    }
}
