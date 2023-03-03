using System;

namespace PedSimulation.Simulation
{
    // Token: 0x0200000B RID: 11
    public class SystemSettings
    {
        // Token: 0x17000026 RID: 38
        // (get) Token: 0x0600006A RID: 106 RVA: 0x0000352C File Offset: 0x0000172C
        // (set) Token: 0x0600006B RID: 107 RVA: 0x00003564 File Offset: 0x00001764
        public double FPS
        {
            get
            {
                bool flag = this.fps == 0.0;
                if (flag)
                {
                    throw new DivideByZeroException("FPS must be positive.");
                }
                return this.fps;
            }
            set
            {
                this.fps = value;
            }
        }

        // Token: 0x17000027 RID: 39
        // (get) Token: 0x0600006C RID: 108 RVA: 0x00003570 File Offset: 0x00001770
        public double Dt
        {
            get
            {
                return 1.0 / this.FPS;
            }
        }

        // Token: 0x0600006D RID: 109 RVA: 0x00003594 File Offset: 0x00001794
        public SystemSettings()
        {
        }

        // Token: 0x0600006E RID: 110 RVA: 0x00003624 File Offset: 0x00001824
        public SystemSettings(SystemSettings original)
        {
            bool flag = original == null;
            if (!flag)
            {
                this.DampingRatio = original.DampingRatio;
                this.FrictionRatio = original.FrictionRatio;
                this.ObstacleRepulsionA = original.ObstacleRepulsionA;
                this.ObstacleRepulsionB = original.ObstacleRepulsionB;
                this.ObstacleRepulsionMax = original.ObstacleRepulsionMax;
                this.ObstacleRepulsionThreshold = original.ObstacleRepulsionThreshold;
                this.FPS = original.FPS;
                this.ObstacleOffset = original.ObstacleOffset;
                this.IsTraceEnabled = original.IsTraceEnabled;
            }
        }

        // Token: 0x0600006F RID: 111 RVA: 0x00003728 File Offset: 0x00001928
        public override bool Equals(object obj)
        {
            SystemSettings systemSettings = obj as SystemSettings;
            bool flag = systemSettings == null;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool flag2 = this.DampingRatio != systemSettings.DampingRatio;
                if (flag2)
                {
                    result = false;
                }
                else
                {
                    bool flag3 = this.FrictionRatio != systemSettings.FrictionRatio;
                    if (flag3)
                    {
                        result = false;
                    }
                    else
                    {
                        bool flag4 = this.ObstacleRepulsionA != systemSettings.ObstacleRepulsionA;
                        if (flag4)
                        {
                            result = false;
                        }
                        else
                        {
                            bool flag5 = this.ObstacleRepulsionB != systemSettings.ObstacleRepulsionB;
                            if (flag5)
                            {
                                result = false;
                            }
                            else
                            {
                                bool flag6 = this.ObstacleRepulsionMax != systemSettings.ObstacleRepulsionMax;
                                if (flag6)
                                {
                                    result = false;
                                }
                                else
                                {
                                    bool flag7 = this.ObstacleRepulsionThreshold != systemSettings.ObstacleRepulsionThreshold;
                                    if (flag7)
                                    {
                                        result = false;
                                    }
                                    else
                                    {
                                        bool flag8 = this.FPS != systemSettings.FPS;
                                        if (flag8)
                                        {
                                            result = false;
                                        }
                                        else
                                        {
                                            bool flag9 = this.ObstacleOffset != systemSettings.ObstacleOffset;
                                            if (flag9)
                                            {
                                                result = false;
                                            }
                                            else
                                            {
                                                bool flag10 = this.IsTraceEnabled != systemSettings.IsTraceEnabled;
                                                result = !flag10;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        // Token: 0x06000070 RID: 112 RVA: 0x00003850 File Offset: 0x00001A50
        public override int GetHashCode()
        {
            int num = 17;
            int num2 = 23;
            int num3 = num;
            num3 = num3 * num2 + this.DampingRatio.GetHashCode();
            num3 = num3 * num2 + this.FrictionRatio.GetHashCode();
            num3 = num3 * num2 + this.ObstacleRepulsionA.GetHashCode();
            num3 = num3 * num2 + this.ObstacleRepulsionB.GetHashCode();
            num3 = num3 * num2 + this.ObstacleRepulsionMax.GetHashCode();
            num3 = num3 * num2 + this.ObstacleRepulsionThreshold.GetHashCode();
            num3 = num3 * num2 + this.FPS.GetHashCode();
            num3 = num3 * num2 + this.ObstacleOffset.GetHashCode();
            return num3 * num2 + this.IsTraceEnabled.GetHashCode();
        }

        // Token: 0x0400002B RID: 43
        public double DampingRatio = 6.0;

        // Token: 0x0400002C RID: 44
        public double FrictionRatio = 0.0;

        // Token: 0x0400002D RID: 45
        public double ObstacleRepulsionA = 50.0;

        // Token: 0x0400002E RID: 46
        public double ObstacleRepulsionB = 2.0;

        // Token: 0x0400002F RID: 47
        public double ObstacleRepulsionMax = 5.0;

        // Token: 0x04000030 RID: 48
        public double ObstacleRepulsionThreshold = 0.5;

        // Token: 0x04000031 RID: 49
        public double ObstacleOffset = 1.0;

        // Token: 0x04000032 RID: 50
        private double fps = 30.0;

        // Token: 0x04000033 RID: 51
        public bool IsTraceEnabled;
    }
}

