using UnityEngine;

/// <summary>
/// Listens for MAX rewarded ad reward events and applies rewards to ResourceA or ResourceB based on the ad unit ID.
/// 
/// Acts as the link between rewarded ads and the in-game resource system.
/// Uses fallback amounts if the MAX-provided reward amount is zero.
/// </summary>
public class ResourcesController : MonoBehaviour
{
    [Header("IDs (from your MaxAdSettings)")]
    [SerializeField] private MaxAdSettings settings; // Holds the ad unit IDs so we can match incoming rewards to the right resource.

    [Header("Fallback amounts")]
    [SerializeField] private int fallbackAmountA = 1;
    [SerializeField] private int fallbackAmountB = 1;

    private void OnEnable()
    {
        // --- Event: Reward granted from a rewarded ad ---
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedReward;
    }

    private void OnDisable()
    {
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnAdReceivedReward;
    }

    /// <summary>
    /// Called whenever any rewarded ad grants a reward.
    /// Determines which resource to update based on ad unit ID.
    ///
    /// Note: See example for hard coded relation between resources and AD UNITs.
    /// A system like below is not suggested for growing apps, this is for presentation only.
    /// </summary>
    private void OnAdReceivedReward(string adUnitId, MaxSdk.Reward reward, MaxSdk.AdInfo info)
    {
        if (settings == null)
        {
            Debug.LogWarning("[ResourcesController] MaxAdSettings not assigned.");
            return;
        }

        // --- Rewarded Ad A: Update Resource A ---
        if (adUnitId == settings.androidRewardedIdA)
        {
            int amount = reward.Amount > 0 ? reward.Amount : fallbackAmountA;
            ResourceA.ReceiveReward(amount);
            Debug.Log($"[ResourcesController] Resource A +{amount} (unit={adUnitId}, label={reward.Label})");
            return;
        }

        // --- Rewarded Ad B: Update Resource B ---
        if (adUnitId == settings.androidRewardedIdB)
        {
            int amount = reward.Amount > 0 ? reward.Amount : fallbackAmountB;
            ResourceB.ReceiveReward(amount);
            Debug.Log($"[ResourcesController] Resource B +{amount} (unit={adUnitId}, label={reward.Label})");
            return;
        }

        // --- Any other rewarded ad unit (not handled here) ---
        Debug.Log($"[ResourcesController] Reward from unknown ad unit: {adUnitId} (ignored).");
    }
}
