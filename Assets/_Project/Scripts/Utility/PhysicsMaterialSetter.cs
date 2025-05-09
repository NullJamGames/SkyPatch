using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace NJG.Utilities
{
    [CreateAssetMenu(fileName = "PhysicsMaterialSetter", menuName = "NJG/Utility/PhysicsMaterialSetter")]
    public class PhysicsMaterialSetter : ScriptableObject
    {
        [SerializeField]
        private PhysicsMaterial materialToApply;

#if UNITY_EDITOR
        [Button]
        private void ApplyMaterial()
        {
            // Get path of this ScriptableObject
            string path = AssetDatabase.GetAssetPath(this);
            string directory = Path.GetDirectoryName(path);

            // Find all assets in the same folder
            string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { directory });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (go == null)
                    continue;

                // Get all colliders in the prefab or GameObject
                Collider[] colliders = go.GetComponentsInChildren<Collider>(true);
                foreach (Collider collider in colliders)
                {
                    Undo.RecordObject(collider, "Assign Physics Material");
                    collider.sharedMaterial = materialToApply;
                    EditorUtility.SetDirty(collider);
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Applied material to all MeshColliders!");
        }
#endif
    }
}