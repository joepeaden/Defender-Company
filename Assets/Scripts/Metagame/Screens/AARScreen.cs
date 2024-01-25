using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Handles the After Action Report Screen obviously
/// </summary>
public class AARScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text missionResult;
    [SerializeField] private TMP_Text missionReward;
    [SerializeField] private GameObject textObject;
    [SerializeField] private Transform namesColumn;
    [SerializeField] private Transform statusColumn;
    [SerializeField] private Transform killsColumn;
    [SerializeField] private Transform xpColumn;

    private void OnEnable()
    {
        MissionData lastMission = GameManager.Instance.CurrentMission;
        bool victory = GameManager.Instance.PlayerWonLastMission;

        missionResult.text = victory ? "Victory" : "Defeat";
        missionResult.color = victory ? Color.green : Color.red;
        missionReward.text = victory ? "$" + lastMission.completionReward.ToString() : "None";

        TMP_Text textEntry = Instantiate(textObject, namesColumn).GetComponent<TMP_Text>();
        textEntry.text = "Name";
        textEntry = Instantiate(textObject, statusColumn).GetComponent<TMP_Text>();
        textEntry.text = "Status";
        textEntry = Instantiate(textObject, killsColumn).GetComponent<TMP_Text>();
        textEntry.text = "Kills";
        textEntry = Instantiate(textObject, xpColumn).GetComponent<TMP_Text>();
        textEntry.text = "XP";

        List<CompanySoldier> soldiers = GameManager.Instance.Company.GetDeployedSoldiersAsList();
        for (int i = 0; i < soldiers.Count; i++)
        {
            CompanySoldier soldier = soldiers[i];
            textEntry = Instantiate(textObject, namesColumn).GetComponent<TMP_Text>();
            textEntry.text = soldier.Name;
            textEntry = Instantiate(textObject, statusColumn).GetComponent<TMP_Text>();
            textEntry.text = soldier.MissionHP > 0 ? "Alive" : "Dead";
            textEntry = Instantiate(textObject, killsColumn).GetComponent<TMP_Text>();
            textEntry.text = soldier.MissionKills.ToString();
            textEntry = Instantiate(textObject, xpColumn).GetComponent<TMP_Text>();
            textEntry.text = soldier.XP.ToString();
        }
    }
}
