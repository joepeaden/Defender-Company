using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

/// <summary>
/// Class for UI that is specific to Mission gameplay.
/// </summary>
/// <remarks>
/// The rewards screen in here isn't used any more, and neither is the game over stuff. However you could add the pause screen in here. Could just replace
/// rewards screen with pause screen (reuse code and UI).
/// </remarks>
public class MissionUI : MonoBehaviour
{
    public static MissionUI Instance { get { return _instance; } }
    private static MissionUI _instance;

    public static UnityEvent<ShopItem> OnRewardsPicked = new UnityEvent<ShopItem>();

    #region BattleUI vars
    [Header("Battle UI")]
    [SerializeField] private GameObject battleUI;
    [SerializeField] private TMP_Text curntWpnTxt;
    [SerializeField] private TMP_Text ammoTxt;
    [SerializeField] private TMP_Text waveTxt;
    [SerializeField] private TMP_Text pointsTxt;
    [SerializeField] private TMP_Text equipmentTxt;
    [SerializeField] private RectTransform reloadBarTransform;
    [SerializeField] private GameObject objectiveMarkerPrefab;
    [SerializeField] private GameObject entityMarkerPrefab;
    [SerializeField] private Image healGreenOutImg;
    [SerializeField] private GameObject gateGameObject;
    #endregion

    #region RewardUI vars
    [Header("Reward UI")]
    [SerializeField] private GameObject rewardUI;
    [SerializeField] private Button confirmRewardButton;
    [SerializeField] private Button shopItemButtonPrefab;
    [SerializeField] private Transform shopItemParent;
    private ShopItem pickedRewardItem;
    private int hoveredButtonIndex;
    private List<Button> shopItemButtons = new List<Button>();
    #endregion

    [Header("Building UI")]
    [SerializeField] private GameObject buildingUI;
    [SerializeField] private TMP_Text remainingTUText;
    private TMP_Text wallBuildingButtonText;
    [SerializeField] private Button wallBuildingButton;
    private TMP_Text stairsBuildingButtonText;
    [SerializeField] private Button stairsBuildingButton;
    private TMP_Text barricadesBuildingButtonText;
    [SerializeField] private Button barricadesBuildingButton;

    private Player player;
    private VolumeProfile postProcProfile;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("More than one Gameplay UI, deleting one.");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        //Scoreboard.OnScoreUpdated.AddListener(UpdateScore);

        confirmRewardButton.onClick.AddListener(HandleRewardConfirm);
        
        PlayerInput.OnConfirm.AddListener(HandleRewardConfirm);
        PlayerInput.OnSelect.AddListener(HandleSelectInput);
        PlayerInput.OnNavigate.AddListener(HandleNavigation);

        ShopItemButton.OnNewHoveredButton.AddListener(UpdateHoveredButton);

        wallBuildingButtonText = wallBuildingButton.transform.GetComponentInChildren<TMP_Text>();
        stairsBuildingButtonText = stairsBuildingButton.transform.GetComponentInChildren<TMP_Text>();
        barricadesBuildingButtonText = barricadesBuildingButton.transform.GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        postProcProfile = CameraManager.Instance.GetPostProcProf();

        // Reset the vingette.
        Vignette v;
        postProcProfile.TryGet(out v);
        v.intensity.Override(0f);

        player = MissionManager.Instance.GetPlayerScript();
        player.OnSwitchWeapons.AddListener(UpdateCurrentWeapon);
        player.OnUpdateEquipment.AddListener(UpdateEquipment);
        Inventory.OnNewItemAddedPlayer.AddListener(UpdateCurrentWeapon);

        // missed the UpdateCurrentWeapon initial event, so just update it.
        //UpdateCurrentWeapon(player.GetInventory().GetEquippedWeapon());
        //UpdateEquipment(player.GetInventory().GetEquipment());

        AddObjectiveMarker(gateGameObject, "DEFEND");

