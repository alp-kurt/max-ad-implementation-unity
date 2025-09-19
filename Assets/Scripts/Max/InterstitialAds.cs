using System;
using System.Collections;
using UnityEngine;

public sealed class InterstitialAds : MonoBehaviour
{
    private string _adUnitId;
    private int _retryAttempt;
    private Coroutine _retryCo;

    public void Initialize(string adUnitId)
    {
        _adUnitId = adUnitId;
        Subscribe();
    }

    public void Preload()
    {
        if (!string.IsNullOrWhiteSpace(_adUnitId))
            MaxSdk.LoadInterstitial(_adUnitId);
    }

    public void Show()
    {
        if (MaxSdk.IsInterstitialReady(_adUnitId))
            MaxSdk.ShowInterstitial(_adUnitId);
        else
            Preload();
    }

    private void Subscribe()
    {
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnLoaded;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnLoadFailed;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnDisplayed;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnDisplayFailed;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnClicked;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnHidden;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnRevenue;
    }

    private void Unsubscribe()
    {
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnLoaded;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnLoadFailed;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent -= OnDisplayed;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= OnDisplayFailed;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent -= OnClicked;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnHidden;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= OnRevenue;
    }

    private void OnDestroy()
    {
        Unsubscribe();
        if (_retryCo != null) StopCoroutine(_retryCo);
    }

    private void OnLoaded(string id, MaxSdk.AdInfo info)
    {
        if (id != _adUnitId) return;
        _retryAttempt = 0;
    }

    private void OnLoadFailed(string id, MaxSdk.ErrorInfo error)
    {
        if (id != _adUnitId) return;
        Retry(Preload);
    }

    private void OnDisplayed(string id, MaxSdk.AdInfo info) { }
    private void OnDisplayFailed(string id, MaxSdk.ErrorInfo error, MaxSdk.AdInfo info) { if (id == _adUnitId) Preload(); }
    private void OnClicked(string id, MaxSdk.AdInfo info) { }
    private void OnHidden(string id, MaxSdk.AdInfo info) { if (id == _adUnitId) Preload(); }
    private void OnRevenue(string id, MaxSdk.AdInfo info) { }

    private void Retry(Action loadAction)
    {
        _retryAttempt++;
        double delay = Math.Pow(2, Mathf.Min(6, _retryAttempt)); // up to 64s
        if (_retryCo != null) StopCoroutine(_retryCo);
        _retryCo = StartCoroutine(RetryAfter((float)delay, loadAction));
    }

    private IEnumerator RetryAfter(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }
}
