using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bipolar.LoopedRooms
{
    [CreateAssetMenu(menuName = CreateAssetsPath.Root + "Level Settings")]
    public class LevelRoomsSettings : ScriptableObject
    {
        [System.Serializable]
        public class RoomPassageMapping
        {
            [field: SerializeField]
            public Room Room { get; private set; }

            [SerializeField]
            private PassageID[] passages;
            public IReadOnlyList<PassageID> Passages => passages;
        }

        [SerializeField]
        private Room[] allRoomsPrototypes;
        public IReadOnlyList<Room> AllRoomsPrototypes => allRoomsPrototypes;

        [SerializeField]
        private PassageConnection[] passageConnections;
        public IReadOnlyList<PassageConnection> PassageConnections => passageConnections;

        [SerializeField]
        private PassageMapping[] passageMappings;
        public IReadOnlyList<PassageMapping> PassageMappings => passageMappings;

        [SerializeField]
        private RoomPassageMapping[] additionalMappings;
        public IReadOnlyList<RoomPassageMapping> AdditionalMappings => additionalMappings;
    }
}
