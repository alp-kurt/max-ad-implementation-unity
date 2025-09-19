using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ads/MAX Ads Profile", fileName = "MaxAdsProfile")]
public sealed class MaxAdsProfile : ScriptableObject
{
    [Header("Banner Placements")]
    public List<BannerPlacement> banners = new();

    [Header("Interstitial Placements")]
    public List<InterstitialPlacement> interstitials = new();

    [Header("Rewarded Placements")]
    public List<RewardedPlacement> rewardeds = new();

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Basic validation for labels & IDs
        var labels = new HashSet<string>();
        var ids = new HashSet<string>();

        void Check(string label, string id, string type)
        {
            if (string.IsNullOrWhiteSpace(label))
                Debug.LogWarning($"[MaxAdsProfile] Empty label found in {type}");
            else if (!labels.Add(label))
                Debug.LogWarning($"[MaxAdsProfile] Duplicate label '{label}' found in {type}");

            if (string.IsNullOrWhiteSpace(id))
                Debug.LogWarning($"[MaxAdsProfile] Empty AdUnitId on '{label}' ({type})");
            else if (!ids.Add(id))
                Debug.LogWarning($"[MaxAdsProfile] Duplicate AdUnitId '{id}' referenced (type: {type}, label: {label})");
        }

        foreach (var b in banners) Check(b.label, b.adUnitId, "Banner");
        foreach (var i in interstitials) Check(i.label, i.adUnitId, "Interstitial");
        foreach (var r in rewardeds) Check(r.label, r.adUnitId, "Rewarded");
    }
#endif
}
