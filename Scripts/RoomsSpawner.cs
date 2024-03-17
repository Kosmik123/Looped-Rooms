using System.Collections.Generic;
using UnityEngine;
#if UNITY_2021_1_OR_NEWER
using UnityEngine.Pool;
#endif

namespace Bipolar.LoopedRooms
{
    public class RoomsSpawner : MonoBehaviour
    {
#if UNITY_2021_1_OR_NEWER
        private readonly Dictionary<Room, ObjectPool<Room>> poolByPrototypes = new Dictionary<Room, ObjectPool<Room>>();
#endif

        public Room GetRoom(Room prototype)
        {
#if UNITY_2021_1_OR_NEWER

            if (poolByPrototypes.TryGetValue(prototype, out var pool) == false)
            {
                pool = new ObjectPool<Room>(() => CreateRoom(prototype));
                poolByPrototypes.Add(prototype, pool);
            }
#else
           
#endif

            var room = pool.Get();
            room.transform.rotation = Quaternion.identity;
            return room;
        }

        private Room CreateRoom(Room prototype)
        {
            var room = Instantiate(prototype, transform);
            room.gameObject.name = $"{prototype.name} ({poolByPrototypes[prototype].CountAll})";
            room.Init(prototype);
            room.gameObject.SetActive(false);
            return room;
        }

        public void Release (Room room)
        {
            var pool = poolByPrototypes[room.Prototype];
            room.Connections.Clear();
            room.gameObject.SetActive(false);
            pool.Release(room); 
        }
    }
}
