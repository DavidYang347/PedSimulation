using System;

namespace PedSimulation.RouteGraph
{

    public class Edge
    {

        public Edge(Vertex v0, Vertex v1)
        {
            this.A = v0;
            this.B = v1;
        }


        public Vertex GetTheOtherVertex(Vertex v)
        {
            bool flag = v == this.A;
            Vertex result;
            if (flag)
            {
                result = this.B;
            }
            else
            {
                bool flag2 = v == this.B;
                if (flag2)
                {
                    result = this.A;
                }
                else
                {
                    result = null;
                }
            }
            return result;
        }


        public bool linkSameVertices(Edge e)
        {
            bool flag = this.A == null || this.B == null || e.A == null || e.B == null;
            return !flag && ((this.A == e.A && this.B == e.B) || (this.A == e.B && this.B == e.A));
        }


        public override bool Equals(object obj)
        {
            Edge edge = obj as Edge;
            bool flag = edge == null;
            return !flag && this.linkSameVertices(edge);
        }


        public override int GetHashCode()
        {
            int num = 17;
            num = num * 23 + this.A.GetHashCode();
            return num * 23 + this.B.GetHashCode();
        }


        public Vertex A;


        public Vertex B;
    }
}

