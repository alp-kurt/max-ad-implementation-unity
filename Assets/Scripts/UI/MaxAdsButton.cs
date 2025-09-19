using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Ads/MAX Ads Button")]
[RequireComponent(typeof(Button))]
public sealed class MaxAdsButton : MonoBehaviour
{
    public enum Action
    {
        ShowInterstitial,
        ShowRewarded,
        ToggleBanner,
        ShowBanner,
        HideBanner,
        PreloadInterstitial,
        PreloadRewarded
    }

    [Header("Target")]
    [Tooltip("Reference to your MaxAds orchestrator. If left empty, the component will try to find one in the scene on Awake().")]
    [SerializeField] private MaxAds ads;

    [Tooltip("Placement label defined in MaxAdsProfile (e.g., \"LevelEnd_Interstitial\", \"Revive_Rewarded\", \"Home_Banner_1\").")]
    [SerializeField] private string label;

    [Header("Behavior")]
    [SerializeField] private Action action = Action.ShowInterstitial;

    [Tooltip("Auto-find MaxAds in the scene when empty (safe: happens once in Awake).")]
    [SerializeField] private bool autoFindAds = true;

    [Tooltip("Disable the button while it’s performing the action to avoid double taps.")]
    [SerializeField] private bool disableWhileInvoking = true;

    [Tooltip("Optional re-enable delay (seconds) after click if 'disableWhileInvoking' is on.")]
    [SerializeField] private float reenableDelay = 0.5f;

    private Button _btn;

    private void Awake()
    {
        _btn = GetComponent<Button>();

        if (ads == null && autoFindAds)
            ads = FindObjectOfType<MaxAds>();

        if (ads == null)
            Debug.LogWarning("[MaxAdsButton] No MaxAds found. Assign it in the Inspector or ensure a MaxAds exists in the scene.");

        _btn.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        if (_btn != null)
            _btn.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        if (ads == null || string.IsNullOrWhiteSpace(label))
        {
            Debug.LogWarning("[MaxAdsButton] Missing MaxAds reference or label.");
            return;
        }

        if (disableWhileInvoking)
            _btn.interactable = false;

        switch (action)
        {
            case Action.ShowInterstitial:
                ads.ShowInterstitial(label);
                break;

            case Action.ShowRewarded:
                ads.ShowRewarded(label);
                break;

            case Action.ToggleBanner:
                ads.ToggleBanner(label);
                break;

            case Action.ShowBanner:
                ads.ShowBanner(label);
                break;

            case Action.HideBanner:
                ads.HideBanner(label);
                break;

            case Action.PreloadInterstitial:
                ads.PreloadInterstitial(label);
                break;

            case Action.PreloadRewarded:
                ads.PreloadRewarded(label);
                break;
        }

        if (disableWhileInvoking)
            Invoke(nameof(Reenable), Mathf.Max(0f, reenableDelay));
    }

    private void Reenable()
    {
        if (_btn != null) _btn.interactable = true;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (reenableDelay < 0f) reenableDelay = 0f;
    }
#endif
}
