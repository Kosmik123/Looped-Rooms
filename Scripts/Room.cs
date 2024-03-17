using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.LoopedRooms
{
    [System.Serializable]
    public struct Connection
    {
        public Room SourceRoom { get; private set; }
        public Passage SourcePassage { get; private set; }
        public Passage TargetPassage { get; private set; }
        public Room TargetRoom { get; private set; }

        public Connection(Room sourceRoom, Passage sourceRoomPassage, Passage targetRoomPassage, Room targetRoom)
        {
            SourceRoom = sourceRoom;
            SourcePassage = sourceRoomPassage;
            TargetPassage = targetRoomPassage;
            TargetRoom = targetRoom;
        }
    }

    public class Room : MonoBehaviour
    {
        public delegate void RoomEventHandler(Room room);

        public event RoomEventHandler OnRoomInited;
        public event RoomEventHandler OnRoomEntered;
        public event RoomEventHandler OnRoomExited;

        private Room prototype;
        public Room Prototype => prototype;

        [SerializeField]
        private Transform[] passagesHolders;

        private Passage[] passages;
        public IReadOnlyList<Passage> Passages
        {
            get
            {
                if (passages == null || passages.Length < 1)
                    PopulatePassages();
                return passages;
            }
        }

        private readonly Dictionary<PassageID, Passage> passagesByID = new Dictionary<PassageID, Passage>();
        public Dictionary<Passage, Connection> Connections { get; } = new Dictionary<Passage, Connection>();

        public void Init(Room prototype)
        {
            this.prototype = prototype;
            OnRoomInited?.Invoke(this);
        }

        private void PopulatePassages()
        {
            Connections.Clear();
            var passagesList = new List<Passage>();
            for (int i = 0; i < passagesHolders.Length; i++)
               passagesList.AddRange(passagesHolders[i].GetComponentsInChildren<Passage>());

            passages = passagesList.ToArray();
            foreach (var passage in passages)
            {
                if (passage)
                {
                    if (passage.Id == null)
                    {
                        Debug.LogWarning($"Room {name} has null passage");
                        continue;
                    }

                    if (passagesByID.ContainsKey(passage.Id))
                    {
                        Debug.LogWarning($"Room {name} has multiple {passage.Id} passages");
                        continue;
                    }

                    passagesByID.Add(passage.Id, passage);
                }
            }
        
        }

        public Passage GetPassage(PassageID id)
        {
            if (passages == null || passages.Length < 1)
                PopulatePassages();
            return passagesByID[id];
        }

        public bool HasPassage(PassageID id)
        {
            return passagesByID.ContainsKey(id);
        }

        public IReadOnlyList<Passage> GetOppositePassages(Passage entrancePassage)
        {
            var oppositePassages = new List<Passage>();
            var entranecPassageForward = entrancePassage.PassageTransform.forward;
            foreach (var passage in passages)
            {
                if (Vector3.Dot(entranecPassageForward, passage.PassageTransform.forward) > 0.01f)
                    continue;

                oppositePassages.Add(passage);
            }

            //int passageIndex = System.Array.IndexOf(passages, entrancePassage);
            //for (int i = 2; i <= 4; i++)
            //{
            //    int oppositeIndex = (passageIndex + i) % passages.Length;
            //    var oppositeDoor = passages[oppositeIndex];
            //    if (oppositeDoor != null)
            //        oppositePassages.Add(oppositeDoor);
            //}

            return oppositePassages;
        }

        public void Enter()
        {
            OnRoomEntered?.Invoke(this);
        }

        public void Exit()
        {
            OnRoomExited?.Invoke(this);
        }

        public override string ToString()
        {
            return gameObject.name;
        }

        private void OnValidate()
        {
            if (Application.isEditor)
                passages = null;
        }
    }
}
