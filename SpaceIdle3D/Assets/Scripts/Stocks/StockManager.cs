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

    public bool sellMode = false;
    private DateTime localNow;
    private GameManager gameManager;

    public void WakeUp()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void StartGame()
    {
        List<Stock> playerStocks = gameManager.playerData.ownedStocks;
        localNow = DateTime.Now;

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
        CheckStockExpiry();
    }

    public void CheckRefreshStockPrices()
    {
        for (int i = 0; i < stocks.Count; i++)
        {
            if (stocks[i].nextRefreshTime.value == 0)
            {
                RefreshStockValue(i);
                stocks[i].nextRefreshTime = (localNow.AddSeconds(stocks[i].stockRefreshTime));

                continue;
            }
            if (localNow > ((DateTime)stocks[i].nextRefreshTime))
            {
                DateTime toBeNextRefresh = ((DateTime)stocks[i].nextRefreshTime).AddSeconds(stocks[i].stockRefreshTime);

                while (localNow > toBeNextRefresh)
                {
                    toBeNextRefresh = toBeNextRefresh.AddSeconds(stocks[i].stockRefreshTime);
                }

                RefreshStockValue(i);
                stocks[i].nextRefreshTime = toBeNextRefresh;
            }
        }
    }

    public void CheckStockExpiry()
    {
        for (int i = 0; i < stocks.Count; i++)
        {
            if (stocks[i].nextExpireTime.value == 0)
            {
                ExpireStock(i);
                stocks[i].nextExpireTime = (localNow.AddSeconds(stocks[i].stockExpireTime));
                continue;
            }
            if (localNow > ((DateTime)stocks[i].nextExpireTime))
            {
                DateTime toBeNextExpired = ((DateTime)stocks[i].nextExpireTime).AddSeconds(stocks[i].stockExpireTime);

                while (localNow > toBeNextExpired)
                {
                    toBeNextExpired = toBeNextExpired.AddSeconds(stocks[i].stockExpireTime);
                }

                ExpireStock(i);
                stocks[i].nextRefreshTime = toBeNextExpired;
            }
        }
    }

    public void TickStockRefresh()
    {
        localNow = DateTime.Now;
        TickCheckRefreshStockPrices();
        TickCheckStockExpiry();
    }

    public void TickCheckRefreshStockPrices()
    {
        for (int i = 0; i < stocks.Count; i++)
        {
            if (localNow > ((DateTime)stocks[i].nextRefreshTime))
            {
                RefreshStockValue(i);
                stocks[i].nextRefreshTime = localNow.AddSeconds(stocks[i].stockRefreshTime);
            }
        }
    }

    public void TickCheckStockExpiry()
    {
        for (int i = 0; i < stocks.Count; i++)
        {
            if (localNow > ((DateTime)stocks[i].nextExpireTime))
            {
                ExpireStock(i);
                stocks[i].nextExpireTime = localNow.AddSeconds(stocks[i].stockExpireTime);
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

    [ContextMenu("Expire Stock Price")]
    public void ExpireStock(int index = 0)
    {
        stocks[index].stockOwned = 0;
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

    public void NextStock()
    {
        selectedStockIndex++;

        if (selectedStockIndex >= stocks.Count)
            selectedStockIndex = 0;
    }

    public void PreviousStock()
    {
        selectedStockIndex--;

        if (selectedStockIndex < 0)
        {
            selectedStockIndex = stocks.Count - 1;
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

    public void SellStocks()
    {
        if (stocks[selectedStockIndex].totalPrice >= stocks[selectedStockIndex].amountToBuy)
        {
            stocks[selectedStockIndex].stockOwned -= stocks[selectedStockIndex].amountToBuy;
            gameManager.playerData.iridium_Current += stocks[selectedStockIndex].totalPrice;
        }
        else
        {
            Debug.LogError($"Not Enough {stocks[selectedStockIndex].amountToBuy} stocks to sell!");
        }
    }

    public void NextAmountBuy()
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

    public void MegaNextAmountBuy()
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

    public void PreviousAmountBuy()
    {
        stocks[selectedStockIndex].amountToBuy -= stocks[selectedStockIndex].previousStep;
        stocks[selectedStockIndex].amountToBuy = Math.Clamp(stocks[selectedStockIndex].amountToBuy, stocks[selectedStockIndex].stockMinimumBuy, long.MaxValue);
        RefreshPrices();
    }

    public void MegaPreviousAmountBuy()
    {
        stocks[selectedStockIndex].amountToBuy -= stocks[selectedStockIndex].megaPreviousStep;
        stocks[selectedStockIndex].amountToBuy = Math.Clamp(stocks[selectedStockIndex].amountToBuy, stocks[selectedStockIndex].stockMinimumBuy, long.MaxValue);
        RefreshPrices();
    }

    public List<Stock> GetStocks()
    {
        return stocks;
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
