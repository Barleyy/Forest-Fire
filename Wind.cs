using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forest_Fire
{
    class Wind
    {
        public static float windAngle(SimulationManager.Direction direction)
        {
            switch (direction)
            {
                case (SimulationManager.Direction.North):
                    return 0f;
                case (SimulationManager.Direction.East):
                    return 90f;
                case (SimulationManager.Direction.South):
                    return 180f;
                case (SimulationManager.Direction.West):
                    return 270f;
                case (SimulationManager.Direction.NorthEast):
                    return 45f;
                case (SimulationManager.Direction.NorthWest):
                    return 315f;
                case (SimulationManager.Direction.SouthEast):
                    return 135f;
                case (SimulationManager.Direction.SouthWest):
                    return 225f;
                default:
                    return 0f;
            }
        }
    }
}
