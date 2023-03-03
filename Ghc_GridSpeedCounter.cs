using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using PedSimulation.Properties;
using PedSimulation.Simulation;
using Rhino.Geometry;

namespace PedSimulation.GHComponents
{
    // Token: 0x02000025 RID: 37
    public class GridSpeedCounterComponent : GH_Component
    {
        // Token: 0x06000193 RID: 403 RVA: 0x00009334 File Offset: 0x00007534
        public GridSpeedCounterComponent() : base("GridSpeedCounter", "GSCounter", "", "PedSim", "PostSim")
        {
            this.Reset();
        }

        // Token: 0x06000194 RID: 404 RVA: 0x00009360 File Offset: 0x00007560
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Reset", "Reset", "Reset the counter", 0);
            pManager.AddGenericParameter("People", "People", "Use the People from PedSimSystemComponent", GH_ParamAccess.list);
            pManager.AddPointParameter("BasePt", "Pt", "The bottom left corner of the grid", 0);
            pManager.AddNumberParameter("CellSize", "S", "", 0);
            pManager.AddIntegerParameter("ExtentX", "Ex", "", 0);
            pManager.AddIntegerParameter("ExtentY", "Ey", "", 0);
            pManager.AddIntegerParameter("NumFrames", "N", "Average over N frames; N >= 1", 0, 20);
        }

        // Token: 0x06000195 RID: 405 RVA: 0x00009411 File Offset: 0x00007611
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Avg Speed", "S", "Average speed in each grid cell.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Max Speed", "SMax", "Max speed in Avg Speed", 0);
        }

        // Token: 0x06000196 RID: 406 RVA: 0x00009444 File Offset: 0x00007644
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool flag = false;
            List<Person> list = new List<Person>();
            Point3d unset = Point3d.Unset;
            double cellSize = 0.0;
            int num = 0;
            int num2 = 0;
            this.nFrames = 1;
            bool flag2 = !DA.GetData<bool>("Reset", ref flag);
            if (!flag2)
            {
                bool flag3 = !DA.GetDataList<Person>("People", list);
                if (!flag3)
                {
                    bool flag4 = !DA.GetData<Point3d>("BasePt", ref unset);
                    if (!flag4)
                    {
                        bool flag5 = !DA.GetData<double>("CellSize", ref cellSize);
                        if (!flag5)
                        {
                            bool flag6 = !DA.GetData<int>("ExtentX", ref num);
                            if (!flag6)
                            {
                                bool flag7 = !DA.GetData<int>("ExtentY", ref num2);
                                if (!flag7)
                                {
                                    bool flag8 = !DA.GetData<int>("NumFrames", ref this.nFrames);
                                    if (!flag8)
                                    {
                                        bool flag9 = num <= 0 || num2 <= 0;
                                        if (flag9)
                                        {
                                            throw new ArgumentOutOfRangeException("ex and ey must be positive integer.");
                                        }
                                        int num3 = num * num2;
                                        bool flag10 = this.avgSpeeds.Count == 0;
                                        if (flag10)
                                        {
                                            this.Initialize(num3);
                                        }
                                        else
                                        {
                                            bool flag11 = this.avgSpeeds.Count != num3 || flag;
                                            if (flag11)
                                            {
                                                this.Reset();
                                                this.Initialize(num3);
                                            }
                                        }
                                        double num4 = double.MinValue;
                                        List<List<double>> list2 = new List<List<double>>();
                                        for (int i = 0; i < num3; i++)
                                        {
                                            list2.Add(new List<double>());
                                        }
                                        this.speedRecords.Enqueue(list2);
                                        bool flag12 = this.speedRecords.Count > this.nFrames;
                                        if (flag12)
                                        {
                                            this.speedRecords.Dequeue();
                                        }
                                        foreach (Person person in list)
                                        {
                                            int cellListIndexByCoordinate = this.GetCellListIndexByCoordinate(person.GetRhinoPoint, unset, cellSize, num, num2);
                                            bool flag13 = cellListIndexByCoordinate < 0;
                                            if (!flag13)
                                            {
                                                list2[cellListIndexByCoordinate].Add(person.getSpeed());
                                            }
                                        }
                                        this.avgSpeeds = this.AverageSpeeds(this.speedRecords, num3);
                                        foreach (double num5 in this.avgSpeeds)
                                        {
                                            bool flag14 = num5 > num4;
                                            if (flag14)
                                            {
                                                num4 = num5;
                                            }
                                        }
                                        DA.SetDataList("Avg Speed", this.avgSpeeds);
                                        DA.SetData("Max Speed", num4);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x06000197 RID: 407 RVA: 0x0000971C File Offset: 0x0000791C
        private void Initialize(int n)
        {
            for (int i = 0; i < n; i++)
            {
                this.avgSpeeds.Add(0.0);
            }
        }

        // Token: 0x06000198 RID: 408 RVA: 0x00009751 File Offset: 0x00007951
        private void Reset()
        {
            this.avgSpeeds = new List<double>();
            this.speedRecords = new Queue<List<List<double>>>();
        }

        // Token: 0x06000199 RID: 409 RVA: 0x0000976C File Offset: 0x0000796C
        private int GetCellListIndexByCoordinate(Point3d pt, Point3d basePt, double cellSize, int ex, int ey)
        {
            double num = cellSize * (double)ex;
            double num2 = cellSize * (double)ey;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = pt.X - basePt.X;
            double num6 = pt.Y - basePt.Y;
            bool flag = num5 < num3 || num5 >= num || num6 < num4 || num6 >= num2;
            int result;
            if (flag)
            {
                result = -1;
            }
            else
            {
                int num7 = (int)Math.Floor(num5 / cellSize);
                int num8 = (int)Math.Floor(num6 / cellSize);
                result = ey * num7 + num8;
            }
            return result;
        }

        // Token: 0x0600019A RID: 410 RVA: 0x00009808 File Offset: 0x00007A08
        private List<double> AverageSpeeds(Queue<List<List<double>>> records, int n)
        {
            List<double> list = new List<double>();
            for (int i = 0; i < n; i++)
            {
                List<double> list2 = new List<double>();
                foreach (List<List<double>> list3 in records)
                {
                    List<double> collection = list3[i];
                    list2.AddRange(collection);
                }
                double item = this.AverageNumber(list2);
                list.Add(item);
            }
            return list;
        }

        // Token: 0x0600019B RID: 411 RVA: 0x000098A0 File Offset: 0x00007AA0
        private double AverageNumber(List<double> numbers)
        {
            bool flag = numbers.Count == 0;
            double result;
            if (flag)
            {
                result = 0.0;
            }
            else
            {
                double num = 0.0;
                foreach (double num2 in numbers)
                {
                    num += num2;
                }
                num /= (double)numbers.Count;
                result = num;
            }
            return result;
        }

        // Token: 0x17000077 RID: 119
        // (get) Token: 0x0600019C RID: 412 RVA: 0x00009924 File Offset: 0x00007B24
        protected override Bitmap Icon
        {
            get
            {
                return Resources.Icon_GridSpeedCounter_02;
            }
        }

        // Token: 0x17000078 RID: 120
        // (get) Token: 0x0600019D RID: 413 RVA: 0x0000993C File Offset: 0x00007B3C
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("622dffb0-8a22-4894-b6f5-4ebcd02c2d1c");
            }
        }

        // Token: 0x040000B5 RID: 181
        private List<double> avgSpeeds;

        // Token: 0x040000B6 RID: 182
        private Queue<List<List<double>>> speedRecords;

        // Token: 0x040000B7 RID: 183
        private int nFrames;
    }
}

