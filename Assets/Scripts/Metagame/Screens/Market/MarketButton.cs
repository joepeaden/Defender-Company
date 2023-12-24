using UnityEngine;
using TMPro;

public class MarketButton : MonoBehaviour
{
    public TMP_Text buttonTitle;

    public void Initialize(GearData gearData)
    {
        buttonTitle.text = gearData.displayName;
    }

    public void Initialize(string itemName)
    {
        buttonTitle.text = itemName;
    }
}
