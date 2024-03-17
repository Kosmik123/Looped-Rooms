using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bipolar.LoopedRooms
{
    [System.Serializable]
    public struct OtherSideData
    {
        public Passage Passage { get; private set; }
        public Room Room { get; private set; }

        public OtherSideData(Passage passage, Room room)
        {
            Passage = passage;
            Room = room;
        }
    }

    [System.Serializable]
    public struct RoomAndPassage
    {
        public Room Room { get; private set; }
        public Passage Passage { get; private set; }

        public RoomAndPassage(Room room, Passage passage)
        {
            Room = room;
            Passage = passage;
        }
    }

    public class RoomsManager : MonoBehaviour
    {
        public event Room.RoomEventHandler OnRoomEntered;

        [Header("Settings")]
        [SerializeField]
        private LevelRoomsSettings settings;
        [SerializeField]
        private RoomsSpawner roomsSpawner;
        [SerializeField]
        private Room[] startingRoomPrefabs;
        [SerializeField]
        private Transform observer;

        private readonly Dictionary<PassageID, PassageID> passagesMappings = new Dictionary<PassageID, PassageID>();
        private readonly Dictionary<PassageID, Room> roomPrototypesByPassageID = new Dictionary<PassageID, Room>();

        [Header("States")]
        [SerializeField]
#if NAUGHTY_ATTRIBUTES
        [NaughtyAttributes.ReadOnly]
#endif
        private Room currentRoom;

        [SerializeField]
#if NAUGHTY_ATTRIBUTES
        [NaughtyAttributes.ReadOnly]
#endif
        private Room nearestNeighbour = null;

        [SerializeField]
#if NAUGHTY_ATTRIBUTES
        [NaughtyAttributes.ReadOnly]
#endif
        private List<Room> activeRooms;

        [SerializeField]
#if NAUGHTY_ATTRIBUTES
        [NaughtyAttributes.ReadOnly]
#endif
        private List<RoomAndPassage> passagesToLoadRoomBehind = new List<RoomAndPassage>();
        public bool IsLoading => passagesToLoadRoomBehind.Count > 0;

        public void TeleportToRoom(Room room)
        {
            observer.transform.position = Vector3.zero;
            room = room.Prototype != null ? room.Prototype : room;
            nearestNeighbour = null;
            foreach (var r in activeRooms)
                roomsSpawner.Release(r);

            activeRooms.Clear();

            room = roomsSpawner.GetRoom(room);
            room.transform.position = Vector3.zero;
            activeRooms.Add(room);
            SetCurrentRoom(room);
        }

        private void Awake()
        {
            activeRooms = new List<Room>();
            LoadMappings();

            int randomIndex = Random.Range(0, startingRoomPrefabs.Length);
            var startingRoomPrototype = startingRoomPrefabs[randomIndex];
            var room = roomsSpawner.GetRoom(startingRoomPrototype);
            room.transform.position = Vector3.zero;
            activeRooms.Add(room);
            SetCurrentRoom(room);
        }

        private void LoadMappings()
        {
            foreach (var doorPair in settings.PassageConnections)
            {
                if (doorPair.LeftPassageID == null || doorPair.RightPassageID == null)
                    continue;

                passagesMappings.Add(doorPair.LeftPassageID, doorPair.RightPassageID);
                if (doorPair.RightPassageID != doorPair.LeftPassageID)
                    passagesMappings.Add(doorPair.RightPassageID, doorPair.LeftPassageID);
            }

            foreach (var doorPair in settings.PassageMappings)
            {
                if (doorPair.Passage1 == null || doorPair.Passage2 == null)
                    continue;

                passagesMappings.Add(doorPair.Passage1, doorPair.Passage2);
                if (doorPair.Passage1 != doorPair.Passage2)
                    passagesMappings.Add(doorPair.Passage2, doorPair.Passage1);
            }

            foreach (var roomPrototype in settings.AllRoomsPrototypes)
                foreach (var passage in roomPrototype.Passages)
                    if (passage && passage.ID)
                        AddPassageToRoomMapping(roomPrototype, passage.ID);

            foreach (var additionalMapping in settings.AdditionalMappings)
                foreach (var passageID in additionalMapping.Passages)
                    if (passageID != null)
                        AddPassageToRoomMapping(additionalMapping.Room, passageID);
        }

        public void AddPassageToRoomMapping(Room roomPrototype, PassageID passageID)
        {
            if (roomPrototypesByPassageID.ContainsKey(passageID))
            {
                Debug.LogWarning($"{roomPrototype.name} contains already existing passage {passageID.name}");
            }
            else
            {
                roomPrototypesByPassageID.Add(passageID, roomPrototype);
            }
        }

        private void LoadMissingNeighbours(Room room)
        {
            LoadNeighbours(room, room.Passages);
        }

        private void LoadNeighbours(Room room, IReadOnlyList<Passage> passages)
        {
            foreach (var passage in passages)
            {
                if (passage == null || passage.ID == null)
                    continue;

                if (room.Connections.ContainsKey(passage))
                    continue;

                int indexOfUnloadedConnection = connectionsToUnload.FindIndex(connection => connection.SourceRoom == room && connection.SourcePassage == passage);
                if (indexOfUnloadedConnection >= 0)
                {
                    room.Connections.Add(passage, connectionsToUnload[indexOfUnloadedConnection]);
                    connectionsToUnload.RemoveAt(indexOfUnloadedConnection);
                }
                else
                {
                    var roomAndPassage = new RoomAndPassage(room, passage);
                    passagesToLoadRoomBehind.Add(roomAndPassage);
                }
            }
        }

        private void LoadRoomBehindDoor(Room room, Passage passage)
        {
            var neighbour = CreateRoomBehindDoor(passage);
            activeRooms.Add(neighbour.Room);

            room.Connections[passage] = new Connection(room, passage, neighbour.Passage, neighbour.Room);
            neighbour.Room.Connections[neighbour.Passage] = new Connection(neighbour.Room, neighbour.Passage, passage, room);
        }

        private OtherSideData CreateRoomBehindDoor(Passage passage)
        {
            var otherDoorID = passagesMappings[passage.ID];
            var roomPrototype = roomPrototypesByPassageID[otherDoorID];
            var room = roomsSpawner.GetRoom(roomPrototype);
            room.gameObject.SetActive(false);
            var otherPassage = room.GetPassage(otherDoorID);

            var distance = Vector3.Distance(room.transform.position, otherPassage.PassageTransform.position);
            Vector3 roomPosition = passage.PassageTransform.position + passage.PassageTransform.rotation * Vector3.forward * distance;

            Quaternion roomRotation = Quaternion.Inverse(otherPassage.PassageTransform.rotation);
            roomRotation *= Quaternion.AngleAxis(180, Vector3.up);
            roomRotation *= passage.PassageTransform.rotation;
            room.gameObject.SetActive(true);

            room.transform.SetPositionAndRotation(roomPosition, roomRotation);
            return new OtherSideData(otherPassage, room);
        }

        private void Update()
        {
            activeRooms.Sort(RoomsToObserverDistanceComparison);

            if (IsLoading)
            {
                var data = passagesToLoadRoomBehind[0];
                passagesToLoadRoomBehind.RemoveAt(0);
                LoadRoomBehindDoor(data.Room, data.Passage);
            }
            else
            {
                // TODO: stop unloading and loading again rooms. If they can stay, let them stay
                UpdateCurrentRoom();
                UpdateNearestNeighbour();
                foreach (var connection in connectionsToUnload)
                {
                    roomsSpawner.Release(connection.TargetRoom);
                    activeRooms.Remove(connection.TargetRoom);
                }
                connectionsToUnload.Clear();
            }
        }

        private readonly List<Connection> connectionsToUnload = new List<Connection>();
        private void UpdateCurrentRoom()
        {
            var nearestRoom = activeRooms[0];
            if (nearestRoom == currentRoom)
                return;

            currentRoom.Exit();
            var previousRoom = currentRoom;

            var connections = new List<Connection>(previousRoom.Connections.Values);
            foreach (var connection in connections)
            {
                var room = connection.TargetRoom;
                if (room == nearestRoom)
                    continue;

                RequestUnloadConnection(previousRoom, connection.SourcePassage);
                //roomsSpawner.Release(room);
                //activeRooms.Remove(room);
            }
            //DeleteAllConnectionsExceptOne(previousRoom, nearestRoom);
            SetCurrentRoom(nearestRoom);
        }

        private void RequestUnloadConnection(Room room, Passage passage)
        {
            if (room.Connections.TryGetValue(passage, out var connection) == false)
            {
                Debug.LogError("Trying to unload not existing connection!");
                return;
            }

            room.Connections.Remove(passage);
            connectionsToUnload.Add(connection);
        }

        private void SetCurrentRoom(Room room)
        {
            currentRoom = room;
            currentRoom.gameObject.SetActive(true);
            LoadMissingNeighbours(currentRoom);
            currentRoom.Enter();
            OnRoomEntered?.Invoke(currentRoom);
        }

        private void UpdateNearestNeighbour()
        {
            if (activeRooms.Count < 2)
                return;

            if (nearestNeighbour == activeRooms[1])
                return;

            if (nearestNeighbour != null && nearestNeighbour != currentRoom)
            {
                var connections = new List<Connection>(nearestNeighbour.Connections.Values);
                foreach (var connection in connections)
                {
                    if (connection.TargetRoom == currentRoom)
                        continue;

                    RequestUnloadConnection(nearestNeighbour, connection.SourcePassage);
                    //activeRooms.Remove(connection.Room);
                    //roomsSpawner.Release(connection.Room);
                }
                //DeleteAllConnectionsExceptOne(nearestNeighbour, currentRoom);
            }

            nearestNeighbour = activeRooms[1];

            var connectionToCurrentRoom = nearestNeighbour.Connections.First(kvp => kvp.Value.TargetRoom == currentRoom);
            var oppositeDoors = nearestNeighbour.GetOppositePassages(connectionToCurrentRoom.Key);

            LoadNeighbours(nearestNeighbour, oppositeDoors);
        }

        private int RoomsToObserverDistanceComparison(Room lhs, Room rhs)
        {
            float leftDistance = (lhs.transform.position - observer.transform.position).sqrMagnitude;
            float rightDistance = (rhs.transform.position - observer.transform.position).sqrMagnitude;
            return leftDistance.CompareTo(rightDistance);
        }

        private void DeleteAllConnectionsExceptOne(Room room, Room exception)
        {
            var passage = room.Connections.FirstOrDefault(kvp => kvp.Value.TargetRoom == exception).Key;
            if (passage == null)
                Debug.LogError("BRAK POŁĄCZEŃ?");
            var exceptionConnection = room.Connections[passage];
            room.Connections.Clear();
            room.Connections.Add(passage, exceptionConnection);
        }
    }
}
