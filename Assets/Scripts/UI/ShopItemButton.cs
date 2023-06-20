using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShopItemButton : MonoBehaviour
{
    /// <summary>
    /// Param is rewardKey to compare. Used for updating button visuals/interactability when another button is selected.
    /// </summary>
    private static UnityEvent<string> OnAShopButtonClicked = new UnityEvent<string>();
    public static UnityEvent<Button> OnNewHoveredButton = new UnityEvent<Button>();

    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_Text cost;

    private ShopItem item;
    private Button button;
    private Image backgroundImage;

    public Color normalLabelColor;
    public Color normalCostColor;
    public Color normalBGColor;
    public Color hoverBGColor;

    private bool isHovered;
    private bool isSelected;
    private bool tooExpensive;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(UpdateReward);
        OnAShopButtonClicked.AddListener(CheckIfSelected);
        OnNewHoveredButton.AddListener(HandleNewButtonHovered);
        backgroundImage = GetComponent<Image>();
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(UpdateReward);
        OnAShopButtonClicked.RemoveListener(CheckIfSelected);
        OnNewHoveredButton.RemoveListener(HandleNewButtonHovered);
    }

    public void SetHover(bool hover)
    {
        // this is repetitive with the HandleNewButtonHovered part. Probably don't need to set isHovered twice bro.
        isHovered = hover;
        OnNewHoveredButton.Invoke(button);
    }

    /// <summary>
    /// Set the shopitem for this button.
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(ShopItem item)
    {
        label.text = item.displayName;
        cost.text = $"${item.cost}";
        this.item = item;
        tooExpensive = Scoreboard.totalScore - item.cost < 0;

        UpdateVisuals();
    }

    /// <summary>
    /// Informs the system of the current reward choice.
    /// </summary>
    private void UpdateReward()
    {
        if (!tooExpensive)
        {
            GameplayUI.Instance.HandleRewardPicked(item);
            OnAShopButtonClicked.Invoke(item.rewardKey);
        }
    }

    private void CheckIfSelected(string key)
    {
        isSelected = key == item.rewardKey;
        UpdateVisuals();
    }

    private void HandleNewButtonHovered(Button b)
    {
        isHovered = b == button;
        UpdateVisuals();
    }

    /// <summary>
    /// Updates button interactability.
    /// </summary>
    /// <param name="key">Currently selected rewardKey, for comparison.</param>
    private void UpdateVisuals()
    {
        backgroundImage.color = isHovered ? hoverBGColor : normalBGColor;

        if (tooExpensive)
        {
            cost.color = Color.red;
            button.interactable = false;
        }
        else
        {
            label.color = isSelected ? Color.yellow : normalLabelColor;
            cost.color = isSelected ? Color.yellow : normalCostColor;
        }
        
    }
}
