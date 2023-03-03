using System;
using System.Collections.Generic;
using PedSimulation.Geometry;
using Rhino.Geometry;

namespace PedSimulation.Simulation
{
    // Token: 0x02000003 RID: 3
    public class AccessPoint
    {
        // Token: 0x06000008 RID: 8 RVA: 0x000020E8 File Offset: 0x000002E8
        public AccessPoint(Point3d pt, Goal parent)
        {
            this.Point = pt;
            this.Visitors = new Queue<Person>();
            this.ParentGoal = parent;
        }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000009 RID: 9 RVA: 0x0000210D File Offset: 0x0000030D
        // (set) Token: 0x0600000A RID: 10 RVA: 0x00002115 File Offset: 0x00000315
        public Point3d Point { get; set; }

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x0600000B RID: 11 RVA: 0x0000211E File Offset: 0x0000031E
        // (set) Token: 0x0600000C RID: 12 RVA: 0x00002126 File Offset: 0x00000326
        public Goal ParentGoal { get; set; }

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x0600000D RID: 13 RVA: 0x00002130 File Offset: 0x00000330
        public Vec2d Position
        {
            get
            {
                return new Vec2d(this.Point.X, this.Point.Y);
            }
        }

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600000E RID: 14 RVA: 0x00002164 File Offset: 0x00000364
        public int QueueLength
        {
            get
            {
                return this.Visitors.Count;
            }
        }

        // Token: 0x0600000F RID: 15 RVA: 0x00002181 File Offset: 0x00000381
        public void AddVisitor(Person p)
        {
            this.Visitors.Enqueue(p);
        }

        // Token: 0x06000010 RID: 16 RVA: 0x00002191 File Offset: 0x00000391
        public void DequeueVisitor()
        {
            this.Visitors.Dequeue();
        }

        // Token: 0x06000011 RID: 17 RVA: 0x000021A0 File Offset: 0x000003A0
        public void Refresh()
        {
            this.Visitors.Clear();
        }

        // Token: 0x04000003 RID: 3
        public Queue<Person> Visitors;
    }
}

