using UnityEngine;

namespace Bipolar.LoopedRooms
{
    public class PassageWithID : Passage
    {
        [SerializeField]
        private PassageID id;
        public override PassageID ID => id;

        public void SetID(PassageID id)
        {
            this.id = id;
        }
    }
}
