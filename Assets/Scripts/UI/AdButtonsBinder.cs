using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Connects UI Buttons to the corresponding methods in MaxAdsService.
/// 
/// This script is a convenience layer for demos or quick UI hookups, so buttons in the scene can call
/// banner toggle, interstitial, and rewarded ad methods without writing extra code.
/// </summary>
public class AdButtonsBinder : MonoBehaviour
{
    [SerializeField] private Button bannerToggleButton;
    [SerializeField] private Button interstitialButton;
    [SerializeField] private Button rewardedAButton;
    [SerializeField] private Button rewardedBButton;

    // Cached UnityActions so we can remove the exact same listeners later
    private UnityAction onBanner, onInter, onRewardA, onRewardB;
    private bool wired; // Tracks if listeners are currently attached

    private void OnEnable()
    {
        // If service is already ready, wire immediately; otherwise, wait for it
        if (MaxAdsService.Instance && MaxAdsService.Instance.IsReady) Wire();
        MaxAdsService.OnServiceReady += Wire;
    }

    private void OnDisable()
    {
        MaxAdsService.OnServiceReady -= Wire;
        Unwire();
    }

    /// <summary>
    /// Adds the button click listeners to call MaxAdsService methods.
    /// </summary>
    private void Wire()
    {
        if (wired) return; // Prevent double wiring
        var svc = MaxAdsService.Instance;
        if (!svc || !svc.IsReady) return;

        // Assign method references from service
        onBanner = svc.ToggleBanner;
        onInter = svc.ShowInterstitial;
        onRewardA = svc.ShowRewardedA;
        onRewardB = svc.ShowRewardedB;

        // --- Event: Banner toggle button clicked ---
        if (bannerToggleButton)
        {
            bannerToggleButton.onClick.AddListener(onBanner);
            Debug.Log("[Binder] Banner wired");
        }

        // --- Event: Interstitial button clicked ---
        if (interstitialButton)
        {
            interstitialButton.onClick.AddListener(onInter);
            Debug.Log("[Binder] Interstitial wired");
        }

        // --- Event: Rewarded A button clicked ---
        if (rewardedAButton)
        {
            rewardedAButton.onClick.AddListener(onRewardA);
            Debug.Log("[Binder] Rewarded A wired");
        }

        // --- Event: Rewarded B button clicked ---
        if (rewardedBButton)
        {
            rewardedBButton.onClick.AddListener(onRewardB);
            Debug.Log("[Binder] Rewarded B wired");
        }

        wired = true;
    }

    /// <summary>
    /// Removes all previously added button click listeners.
    /// </summary>
    private void Unwire()
    {
        if (!wired) return;
        if (bannerToggleButton) bannerToggleButton.onClick.RemoveListener(onBanner);
        if (interstitialButton) interstitialButton.onClick.RemoveListener(onInter);
        if (rewardedAButton) rewardedAButton.onClick.RemoveListener(onRewardA);
        if (rewardedBButton) rewardedBButton.onClick.RemoveListener(onRewardB);
        wired = false;
    }
}
