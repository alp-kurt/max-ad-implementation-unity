using UnityEngine;

public sealed class BannerAds : MonoBehaviour
{
    private string _adUnitId;
    private bool _created;
    private bool _visibleIntent;
    private bool _isLoaded;

    public void Initialize(string adUnitId, MaxSdkBase.AdViewPosition position, Color background, bool startHidden)
    {
        if (string.IsNullOrWhiteSpace(adUnitId))
        {
            Debug.LogWarning("[BannerAds] Missing Ad Unit Id");
            return;
        }

        _adUnitId = adUnitId;

        var cfg = new MaxSdk.AdViewConfiguration(position);
        MaxSdk.CreateBanner(_adUnitId, cfg);
        MaxSdk.SetBannerBackgroundColor(_adUnitId, background);

        Subscribe();
        MaxSdk.LoadBanner(_adUnitId);

        _visibleIntent = !startHidden;
        _created = true;
    }

    public void Show()
    {
        if (!_created) return;
        _visibleIntent = true;
        if (_isLoaded) MaxSdk.ShowBanner(_adUnitId);
        else MaxSdk.LoadBanner(_adUnitId);
    }

    public void Hide()
    {
        if (!_created) return;
        _visibleIntent = false;
        MaxSdk.HideBanner(_adUnitId);
    }

    public void Toggle()
    {
        if (_visibleIntent) Hide();
        else Show();
    }

    private void Subscribe()
    {
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnAdLoaded;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnAdLoadFailed;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnAdClicked;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnAdExpanded;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnAdCollapsed;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaid;
    }

    private void Unsubscribe()
    {
        MaxSdkCallbacks.Banner.OnAdLoadedEvent -= OnAdLoaded;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent -= OnAdLoadFailed;
        MaxSdkCallbacks.Banner.OnAdClickedEvent -= OnAdClicked;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent -= OnAdExpanded;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent -= OnAdCollapsed;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= OnAdRevenuePaid;
    }

    private void OnDestroy() => Unsubscribe();

    private void OnAdLoaded(string id, MaxSdk.AdInfo info)
    {
        if (id != _adUnitId) return;
        _isLoaded = true;
        if (_visibleIntent) MaxSdk.ShowBanner(_adUnitId);
    }

    private void OnAdLoadFailed(string id, MaxSdk.ErrorInfo err) { if (id == _adUnitId) Debug.Log($"[Banner] LoadFailed {err.Code} {err.Message}"); }
    private void OnAdClicked(string id, MaxSdk.AdInfo info) { if (id == _adUnitId) Debug.Log("[Banner] Clicked"); }
    private void OnAdExpanded(string id, MaxSdk.AdInfo info) { if (id == _adUnitId) Debug.Log("[Banner] Expanded"); }
    private void OnAdCollapsed(string id, MaxSdk.AdInfo info) { if (id == _adUnitId) Debug.Log("[Banner] Collapsed"); }
    private void OnAdRevenuePaid(string id, MaxSdk.AdInfo info) { if (id == _adUnitId) Debug.Log($"[Banner] Revenue {info.Revenue} via {info.NetworkName}"); }
}
