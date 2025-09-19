using System;
using System.Collections;
using UnityEngine;

public sealed class RewardedAds : MonoBehaviour
{
    private string _adUnitId;
    private string _label;
    private int _retryAttempt;
    private bool _ready;
    private Coroutine _retryCo;

    public event Action<string> OnRewardGranted;

    public void Initialize(string adUnitId, string label)
    {
        _adUnitId = adUnitId;
        _label = label;
        Subscribe();
    }

    public void Preload()
    {
        if (!string.IsNullOrWhiteSpace(_adUnitId))
            MaxSdk.LoadRewardedAd(_adUnitId);
    }

    public void Show()
    {
        if (_ready && MaxSdk.IsRewardedAdReady(_adUnitId))
            MaxSdk.ShowRewardedAd(_adUnitId);
        else
            Preload();
    }

    private void Subscribe()
    {
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnLoaded;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnLoadFailed;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnDisplayed;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnDisplayFailed;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnClicked;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnHidden;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnReceivedReward;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRevenue;
    }

    private void Unsubscribe()
    {
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnLoaded;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnLoadFailed;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnDisplayed;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnDisplayFailed;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent -= OnClicked;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnHidden;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnReceivedReward;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnRevenue;
    }

    private void OnDestroy()
    {
        Unsubscribe();
        if (_retryCo != null) StopCoroutine(_retryCo);
    }

    private void OnLoaded(string id, MaxSdk.AdInfo info) { if (id == _adUnitId) { _ready = true; _retryAttempt = 0; } }
    private void OnLoadFailed(string id, MaxSdk.ErrorInfo err) { if (id == _adUnitId) Retry(Preload); }
    private void OnDisplayed(string id, MaxSdk.AdInfo info) { }
    private void OnDisplayFailed(string id, MaxSdk.ErrorInfo err, MaxSdk.AdInfo info) { if (id == _adUnitId) Preload(); }
    private void OnClicked(string id, MaxSdk.AdInfo info) { }
    private void OnHidden(string id, MaxSdk.AdInfo info) { if (id == _adUnitId) { _ready = false; Preload(); } }

    private void OnReceivedReward(string id, MaxSdk.Reward reward, MaxSdk.AdInfo info)
    {
        if (id != _adUnitId) return;
        OnRewardGranted?.Invoke(_label);
    }

    private void OnRevenue(string id, MaxSdk.AdInfo info) { if (id == _adUnitId) { /* optional */ } }

    private void Retry(Action loadAction)
    {
        _retryAttempt++;
        double delay = Math.Pow(2, Mathf.Min(6, _retryAttempt));
        if (_retryCo != null) StopCoroutine(_retryCo);
        _retryCo = StartCoroutine(RetryAfter((float)delay, loadAction));
    }

    private IEnumerator RetryAfter(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }
}
