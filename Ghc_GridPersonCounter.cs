using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using PedSimulation.Properties;
using PedSimulation.Simulation;
using Rhino.Geometry;

namespace PedSimulation.GHComponents
{
    // Token: 0x02000024 RID: 36
    public class GridPersonCounterComponent : GH_Component
    {
        // Token: 0x0600017C RID: 380 RVA: 0x00008908 File Offset: 0x00006B08
        public GridPersonCounterComponent() : base("GridPersonCounter", "GPCounter", "", "PedSim", "PostSim")
        {
            this.Reset();
            this.Mode = GridPersonCounterComponent.GridPersonCounterMode.CountPerson;
        }

        // Token: 0x17000074 RID: 116
        // (get) Token: 0x0600017D RID: 381 RVA: 0x00008944 File Offset: 0x00006B44
        // (set) Token: 0x0600017E RID: 382 RVA: 0x0000895C File Offset: 0x00006B5C
        public GridPersonCounterComponent.GridPersonCounterMode Mode
        {
            get
            {
                return this.mode;
            }
            set
            {
                this.mode = value;
                switch (this.mode)
                {
                    case GridPersonCounterComponent.GridPersonCounterMode.CountFrame:
                        base.Message = "Count Frame";
                        break;
                    case GridPersonCounterComponent.GridPersonCounterMode.CountVisit:
                        base.Message = "Count Visit";
                        break;
                    case GridPersonCounterComponent.GridPersonCounterMode.CountPerson:
                        base.Message = "Count Person";
                        break;
                }
            }
        }

        // Token: 0x0600017F RID: 383 RVA: 0x000089BC File Offset: 0x00006BBC
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Reset", "Reset", "Reset the counter", 0);
            pManager.AddGenericParameter("People", "People", "Use the People from PedSimSystemComponent", GH_ParamAccess.list);
            pManager.AddPointParameter("BasePt", "Pt", "The bottom left corner of the grid", 0);
            pManager.AddNumberParameter("CellSize", "S", "", 0);
            pManager.AddIntegerParameter("ExtentX", "Ex", "", 0);
            pManager.AddIntegerParameter("ExtentY", "Ey", "", 0);
        }

