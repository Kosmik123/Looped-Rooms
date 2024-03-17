using UnityEngine;

namespace Bipolar.LoopedRooms
{
    public abstract class Passage : MonoBehaviour
    {
        public abstract PassageID ID { get; }

        [SerializeField]
        private Transform passageTransform;
        public Transform PassageTransform
        {
            get => passageTransform ? passageTransform : transform;
            set
            {
                passageTransform = value ? value : transform;
            }
        }

        public override string ToString()
        {
            return $"Passage with ID: {ID.name}";
        }

        private void OnValidate()
        {
            PassageTransform = PassageTransform;
        }
    }
}
