using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hooks a UI Button to open the AppLovin MAX Mediation Debugger.
/// 
/// This is useful for developers and QA testers to inspect the ad network setup,
/// verify integrations, and debug issues without leaving the app.
/// 
/// Attach this script to a Button GameObject or point it to another Button via the Inspector.
/// </summary>
[RequireComponent(typeof(Button))]
public class ShowMediationDebuggerButton : MonoBehaviour
{
    [Tooltip("Optionally target a different Button. If left empty, uses the Button on this GameObject.")]
    [SerializeField] private Button targetButton;

    private void Awake()
    {
        // If no button assigned in Inspector, try to use the Button on this GameObject,
        // or fallback to the first child Button (including inactive children).
        if (targetButton == null)
        {
            targetButton = GetComponent<Button>() ?? GetComponentInChildren<Button>(true);
        }
    }

    private void OnEnable()
    {
        if (targetButton == null)
        {
            Debug.LogWarning("[MAX] No Button found for ShowMediationDebuggerButton.");
            return;
        }

        // --- Event: Button clicked → Show Mediation Debugger ---
        // Remove listener first to avoid duplicates if object is re-enabled.
        targetButton.onClick.RemoveListener(ShowDebugger);
        targetButton.onClick.AddListener(ShowDebugger);
    }

    private void OnDisable()
    {
        if (targetButton != null)
            targetButton.onClick.RemoveListener(ShowDebugger);
    }

    /// <summary>
    /// Opens the AppLovin MAX Mediation Debugger UI.
    /// </summary>
    private void ShowDebugger()
    {
        MaxSdk.ShowMediationDebugger();
        Debug.Log("[MAX] Mediation Debugger opened.");
    }
}
