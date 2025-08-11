using UnityEngine;

/// <summary>
/// Handles creation, display, and toggling of a MAX BANNER AD.
/// 
/// Reads settings from a MaxAdSettings ScriptableObject so ad unit IDs
/// and positions can be changed without touching code.
/// 
/// Subscribes to all relevant MAX banner events for logging and troubleshooting.
/// </summary>
public class BannerAdController : MonoBehaviour
{
    private string _adUnitId;
    private bool _created;
    private bool _visible;
    private bool _isLoaded;  
    private MaxAdSettings _settings;

    /// <summary>
    /// Initializes the banner ad using provided settings.
    /// Creates the banner, sets its appearance, and optionally shows/hides it on start.
    /// </summary>
    public void Initialize(MaxAdSettings settings)
    {
        _settings = settings;
        _adUnitId = settings.androidBannerId;

        if (string.IsNullOrEmpty(_adUnitId))
        {
            Debug.LogWarning("[Banner] Missing Ad Unit ID");
            return;
        }

        // Create banner at specified position
        var cfg = new MaxSdk.AdViewConfiguration(_settings.bannerPosition);
        MaxSdk.CreateBanner(_adUnitId, cfg);

        // Set background color
        MaxSdk.SetBannerBackgroundColor(_adUnitId, _settings.bannerBackground);

        // --- Event: Banner successfully loaded ---
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += (id, info) =>
        {
            if (id == _adUnitId)
            {
                _isLoaded = true;                         
                Debug.Log($"[Banner] Loaded {id}");
                if (_visible) MaxSdk.ShowBanner(_adUnitId);
            }
        };

        // --- Event: Banner failed to load ---
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += (id, err) =>
        {
            if (id == _adUnitId) Debug.Log($"[Banner] LoadFailed {id} code={err.Code} msg={err.Message}");
        };

        // --- Event: Banner clicked ---
        MaxSdkCallbacks.Banner.OnAdClickedEvent += (id, info) =>
        {
            if (id == _adUnitId) Debug.Log($"[Banner] Clicked {id}");
        };

        // --- Event: Banner expanded (fullscreen takeover or similar) ---
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += (id, info) =>
        {
            if (id == _adUnitId) Debug.Log($"[Banner] Expanded {id}");
        };

        // --- Event: Banner collapsed (returned to normal size) ---
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += (id, info) =>
        {
            if (id == _adUnitId) Debug.Log($"[Banner] Collapsed {id}");
        };

        // --- Event: Revenue tracking ---
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += (id, info) =>
        {
            if (id == _adUnitId) Debug.Log($"[Banner] RevenuePaid unit={id} value={info.Revenue} network={info.NetworkName}");
        };

        MaxSdk.LoadBanner(_adUnitId);

        // Record initial intent; DO NOT show immediately
        _visible = !_settings.startBannerHidden;

        _created = true;
    }

    /// <summary>
    /// Shows the banner if created.
    /// </summary>
    public void Show()
    {
        if (!_created) return;

        _visible = true;

        if (_isLoaded)
        {
            MaxSdk.ShowBanner(_adUnitId);
            Debug.Log("[Banner] Shown");
        }
        else
        {
            // Not ready yet → load; OnAdLoaded will show when ready
            Debug.Log("[Banner] Not loaded yet...");
            MaxSdk.LoadBanner(_adUnitId);
        }
    }

    /// <summary>
    /// Hides the banner.
    /// </summary>
    public void Hide()
    {
        if (!_created) return;
        _visible = false;
        MaxSdk.HideBanner(_adUnitId);
        Debug.Log("[Banner] Hidden");
    }

    /// <summary>
    /// Toggles the banner visibility.
    /// </summary>
    public void Toggle()
    {
        if (!_created)
        {
            Debug.LogWarning("[Banner] Not created yet");
            return;
        }
        if (_visible) Hide();
        else Show();
    }
}
