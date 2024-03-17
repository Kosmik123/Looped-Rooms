using UnityEditor;
using UnityEngine;

namespace Bipolar.LoopedRooms
{
    [CreateAssetMenu(menuName = CreateAssetsPath.Root + "Passage Connection")]
    public class PassageConnection : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private PassageID leftPassageID;
        public PassageID LeftPassageID => leftPassageID;

        [SerializeField, HideInInspector]
        private PassageID rightPassageID;
        public PassageID RightPassageID => rightPassageID;

        private void Reset()
        {
#if UNITY_EDITOR
            leftPassageID = CreateInstance<PassageID>();
            rightPassageID = CreateInstance<PassageID>();
            AssetDatabase.AddObjectToAsset(leftPassageID, this);
            AssetDatabase.AddObjectToAsset(rightPassageID, this);
#endif
        }

        private void OnValidate()
        {
            leftPassageID.name = $"{name} (Left)";
            rightPassageID.name = $"{name} (Right)";
        }
    }
}