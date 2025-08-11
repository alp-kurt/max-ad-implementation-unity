#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public class SyncBundleVersionPreprocess : IPreprocessBuildWithReport
{
    /// <summary>
    /// Determines the order in which build preprocessors are called.
    /// 0 means default/early execution.
    /// </summary>
    public int callbackOrder => 0;

    /// <summary>
    /// Keeps Unity's internal application version (Application.version) 
    /// in sync with CI/CD-managed versions.
    /// </summary>
    public void OnPreprocessBuild(BuildReport report)
    {
        var versionPath = Path.Combine(Directory.GetCurrentDirectory(), "version.txt");
        if (File.Exists(versionPath))
        {
            // Read the file and trim extra whitespace/newlines
            var v = File.ReadAllText(versionPath).Trim();

            // Update if the file is not empty and differs from current bundleVersion
            if (!string.IsNullOrEmpty(v) && UnityEditor.PlayerSettings.bundleVersion != v)
            {
                UnityEditor.PlayerSettings.bundleVersion = v;   // Update Unity's app version

                AssetDatabase.SaveAssets();
                UnityEngine.Debug.Log($"[Build] Synced PlayerSettings.bundleVersion -> {v}");
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("[Build] version.txt not found; bundleVersion not changed.");
        }
    }
}
#endif
