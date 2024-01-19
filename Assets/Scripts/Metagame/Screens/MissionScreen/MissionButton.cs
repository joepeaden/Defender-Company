using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MissionButton : MonoBehaviour
{
    [SerializeField] private TMP_Text missionTitle;
    [SerializeField] private TMP_Text missionSubTitle;

    private MissionData _data;

    private void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveListener(LoadMission);
    }

    public void Initialize(MissionData data)
    {
        _data = data;
        missionTitle.text = _data.missionTitle.ToString();
        missionSubTitle.text = $"{_data.missionDifficulty} / {_data.numberOfTurns} Days / ${_data.completionReward}";

        GetComponent<Button>().onClick.AddListener(LoadMission);
    }

    private void LoadMission()
    {
        GameManager.Instance.SetCurrentMission(_data);
    }
}
