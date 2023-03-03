using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using PedSimulation.Properties;
using PedSimulation.Simulation;

namespace PedSimulation.GHComponents
{
    // Token: 0x02000020 RID: 32
    public class DeconstructPersonRecordComponent : GH_Component
    {
        // Token: 0x06000148 RID: 328 RVA: 0x00007724 File Offset: 0x00005924
        public DeconstructPersonRecordComponent() : base("DeconstructPersonRecord", "DeconsPR", "", "PedSim", "PostSim")
        {
        }

        // Token: 0x06000149 RID: 329 RVA: 0x00007747 File Offset: 0x00005947
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PersonRecord", "Record", "", 0);
        }

        // Token: 0x0600014A RID: 330 RVA: 0x00007764 File Offset: 0x00005964
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Person ID", "ID", "", 0);
            pManager.AddIntegerParameter("Template ID", "TemplateID", "", 0);
            pManager.AddIntegerParameter("Start Time", "StartTime", "", 0);
            pManager.AddIntegerParameter("End Time", "EndTime", "", 0);
            pManager.AddIntegerParameter("Time Sequence", "Time", "", GH_ParamAccess.list);
            pManager.AddPointParameter("Trace Positions", "TracePos", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Trace Speeds", "TraceSpd", "", GH_ParamAccess.list);
            pManager.AddTextParameter("State Sequence", "State", "", GH_ParamAccess.list);
        }

        // Token: 0x0600014B RID: 331 RVA: 0x0000782C File Offset: 0x00005A2C
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            PersonRecord personRecord = null;
            bool flag = !DA.GetData<PersonRecord>(0, ref personRecord);
            if (!flag)
            {
                bool flag2 = personRecord == null;
                if (!flag2)
                {
                    DA.SetData("Person ID", personRecord.ID);
                    DA.SetData("Template ID", personRecord.TemplateID);
                    DA.SetData("Start Time", personRecord.StartFrame);
                    DA.SetData("End Time", personRecord.EndFrame);
                    DA.SetDataList("Time Sequence", personRecord.Trace.TimeInFrames);
                    DA.SetDataList("Trace Positions", personRecord.Trace.Position);
                    DA.SetDataList("Trace Speeds", personRecord.Trace.Speed);
                    DA.SetDataList("State Sequence", this.GetStatesText(personRecord.Trace.State));
                }
            }
        }

        // Token: 0x0600014C RID: 332 RVA: 0x00007920 File Offset: 0x00005B20
        private List<string> GetStatesText(List<PersonState> states)
        {
            List<string> list = new List<string>();
            foreach (PersonState personState in states)
            {
                list.Add(personState.ToString());
            }
            return list;
        }

        // Token: 0x1700006C RID: 108
        // (get) Token: 0x0600014D RID: 333 RVA: 0x0000798C File Offset: 0x00005B8C
        protected override Bitmap Icon
        {
            get
            {
                return Resources.Icon_DeconstructPersonRecord_01;
            }
        }

        // Token: 0x1700006D RID: 109
        // (get) Token: 0x0600014E RID: 334 RVA: 0x000079A4 File Offset: 0x00005BA4
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("3e326579-4fec-4413-ade2-c6ecd88babaf");
            }
        }
    }
}