        // Token: 0x06000180 RID: 384 RVA: 0x00008A54 File Offset: 0x00006C54
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Count", "Count", "Number of People in each grid cell.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("MaxCount", "Max", "Max value in Count", 0);
        }

        // Token: 0x06000181 RID: 385 RVA: 0x00008A88 File Offset: 0x00006C88
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool flag = false;
            this.people = new List<Person>();
            Point3d unset = Point3d.Unset;
            double num = 0.0;
            int num2 = 0;
            int num3 = 0;
            bool flag2 = !DA.GetData<bool>("Reset", ref flag);
            if (!flag2)
            {
                bool flag3 = !DA.GetDataList<Person>("People", this.people);
                if (!flag3)
                {
                    bool flag4 = !DA.GetData<Point3d>("BasePt", ref unset);
                    if (!flag4)
                    {
                        bool flag5 = !DA.GetData<double>("CellSize", ref num);
                        if (!flag5)
                        {
                            bool flag6 = !DA.GetData<int>("ExtentX", ref num2);
                            if (!flag6)
                            {
                                bool flag7 = !DA.GetData<int>("ExtentY", ref num3);
                                if (!flag7)
                                {
                                    bool flag8 = num2 <= 0 || num3 <= 0;
                                    if (flag8)
                                    {
                                        throw new ArgumentOutOfRangeException("ex and ey must be positive integer.");
                                    }
                                    bool flag9 = false;
                                    bool flag10 = num2 != this.ex || num3 != this.ey || num != this.cellSize;
                                    if (flag10)
                                    {
                                        flag9 = true;
                                    }
                                    bool flag11 = !unset.Equals(this.basePt);
                                    if (flag11)
                                    {
                                        flag9 = true;
                                    }
                                    bool flag12 = flag9;
                                    if (flag12)
                                    {
                                        this.ex = num2;
                                        this.ey = num3;
                                        this.cellSize = num;
                                        this.basePt = unset;
                                        this.Reset(this.ex, this.ey, this.cellSize, this.basePt);
                                    }
                                    else
                                    {
                                        bool flag13 = flag;
                                        if (flag13)
                                        {
                                            this.Reset(this.ex, this.ey, this.cellSize, this.basePt);
                                        }
                                    }
                                    this.CountPeopleByPerson();
                                    int num4 = this.currentNumbers.Max();
                                    bool flag14 = num4 > 0;
                                    if (flag14)
                                    {
                                        this.maxPeopleInCell = this.currentNumbers.Max();
                                    }
                                    DA.SetDataList("Count", this.currentNumbers);
                                    DA.SetData("MaxCount", this.maxPeopleInCell);
                                }
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x06000182 RID: 386 RVA: 0x00008C8E File Offset: 0x00006E8E
        private void CountPeopleByPerson()
        {
            Parallel.ForEach<Person>(this.people, delegate (Person p)
            {
                double x = p.getPosition().X;
                double y = p.getPosition().Y;
                int num = (int)Math.Floor((x - this.basePt.X) / this.cellSize);
                int num2 = (int)Math.Floor((y - this.basePt.Y) / this.cellSize);
                bool flag = num >= this.ex || num < 0 || num2 >= this.ey || num2 < 0;
                if (!flag)
                {
                    int regionIndex = this.GetRegionIndex(num2, num);
                    bool flag2 = this.Mode == GridPersonCounterComponent.GridPersonCounterMode.CountPerson;
                    if (flag2)
                    {
                        List<Person> list = this.knownVisitors[regionIndex];
                        bool flag3 = !list.Contains(p);
                        if (flag3)
                        {
                            list.Add(p);
                            List<int> list2 = this.currentNumbers;
                            int num3 = regionIndex;
                            int num4 = list2[num3];
                            list2[num3] = num4 + 1;
                        }
                    }
                    else
                    {
                        bool flag4 = this.Mode == GridPersonCounterComponent.GridPersonCounterMode.CountFrame;
                        if (flag4)
                        {
                            List<int> list3 = this.currentNumbers;
                            int num4 = regionIndex;
                            int num3 = list3[num4];
                            list3[num4] = num3 + 1;
                        }
                        else
                        {
                            bool flag5 = this.Mode == GridPersonCounterComponent.GridPersonCounterMode.CountVisit;
                            if (flag5)
                            {
                                List<Person> list4 = this.currentVisitors[regionIndex];
                                bool flag6 = !list4.Contains(p);
                                if (flag6)
                                {
                                    list4.Add(p);
                                    List<int> list5 = this.currentNumbers;
                                    int num3 = regionIndex;
                                    int num4 = list5[num3];
                                    list5[num3] = num4 + 1;
                                    List<List<Person>> neighborVisitorLists = this.GetNeighborVisitorLists(regionIndex);
                                    for (int i = 0; i < neighborVisitorLists.Count; i++)
                                    {
                                        neighborVisitorLists[i].Remove(p);
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        // Token: 0x06000183 RID: 387 RVA: 0x00008CAC File Offset: 0x00006EAC
        private int GetRegionIndex(int r, int c)
        {
            return c * this.ey + r;
        }

        // Token: 0x06000184 RID: 388 RVA: 0x00008CC8 File Offset: 0x00006EC8
        private Tuple<int, int> GetRC(int regionIndex)
        {
            int item = regionIndex / this.ey;
            int item2 = regionIndex % this.ey;
            return new Tuple<int, int>(item2, item);
        }

        // Token: 0x06000185 RID: 389 RVA: 0x00008CF4 File Offset: 0x00006EF4
        private List<List<Person>> GetNeighborVisitorLists(int regionIndex)
        {
            List<List<Person>> list = new List<List<Person>>();
            Tuple<int, int> rc = this.GetRC(regionIndex);
            int item = rc.Item1;
            int item2 = rc.Item2;
            bool flag = item2 > 0;
            if (flag)
            {
                int regionIndex2 = this.GetRegionIndex(item, item2 - 1);
                list.Add(this.currentVisitors[regionIndex2]);
            }
            bool flag2 = item2 < this.ex - 1;
            if (flag2)
            {
                int regionIndex3 = this.GetRegionIndex(item, item2 + 1);
                list.Add(this.currentVisitors[regionIndex3]);
            }
            bool flag3 = item > 0;
            if (flag3)
            {
                int regionIndex4 = this.GetRegionIndex(item - 1, item2);
                list.Add(this.currentVisitors[regionIndex4]);
            }
            bool flag4 = item < this.ey - 1;
            if (flag4)
            {
                int regionIndex5 = this.GetRegionIndex(item + 1, item2);
                list.Add(this.currentVisitors[regionIndex5]);
            }
            bool flag5 = item2 > 0 && item > 0;
            if (flag5)
            {
                int regionIndex6 = this.GetRegionIndex(item - 1, item2 - 1);
                list.Add(this.currentVisitors[regionIndex6]);
            }
            bool flag6 = item2 > 0 && item < this.ey - 1;
            if (flag6)
            {
                int regionIndex7 = this.GetRegionIndex(item + 1, item2 - 1);
                list.Add(this.currentVisitors[regionIndex7]);
            }
            bool flag7 = item2 < this.ex - 1 && item > 0;
            if (flag7)
            {
                int regionIndex8 = this.GetRegionIndex(item - 1, item2 + 1);
                list.Add(this.currentVisitors[regionIndex8]);
            }
            bool flag8 = item2 < this.ex - 1 && item < this.ey - 1;
            if (flag8)
            {
                int regionIndex9 = this.GetRegionIndex(item + 1, item2 + 1);
                list.Add(this.currentVisitors[regionIndex9]);
            }
            return list;
        }

        // Token: 0x06000186 RID: 390 RVA: 0x00008ED4 File Offset: 0x000070D4
        private Tuple<double, double, double, double> GenerateRegion(int i, double cellSize, Point3d basePt, int ex, int ey)
        {
            int num = i / ey;
            int num2 = i - ey * num;
            double num3 = cellSize * (double)num + basePt.X;
            double item = num3 + cellSize;
            double num4 = cellSize * (double)num2 + basePt.Y;
            double item2 = num4 + cellSize;
            return new Tuple<double, double, double, double>(num3, item, num4, item2);
        }

        // Token: 0x06000187 RID: 391 RVA: 0x00008F24 File Offset: 0x00007124
        private void Reset(int ex, int ey, double cellSize, Point3d basePt)
        {
            this.Reset();
            int num = ex * ey;
            for (int i = 0; i < num; i++)
            {
                Tuple<double, double, double, double> item = this.GenerateRegion(i, cellSize, basePt, ex, ey);
                this.regions.Add(item);
            }
            this.Initialize(num);
        }

        // Token: 0x06000188 RID: 392 RVA: 0x00008F71 File Offset: 0x00007171
        private void Reset()
        {
            this.maxPeopleInCell = 1;
            this.currentNumbers = new List<int>();
            this.knownVisitors = new List<List<Person>>();
            this.currentVisitors = new List<List<Person>>();
            this.regions = new List<Tuple<double, double, double, double>>();
        }

        // Token: 0x06000189 RID: 393 RVA: 0x00008FA8 File Offset: 0x000071A8
        private void Initialize(int n)
        {
            for (int i = 0; i < n; i++)
            {
                this.currentNumbers.Add(0);
                this.knownVisitors.Add(new List<Person>());
                this.currentVisitors.Add(new List<Person>());
            }
        }

        // Token: 0x0600018A RID: 394 RVA: 0x00008FF8 File Offset: 0x000071F8
        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("GridPersonCounterMode", (int)this.mode);
            return base.Write(writer);
        }

        // Token: 0x0600018B RID: 395 RVA: 0x00009024 File Offset: 0x00007224
        public override bool Read(GH_IReader reader)
        {
            GridPersonCounterComponent.GridPersonCounterMode @int = (GridPersonCounterComponent.GridPersonCounterMode)reader.GetInt32("GridPersonCounterMode");
            this.Mode = @int;
            return base.Read(reader);
        }

        // Token: 0x0600018C RID: 396 RVA: 0x00009054 File Offset: 0x00007254
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem toolStripMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Count Frame", new EventHandler(this.Menu_CountFrameClicked), true);
            toolStripMenuItem.ToolTipText = "Counts the staying time of each person inside a cell and add up the numbers";
            ToolStripMenuItem toolStripMenuItem2 = GH_DocumentObject.Menu_AppendItem(menu, "Count Visit", new EventHandler(this.Menu_CountVisitClicked), true);
            toolStripMenuItem2.ToolTipText = "Adds one when a person re-enters the cell.";
            ToolStripMenuItem toolStripMenuItem3 = GH_DocumentObject.Menu_AppendItem(menu, "Count Person", new EventHandler(this.Menu_CountPersonClicked), true);
            toolStripMenuItem3.ToolTipText = "Each person is only counted once.";
        }

        // Token: 0x0600018D RID: 397 RVA: 0x000090D1 File Offset: 0x000072D1
        private void Menu_CountFrameClicked(object sender, EventArgs e)
        {
            base.RecordUndoEvent("GridPersonCounterMode changed");
            this.Reset();
            this.Mode = GridPersonCounterComponent.GridPersonCounterMode.CountFrame;
            this.ExpireSolution(true);
        }

        // Token: 0x0600018E RID: 398 RVA: 0x000090F7 File Offset: 0x000072F7
        private void Menu_CountVisitClicked(object sender, EventArgs e)
        {
            base.RecordUndoEvent("GridPersonCounterMode changed");
            this.Reset();
            this.Mode = GridPersonCounterComponent.GridPersonCounterMode.CountVisit;
            this.ExpireSolution(true);
        }

        // Token: 0x0600018F RID: 399 RVA: 0x0000911D File Offset: 0x0000731D
        private void Menu_CountPersonClicked(object sender, EventArgs e)
        {
            base.RecordUndoEvent("GridPersonCounterMode changed");
            this.Reset();
            this.Mode = GridPersonCounterComponent.GridPersonCounterMode.CountPerson;
            this.ExpireSolution(true);
        }

        // Token: 0x17000075 RID: 117
        // (get) Token: 0x06000190 RID: 400 RVA: 0x00009144 File Offset: 0x00007344
        protected override Bitmap Icon
        {
            get
            {
                return Resources.Icon_GridPersonCounter_02;
            }
        }

        // Token: 0x17000076 RID: 118
        // (get) Token: 0x06000191 RID: 401 RVA: 0x0000915C File Offset: 0x0000735C
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("53a10b87-7908-4fcf-abce-6826681a0b72");
            }
        }

        // Token: 0x040000AA RID: 170
        private int maxPeopleInCell = 1;

        // Token: 0x040000AB RID: 171
        private List<int> currentNumbers;

        // Token: 0x040000AC RID: 172
        private List<List<Person>> knownVisitors;

        // Token: 0x040000AD RID: 173
        private List<List<Person>> currentVisitors;

        // Token: 0x040000AE RID: 174
        private List<Person> people;

        // Token: 0x040000AF RID: 175
        private double cellSize;

        // Token: 0x040000B0 RID: 176
        private int ex;

        // Token: 0x040000B1 RID: 177
        private int ey;

        // Token: 0x040000B2 RID: 178
        private Point3d basePt;

        // Token: 0x040000B3 RID: 179
        private List<Tuple<double, double, double, double>> regions;

        // Token: 0x040000B4 RID: 180
        private GridPersonCounterComponent.GridPersonCounterMode mode;

        // Token: 0x02000032 RID: 50
        public enum GridPersonCounterMode
        {
            // Token: 0x04000103 RID: 259
            CountFrame = 1,
            // Token: 0x04000104 RID: 260
            CountVisit,
            // Token: 0x04000105 RID: 261
            CountPerson
        }
    }
}
