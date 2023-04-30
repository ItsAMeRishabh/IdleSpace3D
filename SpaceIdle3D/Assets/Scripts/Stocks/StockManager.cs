using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class StockManager : MonoBehaviour
{
    public string stockName;
    public double stockValue;

    private long stockOwned;
    private long amountToBuy;
    private long amountToBuyPlus10;
    private long amountToBuyPlus100;

    private double totalPrice;
    private double totalPricePlus10;
    private double totalPricePlus100;

    private GameManager gameManager;

    public void InitializeStocks()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void RefreshPrices()
    {

    }

    public void NextButton()
    {

    }

    public void MegaNextButton()
    {

    }

    public void PreviousButton()
    {

    }

    public void MegaPreviousButton()
    {

    }
}
