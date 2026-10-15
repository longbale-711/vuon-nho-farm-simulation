using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Orchard.Editor
{
    public static class PackageSetup
    {
        private static AddAndRemoveRequest s_request;
        private static double s_deadline;
        public static void Trim()
        {
            s_request = Client.AddAndRemove(new[] { "com.unity.test-framework@1.1.33" }, new[] { "com.unity.feature.development", "com.unity.collab-proxy", "com.unity.visualscripting", "com.unity.timeline" });
            s_deadline = EditorApplication.timeSinceStartup + 300;
            EditorApplication.update += Poll;
        }

        private static void Poll()
        {
            if (!s_request.IsCompleted)
            {
                if (EditorApplication.timeSinceStartup > s_deadline)
                {
                    Debug.LogError("Package setup timed out");
                    EditorApplication.Exit(2);
                }

                return;
            }

            EditorApplication.update -= Poll;
            if (s_request.Status == StatusCode.Success)
            {
                Debug.Log("[Orchard] Unused template packages removed; Test Framework pinned.");
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError(s_request.Error.message);
                EditorApplication.Exit(1);
            }
        }
    }
}
