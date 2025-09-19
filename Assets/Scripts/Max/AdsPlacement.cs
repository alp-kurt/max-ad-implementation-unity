using UnityEngine;

public enum BannerAnchor
{
    TopLeft, TopCenter, TopRight,
    BottomLeft, BottomCenter, BottomRight
}

[System.Serializable]
public class BannerPlacement
{
    [Tooltip("Unique key you will call at runtime, e.g., \"Home_Banner_1\"")]
    public string label;

    [Tooltip("AppLovin MAX Banner Ad Unit ID")]
    public string adUnitId;

    [Tooltip("Where the banner sticks on the screen")]
    public BannerAnchor anchor = BannerAnchor.BottomCenter;

    [Tooltip("Background color for the banner view (some networks use this)")]
    public Color background = Color.black;

    [Tooltip("If true, the banner starts hidden. Toggle from UI when needed.")]
    public bool startHidden = true;
}

[System.Serializable]
public class InterstitialPlacement
{
    [Tooltip("Unique key you will call at runtime, e.g., \"LevelEnd_Interstitial\"")]
    public string label;

    [Tooltip("AppLovin MAX Interstitial Ad Unit ID")]
    public string adUnitId;

    [Tooltip("Preload on Start. If false, you should call Preload() from code.")]
    public bool preloadOnStart = true;
}

[System.Serializable]
public class RewardedPlacement
{
    [Tooltip("Unique key you will call at runtime, e.g., \"Revive_Rewarded\"")]
    public string label;

    [Tooltip("AppLovin MAX Rewarded Ad Unit ID")]
    public string adUnitId;

    [Tooltip("Preload on Start. If false, you should call Preload() from code.")]
    public bool preloadOnStart = true;

    [Header("Optional: Lightweight Reward Routing")]
    [Tooltip("Leave empty to ignore built-in routing. Example: \"coins\" or \"revive\"")]
    public string rewardKey = "";

    [Tooltip("Used if network reward amount is not provided or 0")]
    public int fallbackAmount = 1;
}
