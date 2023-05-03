using System;
using UnityEngine;

[Serializable]
public class Stock
{
    public string stockName;
    [NonSerialized] public double stockBaseValue;
    public double stockCurrentValue;
    [NonSerialized] public Vector2 stockVariance;
    [NonSerialized] public double stockRefreshTime = 3600.0;
    [NonSerialized] public double stockExpireTime = 86400.0;

    [NonSerialized] public long stockMinimumBuy;
    [NonSerialized] public long nextStep;
    [NonSerialized] public long megaNextStep;
    [NonSerialized] public long previousStep;
    [NonSerialized] public long megaPreviousStep;

    public long stockOwned;
    public bool purchasedThisCycle;
    [NonSerialized] public long amountToBuy;
    public JsonDateTime nextRefreshTime;
    public JsonDateTime nextExpireTime;

    [NonSerialized] public double totalPrice;
    [NonSerialized] public double totalPricePlusNext;
    [NonSerialized] public double totalPricePlusMegaNext;

    public Stock(StockSO so)
    {
        stockName = so.stockName;
        stockBaseValue = so.stockBaseValue;
        stockVariance = so.stockVariance;
        stockRefreshTime = so.stockRefreshTime;
        stockExpireTime = so.stockExpireTime;

        stockMinimumBuy = so.stockMinimumBuy;
        amountToBuy = so.stockMinimumBuy;
        nextStep = so.nextStep;
        megaNextStep = so.megaNextStep;
        previousStep = so.previousStep;
        megaPreviousStep = so.megaPreviousStep;
    }

    public Stock(StockSO so, Stock s)
    {
        stockName = so.stockName;
        stockBaseValue = so.stockBaseValue;
        stockCurrentValue = s.stockCurrentValue;
        stockVariance = so.stockVariance;
        stockRefreshTime = so.stockRefreshTime;
        stockExpireTime = so.stockExpireTime;

        stockMinimumBuy = so.stockMinimumBuy;
        amountToBuy = so.stockMinimumBuy;
        nextStep = so.nextStep;
        megaNextStep = so.megaNextStep;
        previousStep = so.previousStep;
        megaPreviousStep = so.megaPreviousStep;

        stockOwned = s.stockOwned;
        purchasedThisCycle = s.purchasedThisCycle;
        nextRefreshTime = s.nextRefreshTime;
        nextExpireTime = s.nextExpireTime;
    }
}
