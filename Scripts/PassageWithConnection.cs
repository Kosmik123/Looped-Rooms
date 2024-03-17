using UnityEngine;

namespace Bipolar.LoopedRooms
{
    public class PassageWithConnection : Passage
    {
        public enum ConnectionSide : byte
        {
            Left = 0,
            Right = 1
        }

        [SerializeField]
        private PassageConnection passageConnection;
        public PassageConnection PassageConnection
        {
            get => passageConnection;
            set => passageConnection = value;
        }


        [SerializeField]
        private ConnectionSide side;
        public ConnectionSide Side
        {
            get => side;
            set => side = value;
        }

        public override PassageID ID => side == ConnectionSide.Left ? passageConnection.LeftPassageID : passageConnection.RightPassageID;
    }
}
