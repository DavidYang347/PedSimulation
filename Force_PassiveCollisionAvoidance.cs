using System;
using PedSimulation.Geometry;

namespace PedSimulation.Simulation.ForceBehaviors
{
    // Token: 0x02000014 RID: 20
    public class PassiveCollisionAvoidance : ForceBehavior
    {
        // Token: 0x06000101 RID: 257 RVA: 0x00005D54 File Offset: 0x00003F54
        public PassiveCollisionAvoidance(Person _parent, Map _parentMap)
        {
            this.parent = _parent;
            this.parentMap = _parentMap;
        }

        // Token: 0x06000102 RID: 258 RVA: 0x00005DA4 File Offset: 0x00003FA4
        public override Vec2d CalculateForce()
        {
            Vec2d vec2d = Vec2d.zeroVector();
            foreach (Person person in this.parent.Neighbors)
            {
                double num = this.parent.getPosition().distance(person.getPosition());
                double num2 = this.parent.BodyRadius + person.BodyRadius;
                double dist = num / num2;
                double threshold = 1.5;
                double repulsionForce_Exp = PassiveCollisionAvoidance.GetRepulsionForce_Exp(dist, this.rep_person_a, this.rep_person_b, this.rep_person_max, threshold);
                Vec2d vec2d2 = this.parent.getPosition().subtract(person.getPosition()).normalize();
                Vec2d b = vec2d2.multiply(repulsionForce_Exp);
                vec2d += b;
            }
            return vec2d;
        }

        // Token: 0x06000103 RID: 259 RVA: 0x00005E98 File Offset: 0x00004098
        private static double GetRepulsionForce_Exp(double dist, double a, double b, double max, double threshold)
        {
            double num = 0.0;
            bool flag = dist == 0.0;
            if (flag)
            {
                num = 0.0;
            }
            else
            {
                bool flag2 = dist < threshold;
                if (flag2)
                {
                    num = a * Math.Exp(-b * dist);
                }
            }
            bool flag3 = num > max;
            if (flag3)
            {
                num = max;
            }
            return num;
        }

        // Token: 0x06000104 RID: 260 RVA: 0x00005EF4 File Offset: 0x000040F4
        public override string ToString()
        {
            return "Passive collision avoidance force";
        }

        // Token: 0x0400007F RID: 127
        private Person parent;

        // Token: 0x04000080 RID: 128
        private Map parentMap;

        // Token: 0x04000081 RID: 129
        private double rep_person_a = 50.0;

        // Token: 0x04000082 RID: 130
        private double rep_person_b = 2.0;

        // Token: 0x04000083 RID: 131
        private double rep_person_max = 100.0;
    }
}

