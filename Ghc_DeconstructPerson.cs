using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Grasshopper.Kernel;
using PedSimulation.Geometry;
using PedSimulation.Properties;
using PedSimulation.RouteGraph;
using PedSimulation.Simulation;
using Rhino.Geometry;
using Path = PedSimulation.RouteGraph.Path;

namespace PedSimulation.GHComponents
{

    public class DeconstructPersonComponent : GH_Component
    {

        public DeconstructPersonComponent() : base("DeconstructPerson", "DeconsP", "V0.12.3 2019-06-23", "PedSim", "PostSim")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Person", "P", "", 0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("ID", "ID", "", 0);
            pManager.AddIntegerParameter("Template ID", "TemplateID", "", 0);
            pManager.AddPointParameter("Position", "Pos", "", 0);
            pManager.AddNumberParameter("BodyRadius", "R", "", 0);
            pManager.AddVectorParameter("Velocity", "V", "", 0);
            pManager.AddVectorParameter("Acceleration", "A", "", 0);
            pManager.AddCurveParameter("CurrentPath", "Path", "", 0);
            pManager.AddTextParameter("Behaviors", "B", "", GH_ParamAccess.list);
            pManager.AddVectorParameter("Forces", "F", "", GH_ParamAccess.list);
            pManager.AddTextParameter("MovementState", "S", "", 0);
            pManager.AddTextParameter("Interests", "Intrsts", "", GH_ParamAccess.list);
            pManager.AddPointParameter("Trace Positions", "TracePos", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Trace Speeds", "TraceSpd", "", GH_ParamAccess.list);

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Person person = null;
            bool flag = !DA.GetData<Person>(0, ref person);
            if (!flag)
            {
                bool flag2 = person == null;
                if (!flag2)
                {
                    Vec2d position = person.getPosition();
                    Vec2d velocity = person.getVelocity();
                    Vec2d acceleration = person.getAcceleration();
                    Point3d point3d = new Point3d(position.X, position.Y, 0.0);
                    //point3d..ctor(position.X, position.Y, 0.0);
                    Vector3d vector3d = new Vector3d(velocity.X, velocity.Y, 0.0);
                    //vector3d..ctor(velocity.X, velocity.Y, 0.0);
                    Vector3d vector3d2 = new Vector3d(acceleration.X, acceleration.Y, 0.0);
                    //vector3d2..ctor(acceleration.X, acceleration.Y, 0.0);
                    DA.SetData("ID", person.ID);
                    DA.SetData("Template ID", person.TemplateID);
                    DA.SetData("Position", point3d);
                    DA.SetData("BodyRadius", person.BodyRadius);
                    DA.SetData("Velocity", vector3d);
                    DA.SetData("Acceleration", vector3d2);
                    DA.SetDataList("Behaviors", person.BehaviorNames);
                    DA.SetDataList("Forces", person.Forces);
                    DA.SetData("CurrentPath", this.GetCurrentPath(person));
                    DA.SetData("MovementState", person.State.ToString());
                    DA.SetData("Interests", person.InterestsToString());
                    DA.SetDataList("Trace Positions", person.Trace.Position);
                    DA.SetDataList("Trace Speeds", person.Trace.Speed);
                }
            }
        }


        private Curve GetCurrentPath(Person person)
        {
            Path currentPath = person.CurrentPath;
            bool flag = currentPath == null;
            Curve result;
            if (flag)
            {
                result = null;
            }
            else
            {
                List<Point3d> list = new List<Point3d>();
                foreach (Vertex vertex in currentPath.nodes)
                {
                    Vec2d position = vertex.Position;
                    list.Add(new Point3d(position.X, position.Y, 0.0));
                }
                PolylineCurve polylineCurve = new PolylineCurve(list);
                result = polylineCurve;
            }
            return result;
        }

        protected override Bitmap Icon
        {
            get
            {
                return Resources.Icon_DeconstructPerson_01;
            }
        }

        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("08DC6035-6BBE-4AC6-AEC6-DE0FB4818757");
            }
        }
    }
}
