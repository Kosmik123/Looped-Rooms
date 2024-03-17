using UnityEngine;

namespace Bipolar.LoopedRooms
{
    public class Passage : MonoBehaviour
    {
        [SerializeField]
        private PassageID id;
        public PassageID Id
        {
            get => id;
            set
            {
                id = value;
            }
        }

        [SerializeField]
        private Transform passageTransform;
        public Transform PassageTransform => passageTransform ? passageTransform : transform;
        
        public override string ToString()
        {
            return $"Passage with ID: {id.name}";
        }

        private void OnValidate()
        {
            if (passageTransform == null)
                passageTransform = transform;
        }
    }
}
