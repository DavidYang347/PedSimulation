using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using PedSimulation.Properties;
using PedSimulation.Simulation;
using Rhino;
using Rhino.Geometry;

namespace PedSimulation.GHComponents
{

    public class PersonTemplateComponent : GH_Component
    {

        public PersonTemplateComponent() : base("PersonTemplate", "PT", "This component creates and visualizes one or multiple PersonTemplate and visualizes them. \r\n                Please check out the right-click menu for options and zoom in for more input.\r\n                Number of Start Gates and Destination Gates must match. Use Grasshopper Cross Reference Component to cross reference.", "PedSim", "PreSim")
        {
            this.Mode = PersonTemplateComponent.PersonTemplateDirectionMode.OriginalDirection;
        }


        private PersonTemplateComponent.PersonTemplateDirectionMode Mode
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
                    case PersonTemplateComponent.PersonTemplateDirectionMode.OriginalDirection:
                        base.Message = "Original Dir.";
                        break;
                    case PersonTemplateComponent.PersonTemplateDirectionMode.ReversedDirection:
                        base.Message = "Reversed Dir.";
                        break;
                    case PersonTemplateComponent.PersonTemplateDirectionMode.BothDirections:
                        base.Message = "Both Dir.";
                        break;
                }
            }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Start Gate", "S", this.toolTip_gate + this.toolTip_matchListLength, GH_ParamAccess.list);
            pManager.AddGenericParameter("Destination Gate", "D", this.toolTip_gate + this.toolTip_matchListLength, GH_ParamAccess.list);
            pManager.AddGenericParameter("Interests", "Intr", this.toolTip_interests + this.toolTip_sharedValue, GH_ParamAccess.list);
            pManager.AddIntegerParameter("Need Value", "Need", this.toolTip_needValues + this.toolTip_sharedValue, GH_ParamAccess.list, 1);
            pManager.AddIntegerParameter("Start Number", "N", this.toolTip_startNumber, 0, 0);
            pManager.AddNumberParameter("Probability", "Prob", this.toolTip_probability + this.toolTip_sharedValue, 0, this.defaultProbability);
            pManager.AddIntegerParameter("Time Limit", "TL", this.toolTip_timeLimit + this.toolTip_sharedValue, 0, this.defaultTimeLimit);
            pManager.AddGenericParameter("Vision", "Vision", "", 0);
            pManager.AddNumberParameter("Body Radius", "R", this.toolTip_sharedValue, 0, this.defaultBodyRadius);
            pManager.AddNumberParameter("Target Force", "TF", this.toolTip_targetForce + this.toolTip_sharedValue, 0, this.defaultTargetForce);
            pManager.AddColourParameter("Display Color", "Color", this.toolTip_color + this.toolTip_sharedValue, 0, this.lineColor);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PersonTemplate", "PTemp", "A PersonTemplate is used to create people in a simulation.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            this.startGates = new List<Gate>();
            this.destiGates = new List<Gate>();
            this.interests = new List<Program>();
            this.needValues = new List<int>();
            double num = this.defaultProbability;
            int timeLimit = this.defaultTimeLimit;
            int num2 = 0;
            PersonVision personVision = null;
            double bodyRadius = this.defaultBodyRadius;
            double mass = this.defaultMass;
            double targetForce = this.defaultTargetForce;
            bool flag = !DA.GetDataList<Gate>("Start Gate", this.startGates);
            if (!flag)
            {
                bool flag2 = !DA.GetDataList<Gate>("Destination Gate", this.destiGates);
                if (!flag2)
                {
                    bool flag3 = !DA.GetDataList<Program>("Interests", this.interests);
                    if (!flag3)
                    {
                        bool flag4 = !DA.GetDataList<int>("Need Value", this.needValues);
                        if (!flag4)
                        {
                            bool flag5 = !DA.GetData<int>("Start Number", ref num2);
                            if (!flag5)
                            {
                                bool flag6 = !DA.GetData<double>("Probability", ref num);
                                if (!flag6)
                                {
                                    bool flag7 = !DA.GetData<int>("Time Limit", ref timeLimit);
                                    if (!flag7)
                                    {
                                        bool flag8 = !DA.GetData<Color>("Display Color", ref this.lineColor);
                                        if (!flag8)
                                        {
                                            bool flag9 = !DA.GetData<PersonVision>("Vision", ref personVision);
                                            if (!flag9)
                                            {
                                                bool flag10 = !DA.GetData<double>("Body Radius", ref bodyRadius);
                                                if (!flag10)
                                                {
                                                    bool flag11 = !DA.GetData<double>("Target Force", ref targetForce);
                                                    if (!flag11)
                                                    {
                                                        bool flag12 = this.startGates == null || this.startGates.Count == 0;
                                                        if (flag12)
                                                        {
                                                            throw new Exception("Start Gate is not valid.");
                                                        }
                                                        bool flag13 = this.destiGates == null || this.destiGates.Count == 0;
                                                        if (flag13)
                                                        {
                                                            throw new Exception("Destination Gate is not valid.");
                                                        }
                                                        bool flag14 = this.startGates.Count != this.destiGates.Count;
                                                        if (flag14)
                                                        {
                                                            throw new ArgumentException("Numbers of Start Gates and Destination Gates do not match.");
                                                        }
                                                        this.interests.RemoveAll((Program item) => item == null);
                                                        bool flag15 = this.interests.Count < 1;
                                                        if (flag15)
                                                        {
                                                            throw new ArgumentNullException("There is no valid interest.");
                                                        }
                                                        bool flag16 = this.needValues.Count < 1 || (this.needValues.Count != this.interests.Count && this.needValues.Count != 1);
                                                        if (flag16)
                                                        {
                                                            throw new Exception("Number of Need Values does not match number of interests");
                                                        }
                                                        while (this.needValues.Count < this.interests.Count)
                                                        {
                                                            this.needValues.Add(this.needValues[0]);
                                                        }
                                                        foreach (int num3 in this.needValues)
                                                        {
                                                            bool flag17 = num3 < 0;
                                                            if (flag17)
                                                            {
                                                                throw new ArgumentOutOfRangeException("Need values must be non-negative.");
                                                            }
                                                        }
                                                        bool flag18 = !RhinoMath.IsValidDouble(num) || num < 0.0;
                                                        if (flag18)
                                                        {
                                                            throw new Exception("Probability not valid.");
                                                        }
                                                        this.results = new List<PersonTemplate>();
                                                        this.results_rev = new List<PersonTemplate>();
                                                        int num4 = num2;
                                                        for (int i = 0; i < this.startGates.Count; i++)
                                                        {
                                                            Gate gate = this.startGates[i];
                                                            Gate gate2 = this.destiGates[i];
                                                            bool flag19 = this.Mode == PersonTemplateComponent.PersonTemplateDirectionMode.ReversedDirection;
                                                            if (flag19)
                                                            {
                                                                Gate gate3 = gate;
                                                                gate = gate2;
                                                                gate2 = gate3;
                                                            }
                                                            bool flag20 = this.Mode == PersonTemplateComponent.PersonTemplateDirectionMode.OriginalDirection || this.Mode == PersonTemplateComponent.PersonTemplateDirectionMode.ReversedDirection;
                                                            if (flag20)
                                                            {
                                                                PersonTemplate personTemplate = new PersonTemplate(gate, gate2, timeLimit, num, num4, bodyRadius, mass, targetForce)
                                                                {
                                                                    Interests = this.interests,
                                                                    NeedValues = this.needValues
                                                                };
                                                                bool flag21 = personVision != null;
                                                                if (flag21)
                                                                {
                                                                    personTemplate.Vision = personVision;
                                                                }
                                                                num4++;
                                                                this.results.Add(personTemplate);
                                                            }
                                                            else
                                                            {
                                                                PersonTemplate personTemplate = new PersonTemplate(gate, gate2, timeLimit, num, num4, bodyRadius, mass, targetForce);
                                                                num4++;
                                                                PersonTemplate personTemplate2 = new PersonTemplate(gate2, gate, timeLimit, num, num4, bodyRadius, mass, targetForce);
                                                                num4++;
                                                                bool flag22 = this.interests.Count > 0;
                                                                if (flag22)
                                                                {
                                                                    personTemplate.Interests = this.interests;
                                                                    personTemplate.NeedValues = this.needValues;
                                                                    personTemplate2.Interests = this.interests;
                                                                    personTemplate2.NeedValues = this.needValues;
                                                                }
                                                                bool flag23 = personVision != null;
                                                                if (flag23)
                                                                {
                                                                    personTemplate.Vision = personVision;
                                                                    personTemplate2.Vision = personVision;
                                                                }
                                                                this.results.Add(personTemplate);
                                                                this.results_rev.Add(personTemplate2);
                                                            }
                                                        }
                                                        this.results.AddRange(this.results_rev);
                                                        DA.SetDataList("PersonTemplate", this.results);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            bool flag = this.results == null || this.results.Count == 0;
            if (!flag)
            {
                for (int i = 0; i < this.results.Count; i++)
                {
                    PersonTemplate pTemp = this.results[i];
                    this.DrawPersonTemplate(pTemp, args);
                }
            }
        }

        private void DrawPersonTemplate(PersonTemplate pTemp, IGH_PreviewArgs args)
        {
            int num = 12;
            double num2 = 0.3;
            double num3 = 20.0;
            double num4 = this.defaultGateRadius;
            Point3d tagPosition = pTemp.StartGate.TagPosition;
            Point3d tagPosition2 = pTemp.DestinationGate.TagPosition;
            Circle circle = new Circle(tagPosition, num4);
            //circle..ctor(tagPosition, num4);
            Circle circle2 = new Circle(tagPosition2, num4);
            //circle2..ctor(tagPosition2, num4);
            args.Display.DrawCircle(circle, this.defaultGateColor, this.defaultThickness);
            args.Display.DrawCircle(circle2, this.defaultGateColor, this.defaultThickness);
            int count = pTemp.Interests.Count;
            List<Point3d> targetMarkerCenters = this.GetTargetMarkerCenters(pTemp);
            for (int i = 0; i < count; i++)
            {
                Point3d point3d = targetMarkerCenters[i];
                Circle circle3 = new Circle(point3d, this.defaultTargetRadius);
                //circle3..ctor(point3d, this.defaultTargetRadius);
                Color displayColor = pTemp.Interests[i].DisplayColor;
                args.Display.DrawCircle(circle3, displayColor, this.defaultThickness);
            }
            List<Line> personTemplateMarkerLines = this.GetPersonTemplateMarkerLines(pTemp, targetMarkerCenters);
            args.Display.DrawLines(personTemplateMarkerLines, this.lineColor, this.defaultThickness);
            Vector3d vector3d = tagPosition2 - tagPosition;
            args.Display.DrawArrowHead(tagPosition2, vector3d, this.lineColor, num3, num2);
            Point3d point3d2 = tagPosition + vector3d * 0.6;
            args.Display.Draw2dText(pTemp.Number.ToString(), this.defaultTextColor, point3d2, true, num);
        }


        private List<Point3d> GetTargetMarkerCenters(PersonTemplate pTemp)
        {
            List<Point3d> list = new List<Point3d>();
            int count = pTemp.Interests.Count;
            bool flag = count == 0;
            List<Point3d> result;
            if (flag)
            {
                result = list;
            }
            else
            {
                Point3d tagPosition = pTemp.StartGate.TagPosition;
                Point3d tagPosition2 = pTemp.DestinationGate.TagPosition;
                Vector3d vector3d = tagPosition2 - tagPosition;
                Point3d point3d = tagPosition + vector3d * 0.5;
                vector3d.Unitize();
                Vector3d vector3d2 = vector3d * this.defaultTargetMarkerDist;
                double num = tagPosition.DistanceTo(tagPosition2);
                bool flag2 = (double)(count + 1) * this.defaultTargetMarkerDist > num;
                if (flag2)
                {
                    vector3d2 = vector3d * (num / (double)(count + 1));
                }
                Vector3d vector3d3 = -vector3d2 * (double)(count - 1) * 0.5;
                Point3d point3d2 = point3d + vector3d3;
                for (int i = 0; i < count; i++)
                {
                    list.Add(point3d2 + vector3d2 * (double)i);
                }
                result = list;
            }
            return result;
        }


        private List<Line> GetPersonTemplateMarkerLines(PersonTemplate pTemp, List<Point3d> targetCenters)
        {
            Point3d tagPosition = pTemp.StartGate.TagPosition;
            Point3d tagPosition2 = pTemp.DestinationGate.TagPosition;
            Vector3d vector3d = tagPosition2 - tagPosition;
            vector3d.Unitize();
            Vector3d vector3d2 = vector3d * this.defaultGateRadius;
            Vector3d vector3d3 = vector3d * this.defaultTargetRadius;
            List<Line> list = new List<Line>();
            int count = targetCenters.Count;
            bool flag = count > 0;
            if (flag)
            {
                Point3d point3d = tagPosition + vector3d2;
                Point3d point3d2 = targetCenters[0] - vector3d3;
                list.Add(new Line(point3d, point3d2));
                bool flag2 = count > 1;
                if (flag2)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        point3d = targetCenters[i] + vector3d3;
                        point3d2 = targetCenters[i + 1] - vector3d3;
                        list.Add(new Line(point3d, point3d2));
                    }
                }
                point3d = targetCenters[count - 1] + vector3d3;
                point3d2 = tagPosition2 - vector3d2;
                list.Add(new Line(point3d, point3d2));
            }
            else
            {
                Point3d point3d = tagPosition + vector3d2;
                Point3d point3d2 = tagPosition2 - vector3d2;
                list.Add(new Line(point3d, point3d2));
            }
            return list;
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("PersonTemplateDirectionMode", (int)this.mode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            this.Mode = (PersonTemplateComponent.PersonTemplateDirectionMode)reader.GetInt32("PersonTemplateDirectionMode");
            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem toolStripMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Original Direction", new EventHandler(this.Menu_OriginalDirClicked), true);
            toolStripMenuItem.ToolTipText = "People travel from Start to Destination.";
            ToolStripMenuItem toolStripMenuItem2 = GH_DocumentObject.Menu_AppendItem(menu, "Reversed Direction", new EventHandler(this.Menu_ReversedDirClicked), true);
            toolStripMenuItem2.ToolTipText = "People travel from Destination to Start.";
            ToolStripMenuItem toolStripMenuItem3 = GH_DocumentObject.Menu_AppendItem(menu, "Both Directions", new EventHandler(this.Menu_BothDirClicked), true);
            toolStripMenuItem3.ToolTipText = "People can travel both ways.";
        }

        private void Menu_OriginalDirClicked(object sender, EventArgs e)
        {
            base.RecordUndoEvent("PersonTemplateDirectionMode changed");
            this.Mode = PersonTemplateComponent.PersonTemplateDirectionMode.OriginalDirection;
            this.ExpireSolution(true);
        }

        private void Menu_ReversedDirClicked(object sender, EventArgs e)
        {
            base.RecordUndoEvent("PersonTemplateDirectionMode changed");
            this.Mode = PersonTemplateComponent.PersonTemplateDirectionMode.ReversedDirection;
            this.ExpireSolution(true);
        }

        private void Menu_BothDirClicked(object sender, EventArgs e)
        {
            base.RecordUndoEvent("PersonTemplateDirectionMode changed");
            this.Mode = PersonTemplateComponent.PersonTemplateDirectionMode.BothDirections;
            this.ExpireSolution(true);
        }

        protected override Bitmap Icon
        {
            get
            {
                return Resources.Icon_PersonTemplate_01;
            }
        }

        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("83C8964E-9625-4ACC-9EF3-B9797428C71C");
            }
        }


        private List<PersonTemplate> results;


        private List<PersonTemplate> results_rev;

        private PersonTemplateComponent.PersonTemplateDirectionMode mode;


        private readonly double defaultTargetRadius = 0.45;


        private readonly double defaultGateRadius = 0.45;


        private readonly Color defaultGateColor = Color.FromArgb(155, 200, 200, 200);


        private Color lineColor = Color.FromArgb(155, 100, 100, 100);


        private readonly Color defaultTextColor = Color.FromArgb(255, 180, 180, 180);


        private readonly int defaultThickness = 2;


        private readonly double defaultTargetMarkerDist = 2.0;


        private List<Gate> startGates = new List<Gate>();


        private List<Gate> destiGates = new List<Gate>();


        private List<Program> interests = new List<Program>();


        private List<int> needValues = new List<int>();


        private readonly double defaultBodyRadius = 0.35;


        private readonly double defaultProbability = 1.0;


        private readonly int defaultTimeLimit = -1;


        private readonly double defaultMass = 1.0;


        private readonly double defaultTargetForce = 20.0;


        private readonly string toolTip_matchListLength = "MATCH LIST LENGTH: Number of items in related input lists must match. ";


        private readonly string toolTip_sharedValue = "SHARED: This value is shared among all output PersonTemplates. ";


        private readonly string toolTip_gate = "Expected type: Gate. ";


        private readonly string toolTip_interests = "The Person may go to targets with these programs once they see them. ";


        private readonly string toolTip_needValues = "Number of visits needed to meet the corresponding interest.If there is one Need Value and multiple interests, the value is shared by all the interests.";


        private readonly string toolTip_startNumber = "(Optional, int) ID for the first PersonTemplate created here. ";


        private readonly string toolTip_probability = "This parameter, together with Probability of other PersonTemplates, determine the proportion of the people of each template. ";


        private readonly string toolTip_timeLimit = "The Person drops all the interests when it reaches Time Limit. Negative: no time limit. ";


        private readonly string toolTip_color = "Optional color for drawing the connection line. ";


        private readonly string toolTip_targetForce = "Larger Target Force makes a person move faster.";


        private enum PersonTemplateDirectionMode
        {

            OriginalDirection = 1,

            ReversedDirection,

            BothDirections
        }
    }
}

