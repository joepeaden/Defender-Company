using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Handles the Market Screen obviously. UI & processing purchases etc.
/// </summary>
public class MarketScreen : MonoBehaviour
{
    // UI stufff
    [SerializeField] private TMP_Text cashText;
    [SerializeField] private TMP_Text infoPanelTitle;
    [SerializeField] private TMP_Text infoPanelCost;
    [SerializeField] private Transform marketGearParent;
    [SerializeField] private Transform ownedGearParent;
    [SerializeField] private Button marketItemPrefab;
    [SerializeField] private Button purchaseButton;

    // data store for market items
    [SerializeField] private InventoryItemDataStorage dataStore;

    private GearData displayedGear;

    private void Awake()
    {
        purchaseButton.onClick.AddListener(PurchaseItem);
    }

    private void OnEnable()
    {
        RefreshMarket();
    }

    private void OnDestroy()
    {
        purchaseButton.onClick.RemoveListener(PurchaseItem);
    }

    private void PurchaseItem()
    {
        if (displayedGear.cost <= GameManager.Instance.Company.PlayerCash)
        {
            GameManager.Instance.Company.AddCash(-displayedGear.cost);
            GameManager.Instance.Company.AddOwnedGear(displayedGear);

            // can optimize by just removing the button from market and adding to owned.
            RefreshMarket();
        }
    }

    // should this really be called each time the UI is re-opened?
    // The answer is no.
    private void RefreshMarket()
    {
        // clear all children
        for (int i = 0; i < marketGearParent.childCount; i++)
        {
            Destroy(marketGearParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < ownedGearParent.childCount; i++)
        {
            Destroy(ownedGearParent.GetChild(i).gameObject);
        }
        
        List<GearData> marketItems = GetMarketItems();
        Dictionary<GearData.GearType, GearData> ownedGear = GameManager.Instance.Company.GetOwnedGear();

        // add market weapon buttons
        foreach (GearData item in marketItems)
        {
            // don't add things the player already owns
            if (ownedGear.ContainsKey(item.gearType))
            {
                continue;
            }

            MarketButton marketButton = Instantiate(marketItemPrefab, marketGearParent).GetComponent<MarketButton>();
            marketButton.Initialize(item);
            marketButton.GetComponent<Button>().onClick.AddListener(() => DisplayWeaponInfo(item, false));
        }

        // add owned weapon buttons
        foreach (GearData item in ownedGear.Values)
        {
            MarketButton marketButton = Instantiate(marketItemPrefab, ownedGearParent).GetComponent<MarketButton>();
            marketButton.Initialize(item);
            marketButton.GetComponent<Button>().onClick.AddListener(() => DisplayWeaponInfo(item, true));
        }

        // update player cash
        cashText.text = "$" + GameManager.Instance.Company.PlayerCash;
    }

    /// <summary>
    /// Get currently available market items
    /// </summary>
    /// <returns></returns>
    private List<GearData> GetMarketItems()
    {
        List<GearData> marketItems = new List<GearData>();
        marketItems.Add(dataStore.medkit);
        marketItems.Add(dataStore.assaultRifle);

        // removed temporarily until figure out graphics and balance for them
        //marketItems.Add(dataStore.subMachineGun);
        //marketItems.Add(dataStore.semiAutoRifle);
        //marketItems.Add(dataStore.shotgun);
        //marketItems.Add(dataStore.psaSabre);
        return marketItems;
    }

    /// <summary>
    /// Display selected weapon info in center pane.
    /// </summary>
    public void DisplayWeaponInfo(GearData gearData, bool gearIsOwned)
    {
        infoPanelTitle.text = gearData.name;
        displayedGear = gearData;

        infoPanelCost.gameObject.SetActive(!gearIsOwned);
        purchaseButton.gameObject.SetActive(!gearIsOwned);

        if (!gearIsOwned)
        {
            bool playerCanAfford = PlayerCanAfford(gearData.cost);

            infoPanelCost.text = "$" + gearData.cost;
            infoPanelCost.color = playerCanAfford ? Color.white : Color.red;
            purchaseButton.interactable = playerCanAfford;
        }
    }

    private bool PlayerCanAfford(int cost)
    {
        return GameManager.Instance.Company.PlayerCash - cost >= 0; 
    }
}
