using UnityEngine;
using TMPro;

/// <summary>
/// Handles the Market Screen obviously
/// </summary>
public class MarketScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text cashText;

    private void OnEnable()
    {
        cashText.text = "$" + GameManager.Instance.PlayerCash;
    }
}
