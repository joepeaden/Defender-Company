using System.Collections.Generic;
using UnityEngine;

public class MissionScreen : MonoBehaviour
{
    [SerializeField] private List<MissionData> missions;
    [SerializeField] private Transform missionLayoutParent;
    [SerializeField] private GameObject buttonTemplate;

    private void OnEnable()
    {
        // clear out mission buttons
        for (int i = 0; i < missionLayoutParent.childCount; i++)
        {
            Destroy(missionLayoutParent.GetChild(i).gameObject);
        }

        foreach (MissionData mData in missions)
        {
            MissionButton mButton = Instantiate(buttonTemplate, missionLayoutParent).GetComponent<MissionButton>();
            mButton.Initialize(mData);
        }
    }
}
