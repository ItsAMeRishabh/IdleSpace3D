using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class Building : MonoBehaviour
{
    private float clickTimeout = 0.2f;
    private WaitForSeconds clickWait;
    private Coroutine clickTimeoutCoroutine;
    private bool clickExplired = false;
    private bool clickedOnMe = false;

    public BuildingSO buildingSO;
    public BuildingData buildingData;

    public VisualEffect upgradeEffect;

    private GameManager gameManager;

    public void Initialize()
    {
        gameManager = FindObjectOfType<GameManager>();
        upgradeEffect = GetComponent<VisualEffect>();

        clickWait = new WaitForSeconds(clickTimeout);
    }

    public void SpawnUpgradeEffect(VisualEffectAsset vfxAsset)
    {
        upgradeEffect.visualEffectAsset = vfxAsset;
        upgradeEffect.Play();
    }

    void OnMouseDown()
    {
        if (!DetectClickOnUI.IsPointerOverUIElement())
        {
            if(clickTimeoutCoroutine != null)
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
        if(!clickExplired && clickedOnMe)
        {
            gameManager.ClickedOnBuilding(this);
        }
        clickedOnMe = false;
    }

    public double GetIridiumPerTick()
    {
        double x = 0;
        foreach (Troop troop in buildingData.building_OwnedTroops)
        {
            x += troop.GetIridiumPerTick() * Math.Pow(buildingSO.building_IridiumBoostPerLevel, buildingData.building_Level - 1);
        }
        return x;
    }

    public double GetDarkElixirPerTick()
    {
        if (buildingData.building_Level == 0) return 0;
        else return buildingSO.building_DarkElixirPerSecond * Math.Pow(buildingSO.building_DarkElixirBoostPerLevel, buildingData.building_Level - 1);
    }

    private IEnumerator ClickTimeoutCoroutine()
    {
        clickExplired = false;

        yield return clickWait;

        clickExplired = true;
    }
}
