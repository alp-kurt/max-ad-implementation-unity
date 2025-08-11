using UnityEngine;

/// <summary>
/// Handles initialization of the AppLovin MAX SDK for this app.
/// Attach this to a GameObject that exists from the start of the app lifecycle.
/// Keeps itself alive between scenes and sets up logging, test devices, and mediation debugging.
/// </summary>
public class MaxInitializer : MonoBehaviour
{
    [SerializeField] private bool verboseLogging = true; // Toggle verbose logging for development. Shows detailed MAX logs in the console.

    // For testing ads on a specific device, paste your GAID (Google Advertising ID) here.
    // You can find GAID on Android via Settings → Google → Ads.
    // it should look like this: "123e4567-e89b-12d3-a456-426614174000"
    [SerializeField] private string testDeviceGAID = "";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Keep this GameObject alive across all scenes

        MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitialized; // Subscribe to SDK initialization callback

        if (verboseLogging) MaxSdk.SetVerboseLogging(true); // Enable detailed debug logs (only in dev builds)

        // Register this device as a test device BEFORE initializing the SDK
        if (!string.IsNullOrEmpty(testDeviceGAID))
        {
            MaxSdk.SetTestDeviceAdvertisingIdentifiers(new string[] { testDeviceGAID });
        }

        MaxSdkCallbacks.OnSdkInitializedEvent += cfg =>
        // Additional debug log for when the SDK is ready
        {
            Debug.Log($"[MAX] SDK initialized. TestMode={cfg.IsTestModeEnabled}, Country={cfg.CountryCode}");
        };

        // Initialize MAX SDK
        // (must be called once at app start)
        MaxSdk.InitializeSdk();
    }

    /// <summary>
    /// Opens the built-in Mediation Debugger UI,
    /// doesn't work on Unity Editor.
    /// </summary>
    public void ShowMediationDebugger() => MaxSdk.ShowMediationDebugger();

    /// <summary>
    /// Called automatically once the MAX SDK has finished initializing.
    /// Logs useful environment details for troubleshooting.
    /// </summary>
    private void OnSdkInitialized(MaxSdkBase.SdkConfiguration cfg)
    {
        Debug.Log($"[MAX] SDK initialized. TestMode={cfg.IsTestModeEnabled}, Country={cfg.CountryCode}");
    }
}
