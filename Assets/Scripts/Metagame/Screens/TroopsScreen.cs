using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Handles the troops the player has recruited
/// </summary>
public class TroopsScreen : MonoBehaviour
{
    // UI stufff
    [SerializeField] private TMP_Text infoPanelName;
    [SerializeField] private TMP_Text infoPanelLevel;
    [SerializeField] private TMP_Text infoPanelHitPoints;
    [SerializeField] private TMP_Text infoPanelSpeed;
    [SerializeField] private TMP_Text infoPanelAccuracy;
    [SerializeField] private TMP_Text deployButtonText;
    [SerializeField] private Button weaponButton;
    [SerializeField] private Button deployButton;
    [SerializeField] private Button equipmentItemPrefab;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Transform equipmentListParent;
    [SerializeField] private GameObject equipmentListScrollView;

    private CompanySoldier displayedSoldier;
    private List<CompanySoldier> soldiers;
    private int currentSoldierIndex;

    private void Awake()
    {
        nextButton.onClick.AddListener(NextSoldier);
        prevButton.onClick.AddListener(PrevSoldier);
        weaponButton.onClick.AddListener(ShowWeaponsOptions);
        deployButton.onClick.AddListener(ToggleSoldierDeployed);
    }

    private void OnEnable()
    {
        RefreshScreen();
    }

    private void OnDestroy()
    {
        nextButton.onClick.RemoveListener(NextSoldier);
        prevButton.onClick.RemoveListener(PrevSoldier);
        weaponButton.onClick.RemoveListener(ShowWeaponsOptions);
        deployButton.onClick.RemoveListener(ToggleSoldierDeployed);
    }


    private void ShowWeaponsOptions()
    {
        equipmentListScrollView.SetActive(true);
        if (equipmentListParent.childCount == 0)
        {
            Dictionary<GearData.GearType, GearData> ownedGear = GameManager.Instance.Company.GetOwnedGear();
            foreach (KeyValuePair<GearData.GearType, GearData> kvp in ownedGear)
            {
                if (kvp.Value as WeaponData != null)
                {
                    MarketButton marketButton = Instantiate(equipmentItemPrefab, equipmentListParent).GetComponent<MarketButton>();
                    marketButton.Initialize(kvp.Value.displayName);
                    marketButton.GetComponent<Button>().onClick.AddListener(() => SetNewWeapon(kvp.Value as WeaponData));
                }
            }
        }
    }

    private void SetNewWeapon(WeaponData newWeapon)
    {
        displayedSoldier.CurrentWeapon = newWeapon;
        weaponButton.GetComponentInChildren<TMP_Text>().text = displayedSoldier.CurrentWeapon.displayName;
        equipmentListScrollView.SetActive(false);
    }

    private void NextSoldier()
    {
        currentSoldierIndex++;
        if (currentSoldierIndex > soldiers.Count-1)
        {
            currentSoldierIndex = 0;
        }

        DisplaySoldierInfo(soldiers[currentSoldierIndex]);
    }

    private void PrevSoldier()
    {
        currentSoldierIndex--;
        if (currentSoldierIndex < 0)
        {
            currentSoldierIndex = soldiers.Count - 1;
        }

        DisplaySoldierInfo(soldiers[currentSoldierIndex]);
    }

    // should this really be called each time the UI is re-opened?
    // The answer is no.
    private void RefreshScreen()
    {
        soldiers = GameManager.Instance.Company.GetLivingSoldiersAsList();
        if (soldiers.Count > 0)
        {
            DisplaySoldierInfo(soldiers[0]);
        }
    }

    /// <summary>
    /// Display selected weapon info in center pane.
    /// </summary>
    private void DisplaySoldierInfo(CompanySoldier soldier)
    {
        infoPanelName.text = soldier.Name;
        displayedSoldier = soldier;

        infoPanelLevel.text = "Level: " + displayedSoldier.Level + " (" + displayedSoldier.XP + "/" + displayedSoldier.Level * 10 + "XP)";
        infoPanelHitPoints.text = "Hitpoints:" + displayedSoldier.HitPoints;
        infoPanelSpeed.text = "Speed: " + displayedSoldier.MoveSpeed;
        infoPanelAccuracy.text = "Accuracy Rating: " + displayedSoldier.AccuracyRating;

        weaponButton.GetComponentInChildren<TMP_Text>().text = displayedSoldier.CurrentWeapon.displayName;

        UpdateDeployButton();
    }

    private void ToggleSoldierDeployed()
    {
        HashSet<string> deployedSoldiers = GameManager.Instance.Company.DeployedSoldiers;
        if (deployedSoldiers.Contains(displayedSoldier.ID))
        {
            deployedSoldiers.Remove(displayedSoldier.ID);
        }
        else
        {
            deployedSoldiers.Add(displayedSoldier.ID);
        }
        UpdateDeployButton();
    }

    private void UpdateDeployButton()
    {

        HashSet<string> deployedSoldiers = GameManager.Instance.Company.DeployedSoldiers;
        if (deployedSoldiers.Contains(displayedSoldier.ID))
        {
            deployButtonText.text = "On Deployment";
        }
        else
        {
            deployButtonText.text = "In Reserve";
        }
    }
}
