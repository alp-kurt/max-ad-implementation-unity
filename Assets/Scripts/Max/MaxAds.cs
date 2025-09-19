using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-900)]
public sealed class MaxAds : MonoBehaviour
{
    [Header("Profile (Editor/QA/Prod variants)")]
    [SerializeField] private MaxAdsProfile profile;

    // Dictionaries for quick lookup by label
    private readonly Dictionary<string, BannerAds> _banners = new();
    private readonly Dictionary<string, InterstitialAds> _inters = new();
    private readonly Dictionary<string, RewardedAds> _rewardeds = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!profile)
        {
            Debug.LogError("[MaxAds] Missing MaxAdsProfile reference.");
            return;
        }

        if (MaxSdk.IsInitialized()) InitializePlacements();
        else MaxSdkCallbacks.OnSdkInitializedEvent += _ => InitializePlacements();
    }

    private void InitializePlacements()
    {
        // Banners
        foreach (var b in profile.banners)
        {
            var c = gameObject.AddComponent<BannerAds>();
            c.Initialize(b.adUnitId, ToMaxAnchor(b.anchor), b.background, b.startHidden);
            SafeAdd(_banners, b.label, c, "Banner");
        }

        // Interstitials
        foreach (var i in profile.interstitials)
        {
            var c = gameObject.AddComponent<InterstitialAds>();
            c.Initialize(i.adUnitId);
            if (i.preloadOnStart) c.Preload();
            SafeAdd(_inters, i.label, c, "Interstitial");
        }

        // Rewardeds
        foreach (var r in profile.rewardeds)
        {
            var c = gameObject.AddComponent<RewardedAds>();
            c.Initialize(r.adUnitId, r.label);
            if (r.preloadOnStart) c.Preload();
            c.OnRewardGranted += _ => RouteReward(r);
            SafeAdd(_rewardeds, r.label, c, "Rewarded");
        }

        Debug.Log("[MaxAds] Placements initialized.");
    }

    // ---- Public API (call by label) ----
    public void ToggleBanner(string label) { if (_banners.TryGetValue(label, out var c)) c.Toggle(); else WarnMissing(label, "Banner"); }
    public void ShowInterstitial(string label) { if (_inters.TryGetValue(label, out var c)) c.Show(); else WarnMissing(label, "Interstitial"); }
    public void ShowRewarded(string label) { if (_rewardeds.TryGetValue(label, out var c)) c.Show(); else WarnMissing(label, "Rewarded"); }
    public void ShowBanner(string label) { if (_banners.TryGetValue(label, out var c)) c.Show(); else WarnMissing(label, "Banner"); }
    public void HideBanner(string label) { if (_banners.TryGetValue(label, out var c)) c.Hide(); else WarnMissing(label, "Banner"); }

    // Optional preload hooks (if you disable preloadOnStart in profile)
    public void PreloadInterstitial(string label) { if (_inters.TryGetValue(label, out var c)) c.Preload(); }
    public void PreloadRewarded(string label) { if (_rewardeds.TryGetValue(label, out var c)) c.Preload(); }

    // ---- Helpers ----
    private void RouteReward(RewardedPlacement r)
    {
        if (string.IsNullOrEmpty(r.rewardKey)) return;
    }

    private static void SafeAdd<T>(Dictionary<string, T> map, string label, T controller, string type)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            Debug.LogWarning($"[MaxAds] {type} has empty label; controller ignored.");
            return;
        }

        if (!map.TryAdd(label, controller))
            Debug.LogWarning($"[MaxAds] Duplicate label '{label}' for {type}; last one wins.");
    }

    private static MaxSdkBase.AdViewPosition ToMaxAnchor(BannerAnchor a) => a switch
    {
        BannerAnchor.TopLeft => MaxSdkBase.AdViewPosition.TopLeft,
        BannerAnchor.TopCenter => MaxSdkBase.AdViewPosition.TopCenter,
        BannerAnchor.TopRight => MaxSdkBase.AdViewPosition.TopRight,
        BannerAnchor.BottomLeft => MaxSdkBase.AdViewPosition.BottomLeft,
        BannerAnchor.BottomCenter => MaxSdkBase.AdViewPosition.BottomCenter,
        BannerAnchor.BottomRight => MaxSdkBase.AdViewPosition.BottomRight,
        _ => MaxSdkBase.AdViewPosition.BottomCenter
    };

    private static void WarnMissing(string label, string type) =>
        Debug.LogWarning($"[MaxAds] {type} with label '{label}' not found. Check your MaxAdsProfile.");
}
