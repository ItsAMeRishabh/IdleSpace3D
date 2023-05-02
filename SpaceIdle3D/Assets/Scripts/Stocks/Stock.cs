using System;
using UnityEngine;

[Serializable]
public class Stock
{
    public string stockName;
    [NonSerialized] public double stockBaseValue;
    [NonSerialized] public double stockCurrentValue;
    [NonSerialized] public Vector2 stockVariance;
    [NonSerialized] public double stockRefreshTime = 3600.0;

    [NonSerialized] public long stockMinimumBuy;
    [NonSerialized] public long nextStep;
    [NonSerialized] public long megaNextStep;
    [NonSerialized] public long previousStep;
    [NonSerialized] public long megaPreviousStep;

    public long stockOwned;
    [NonSerialized] public long amountToBuy;
    public JsonDateTime nextRefreshTime;

    [NonSerialized] public double totalPrice;
    [NonSerialized] public double totalPricePlusNext;
    [NonSerialized] public double totalPricePlusMegaNext;

    public Stock(StockSO so)
    {
        stockName = so.stockName;
        stockBaseValue = so.stockBaseValue;
        stockVariance = so.stockVariance;
        stockRefreshTime = so.stockRefreshTime;

        stockMinimumBuy = so.stockMinimumBuy;
        nextStep = so.nextStep;
        megaNextStep = so.megaNextStep;
        previousStep = so.previousStep;
        megaPreviousStep = so.megaPreviousStep;
    }

    public Stock(StockSO so, Stock s)
    {
        stockName = so.stockName;
        stockBaseValue = so.stockBaseValue;
        stockVariance = so.stockVariance;
        stockRefreshTime = so.stockRefreshTime;

        stockMinimumBuy = so.stockMinimumBuy;
        nextStep = so.nextStep;
        megaNextStep = so.megaNextStep;
        previousStep = so.previousStep;
        megaPreviousStep = so.megaPreviousStep;

        stockOwned = s.stockOwned;
        nextRefreshTime = s.nextRefreshTime;
    }
}
