using System;
using PedSimulation.Geometry;

namespace PedSimulation.Simulation.ForceBehaviors
{

    public class TargetForceBehavior : ForceBehavior
    {

        public TargetForceBehavior(Person _parent, Map _parentMap)
        {
            this.parent = _parent;
            this.parentMap = _parentMap;
            bool flag = this.parent != null;
            if (flag)
            {
                this.targetForceConstant = this.parent.TargetForce;
            }
        }

        public override Vec2d CalculateForce()
        {
            bool flag = this.parent.CurrentNode != null;
            Vec2d result;
            if (flag)
            {
                Vec2d vec2d = this.parent.CurrentNode.Position.subtract(this.parent.getPosition());
                vec2d = vec2d.normalize().multiply(this.targetForceConstant);
                result = vec2d;
            }
            else
            {
                result = Vec2d.zeroVector();
            }
            return result;
        }

        public override string ToString()
        {
            return "Target force";
        }

        private Person parent;

        private Map parentMap;

        private double targetForceConstant;
    }
}

