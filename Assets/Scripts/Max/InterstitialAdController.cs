using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Handles loading, showing, and retrying of MAX interstitial ads.
/// 
/// Each controller instance manages a single ad unit ID.
/// Subscribes to all MAX interstitial events for logging and automatic retries.
/// </summary>
public class InterstitialAdController : MonoBehaviour
{
    private string _adUnitId;
    private int _retryAttempt;

    /// <summary>
    /// Sets up the interstitial controller and subscribes to MAX events.
    /// </summary>
    public void Initialize(string adUnitId)
    {
        _adUnitId = adUnitId;

        // --- Event: Ad successfully loaded ---
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += (id, info) =>
        {
            if (id != _adUnitId) return;
            _retryAttempt = 0; // Reset retry counter
            Debug.Log($"[Interstitial] Loaded {id}");
        };

        // --- Event: Ad failed to load ---
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += (id, error) =>
        {
            if (id != _adUnitId) return;
            Debug.Log($"[Interstitial] LoadFailed {id} code={error.Code} msg={error.Message}");
            Retry(Preload); // Try again later
        };

        // --- Event: Ad displayed successfully ---
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += (id, info) =>
        {
            if (id == _adUnitId) Debug.Log($"[Interstitial] Displayed {id}");
        };

        // --- Event: Ad failed to display ---
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += (id, error, info) =>
        {
            if (id != _adUnitId) return;
            Debug.Log($"[Interstitial] DisplayFailed {id} code={error.Code} msg={error.Message}");
            Preload(); // Load again after failure
        };

        // --- Event: Ad clicked ---
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += (id, info) =>
        {
            if (id == _adUnitId) Debug.Log($"[Interstitial] Clicked {id}");
        };

        // --- Event: Ad closed/hidden by user ---
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += (id, info) =>
        {
            if (id != _adUnitId) return;
            Debug.Log($"[Interstitial] Hidden {id} — preloading next");
            Preload(); // Prepare next ad
        };

        // --- Event: Revenue tracking ---
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += (id, info) =>
        {
            if (id == _adUnitId) Debug.Log($"[Interstitial] RevenuePaid unit={id} value={info.Revenue} network={info.NetworkName}");
        };
    }

    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    public void Preload() => MaxSdk.LoadInterstitial(_adUnitId);

    /// <summary>
    /// Shows the interstitial if ready, otherwise reloads it.
    /// </summary>
    public void Show()
    {
        if (MaxSdk.IsInterstitialReady(_adUnitId))
        {
            MaxSdk.ShowInterstitial(_adUnitId);
        }
        else
        {
            Debug.Log("[Interstitial] Not ready; reloading…");
            Preload();
        }
    }

    /// <summary>
    /// Handles exponential backoff retry for failed loads.
    /// </summary>
    private void Retry(Action loadAction)
    {
        _retryAttempt++;
        double delay = Math.Pow(2, Mathf.Min(6, _retryAttempt)); // Delay grows up to 64s
        StartCoroutine(RetryAfter((float)delay, loadAction));
        Debug.Log($"[Interstitial] Retry in {delay:0}s (attempt {_retryAttempt})");
    }

    /// <summary>
    /// Waits for given seconds, then calls the given action.
    /// </summary>
    private IEnumerator RetryAfter(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }
}
