using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using PedSimulation.Properties;
using PedSimulation.Simulation;
using Rhino.Display;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PedSimulation.GHComponents
{
    public class DrawPersonComponent : GH_Component, IGH_VariableParameterComponent
    {
        public DrawPersonComponent() : base("DrawPerson", "DrawPerson", "Right-click for options", "PedSim", "PostSim")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("People", "P", "", GH_ParamAccess.list);
            pManager.AddColourParameter("ColorOfPeople", "Color", "", 0, Color .Black);//添加颜色

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //添加颜色
            Color personColor = Color.Black;
            DA.GetData<Color>(1, ref personColor);
            personFillMat = new DisplayMaterial(personColor, 0.5);
            //添加颜色

            this.people = new List<Person>();
            bool flag = !DA.GetDataList<Person>( 0, this.people);
            if (!flag)
            {
                bool flag2 = base.Params.Input.Count > 2;
                if (flag2)
                {
                    bool flag3 = !DA.GetData<DisplayMaterial>("FOVMaterial", ref this.userFOVMaterial);
                    if (flag3)
                    {

                    }
                }
            }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            bool flag = this.people == null;
            if (!flag)
            {
                foreach (Person person in this.people)
                {
                    bool flag2 = this.drawFOV;
                    if (flag2)
                    {
                        this.DrawFOV(person, args, true);
                    }
                    bool flag3 = this.drawCurrentTargetFill;
                    if (flag3)
                    {
                        this.DrawPersonCurrentInterest(person, args);
                    }
                    else
                    {
                        Point3d getRhinoPoint = person.GetRhinoPoint;
                        Circle circle = new Circle(getRhinoPoint, person.BodyRadius);
                        Brep brep = Brep.CreateFromSurface(Surface.CreateExtrusionToPoint(new ArcCurve(circle), getRhinoPoint));
                        args.Display.DrawBrepShaded(brep, this.personFillMat);
                    }
                    bool flag4 = this.drawNumber;
                    if (flag4)
                    {
                        this.DrawPersonNumber(person, args);
                    }
                }
            }
        }

        private void DrawFOV(Person person, IGH_PreviewArgs args, bool drawFill)
        {
            bool flag = person.FOV != null && person.FOV.Count != 0;
            if (flag)
            {
                foreach (Polyline polyline in person.FOV)
                {
                    if (drawFill)
                    {
                        Mesh mesh = Mesh.CreateFromClosedPolyline(polyline);
                        bool flag2 = mesh != null;
                        if (flag2)
                        {
                            args.Display.DrawMeshShaded(mesh, this.userFOVMaterial);
                        }
                    }
                    else
                    {
                        Color diffuse = this.userFOVMaterial.Diffuse;
                        args.Display.DrawPolyline(polyline, diffuse);
                    }
                }
            }
        }

        private void DrawPersonCurrentInterest(Person person, IGH_PreviewArgs args)
        {
            Point3d getRhinoPoint = person.GetRhinoPoint;
            Circle circle = new Circle(getRhinoPoint, person.BodyRadius);
            //circle..ctor(getRhinoPoint, person.BodyRadius);
            ArcCurve arcCurve = new ArcCurve(circle);
            Brep brep = Brep.CreatePlanarBreps(arcCurve)[0];
            
            bool flag = brep.Faces[0].NormalAt(0.0, 0.0).Z < 0.0;
            if (flag)
            {
                brep.Flip();
            }
            bool flag2 = person.CurrentGoal is Target;
            if (flag2)
            {
                Target target = (Target)person.CurrentGoal;
                DisplayMaterial displayMaterial = new DisplayMaterial(target.TargetProgram.DisplayColor, this.bodyTransparency);
                args.Display.DrawBrepShaded(brep, displayMaterial);
            }
            else
            {
                Gate gate = (Gate)person.CurrentGoal;
                DisplayMaterial displayMaterial2 = new DisplayMaterial(gate.GateColor, this.bodyTransparency);
                args.Display.DrawBrepShaded(brep, displayMaterial2);
            }
        }

        private void DrawPersonNumber(Person person, IGH_PreviewArgs args)
        {
            Point3d getRhinoPoint = person.GetRhinoPoint;
            string text = person.TemplateID.ToString();
            double num;
            Vector3d vector3d = this.ComputeTextOffsetInCircle(text, person.BodyRadius, out num);
            Plane plane = new Plane(getRhinoPoint + vector3d, Vector3d.ZAxis);
            //plane..ctor(getRhinoPoint + vector3d, Vector3d.ZAxis);
            args.Display.Draw3dText(text, this.defaultTextColor, plane, num, "Courier New");
        }

        private Vector3d ComputeTextOffsetInCircle(string text, double r, out double h)
        {
            int length = text.Length;
            double num = 1.0;
            double num2 = 1.6 * r;
            h = Math.Sqrt(num2 * num2 / ((double)(length * length) * num * num + 1.0));
            double num3 = num * h;
            double num4 = (double)(-(double)length) * num3 / 2.0;
            double num5 = -h / 2.0;
            return new Vector3d(num4, num5, 0.0);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("drawCurrentTargetFill", this.drawCurrentTargetFill);
            writer.SetBoolean("drawFOV", this.drawFOV);
            writer.SetBoolean("drawNumber", this.drawNumber);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            this.drawCurrentTargetFill = reader.GetBoolean("drawCurrentTargetFill");
            this.drawFOV = reader.GetBoolean("drawFOV");
            this.drawNumber = reader.GetBoolean("drawNumber");
            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem toolStripMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Draw current target color", new EventHandler(this.Menu_DrawCurrentTargetClicked), true, this.drawCurrentTargetFill);
            toolStripMenuItem.ToolTipText = "Draw the person as a solid circle in current target color";
            ToolStripMenuItem toolStripMenuItem2 = GH_DocumentObject.Menu_AppendItem(menu, "Draw FOV", new EventHandler(this.Menu_DrawFOVClicked), true, this.drawFOV);
            toolStripMenuItem2.ToolTipText = "Draw Field of View for each person (slow).";
            ToolStripMenuItem toolStripMenuItem3 = GH_DocumentObject.Menu_AppendItem(menu, "Draw Number", new EventHandler(this.Menu_DrawNumberClicked), true, this.drawNumber);
            toolStripMenuItem3.ToolTipText = "Draw Person Template number on each person";
        }

        private void Menu_DrawCurrentTargetClicked(object sender, EventArgs e)
        {
            base.RecordUndoEvent("DrawPersonMode changed");
            this.drawCurrentTargetFill = !this.drawCurrentTargetFill;
            this.ExpireSolution(true);
        }

        private void Menu_DrawFOVClicked(object sender, EventArgs e)
        {
            base.RecordUndoEvent("DrawPersonMode changed");
            this.drawFOV = !this.drawFOV;
            this.ExpireSolution(true);
        }

        private void Menu_DrawNumberClicked(object sender, EventArgs e)
        {
            base.RecordUndoEvent("DrawPersonMode changed");
            this.drawNumber = !this.drawNumber;
            this.ExpireSolution(true);
        }

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

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            return new Param_GenericObject
            {
                Name = "FOVMaterial",
                NickName = "M",
                Description = "FOVMaterial will take effect if corresponding drawing option is selected. Suggest use Create Material Component",
                Access = 0
            };
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            this.userFOVMaterial = new DisplayMaterial(Color.Black, 0.94);
            return true;
        }

        public void VariableParameterMaintenance()
        {
        }
        protected override Bitmap Icon
        {
            get
            {
                return Resources.Icon_DrawPerson_02;
            }
        }

        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("f991f310-f60b-4df2-944d-b34fec06a1b5");
            }
        }

        private List<Person> people;

        private double bodyTransparency = 0.2;

        //private DisplayMaterial personFillMat = new DisplayMaterial(Color.Black, 0.5);
        private DisplayMaterial personFillMat = new DisplayMaterial();

        private DisplayMaterial curInterestMat = new DisplayMaterial(Color.Black, 0.5);

        private DisplayMaterial userFOVMaterial = new DisplayMaterial(Color.Black, 0.94);

        private Color defaultTextColor = Color.White;

        private bool drawCurrentTargetFill = false;

        private bool drawFOV = false;

        private bool drawNumber = false;
    }
}

