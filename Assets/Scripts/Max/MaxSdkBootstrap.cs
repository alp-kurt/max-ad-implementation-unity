using UnityEngine;

[DefaultExecutionOrder(-1000)]
public sealed class MaxSdkBootstrap : MonoBehaviour
{
    [Tooltip("Enable verbose MAX logging in Editor/dev")]
    [SerializeField] private bool verboseLogging = true;

    [Tooltip("Optional GAID for test device; leave empty if not needed")]
    [SerializeField] private string testDeviceGAID = "";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (verboseLogging) MaxSdk.SetVerboseLogging(true);
        if (!string.IsNullOrEmpty(testDeviceGAID))
            MaxSdk.SetTestDeviceAdvertisingIdentifiers(new[] { testDeviceGAID });

        MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitialized;
        MaxSdk.InitializeSdk();
    }

    private void OnDestroy()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent -= OnSdkInitialized;
    }

    private void OnSdkInitialized(MaxSdkBase.SdkConfiguration cfg)
    {
        Debug.Log($"[MAX] SDK initialized. TestMode={cfg.IsTestModeEnabled}, Country={cfg.CountryCode}");
    }

    [ContextMenu("Show Mediation Debugger")]
    public void ShowMediationDebugger() => MaxSdk.ShowMediationDebugger();
}
