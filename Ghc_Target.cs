using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using PedSimulation.Properties;
using PedSimulation.Simulation;
using Rhino.Geometry;

namespace PedSimulation.GHComponents
{
    // Token: 0x0200002B RID: 43
    public class TargetComponent : GH_Component, IGH_VariableParameterComponent
    {
        // Token: 0x17000085 RID: 133
        // (get) Token: 0x060001E0 RID: 480 RVA: 0x0000BC10 File Offset: 0x00009E10
        // (set) Token: 0x060001E1 RID: 481 RVA: 0x0000BC28 File Offset: 0x00009E28
        public bool CreatingMultipleTargets
        {
            get
            {
                return this.creatingMultipleTargets;
            }
            set
            {
                this.creatingMultipleTargets = value;
                bool flag = this.creatingMultipleTargets;
                if (flag)
                {
                    base.Message = "Create multiple";
                }
                else
                {
                    base.Message = "Create single";
                }
            }
        }

        // Token: 0x060001E2 RID: 482 RVA: 0x0000BC64 File Offset: 0x00009E64
        public TargetComponent() : base("Target", "Target", "This component defines one or more Targets and visualizes them. Please check out the right-click menu for options. ", "PedSim", "PreSim")
        {
            this.CreatingMultipleTargets = true;
        }

        // Token: 0x060001E3 RID: 483 RVA: 0x0000BCD0 File Offset: 0x00009ED0
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Position", "P", "A list of points. Specify whether you want to create one or multiple targets in the menu.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Program", "Prog", "A single program. Use the Program component.", 0);
            pManager.AddIntegerParameter("VisitingTime", "T", "time in frames", 0, 200);
            pManager.AddNumberParameter("AccessRadius", "R", "", 0, 1.0);
            pManager.AddBooleanParameter("DrawDot", "DrawDot", "If true, display this target as a Rhino Dot; otherwise, display it as text.", 0, this.drawDot);
        }

