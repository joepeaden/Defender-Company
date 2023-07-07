using UnityEngine;
using TMPro;

/// <summary>
/// Handles the After Action Report Screen obviously
/// </summary>
public class AARScreen : MonoBehaviour
{
    public TMP_Text missionTitle;
    public TMP_Text missionResult;
    public TMP_Text missionReward;

    private void OnEnable()
    {
        MissionData lastMission = GameManager.Instance.CurrentMission;
        bool victory = GameManager.Instance.PlayerWonLastMission;

        missionTitle.text = lastMission.missionTitle;
        missionResult.text = victory ? "Victory" : "Defeat";
        missionResult.color = victory ? Color.green : Color.red;
        missionReward.text = victory ? "$" + lastMission.completionReward.ToString() : "None";
    }
}
