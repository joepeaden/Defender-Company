using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Handles the Market Screen obviously. UI & processing purchases etc.
/// </summary>
public class RecruitmentScreen : MonoBehaviour
{
    // UI stufff
    [SerializeField] private TMP_Text cashText;
    [SerializeField] private TMP_Text infoPanelTitle;
    [SerializeField] private TMP_Text infoPanelCost;
    [SerializeField] private TMP_Text infoPanelHitPoints;
    [SerializeField] private TMP_Text infoPanelSpeed;
    [SerializeField] private TMP_Text infoPanelAccuracy;
    [SerializeField] private Transform recruitListParent;
    [SerializeField] private Button recruitItemPrefab;
    [SerializeField] private Button hireButton;

    private CompanySoldier displayedRecruit;

    private void Awake()
    {
        hireButton.onClick.AddListener(RecruitSoldier);
    }

    private void OnEnable()
    {
        RefreshScreen();
    }

    private void OnDestroy()
    {
        hireButton.onClick.RemoveListener(RecruitSoldier);
    }

    private void RecruitSoldier()
    {
        if (displayedRecruit.HireCost <= GameManager.Instance.Company.PlayerCash)
        {
            GameManager.Instance.Company.AddCash(-displayedRecruit.HireCost);
            GameManager.Instance.Company.AddRecruit(displayedRecruit);

            // can optimize by just removing the button from market and adding to owned.
            RefreshScreen();
        }
    }

    // should this really be called each time the UI is re-opened?
    // The answer is no.
    private void RefreshScreen()
    {
        // clear all children
        for (int i = 0; i < recruitListParent.childCount; i++)
        {
            Destroy(recruitListParent.GetChild(i).gameObject);
        }
        
        List<CompanySoldier> recruits = GameManager.Instance.Company.GetNewRandomRecruits(10);

        // add market weapon buttons
        for (int i = 0; i < recruits.Count; i++)
        {
            CompanySoldier recruit = recruits[i];
            MarketButton marketButton = Instantiate(recruitItemPrefab, recruitListParent).GetComponent<MarketButton>();
            marketButton.Initialize(recruit.Name);
            marketButton.GetComponent<Button>().onClick.AddListener(() => DisplayRecruitInfo(recruit));
        }

        // update player cash
        cashText.text = "$" + GameManager.Instance.Company.PlayerCash;

        DisplayRecruitInfo(recruits[0]);
    }

    /// <summary>
    /// Display selected weapon info in center pane.
    /// </summary>
    public void DisplayRecruitInfo(CompanySoldier recruit)
    {
        infoPanelTitle.text = recruit.Name;
        displayedRecruit = recruit;

        //infoPanelCost.gameObject.SetActive(!gearIsOwned);
        //hireButton.gameObject.SetActive(!gearIsOwned);

        bool playerCanAfford = PlayerCanAfford(displayedRecruit.HireCost);

        infoPanelCost.text = "Cost: $" + displayedRecruit.HireCost;
        infoPanelCost.color = playerCanAfford ? Color.white : Color.red;

        infoPanelHitPoints.text = "Hitpoints:" + displayedRecruit.HitPoints;
        infoPanelSpeed.text = "Speed: " + displayedRecruit.MoveSpeed;
        infoPanelAccuracy.text = "Accuracy Rating: " + displayedRecruit.AccuracyRating;


        hireButton.interactable = playerCanAfford;
    }

    private bool PlayerCanAfford(int cost)
    {
        return GameManager.Instance.Company.PlayerCash - cost >= 0; 
    }
}