        // Token: 0x060001E4 RID: 484 RVA: 0x0000BD65 File Offset: 0x00009F65
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Target", "T", "", GH_ParamAccess.list);
        }

        // Token: 0x060001E5 RID: 485 RVA: 0x0000BD80 File Offset: 0x00009F80
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> list = new List<Point3d>();
            Program targetPr = null;
            this.targets = new List<Target>();
            int num = 0;
            double accessRadius = 0.0;
            bool flag = !DA.GetDataList<Point3d>("Position", list);
            if (!flag)
            {
                bool flag2 = !DA.GetData<Program>("Program", ref targetPr);
                if (!flag2)
                {
                    bool flag3 = !DA.GetData<int>("VisitingTime", ref num);
                    if (!flag3)
                    {
                        bool flag4 = !DA.GetData<double>("AccessRadius", ref accessRadius);
                        if (!flag4)
                        {
                            bool flag5 = !DA.GetData<bool>("DrawDot", ref this.drawDot);
                            if (!flag5)
                            {
                                List<string> list2 = new List<string>();
                                bool flag6 = base.Params.Input.Count > this.numInputParams;
                                if (flag6)
                                {
                                    bool flag7 = !DA.GetDataList<string>("TargetName", list2);
                                    if (flag7)
                                    {
                                        return;
                                    }
                                }
                                bool flag8 = this.CreatingMultipleTargets;
                                if (flag8)
                                {
                                    bool flag9 = list2.Count > 0;
                                    if (flag9)
                                    {
                                        bool flag10 = list.Count != list2.Count;
                                        if (flag10)
                                        {
                                            throw new ArgumentException("The number of points and the number of names do not match.");
                                        }
                                        for (int i = 0; i < list.Count; i++)
                                        {
                                            Point3d position = list[i];
                                            string targetName = list2[i];
                                            Target item = new Target(position, targetPr, targetName)
                                            {
                                                VisitDuration = (double)num,
                                                AccessRadius = accessRadius
                                            };
                                            this.targets.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        for (int j = 0; j < list.Count; j++)
                                        {
                                            Point3d position2 = list[j];
                                            Target item2 = new Target(position2, targetPr)
                                            {
                                                VisitDuration = (double)num,
                                                AccessRadius = accessRadius
                                            };
                                            this.targets.Add(item2);
                                        }
                                    }
                                }
                                else
                                {
                                    bool flag11 = list2.Count >= 1;
                                    if (flag11)
                                    {
                                        string targetName2 = list2[0];
                                        Target item3 = new Target(list, targetPr, targetName2)
                                        {
                                            VisitDuration = (double)num,
                                            AccessRadius = accessRadius
                                        };
                                        this.targets.Add(item3);
                                    }
                                    else
                                    {
                                        Target item4 = new Target(list, targetPr)
                                        {
                                            VisitDuration = (double)num,
                                            AccessRadius = accessRadius
                                        };
                                        this.targets.Add(item4);
                                    }
                                }
                                DA.SetDataList(0, this.targets);
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x060001E6 RID: 486 RVA: 0x0000BFFC File Offset: 0x0000A1FC
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            bool flag = this.targets != null;
            if (flag)
            {
                bool flag2 = this.CreatingMultipleTargets;
                if (flag2)
                {
                    foreach (Target target in this.targets)
                    {
                        string text = target.TargetProgram.Name;
                        bool flag3 = target.Name != null;
                        if (flag3)
                        {
                            text = text + "\n" + target.Name;
                        }
                        bool flag4 = this.drawDot;
                        if (flag4)
                        {
                            args.Display.DrawDot(target.TagPosition, text, target.TargetProgram.DisplayColor, this.defaultTextColor);
                        }
                        else
                        {
                            args.Display.Draw2dText(text, target.TargetProgram.DisplayColor, target.TagPosition, true, this.textHeight, this.font);
                        }
                    }
                }
                else
                {
                    Target target2 = this.targets[0];
                    string text2 = target2.TargetProgram.Name;
                    bool flag5 = target2.Name != null;
                    if (flag5)
                    {
                        text2 = text2 + "\n" + target2.Name;
                    }
                    bool flag6 = this.drawDot;
                    if (flag6)
                    {
                        args.Display.DrawDot(target2.TagPosition, text2, target2.TargetProgram.DisplayColor, this.defaultTextColor);
                    }
                    else
                    {
                        args.Display.Draw2dText(text2, target2.TargetProgram.DisplayColor, target2.TagPosition, true, this.textHeight, this.font);
                    }
                }
            }
        }

        // Token: 0x060001E7 RID: 487 RVA: 0x0000C1B4 File Offset: 0x0000A3B4
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

        // Token: 0x060001E8 RID: 488 RVA: 0x0000C204 File Offset: 0x0000A404
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

        // Token: 0x060001E9 RID: 489 RVA: 0x0000C238 File Offset: 0x0000A438
        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            return new Param_GenericObject
            {
                Name = "TargetName",
                NickName = "Name",
                Description = "A list of name that matches the target positions",
                Access = GH_ParamAccess.list
            };
        }

        // Token: 0x060001EA RID: 490 RVA: 0x0000C280 File Offset: 0x0000A480
        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        // Token: 0x060001EB RID: 491 RVA: 0x00007A82 File Offset: 0x00005C82
        public void VariableParameterMaintenance()
        {
        }

        // Token: 0x060001EC RID: 492 RVA: 0x0000C294 File Offset: 0x0000A494
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("createMultipleTargets", this.CreatingMultipleTargets);
            return base.Write(writer);
        }

        // Token: 0x060001ED RID: 493 RVA: 0x0000C2C0 File Offset: 0x0000A4C0
        public override bool Read(GH_IReader reader)
        {
            this.CreatingMultipleTargets = reader.GetBoolean("createMultipleTargets");
            return base.Read(reader);
        }

        // Token: 0x060001EE RID: 494 RVA: 0x0000C2EC File Offset: 0x0000A4EC
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem toolStripMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Create multiple Targets", new EventHandler(this.Menu_CreateMultipleTargetClicked), true, this.CreatingMultipleTargets);
            toolStripMenuItem.ToolTipText = "Draw the person as a solid circle in current target color";
        }

        // Token: 0x060001EF RID: 495 RVA: 0x0000C325 File Offset: 0x0000A525
        private void Menu_CreateMultipleTargetClicked(object sender, EventArgs e)
        {
            base.RecordUndoEvent("createMultipleTargets changed");
            this.CreatingMultipleTargets = !this.CreatingMultipleTargets;
            this.ExpireSolution(true);
        }

        // Token: 0x17000086 RID: 134
        // (get) Token: 0x060001F0 RID: 496 RVA: 0x0000C34C File Offset: 0x0000A54C
        protected override Bitmap Icon
        {
            get
            {
                return Resources.Icon_Target_01;
            }
        }

        // Token: 0x17000087 RID: 135
        // (get) Token: 0x060001F1 RID: 497 RVA: 0x0000C364 File Offset: 0x0000A564
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("F991069F-023A-4E0C-BF19-BA2F343C6644");
            }
        }

        // Token: 0x040000E6 RID: 230
        private readonly Color defaultTextColor = Color.Black;

        // Token: 0x040000E7 RID: 231
        private readonly int textHeight = 12;

        // Token: 0x040000E8 RID: 232
        private readonly string font = "Arial";

        // Token: 0x040000E9 RID: 233
        private List<Target> targets;

        // Token: 0x040000EA RID: 234
        private bool creatingMultipleTargets = false;

        // Token: 0x040000EB RID: 235
        private bool drawDot = false;

        // Token: 0x040000EC RID: 236
        private readonly int numInputParams = 5;
    }
}

