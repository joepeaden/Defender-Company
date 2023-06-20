using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

/// <summary>
/// Class for UI that is specific to in-level gameplay.
/// </summary>
public class GameplayUI : MonoBehaviour
{
    public static GameplayUI Instance { get { return _instance; } }
    private static GameplayUI _instance;

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
    [SerializeField] private Image healGreenOutImg;
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

    private Player player;
    private VolumeProfile postProcProfile;

    // Input should really be centralized in an input handler class. We know this.
    // And we don't give a fuck. Not today at least.
    private PlayerControls controls;

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

        WaveManager.OnPrepForNextWave.AddListener(StartNewWaveCoroutine);
        WaveManager.OnWaveEnd.AddListener(ShowRewardUI);
        Scoreboard.OnScoreUpdated.AddListener(UpdateScore);

        confirmRewardButton.onClick.AddListener(HandleRewardConfirm);

        controls = new PlayerControls();
        controls.UI.Confirm.performed += HandleConfirmInput;
        controls.UI.Select.performed += HandleSelectInput;
        controls.UI.Navigate.started += HandleNavigation;

        ShopItemButton.OnNewHoveredButton.AddListener(UpdateHoveredButton);
    }

    private void Start()
    {
        postProcProfile = CameraManager.Instance.GetPostProcProf();

        // Reset the vingette.
        Vignette v;
        postProcProfile.TryGet(out v);
        v.intensity.Override(0f);

        player = GameManager.Instance.GetPlayerScript();
        player.OnSwitchWeapons.AddListener(UpdateCurrentWeapon);
        player.OnUpdateEquipment.AddListener(UpdateEquipment);
        Inventory.OnNewItemAddedPlayer.AddListener(UpdateCurrentWeapon);

        // missed the UpdateCurrentWeapon initial event, so just update it.
        UpdateCurrentWeapon(player.GetInventory().GetEquippedWeapon());
        UpdateEquipment(player.GetInventory().GetEquipment());
    }

    private void Update()
    {
        if (player)
        {
            (int loaded, int total) = player.GetAmmo();

            // int.MinValue means that the gun has infinite backup ammo (pistol)
            string totalAmmoString = (total == int.MinValue) ? "INF" : total.ToString();

            ammoTxt.text = "Ammo: " + loaded + "/" + totalAmmoString;
        }
    }

    private void OnDestroy()
    {
        WaveManager.OnPrepForNextWave.RemoveListener(StartNewWaveCoroutine);
        WaveManager.OnWaveEnd.RemoveListener(ShowRewardUI);
        player.OnSwitchWeapons.RemoveListener(UpdateCurrentWeapon);
        player.OnUpdateEquipment.RemoveListener(UpdateEquipment);
        Scoreboard.OnScoreUpdated.RemoveListener(UpdateScore);
        confirmRewardButton.onClick.RemoveListener(HandleRewardConfirm);
        controls.UI.Confirm.performed -= HandleConfirmInput;
        controls.UI.Select.performed -= HandleSelectInput;
        controls.UI.Navigate.started -= HandleNavigation;
        ShopItemButton.OnNewHoveredButton.RemoveListener(UpdateHoveredButton);
    }

    

    private void HandleConfirmInput(InputAction.CallbackContext cntxt)
    {
        HandleRewardConfirm();
    }

    private void HandleSelectInput(InputAction.CallbackContext cntxt)
    {
        if (shopItemButtons[hoveredButtonIndex]!= null)
        {
            shopItemButtons[hoveredButtonIndex].onClick.Invoke();
        }
    }

    private void HandleNavigation(InputAction.CallbackContext cntxt)
    {
        Vector2 input = cntxt.ReadValue<Vector2>();

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
        if (shopItem.rewardKey == pickedRewardItem.rewardKey)
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
        controls.UI.Disable();
        player.EnableControls();
        ShowBattleUI();
    }

    /// <summary>
    /// Activates reward UI, shows mouse, and populates shop items
    /// </summary>
    private void ShowRewardUI()
    {
        controls.UI.Enable();
        player.DisableControls();

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

    private void ShowBattleUI()
    {
        rewardUI.SetActive(false);
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

    private void StartNewWaveCoroutine()
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
