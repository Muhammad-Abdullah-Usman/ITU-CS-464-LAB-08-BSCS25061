using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutController : MonoBehaviour
{
    [Header("Equipment")]
    [SerializeField] Button[] weaponButtons = new Button[3];
    [SerializeField] TMP_Text equipmentDetails;
    [SerializeField] Button equipButton;

    [Header("Upgrades")]
    [SerializeField] TMP_Text balanceText;
    [SerializeField] TMP_Text upgradeDetails;
    [SerializeField] Button upgradeButton;

    const string CoinsKey = "WanderingBlade.Coins";
    const string WeaponKey = "WanderingBlade.Weapon";
    const string FocusKey = "WanderingBlade.Focus";
    const int MaxFocusLevel = 3;

    readonly string[] weaponNames =
    {
        "Katana", "Naginata", "Twin Blades"
    };

    readonly string[] weaponDescriptions =
    {
        "A single curved sword with a traditional profile.",
        "A curved blade mounted on a long wooden shaft.",
        "A paired set of short blades carried together."
    };

    public int Coins { get; private set; }
    public int EquippedWeaponIndex { get; private set; }
    public int FocusLevel { get; private set; }

    public string EquippedWeaponName =>
        weaponNames[EquippedWeaponIndex];

    public float StrikeCooldown =>
        0.65f - FocusLevel * 0.1f;

    public int UpgradeCost => 60 + FocusLevel * 40;

    int selectedWeapon;
    Vector3 balanceScale;
    Sequence balanceTween;

    void Awake()
    {
        Coins = Mathf.Max(0, PlayerPrefs.GetInt(CoinsKey, 200));

        EquippedWeaponIndex = Mathf.Clamp(
            PlayerPrefs.GetInt(WeaponKey, 0),
            0, weaponNames.Length - 1);

        FocusLevel = Mathf.Clamp(
            PlayerPrefs.GetInt(FocusKey, 0),
            0, MaxFocusLevel);

        selectedWeapon = EquippedWeaponIndex;
    }

    void Start()
    {
        balanceScale = balanceText.transform.localScale;
        RefreshUI();
    }

    public void SelectWeapon(int index)
    {
        if (index < 0 || index >= weaponNames.Length)
            return;

        selectedWeapon = index;
        RefreshUI();
    }

    public void EquipSelected()
    {
        if (selectedWeapon == EquippedWeaponIndex)
            return;

        EquippedWeaponIndex = selectedWeapon;
        SaveAndRefresh();
        UIAudio.Instance.Success();
    }

    public void BuyFocusUpgrade()
    {
        if (FocusLevel >= MaxFocusLevel || Coins < UpgradeCost)
        {
            UIAudio.Instance.Error();
            return;
        }

        Coins -= UpgradeCost;
        FocusLevel++;
        SaveAndRefresh();
        PulseBalance();
        UIAudio.Instance.Success();
    }

    // The mission controller will call this after a successful mission.
    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        Coins += amount;
        SaveAndRefresh();
    }

    void SaveAndRefresh()
    {
        PlayerPrefs.SetInt(CoinsKey, Coins);
        PlayerPrefs.SetInt(WeaponKey, EquippedWeaponIndex);
        PlayerPrefs.SetInt(FocusKey, FocusLevel);
        PlayerPrefs.Save();

        RefreshUI();
    }

    void RefreshUI()
    {
        Color32 ink = new Color32(36, 38, 34, 255);
        Color32 normal = new Color32(216, 207, 190, 255);
        Color32 selected = new Color32(181, 166, 139, 255);

        for (int i = 0; i < weaponButtons.Length; i++)
        {
            TMP_Text label =
                weaponButtons[i].GetComponentInChildren<TMP_Text>(true);

            string prefix = i == selectedWeapon ? "> " : "";
            string suffix = i == EquippedWeaponIndex
                ? " [EQUIPPED]" : "";

            label.text =
                prefix + weaponNames[i].ToUpperInvariant() + suffix;

            label.color = ink;

            weaponButtons[i].image.color =
                i == selectedWeapon ? selected : normal;
        }

        equipmentDetails.text =
            $"Selected: {weaponNames[selectedWeapon]}\n" +
            $"{weaponDescriptions[selectedWeapon]}\n" +
            $"Equipped: {EquippedWeaponName}";

        bool canEquip = selectedWeapon != EquippedWeaponIndex;

        SetActionButton(
            equipButton,
            canEquip,
            canEquip ? "EQUIP" : "EQUIPPED");

        balanceText.text = $"COINS: {Coins}";

        if (FocusLevel >= MaxFocusLevel)
        {
            upgradeDetails.text =
                $"Focus level: {FocusLevel} / {MaxFocusLevel}\n\n" +
                $"Strike recovery: {StrikeCooldown:0.00} seconds\n" +
                "Training complete.";

            SetActionButton(upgradeButton, false, "MAX LEVEL");
        }
        else
        {
            upgradeDetails.text =
                $"Focus level: {FocusLevel} / {MaxFocusLevel}\n\n" +
                $"Current recovery: {StrikeCooldown:0.00} seconds\n" +
                $"After upgrade: {StrikeCooldown - 0.1f:0.00} seconds\n" +
                $"Cost: {UpgradeCost} coins";

            bool affordable = Coins >= UpgradeCost;

            SetActionButton(
                upgradeButton,
                affordable,
                affordable
                    ? $"UPGRADE - {UpgradeCost} COINS"
                    : $"NEED {UpgradeCost} COINS");
        }
    }

    void SetActionButton(Button button, bool enabled, string text)
    {
        button.interactable = enabled;

        TMP_Text label =
            button.GetComponentInChildren<TMP_Text>(true);

        label.text = text;
        label.color = enabled
            ? new Color32(238, 231, 216, 255)
            : new Color32(36, 38, 34, 255);
    }

    void PulseBalance()
    {
        if (balanceTween != null)
            balanceTween.Kill();

        balanceText.transform.localScale = balanceScale;

        balanceTween = DOTween.Sequence().SetUpdate(true)
            .Append(balanceText.transform
                .DOScale(balanceScale * 1.08f, 0.08f)
                .SetEase(Ease.OutQuad))
            .Append(balanceText.transform
                .DOScale(balanceScale, 0.16f)
                .SetEase(Ease.OutCubic));
    }

    void OnDestroy()
    {
        if (balanceTween != null)
            balanceTween.Kill();
    }
}