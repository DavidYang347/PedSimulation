using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace PedSimulation.Simulation
{
    // Token: 0x0200000C RID: 12
    public class Target : Goal
    {
        // Token: 0x06000071 RID: 113 RVA: 0x00003900 File Offset: 0x00001B00
        public Target(Point3d position, Program targetPr)
        {
            AccessPoint item = new AccessPoint(position, this);
            this.AccessPoints.Add(item);
            this.TargetProgram = targetPr;
        }

        // Token: 0x06000072 RID: 114 RVA: 0x00003932 File Offset: 0x00001B32
        public Target(Point3d position, Program targetPr, string targetName) : this(position, targetPr)
        {
            base.Name = targetName;
        }

        // Token: 0x06000073 RID: 115 RVA: 0x00003948 File Offset: 0x00001B48
        public Target(List<Point3d> positions, Program targetPr)
        {
            foreach (Point3d pt in positions)
            {
                AccessPoint item = new AccessPoint(pt, this);
                this.AccessPoints.Add(item);
            }
            this.TargetProgram = targetPr;
        }

        // Token: 0x06000074 RID: 116 RVA: 0x000039B8 File Offset: 0x00001BB8
        public Target(List<Point3d> positions, Program targetPr, string targetName) : this(positions, targetPr)
        {
            base.Name = targetName;
        }

        // Token: 0x17000028 RID: 40
        // (get) Token: 0x06000075 RID: 117 RVA: 0x000039CC File Offset: 0x00001BCC
        // (set) Token: 0x06000076 RID: 118 RVA: 0x000039D4 File Offset: 0x00001BD4
        public Program TargetProgram { get; set; }

        // Token: 0x17000029 RID: 41
        // (get) Token: 0x06000077 RID: 119 RVA: 0x000039E0 File Offset: 0x00001BE0
        public int MinQueueLength
        {
            get
            {
                int num = int.MaxValue;
                foreach (AccessPoint accessPoint in this.AccessPoints)
                {
                    bool flag = accessPoint.QueueLength < num;
                    if (flag)
                    {
                        num = accessPoint.QueueLength;
                    }
                }
                return num;
            }
        }

        // Token: 0x1700002A RID: 42
        // (get) Token: 0x06000078 RID: 120 RVA: 0x00003A54 File Offset: 0x00001C54
        public override Point3d TagPosition
        {
            get
            {
                bool flag = this.AccessPoints.Count == 1;
                Point3d result;
                if (flag)
                {
                    result = this.AccessPoints[0].Point;
                }
                else
                {
                    result = base.getAvgPoint();
                }
                return result;
            }
        }

        // Token: 0x06000079 RID: 121 RVA: 0x00003A94 File Offset: 0x00001C94
        public List<AccessPoint> GetLeastOccupiedAP()
        {
            int num = int.MaxValue;
            List<AccessPoint> list = new List<AccessPoint>();
            foreach (AccessPoint accessPoint in this.AccessPoints)
            {
                bool flag = accessPoint.QueueLength < num;
                if (flag)
                {
                    list.Clear();
                    list.Add(accessPoint);
                    num = accessPoint.QueueLength;
                }
                else
                {
                    bool flag2 = accessPoint.QueueLength == num;
                    if (flag2)
                    {
                        list.Add(accessPoint);
                    }
                }
            }
            return list;
        }

        // Token: 0x0600007A RID: 122 RVA: 0x00003B3C File Offset: 0x00001D3C
        public override void Refresh()
        {
            base.Refresh();
            this.RefreshAccessPoints();
        }

        // Token: 0x0600007B RID: 123 RVA: 0x00003B50 File Offset: 0x00001D50
        private void RefreshAccessPoints()
        {
            foreach (AccessPoint accessPoint in this.AccessPoints)
            {
                accessPoint.Refresh();
            }
        }
    }
}

