using System;
using System.Drawing;
using Grasshopper.Kernel;
using PedSimulation.Properties;
using PedSimulation.Simulation;

namespace PedSimulation.GHComponents
{
    public class ProgramComponent : GH_Component
    {
        public ProgramComponent() : base("Program", "Program", "A program describes a target's function or a person's interest.", "PedSim", "PreSim")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("ProgramName", "Name", "", 0, "");
            pManager.AddColourParameter("DisplayColor", "Color", "", 0, Color.Empty);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Program", "Prog", "", 0);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "";
            Color empty = Color.Empty;
            bool flag = !DA.GetData<string>("ProgramName", ref name);
            if (!flag)
            {
                bool flag2 = !DA.GetData<Color>("DisplayColor", ref empty);
                if (!flag2)
                {
                    this.program = new Program(name, empty);
                    DA.SetData(0, this.program);
                }
            }
        }


        protected override Bitmap Icon
        {
            get
            {
                return Resources.Icon_Program_01;
            }
        }

        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("36a4cc59-c68e-4483-902d-a82882e18716");
            }
        }

        private Program program;
    }
}
