using UnityEngine;

namespace Bipolar.LoopedRooms
{
    [CreateAssetMenu(menuName = CreateAssetsPath.Root + "Passage Connection")]
    public class PassageConnection : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        private PassageID leftPassageID;
        public PassageID LeftPassageID => leftPassageID;

        [SerializeField]
        [HideInInspector]
        private PassageID rightPassageID;
        public PassageID RightPassageID => rightPassageID;

        protected virtual void Reset()
        {
#if UNITY_2022_1_OR_NEWER
            CreateMissingPassages();
#else
            PassageConnectionPostprocessor.connectionWasCreated = true;
#endif
        }

        [ContextMenu("Create Missing")]
        private void CreateMissingPassages()
        {
#if UNITY_EDITOR
            if (leftPassageID == null)
                leftPassageID = CreatePassageID();

            if (rightPassageID == null)
                rightPassageID = CreatePassageID();

            leftPassageID.name = $"{name} (Left)";
            rightPassageID.name = $"{name} (Right)";
            PassageID CreatePassageID()
            {
                var passageID = CreateInstance<PassageID>();
                UnityEditor.AssetDatabase.AddObjectToAsset(passageID, this);
                UnityEditor.EditorUtility.SetDirty(passageID);
                UnityEditor.EditorUtility.SetDirty(this);
                return passageID;
            }
#endif
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorUtility.IsPersistent(this))
                CreateMissingPassages();
#endif
        }

#if (!UNITY_2022_1_OR_NEWER && UNITY_EDITOR)
        internal class PassageConnectionPostprocessor : UnityEditor.AssetPostprocessor
        {
            internal static bool connectionWasCreated;

            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                if (connectionWasCreated == false)
                    return;

                foreach (var path in importedAssets)
                {
                    var connection = UnityEditor.AssetDatabase.LoadAssetAtPath<PassageConnection>(path);
                    if (connection != null)
                    {
                        connection.CreateMissingPassages();
                        UnityEditor.AssetDatabase.SaveAssetIfDirty(connection);
                    }
                    connectionWasCreated = false;
                }
            }
        }
#endif
    }
}
