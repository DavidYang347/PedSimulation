using System;
using PedSimulation.Geometry;

namespace PedSimulation.Simulation.ForceBehaviors
{
    // Token: 0x02000013 RID: 19
    public class MaintainClosenessToGoalBehavior : ForceBehavior
    {
        public MaintainClosenessToGoalBehavior(Person _parent, Map _parentMap)
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
            bool flag = this.parent.CurrentAccessPoint == null;
            if (flag)
            {
                throw new NullReferenceException("MaintainClosenessToGoalBehavior does not know which access point is used for test.");
            }
            bool flag2 = this.ParentHasGoal() && this.IsParentTooFarFromGoal();
            Vec2d result;
            if (flag2)
            {
                Vec2d vec2d = this.parent.CurrentAccessPoint.Position - this.parent.getPosition();
                vec2d = vec2d.normalize().multiply(this.targetForceConstant);
                result = vec2d;
            }
            else
            {
                result = Vec2d.zeroVector();
            }
            return result;
        }

        private bool ParentHasGoal()
        {
            return this.parent.CurrentGoal != null;
        }

        private bool IsParentTooFarFromGoal()
        {
            double num = this.parent.CurrentAccessPoint.Position.distance(this.parent.getPosition());
            return num > this.parent.CurrentGoal.AccessRadius + this.parent.BodyRadius;
        }

        public override string ToString()
        {
            return "Maintain Closeness To Goal";
        }

        private Person parent;

        private Map parentMap;

        private double targetForceConstant;
    }
}

