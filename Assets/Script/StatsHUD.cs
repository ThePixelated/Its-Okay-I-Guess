using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Syncs the 4 main stats (Health, Energy, Money, Social) from PlayerData
/// to their corresponding UI Sliders. Subscribes to PlayerData.OnStatsChanged
/// so it auto-refreshes any time ApplyStatEffect/Load/Reset happens —
/// no manual polling needed.
///
/// Attach to: a HUD GameObject in the main Canvas (always active during gameplay).
/// </summary>
public class StatsHUD : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private PlayerData playerData;

    [Header("Sliders (range 0–100)")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider energySlider;
    [SerializeField] private Slider moneySlider;
    [SerializeField] private Slider socialSlider;

    [Header("Animation")]
    [Tooltip("If true, sliders smoothly animate toward new values instead of snapping instantly")]
    [SerializeField] private bool animateChanges = true;
    [SerializeField] private float animationSpeed = 3f;

    private float _targetHealth, _targetEnergy, _targetMoney, _targetSocial;

    private void OnEnable()
    {
        if (playerData == null)
        {
            Debug.LogError("[StatsHUD] PlayerData reference is missing!");
            return;
        }

        playerData.OnStatsChanged += RefreshFromData;

        // Initial sync — important if PlayerData.Load() already ran before this HUD is enabled
        RefreshFromData();
        SnapSlidersToTarget();
    }

    private void OnDisable()
    {
        if (playerData != null)
            playerData.OnStatsChanged -= RefreshFromData;
    }

    private void Update()
    {
        if (!animateChanges) return;

        AnimateSlider(healthSlider, _targetHealth);
        AnimateSlider(energySlider, _targetEnergy);
        AnimateSlider(moneySlider, _targetMoney);
        AnimateSlider(socialSlider, _targetSocial);
    }

    /// <summary>Pulls current values from PlayerData and sets new animation targets.</summary>
    private void RefreshFromData()
    {
        _targetHealth = playerData.Health;
        _targetEnergy = playerData.Energy;
        _targetMoney = playerData.Money;
        _targetSocial = playerData.Social;

        if (!animateChanges)
            SnapSlidersToTarget();
    }

    private void SnapSlidersToTarget()
    {
        if (healthSlider != null) healthSlider.value = _targetHealth;
        if (energySlider != null) energySlider.value = _targetEnergy;
        if (moneySlider != null) moneySlider.value = _targetMoney;
        if (socialSlider != null) socialSlider.value = _targetSocial;
    }

    private void AnimateSlider(Slider slider, float target)
    {
        if (slider == null) return;
        if (Mathf.Approximately(slider.value, target)) return;

        slider.value = Mathf.Lerp(slider.value, target, Time.unscaledDeltaTime * animationSpeed);

        if (Mathf.Abs(slider.value - target) < 0.1f)
            slider.value = target;
    }
}