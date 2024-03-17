using UnityEngine;

namespace Bipolar.LoopedRooms
{
    public class RoomsTeleporter : MonoBehaviour
    {
        [SerializeField]
        private RoomsManager roomsManager;

        [SerializeField]
        private Room target;

#if NAUGHTY_ATTRIBUTES
        [NaughtyAttributes.Button]
#endif
        private void Teleport()
        {
            roomsManager.TeleportToRoom(target);
            target.transform.position = Vector3.zero;
        }
    }
}
