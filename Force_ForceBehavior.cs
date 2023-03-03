using System;
using PedSimulation.Geometry;

namespace PedSimulation.Simulation.ForceBehaviors
{
    // Token: 0x02000015 RID: 21
    public abstract class ForceBehavior
    {
        // Token: 0x06000105 RID: 261
        public abstract Vec2d CalculateForce();
    }
}