        //PlayerInput.OnDragStarted.AddListener(OnDragStartRegistered);
        //PlayerInput.OnDragEnded.AddListener(OnDragEndRegistered);
    }

    // fuck it for now
    private bool isSelectionDragging;
    //private void OnDragStartRegistered()
    //{
    //    isSelectionDragging = true;

    //    selectionBox.SetActive(true);
    //}
    //private void OnDragEndRegistered()
    //{
    //    isSelectionDragging = false;

    //    selectionBox.SetActive(false);
    //}

    private void Update()
    {
        if (player)
        {
            (int loaded, int total) = player.GetAmmo();

            // int.MinValue means that the gun has infinite backup ammo (pistol)
            string totalAmmoString = (total == int.MinValue) ? "INF" : total.ToString();

            ammoTxt.text = "Ammo: " + loaded + "/" + totalAmmoString;
        }

        if (isSelectionDragging)
        {
            Vector2 dragStart = Camera.main.WorldToScreenPoint(PlayerInput.dragStart);
            Vector2 dragEnd = Camera.main.WorldToScreenPoint(PlayerInput.dragEnd);


            selectionBox.GetComponent<RectTransform>().anchorMax = dragStart;
            selectionBox.GetComponent<RectTransform>().anchorMin = dragEnd;

            //Rect r = selectionBox.GetComponent<RectTransform>().rect;

            //r.xMin = PlayerInput.dragStart.x;
            //r.xMax = PlayerInput.dragEnd.x;
            //r.yMax = PlayerInput.dragStart.y;
            //r.yMin = PlayerInput.dragEnd.y;

            //selectionBox.GetComponent<RectTransform>().rect = r;
        }
    }

    public GameObject selectionBox;

    private void OnDestroy()
    {
        player.OnSwitchWeapons.RemoveListener(UpdateCurrentWeapon);
        player.OnUpdateEquipment.RemoveListener(UpdateEquipment);
        //Scoreboard.OnScoreUpdated.RemoveListener(UpdateScore);
        confirmRewardButton.onClick.RemoveListener(HandleRewardConfirm);
        PlayerInput.OnConfirm.RemoveListener(HandleRewardConfirm);
        PlayerInput.OnSelect.RemoveListener(HandleSelectInput);
        PlayerInput.OnNavigate.RemoveListener(HandleNavigation);
        ShopItemButton.OnNewHoveredButton.RemoveListener(UpdateHoveredButton);
    }

    private void HandleSelectInput()
    {
        if (shopItemButtons[hoveredButtonIndex]!= null)
        {
            shopItemButtons[hoveredButtonIndex].onClick.Invoke();
        }
    }

    private void HandleNavigation(Vector2 input)
    {

        // un-hover last button
        shopItemButtons[hoveredButtonIndex].GetComponent<ShopItemButton>().SetHover(false);

        // just handling horizontal for now
        if (input.x > 0)
        {
            hoveredButtonIndex++;
            if (hoveredButtonIndex >= shopItemButtons.Count)
            {
                hoveredButtonIndex--;
            }
        }
        else
        {
            hoveredButtonIndex--;
            if (hoveredButtonIndex < 0)
            {
                hoveredButtonIndex++;
            }
        }

        // hover new button
        shopItemButtons[hoveredButtonIndex].GetComponent<ShopItemButton>().SetHover(true);
    }

    private void UpdateHoveredButton(Button b)
    {
        hoveredButtonIndex = shopItemButtons.IndexOf(b);
    }

    public void AddObjectiveMarker(GameObject objectToMark, string label)
    {
        GameObject marker = Instantiate(objectiveMarkerPrefab, battleUI.transform);
        marker.GetComponent<ObjectiveMarker>().SetData(objectToMark.transform, label);
    }

    public void AddEntityMarker(ActorController controller, string label)
    {
        GameObject marker = Instantiate(entityMarkerPrefab, battleUI.transform);
        marker.GetComponent<EntityMarker>().SetData(controller, label);
    }

    public bool InMenu()
    {
        return rewardUI.activeInHierarchy;
    }

    /// <summary>
    /// Handle a reward choice.
    /// </summary>
    /// <param name="rewardKey">String code for the inventory item chosen.</param>
    public void HandleRewardPicked(ShopItem shopItem)
    {
        // if it's the same button, deselect the reward by making a blank one.
        if (shopItem.gearType == pickedRewardItem.gearType)
        {
            // this could be better. Instantiating a new object every time isn't really necessary.
            // I don't care though. But now you know that I know. Bro.
            shopItem = new ShopItem();
        }

        pickedRewardItem = shopItem;
    }    

    private void HandleRewardConfirm()
    {
        OnRewardsPicked.Invoke(pickedRewardItem);
        // have to reset picked reward and structs are non-nullable
        pickedRewardItem = new ShopItem();
        PlayerInput.DisableMenuControls();
        PlayerInput.EnableGameplayControls();
        ShowBattleUI();
    }

    /// <summary>
    /// Activates reward UI, shows mouse, and populates shop items
    /// </summary>
    private void ShowRewardUI()
    {
        PlayerInput.EnableMenuControls();
        PlayerInput.DisableGameplayControls();

        rewardUI.SetActive(true);
        battleUI.SetActive(false);

        // clear children
        foreach (Button shopButton in shopItemButtons)
        {
            Destroy(shopButton.gameObject);
        }
        shopItemButtons.Clear();

        bool firstOne = true;
        // add reward buttons
        foreach (ShopItem item in Rewards.Instance.GetRewardShopItems())
        {
            ShopItemButton shopButtonScript = Instantiate(shopItemButtonPrefab, shopItemParent).GetComponent<ShopItemButton>();
            shopButtonScript.SetItem(item);
            shopItemButtons.Add(shopButtonScript.GetComponent<Button>());

            // make the first one appear hovered
            if (firstOne)
            {
                shopButtonScript.SetHover(true);
                hoveredButtonIndex = 0;
                firstOne = false;
            }
        }
    }

    public void UpdateRemainingTU(int newVal)
    {
        remainingTUText.text = "Remaining Time Units: " + newVal;
        
        wallBuildingButton.interactable = newVal > BuildingManager.Instance.buildingTUCost;
        wallBuildingButtonText.color = wallBuildingButton.interactable ? Color.white : Color.red;
        stairsBuildingButton.interactable = newVal > BuildingManager.Instance.buildingTUCost;
        stairsBuildingButtonText.color = stairsBuildingButton.interactable ? Color.white : Color.red;
        barricadesBuildingButton.interactable = newVal > BuildingManager.Instance.buildingTUCost;
        barricadesBuildingButtonText.color = barricadesBuildingButton.interactable ? Color.white : Color.red;
    }

    public void ShowBattleUI()
    {
        ShowWaveText();

        rewardUI.SetActive(false);
        buildingUI.SetActive(false);
        battleUI.SetActive(true);
    }

    private void UpdateScore(int totalPoints)
    {
        pointsTxt.text = "Points: $" + totalPoints + ".00";
    }

    private void UpdateCurrentWeapon()
    {
        curntWpnTxt.text = player.GetInventory().GetEquippedWeapon().data.displayName;
    }

    private void UpdateCurrentWeapon(InventoryWeapon weapon)
    {
        curntWpnTxt.text = weapon.data.displayName;
    }

    private void UpdateEquipment(Equipment eq)
    {
        string str;
        if (eq == null)
        {
            str = "No Equipment";
        }
        else
        {
            str = $"{eq.data.displayName} {eq.amount}";
        }
        equipmentTxt.text = str;
    }

    private void ShowWaveText()
    {
        StartCoroutine(WaveTextFade());
    }

    private IEnumerator WaveTextFade()
    {
        float timePassed = 0f;
        waveTxt.alpha = 0f;
        waveTxt.gameObject.SetActive(true);

        while (timePassed <  1f)
        {
            float percent = timePassed / 1f;
            waveTxt.alpha = percent;

            timePassed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        while (timePassed > 0f)
        {
            float percent = timePassed / 1f;
            waveTxt.alpha = percent;

            timePassed -= Time.deltaTime;
            yield return null;
        }

        waveTxt.gameObject.SetActive(false);
    }

    /// <summary>
    /// Start the reload bar animation.
    /// </summary>
    /// <param name="time">Time over which the bar should stretch.</param>
    public void StartReloadBarAnimation(float time)
    {
        StartCoroutine(ReloadBarStretch(time));
    }

    /// <summary>
    /// Stretches the reload bar from 0 to some value to show feedback for reloading.
    /// </summary>
    /// <param name="time">Time over which the bar should stretch.</param>
    public IEnumerator ReloadBarStretch(float time)
    {
        float timePassed = 0f;

        float origSize = reloadBarTransform.sizeDelta.x;
        reloadBarTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
        reloadBarTransform.gameObject.SetActive(true);

        while (timePassed < time)
        {
            float percent = timePassed / time;
            reloadBarTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (origSize * percent));

            timePassed += Time.deltaTime;
            yield return null;
        }

        reloadBarTransform.gameObject.SetActive(false);
    }

    public void SetVignette(float percent)
    {

        StartCoroutine(AnimateVingette(percent));
    }

    // clean this up. It just makes the vingette effect move visibly rather than just pop to a new val in single frame.
    private IEnumerator AnimateVingette(float newVingetteIntensity)
    {
        Vignette v;
        postProcProfile.TryGet(out v);

        float oldVingetteIntensity = v.intensity.value;
        float increment;

        // if new val less than current val, player is being healed.
        if (newVingetteIntensity < oldVingetteIntensity)
        {
            increment = -0.01f;

            float currentVingetteIntensity = oldVingetteIntensity;
            while (currentVingetteIntensity > newVingetteIntensity)
            {
                currentVingetteIntensity += increment;
                v.intensity.Override(currentVingetteIntensity);

                yield return null;
            }
        }
        else
        {
            // this should be a variable. Maybe in a SO. Idk though might be too many data objects? Maybe? Also just one prefab of this in a game so not like it's a size issue... hmm.
            increment = 0.01f;

            float currentVingetteIntensity = oldVingetteIntensity;
            while (currentVingetteIntensity < newVingetteIntensity)
            {
                currentVingetteIntensity += increment;
                v.intensity.Override(currentVingetteIntensity);

                yield return null;
            }
        }

        v.color.Override(Color.red);
    }

    public void HealthFlash()
    {
        StartCoroutine(StartHealthFlash());
    }

    // just debug/iteration/tweaking public vars for the following method.
    //public float healthFlashDuration;
    //public float healthFlashAlpha;

    private IEnumerator StartHealthFlash()
    {
        float healthFlashDuration = .75f;
        float healthFlashAlpha = .33f;

        float remainingTime = healthFlashDuration;

        while (remainingTime > 0)
        {
            float percent = remainingTime / healthFlashDuration;
            Color newColor = healGreenOutImg.color;
            newColor.a = healthFlashAlpha * percent;
            healGreenOutImg.color = newColor;

            remainingTime -= Time.deltaTime;
            yield return null;
        }
    }
}
