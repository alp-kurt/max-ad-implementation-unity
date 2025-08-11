using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Handles loading, showing, and retry logic for a single MAX REWARDED AD UNIT.
/// Supports assigning a label so multiple rewarded ads can be distinguished (e.g., "Extra Life", "Bonus Coins").
/// </summary>
public class RewardedAdController : MonoBehaviour
{
    private string _adUnitId;
    private string _label;
    private int _retryAttempt;
    private bool _ready;

    /// <summary>
    /// Fired when the reward should be granted to the player.
    /// Passes the configured label or adUnitId.
    /// </summary>
    public event Action<string> OnRewardGranted;

    /// <summary>
    /// Initializes the rewarded ad controller and subscribes to MAX events.
    /// </summary>
    public void Initialize(string adUnitId, string label)
    {
        _adUnitId = adUnitId;
        _label = label;

        // --- Event: Rewarded ad successfully loaded ---
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += (id, info) =>
        {
            if (id != _adUnitId) return;
            _ready = true;
            _retryAttempt = 0;
            Debug.Log($"[Rewarded-{_label}] Loaded {id}");
        };

        // --- Event: Rewarded ad failed to load ---
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += (id, error) =>
        {
            if (id != _adUnitId) return;
            Debug.Log($"[Rewarded-{_label}] LoadFailed {id} code={error.Code} msg={error.Message}");
            Retry(Preload); // Retry with exponential backoff
        };

        // --- Event: Rewarded ad displayed successfully ---
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += (id, info) =>
        {
            if (id == _adUnitId) Debug.Log($"[Rewarded-{_label}] Displayed {id}");
        };

        // --- Event: Rewarded ad failed to display ---
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += (id, error, info) =>
        {
            if (id != _adUnitId) return;
            Debug.Log($"[Rewarded-{_label}] DisplayFailed {id} code={error.Code} msg={error.Message}");
            Preload(); // Load again immediately
        };

        // --- Event: Rewarded ad clicked ---
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += (id, info) =>
        {
            if (id == _adUnitId) Debug.Log($"[Rewarded-{_label}] Clicked {id}");
        };

        // --- Event: Rewarded ad closed/hidden ---
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += (id, info) =>
        {
            if (id != _adUnitId) return;
            _ready = false;
            Debug.Log($"[Rewarded-{_label}] Hidden {id} — preloading next");
            Preload(); // Prepare next ad
        };

        // --- Event: Reward granted to the player ---
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += (id, reward, info) =>
        {
            if (id != _adUnitId) return;
            Debug.Log($"[Rewarded-{_label}] Reward RECEIVED => {reward.Amount} {reward.Label}");
            OnRewardGranted?.Invoke(_label); // Notify listeners to give reward
        };

        // --- Event: Revenue tracking ---
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += (id, info) =>
        {
            if (id == _adUnitId) Debug.Log($"[Rewarded-{_label}] RevenuePaid unit={id} value={info.Revenue} network={info.NetworkName}");
        };
    }

    /// <summary>
    /// Loads the rewarded ad.
    /// </summary>
    public void Preload() => MaxSdk.LoadRewardedAd(_adUnitId);

    /// <summary>
    /// Shows the rewarded ad if ready; otherwise reloads it.
    /// </summary>
    public void Show()
    {
        if (_ready && MaxSdk.IsRewardedAdReady(_adUnitId))
        {
            MaxSdk.ShowRewardedAd(_adUnitId);
        }
        else
        {
            Debug.Log($"[Rewarded-{_label}] Not ready; reloading…");
            Preload();
        }
    }

    /// <summary>
    /// Handles retry for failed loads.
    /// </summary>
    private void Retry(Action loadAction)
    {
        _retryAttempt++;
        double delay = Math.Pow(2, Mathf.Min(6, _retryAttempt)); // 1..64s
        StartCoroutine(RetryAfter((float)delay, loadAction));
        Debug.Log($"[Rewarded-{_label}] Retry in {delay:0}s (attempt {_retryAttempt})");
    }

    /// <summary>
    /// Waits for a given delay before calling the specified action.
    /// </summary>
    private IEnumerator RetryAfter(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }
}
