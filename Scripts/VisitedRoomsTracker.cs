using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.LoopedRooms
{
    public class VisitedRoomsTracker : MonoBehaviour
    {
        [SerializeField]
        private RoomsManager roomsManager;

        [SerializeField]
#if NAUGHTY_ATTRIBUTES
        [NaughtyAttributes.ReadOnly]
#endif
        private List<Room> visitedRooms;
        public IReadOnlyList<Room> VisitedRooms => visitedRooms;

        private void OnEnable()
        {
            roomsManager.OnRoomEntered += RegisterEnteredRoom;
        }

        private void RegisterEnteredRoom(Room room)
        {
            if (visitedRooms.Contains(room.Prototype) == false)
                visitedRooms.Add(room.Prototype);
        }

        private void OnDisable()
        {
            roomsManager.OnRoomEntered -= RegisterEnteredRoom;
        }
    }
}
