using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.LoopedRooms
{
    public class DoorsRandomizer : MonoBehaviour
    {
        [SerializeField]
        private Room[] rooms;

        private void Start()
        {
            RandomizeRooms();
        }

        [ContextMenu("Randomize")]
        private void RandomizeRooms()
        {
            foreach (var room in rooms)
            {
                Randomize(room);
            }
        }

        private void Randomize(Room room)
        {
            var doors = room.Passages;
            var doorIDs = new List<PassageID>();
            foreach (var door in doors)
                if (door is PassageWithID)
                    doorIDs.Add(door.ID);

            foreach (var door in doors)
            {
                if (door is PassageWithID passageWithID)
                {
                    int randomIndex = Random.Range(0, doorIDs.Count);
                    passageWithID.SetID(doorIDs[randomIndex]);
                    doorIDs.RemoveAt(randomIndex);
                }
            }
        }
    }
}
