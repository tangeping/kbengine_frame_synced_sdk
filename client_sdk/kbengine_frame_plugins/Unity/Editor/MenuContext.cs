using System.IO;
using UnityEditor;
using UnityEngine;

namespace KBEngine {

    /**
    * @brief Represents the FrameSync menu context bar.
    **/
    public class MenuContext {

        private static string ASSETS_PREFABS_PATH = "Assets/Plugins/kbengine_frame_plugins/Unity/Prefabs/{0}.prefab";

        private static void InstantiatePrefab(string path) {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format(ASSETS_PREFABS_PATH, path));
            PrefabUtility.InstantiatePrefab(prefab);
        }

        private static void CreateFrameSyncConfigAsset() {
            FrameSyncConfig asset = ScriptableObject.CreateInstance<FrameSyncConfig>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "") {
                path = "Assets";
            } else if (Path.GetExtension(path) != "") {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/FrameSyncConfig.asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/FrameSyncConfig", false, 0)]
        static void CreateFrameSyncConfig() {
            CreateFrameSyncConfigAsset();
        }

        [MenuItem("GameObject/FrameSync/Cube", false, 0)]
        static void CreatePrefabCube() {
            InstantiatePrefab("Basic/Cube");
        }

        [MenuItem("GameObject/FrameSync/Sphere", false, 0)]
        static void CreatePrefabSphere() {
            InstantiatePrefab("Basic/Sphere");
        }

        [MenuItem("GameObject/FrameSync/Capsule", false, 0)]
        static void CreatePrefabCapsule() {
            InstantiatePrefab("Basic/Capsule");
        }

        [MenuItem("GameObject/FrameSync/FrameSyncManager", false, 11)]
        static void CreatePrefabFrameSync() {            
            InstantiatePrefab("FrameSyncManager");
        }

    }

}