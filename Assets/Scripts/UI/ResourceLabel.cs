using UnityEngine;
using TMPro;

/// <summary>
/// Simple UI label that displays the value of ResourceA or ResourceB.
/// 
/// Subscribes to the resource's OnChanged event to automatically update when the value changes.
/// </summary>
public class SimpleResourceLabel : MonoBehaviour
{
    /// <summary>
    /// Which resource this label should display.
    /// </summary>
    public enum Target { A, B }
    public Target target = Target.A;

    [Tooltip("Text before the value, e.g. 'Resource A'")]
    public string resourceName = "Resource";

    private TMP_Text label;

    private void Awake()
    {
        // Get the TextMeshPro component on this GameObject
        label = GetComponent<TMP_Text>();
        if (!label) Debug.LogWarning("[SimpleResourceLabel] Attach to a TMP_Text object.");
    }

    private void OnEnable()
    {
        // --- Event: Resource value changed ---
        if (target == Target.A) ResourceA.OnChanged += UpdateText;
        else ResourceB.OnChanged += UpdateText;

        // Draw initial value immediately
        UpdateText(target == Target.A ? ResourceA.Value : ResourceB.Value);
    }

    private void OnDisable()
    {
        if (target == Target.A) ResourceA.OnChanged -= UpdateText;
        else ResourceB.OnChanged -= UpdateText;
    }

    /// <summary>
    /// Updates the label text with the latest value.
    /// </summary>
    private void UpdateText(int value)
    {
        if (label) label.text = $"{resourceName}: {value}";
    }
}
