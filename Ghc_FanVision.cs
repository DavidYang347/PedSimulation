using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using PedSimulation.Properties;
using PedSimulation.Simulation;

namespace PedSimulation.GHComponents
{
    // Token: 0x02000022 RID: 34
    public class FanVisionComponent : GH_Component, IGH_VariableParameterComponent
    {

        public FanVisionComponent() : base("FanShapedVision", "FanVision", "This component creates Fan-Shaped Vision for a PersonTemplate. It is used for spotting new Targets. To visualize the field of view, use DrawPerson component for Person.", "PedSim", "PreSim")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Angle", "A", "", 0, 149.994);
            pManager.AddNumberParameter("Distance", "D", "", 0, 50.0);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Vision", "V", "Fan-shaped vision", 0);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double num = 0.0;
            double distance = 0.0;
            double angularRes = 2.0;
            bool flag = !DA.GetData<double>("Angle", ref num);
            if (!flag)
            {
                bool flag2 = !DA.GetData<double>("Distance", ref distance);
                if (!flag2)
                {
                    bool flag3 = num > 360.0;
                    if (flag3)
                    {
                        num = 360.0;
                    }
                    bool flag4 = num < 0.0;
                    if (flag4)
                    {
                        num = 0.0;
                    }
                    num = num / 180.0 * 3.141592653589793;
                    bool flag5 = base.Params.Input.Count > 2;
                    if (flag5)
                    {
                        bool flag6 = !DA.GetData<double>("AngularResolution", ref angularRes);
                        if (flag6)
                        {
                            return;
                        }
                    }
                    FanVision fanVision = new FanVision(num, distance, angularRes);
                    DA.SetData(0, fanVision);
                }
            }
        }

        // Token: 0x06000169 RID: 361 RVA: 0x000082C4 File Offset: 0x000064C4
        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            bool flag = side == 0;
            bool result;
            if (flag)
            {
                bool flag2 = index == 2 && base.Params.Input.Count < 3;
                result = flag2;
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600016A RID: 362 RVA: 0x00008308 File Offset: 0x00006508
        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            bool flag = side == 0;
            bool result;
            if (flag)
            {
                bool flag2 = index == 2;
                result = flag2;
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600016B RID: 363 RVA: 0x00008334 File Offset: 0x00006534
        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            return new Param_Number
            {
                Name = "AngularResolution",
                NickName = "Res",
                Description = "Angular Resolution, unit: degree",
                Access = 0
            };
        }

        // Token: 0x0600016C RID: 364 RVA: 0x0000837C File Offset: 0x0000657C
        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        // Token: 0x0600016D RID: 365 RVA: 0x00007A82 File Offset: 0x00005C82
        public void VariableParameterMaintenance()
        {
        }

        // Token: 0x17000070 RID: 112
        // (get) Token: 0x0600016E RID: 366 RVA: 0x00008390 File Offset: 0x00006590
        protected override Bitmap Icon
        {
            get
            {
                return Resources.Icon_FanVision_01;
            }
        }

        // Token: 0x17000071 RID: 113
        // (get) Token: 0x0600016F RID: 367 RVA: 0x000083A8 File Offset: 0x000065A8
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("ee72c6b4-4f76-45fe-b84f-9a08ba754a9c");
            }
        }
    }
}

