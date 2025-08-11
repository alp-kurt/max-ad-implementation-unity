using UnityEngine;

/// <summary>
/// Central service for managing all MAX ad types in the app.
/// Acts as a single point of contact for banners, interstitials, and rewarded ads.
/// </summary>
public class MaxAdsService : MonoBehaviour
{
    public static MaxAdsService Instance { get; private set; }     // Singleton instance for easy global access

    [Header("Config (ScriptableObject)")]
    [SerializeField] private MaxAdSettings settings; // Stores ad unit IDs and configuration for each AD UNIT.

    // Individual ad type controllers, see their own classess for detailed debugging.
    private BannerAdController banner;
    private InterstitialAdController interstitial;
    private RewardedAdController rewardedA;
    private RewardedAdController rewardedB;

    private bool _initialized;
    public bool IsReady => _initialized;    // Quick check for ready status

    public static event System.Action OnServiceReady;     // Event fired when this service is fully initialized

    private void Awake()
    {
        // Ensure only one instance exists,
        // check Singleton pattern if not sure what this means
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create controller components for each UNIT AD
        // This is a hard coded feature, optimize it in the future to reduce complexity.
        if (!banner) banner = gameObject.AddComponent<BannerAdController>();
        if (!interstitial) interstitial = gameObject.AddComponent<InterstitialAdController>();
        if (!rewardedA) rewardedA = gameObject.AddComponent<RewardedAdController>();
        if (!rewardedB) rewardedB = gameObject.AddComponent<RewardedAdController>();
    }

    private void Start()
    {
        // Initialize immediately if SDK is already ready, otherwise wait until OnSdkInitializedEvent
        if (MaxSdk.IsInitialized()) Initialize();
        else MaxSdkCallbacks.OnSdkInitializedEvent += _ => Initialize();
    }

    /// <summary>
    /// Initializes all ad controllers and preloads ads.
    /// </summary>
    private void Initialize()
    {
        if (_initialized) return; // Avoid double init
        _initialized = true;

        // Init controllers with settings
        banner.Initialize(settings);
        interstitial.Initialize(settings.androidInterstitialId);

        // Rewarded Ads use 'label' field for easier tracking
        rewardedA.Initialize(settings.androidRewardedIdA, label: "Rewarded A");
        rewardedB.Initialize(settings.androidRewardedIdB, label: "Rewarded B");

        // Preload ads for faster first display
        interstitial.Preload();
        rewardedA.Preload();
        rewardedB.Preload();

        // Announce that the service is ready
        OnServiceReady?.Invoke();
        Debug.Log("[MAX] Service initialized");
    }

    // -------- Public API for UI buttons and other scripts --------

    /// <summary>
    /// Show or hide the banner ad.
    /// </summary>
    public void ToggleBanner() => banner.Toggle();

    /// <summary>
    /// Show an interstitial ad.
    /// </summary>
    public void ShowInterstitial() => interstitial.Show();

    /// <summary>
    /// Show Rewarded Ad A (configured for one reward scenario).
    /// </summary>
    public void ShowRewardedA() => rewardedA.Show();

    /// <summary>
    /// Show Rewarded Ad B (configured for a different reward scenario).
    /// </summary>
    public void ShowRewardedB() => rewardedB.Show();
}
