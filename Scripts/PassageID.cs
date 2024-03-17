using UnityEngine;

namespace Bipolar.LoopedRooms
{
    [CreateAssetMenu(menuName = CreateAssetsPath.Root + "Passage ID")]
    public class PassageID : ScriptableObject
    {
        public override string ToString()
        {
            return $"Passage ID: {name}";
        }
    }
}
