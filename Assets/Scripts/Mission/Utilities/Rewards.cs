using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The backend for the shop etc.
/// </summary>
public class Rewards : MonoBehaviour
{
    private static Rewards _instance;
    public static Rewards Instance { get { return _instance; } }

    [SerializeField] private InventoryItemDataStorage dataStore;

    public GameObject lootPrefab;
    public List<Transform> spawnPoints = new List<Transform>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
            Debug.LogWarning("More than one instance of Rewards; deleting this one.");
        }
    }

    private void Start()
    {
        MissionUI.OnRewardsPicked.AddListener(HandleRewardPicked);
    }

    private void OnDestroy()
    {
        MissionUI.OnRewardsPicked.RemoveListener(HandleRewardPicked);
    }

    public List<ShopItem> GetRewardShopItems()
    {
        List<ShopItem> shopItems = new List<ShopItem>();
        shopItems.Add(new ShopItem(dataStore.medkit));
        shopItems.Add(new ShopItem(dataStore.assaultRifle));
        shopItems.Add(new ShopItem(dataStore.subMachineGun));
        shopItems.Add(new ShopItem(dataStore.semiAutoRifle));
        shopItems.Add(new ShopItem(dataStore.shotgun));
        shopItems.Add(new ShopItem(dataStore.psaSabre));

        return shopItems;
    }
    
    private void HandleRewardPicked(ShopItem reward)
    {
        // no reward was picked
        // this won't work anymore! Used to be a string but NO MORE!
        if (reward.gearType == null)
        {
            return;
        }

        // make sure the player can afford
        bool hadEnoughPoints = Scoreboard.TryRemovePoints(reward.cost);
        if (hadEnoughPoints)
        {
            // spawn the loot somewhere
            int spawnPointIndex = 0;
            do
            {
                spawnPointIndex = Random.Range(0, spawnPoints.Count);
            } while (!spawnPoints[spawnPointIndex].gameObject.activeInHierarchy);

            Loot loot = Instantiate(lootPrefab, spawnPoints[spawnPointIndex].position, Quaternion.identity).GetComponent<Loot>();
            if (loot)
            {
                loot.gearType = reward.gearType;
            }
            else
            {
                Debug.LogError("No loot script on loot object");
            }
        }
        else
        {
            // the idea is that the button catches this situation and we don't ever end up here. But just in case.
            Debug.LogError("User tried to purchase item they could not afford");
        }
    }
}

public struct ShopItem
{
    public GearData.GearType gearType;
    public int cost;
    public string displayName;

    public ShopItem(GearData data)
    {
        gearType = data.gearType;
        cost = data.cost;
        displayName = data.displayName;
    }
}
