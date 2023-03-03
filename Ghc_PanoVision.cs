using System;
using System.Drawing;
using Grasshopper.Kernel;
using PedSimulation.Properties;
using PedSimulation.Simulation;

namespace PedSimulation.GHComponents
{
    public class PanoVisionComponent : GH_Component
    {
        public PanoVisionComponent() : base("PanoramaVision", "PanoVision", "This component creates panorama vision for a PersonTemplate when its Vision parameter is exposed. It is also the default vision type.There is no visualization for panorama vision", "PedSim", "PreSim")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Vision", "V", "panorama vision", 0);
        }

        // Token: 0x060001A1 RID: 417 RVA: 0x00009998 File Offset: 0x00007B98
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            PanoVision panoVision = new PanoVision();
            DA.SetData(0, panoVision);
        }

        // Token: 0x17000079 RID: 121
        // (get) Token: 0x060001A2 RID: 418 RVA: 0x000099B8 File Offset: 0x00007BB8
        protected override Bitmap Icon
        {
            get
            {
                return Resources.Icon_PanoVision_01;
            }
        }

        // Token: 0x1700007A RID: 122
        // (get) Token: 0x060001A3 RID: 419 RVA: 0x000099D0 File Offset: 0x00007BD0
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("09147a15-0e73-4a7f-8f7e-f8abb6ea1d20");
            }
        }
    }
}

