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
    [SerializeField] private TMP_Text cashText;
    [SerializeField] private TMP_Text infoPanelTitle;
    [SerializeField] private TMP_Text infoPanelHitPoints;
    [SerializeField] private TMP_Text infoPanelSpeed;
    [SerializeField] private TMP_Text infoPanelAccuracy;
    [SerializeField] private Transform weaponListParent;
    [SerializeField] private Button weaponItemPrefab;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    private CompanySoldier displayedRecruit;
    private List<CompanySoldier> soldiers;
    private int currentSoldierIndex;

    private void Awake()
    {
        soldiers = GameManager.Instance.Company.GetSoldiers();

        nextButton.onClick.AddListener(NextSoldier);
        prevButton.onClick.AddListener(PrevSoldier);
    }

    private void OnEnable()
    {
        RefreshScreen();
    }

    private void OnDestroy()
    {
        nextButton.onClick.RemoveListener(NextSoldier);
        prevButton.onClick.RemoveListener(PrevSoldier);
    }

    private void NextSoldier()
    {
        DisplaySoldierInfo(soldiers[++currentSoldierIndex]);
    }

    private void PrevSoldier()
    {
        DisplaySoldierInfo(soldiers[--currentSoldierIndex]);
    }

    // should this really be called each time the UI is re-opened?
    // The answer is no.
    private void RefreshScreen()
    {
        if (soldiers.Count > 0)
        {
            DisplaySoldierInfo(soldiers[0]);
        }
    }

    /// <summary>
    /// Display selected weapon info in center pane.
    /// </summary>
    public void DisplaySoldierInfo(CompanySoldier recruit)
    {
        infoPanelTitle.text = recruit.Name;
        displayedRecruit = recruit;

        infoPanelHitPoints.text = "Hitpoints:" + displayedRecruit.HitPoints;
        infoPanelSpeed.text = "Speed: " + displayedRecruit.MoveSpeed;
        infoPanelAccuracy.text = "Accuracy Rating: " + displayedRecruit.AccuracyRating;
    }
}
