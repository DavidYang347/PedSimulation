using System;
using System.Drawing;
using PedSimulation.Geometry;
using Rhino.Geometry;

namespace PedSimulation.Simulation
{
    // Token: 0x02000005 RID: 5
    public class Gate : Goal
    {
        // Token: 0x0600001D RID: 29 RVA: 0x000027C8 File Offset: 0x000009C8
        public Gate(Point3d position, int exitTime, Color color)
        {
            AccessPoint item = new AccessPoint(position, this);
            this.AccessPoints.Add(item);
            this.ExitTime = exitTime;
            this.ExitTimeCount = 0;
            this.GateColor = color;
        }

        // Token: 0x0600001E RID: 30 RVA: 0x0000280A File Offset: 0x00000A0A
        public Gate(Point3d pt, string name, int exitTime, Color color) : this(pt, exitTime, color)
        {
            base.Name = name;
        }

        // Token: 0x1700000E RID: 14
        // (get) Token: 0x0600001F RID: 31 RVA: 0x00002820 File Offset: 0x00000A20
        public AccessPoint DefaultAP
        {
            get
            {
                bool flag = this.AccessPoints.Count == 1;
                if (flag)
                {
                    return this.AccessPoints[0];
                }
                throw new NotImplementedException();
            }
        }

        // Token: 0x06000020 RID: 32 RVA: 0x00002858 File Offset: 0x00000A58
        public Vec2d GetPosition()
        {
            bool flag = this.AccessPoints.Count == 1;
            if (flag)
            {
                Point3d point = this.AccessPoints[0].Point;
                return new Vec2d(point.X, point.Y);
            }
            throw new NotImplementedException();
        }

        // Token: 0x1700000F RID: 15
        // (get) Token: 0x06000021 RID: 33 RVA: 0x000028A8 File Offset: 0x00000AA8
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

        // Token: 0x17000010 RID: 16
        // (get) Token: 0x06000022 RID: 34 RVA: 0x000028E6 File Offset: 0x00000AE6
        // (set) Token: 0x06000023 RID: 35 RVA: 0x000028EE File Offset: 0x00000AEE
        public int ExitTime { get; set; }

        // Token: 0x17000011 RID: 17
        // (get) Token: 0x06000024 RID: 36 RVA: 0x000028F7 File Offset: 0x00000AF7
        // (set) Token: 0x06000025 RID: 37 RVA: 0x000028FF File Offset: 0x00000AFF
        public int ExitTimeCount { get; set; }

        // Token: 0x17000012 RID: 18
        // (get) Token: 0x06000026 RID: 38 RVA: 0x00002908 File Offset: 0x00000B08
        // (set) Token: 0x06000027 RID: 39 RVA: 0x00002910 File Offset: 0x00000B10
        public Color GateColor { get; set; }

        // Token: 0x06000028 RID: 40 RVA: 0x0000291C File Offset: 0x00000B1C
        public bool CheckExitTime()
        {
            return this.ExitTimeCount == 0;
        }

        // Token: 0x06000029 RID: 41 RVA: 0x00002938 File Offset: 0x00000B38
        public void IncrementTimeCounts()
        {
            bool flag = this.ExitTimeCount > 0;
            if (flag)
            {
                bool flag2 = this.ExitTimeCount + 1 == this.ExitTime;
                if (flag2)
                {
                    this.ExitTimeCount = 0;
                }
                else
                {
                    int exitTimeCount = this.ExitTimeCount;
                    this.ExitTimeCount = exitTimeCount + 1;
                }
            }
        }

        // Token: 0x0600002A RID: 42 RVA: 0x00002984 File Offset: 0x00000B84
        public void StartExitCounter()
        {
            bool flag = this.ExitTime > 1;
            if (flag)
            {
                this.ExitTimeCount = 1;
            }
        }
    }
}

