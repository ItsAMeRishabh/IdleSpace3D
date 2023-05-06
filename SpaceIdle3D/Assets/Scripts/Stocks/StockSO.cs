using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stock", menuName = "SpaceIdle3D/Stock")]
public class StockSO : ScriptableObject
{
    public string stockName = "StockName";
    public double stockBaseValue = 10;
    public Vector2 stockVariance = new Vector2(0.5f, 2f);
    public double stockRefreshTime = 3600.0;
    public double stockExpireTime = 86400.0;

    public long stockMinimumBuy = 10;
    public long stockMinimumSell = 10;
    public long nextStep = 10;
    public long megaNextStep = 100;
    public long previousStep = 10;
    public long megaPreviousStep = 100;
}
