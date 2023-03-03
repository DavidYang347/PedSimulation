using System;
using System.Drawing;
using Grasshopper.Kernel;
using PedSimulation.Properties;
using PedSimulation.Simulation;

namespace PedSimulation.GHComponents
{
    public class SystemSettingsComponent : GH_Component
    {
        public SystemSettingsComponent() : base("SystemSettings", "Settings", "Optional, for fine-tuning the simulation\r\n                V0.12.3 2019-06-23", "PedSim", "Engine")
        {
            this.settings = new SystemSettings();
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("FPS", "FPS", "", 0, this.fps);
            pManager.AddNumberParameter("ObstacleOffset", "ObsOffset", "", 0, this.obsOffset);
            pManager.AddNumberParameter("ObstacleRepulsionCoefA", "ObsCoefA", "", 0, this.obstacleRepulsionA);
            pManager.AddNumberParameter("ObstacleRepulsionCoefB", "ObsCoefB", "", 0, this.obstacleRepulsionB);
            pManager.AddNumberParameter("ObstacleRepulsionMax", "ObsFMax", "", 0, this.obstacleRepulsionMax);
            pManager.AddNumberParameter("ObstacleRepulsionThreshold", "ObsTreshold", "Unit: m", 0, this.obstacleRepulsionThreshold);
            pManager.AddBooleanParameter("EnableTrace", "EnableTrace", "", 0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("SystemSettings", "Settings", "", 0);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool flag = !DA.GetData<double>("FPS", ref this.fps);
            if (!flag)
            {
                bool flag2 = !DA.GetData<double>("ObstacleOffset", ref this.obsOffset);
                if (!flag2)
                {
                    bool flag3 = !DA.GetData<double>("ObstacleRepulsionCoefA", ref this.obstacleRepulsionA);
                    if (!flag3)
                    {
                        bool flag4 = !DA.GetData<double>("ObstacleRepulsionCoefB", ref this.obstacleRepulsionB);
                        if (!flag4)
                        {
                            bool flag5 = !DA.GetData<double>("ObstacleRepulsionMax", ref this.obstacleRepulsionMax);
                            if (!flag5)
                            {
                                bool flag6 = !DA.GetData<double>("ObstacleRepulsionThreshold", ref this.obstacleRepulsionThreshold);
                                if (!flag6)
                                {
                                    bool flag7 = !DA.GetData<bool>("EnableTrace", ref this.isTraceEnabled);
                                    if (!flag7)
                                    {
                                        this.settings.FPS = this.fps;
                                        this.settings.ObstacleOffset = this.obsOffset;
                                        this.settings.ObstacleRepulsionA = this.obstacleRepulsionA;
                                        this.settings.ObstacleRepulsionB = this.obstacleRepulsionB;
                                        this.settings.ObstacleRepulsionMax = this.obstacleRepulsionMax;
                                        this.settings.ObstacleRepulsionThreshold = this.obstacleRepulsionThreshold;
                                        this.settings.IsTraceEnabled = this.isTraceEnabled;
                                        DA.SetData(0, this.settings);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override Bitmap Icon
        {
            get
            {
                return Resources.Icon_PedSimSetting_01;
            }
        }

        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("1f8676f1-1e27-42ec-bea2-2aeb6017eef4");
            }
        }

        private readonly SystemSettings settings;

        private double fps = 30.0;

        private double obsOffset = 1.0;

        private double obstacleRepulsionA = 50.0;

        private double obstacleRepulsionB = 2.0;

        private double obstacleRepulsionMax = 5.0;

        private double obstacleRepulsionThreshold = 0.5;

        private bool isTraceEnabled = true;
    }
}

