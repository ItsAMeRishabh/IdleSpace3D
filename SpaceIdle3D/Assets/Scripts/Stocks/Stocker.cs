using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stocker : MonoBehaviour
{
    private float clickTimeout = 0.2f;
    private float spawnCheckDelay = 30f;

    private WaitForSeconds clickWait;
    public WaitForSeconds spawnWait;
    private Coroutine clickTimeoutCoroutine;
    private bool clickExplired = false;
    private bool clickedOnMe = false;

    private GameManager gameManager;

    public void StartGame(GameManager gameManager)
    {
        Debug.Log("stock start");
        this.gameManager = gameManager;

        clickWait = new WaitForSeconds(clickTimeout);
        spawnWait = new WaitForSeconds(spawnCheckDelay);
    }

    void OnMouseDown()
    {
        if (!DetectClickOnUI.IsPointerOverUIElement())
        {
            if (clickTimeoutCoroutine != null)
            {
                StopCoroutine(clickTimeoutCoroutine);
                clickTimeoutCoroutine = null;
            }

            clickTimeoutCoroutine = StartCoroutine(ClickTimeoutCoroutine());
            clickedOnMe = true;
        }
    }

    private void OnMouseUp()
    {
        if (!clickExplired && clickedOnMe)
        {
            gameManager.UIManagerRef.OpenStockBuyMenu();
        }
        clickedOnMe = false;
    }

    private IEnumerator ClickTimeoutCoroutine()
    {
        clickExplired = false;

        yield return clickWait;

        clickExplired = true;
    }
}
