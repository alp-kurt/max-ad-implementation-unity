using UnityEngine;

/// <summary>
/// Centralized configuration for all MAX ad unit IDs and banner settings.
/// 
/// This is a ScriptableObject — create one via:
///     Assets → Create → Ads → MAX Ad Settings
/// 
/// Storing ad unit IDs here keeps them editable in the Unity Inspector
/// and avoids hardcoding them in multiple places.
/// In another saying, one hard coding to rule them all.
/// </summary>
[CreateAssetMenu(menuName = "Ads/MAX Ad Settings", fileName = "MaxAdSettings")]
public class MaxAdSettings : ScriptableObject
{
    [Header("Android Ad Unit IDs")]
    [Tooltip("MAX ad unit ID for the Banner ad.")]
    public string androidBannerId;

    [Tooltip("MAX ad unit ID for the Interstitial ad.")]
    public string androidInterstitialId;

    [Tooltip("MAX ad unit ID for the first Rewarded ad (e.g., 'Skip Question').")]
    public string androidRewardedIdA;

    [Tooltip("MAX ad unit ID for the second Rewarded ad (e.g., 'Bonus Coins').")]
    public string androidRewardedIdB;

    [Header("Banner Appearance")]
    [Tooltip("Screen position where the banner should appear.")]
    public MaxSdkBase.AdViewPosition bannerPosition = MaxSdkBase.AdViewPosition.BottomCenter;

    [Tooltip("Background color behind the banner ad.")]
    public Color bannerBackground = Color.black;

    [Header("Behavior")]
    [Tooltip("If true, the banner starts hidden and must be shown via script.")]
    public bool startBannerHidden = true;
}
