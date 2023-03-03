using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using PedSimulation.Properties;
using PedSimulation.Simulation;
using Rhino.Geometry;

namespace PedSimulation.GHComponents
{
    public class GateComponent : GH_Component, IGH_VariableParameterComponent
    {
        public GateComponent() : base("Gate", "Gate", "This component defines one or multiple Gates and visualizes them.", "PedSim", "PreSim")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Position", "P", "", GH_ParamAccess.list);
            pManager.AddTextParameter("GateName", "Name", "", GH_ParamAccess.list, "");
            pManager.AddIntegerParameter("ExitTime", "ET", "Used to control the rate of people exiting from the gate.", 0, 1);
            pManager.AddNumberParameter("AccessRadius", "R", "", 0);
            pManager.AddBooleanParameter("DrawDot", "DrawDot", "If true, display this target as a Rhino Dot; otherwise, display it as text.", 0, this.drawDot);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Gate", "G", "", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> list = new List<Point3d>();
            List<string> list2 = new List<string>();
            int exitTime = 0;
            double accessRadius = 0.0;
            bool flag = !DA.GetDataList<Point3d>("Position", list);
            if (!flag)
            {
                bool flag2 = !DA.GetDataList<string>("GateName", list2);
                if (!flag2)
                {
                    bool flag3 = !DA.GetData<int>("ExitTime", ref exitTime);
                    if (flag3)
                    {
                        exitTime = 1;
                    }
                    bool flag4 = !DA.GetData<double>("AccessRadius", ref accessRadius);
                    if (!flag4)
                    {
                        bool flag5 = !DA.GetData<bool>("DrawDot", ref this.drawDot);
                        if (!flag5)
                        {
                            bool flag6 = base.Params.Input.Count > this.numInputParams;
                            if (flag6)
                            {
                                bool flag7 = !DA.GetData<Color>("Color", ref this.gateColor);
                                if (flag7)
                                {
                                    return;
                                }
                            }
                            Color color = this.gateColor;
                            bool flag8 = false;
                            if (flag8)
                            {
                                this.gateColor = this.defaultGateColor;
                            }
                            this.gates = new List<Gate>();
                            bool flag9 = list2.Count == 1;
                            if (flag9)
                            {
                                string name = list2[0];
                                foreach (Point3d pt in list)
                                {
                                    Gate item = new Gate(pt, name, exitTime, this.gateColor)
                                    {
                                        AccessRadius = accessRadius
                                    };
                                    this.gates.Add(item);
                                }
                            }
                            else
                            {
                                bool flag10 = list.Count != list2.Count;
                                if (flag10)
                                {
                                    throw new ArgumentException("The number of points and the number of names do not match.");
                                }
                                for (int i = 0; i < list.Count; i++)
                                {
                                    Point3d pt2 = list[i];
                                    string name2 = list2[i];
                                    Gate item2 = new Gate(pt2, name2, exitTime, this.gateColor)
                                    {
                                        AccessRadius = accessRadius
                                    };
                                    this.gates.Add(item2);
                                }
                            }
                            DA.SetDataList(0, this.gates);
                        }
                    }
                }
            }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            bool flag = this.gates != null && this.gates.Count > 0;
            if (flag)
            {
                foreach (Gate gate in this.gates)
                {
                    string text = gate.Name;
                    bool flag2 = gate.Name != null;
                    if (flag2)
                    {
                        text = gate.Name;
                    }
                    else
                    {
                        text = "";
                    }
                    bool flag3 = this.drawDot;
                    if (flag3)
                    {
                        args.Display.DrawDot(gate.TagPosition, text, this.gateColor, this.defaultTextColor);
                    }
                    else
                    {
                        args.Display.Draw2dText(text, this.gateColor, gate.TagPosition, true, this.textHeight, this.font);
                    }
                }
            }
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            bool flag = side == 0;
            bool result;
            if (flag)
            {
                bool flag2 = index == this.numInputParams && base.Params.Input.Count < this.numInputParams + 1;
                result = flag2;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            bool flag = side == 0;
            bool result;
            if (flag)
            {
                bool flag2 = index == this.numInputParams;
                result = flag2;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            return new Param_Colour
            {
                Name = "Color",
                NickName = "Color",
                Description = "Optional color for the Gate visual",
                Access = 0
            };
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            this.gateColor = this.defaultGateColor;
            return true;
        }

        public void VariableParameterMaintenance()
        {
        }

        protected override Bitmap Icon
        {
            get
            {
                return Resources.Icon_Gate_01;
            }
        }

        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("9fda6099-cbac-4de4-863c-211cc83f1c38");
            }
        }

        private readonly Color defaultTextColor = Color.White;

        private readonly Color defaultGateColor = Color.Black;

        private Color gateColor;

        private List<Gate> gates;

        private readonly int textHeight = 12;

        private readonly string font = "Arial";

        private bool drawDot = false;

        private readonly int numInputParams = 5;
    }
}

