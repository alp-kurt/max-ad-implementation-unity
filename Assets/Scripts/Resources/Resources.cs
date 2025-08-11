using System;
using UnityEngine;

//
// Two globally accessible resources with simple APIs:
//   ResourceA.ReceiveReward(int)
//   ResourceB.ReceiveReward(int)
//
// Each raises OnChanged so UI can subscribe if needed.
// This is a lightweight example for testing rewarded ads.
// 

/// <summary>
/// Static resource store for "Resource A".
/// Tracks a numeric value and notifies listeners when it changes.
/// </summary>
public static class ResourceA
{
    public static int Value { get; private set; }
    public static event Action<int> OnChanged; // Fired whenever value changes

    /// <summary>
    /// Triggers OnChanged so UI can update automatically.
    /// </summary>
    public static void ReceiveReward(int amount)
    {
        if (amount <= 0) return; // Ignore invalid or zero rewards
        Value += amount;
        OnChanged?.Invoke(Value);
        Debug.Log($"[ResourceA] +{amount} => {Value}");
    }
}

/// <summary>
/// Static resource store for "Resource B".
/// Same pattern as Resource A, for a separate resource type.
/// </summary>
public static class ResourceB
{
    public static int Value { get; private set; }
    public static event Action<int> OnChanged;

    /// <summary>
    /// Triggers OnChanged so UI can update automatically.
    /// </summary>
    public static void ReceiveReward(int amount)
    {
        if (amount <= 0) return;
        Value += amount;
        OnChanged?.Invoke(Value);
        Debug.Log($"[ResourceB] +{amount} => {Value}");
    }
}

/// <summary>
/// Unity MonoBehaviour wrapper for ResourceA and ResourceB.
/// Lets you call reward methods directly from the Inspector or UnityEvents.
/// Useful for testing without writing extra code.
/// </summary>
public class ResourceReceivers : MonoBehaviour
{
    [Header("Optional default amounts if calling without params")]
    public int defaultAmountA = 1;
    public int defaultAmountB = 1;

    // --- Direct API with parameters (call from code) ---
    public void GiveA(int amount) => ResourceA.ReceiveReward(amount);
    public void GiveB(int amount) => ResourceB.ReceiveReward(amount);

    // --- Inspector-friendly no-argument API ---
    // UnityEvents can’t call static methods with parameters,
    // so these helpers make it easy to hook up buttons without arguments.
    public void GiveADefault() => ResourceA.ReceiveReward(defaultAmountA);
    public void GiveBDefault() => ResourceB.ReceiveReward(defaultAmountB);
}
