using System;
using System.Collections.Generic;

namespace PedSimulation.RouteGraph
{
    // Token: 0x0200001A RID: 26
    public class Graph
    {
        // Token: 0x06000118 RID: 280 RVA: 0x00006520 File Offset: 0x00004720
        public Graph()
        {
            this.vertices = new List<Vertex>();
            this.edges = new List<Edge>();
        }

        // Token: 0x06000119 RID: 281 RVA: 0x00006540 File Offset: 0x00004740
        public Graph(Graph original)
        {
            this.vertices = new List<Vertex>();
            this.edges = new List<Edge>();
            foreach (Vertex u in original.Vertices)
            {
                Vertex item = new Vertex(u);
                this.Vertices.Add(item);
            }
            foreach (Edge edge in original.Edges)
            {
                Vertex a = edge.A;
                Vertex b = edge.B;
                int index = original.vertices.IndexOf(a);
                int index2 = original.vertices.IndexOf(b);
                Vertex v = this.vertices[index];
                Vertex v2 = this.vertices[index2];
                Edge e = new Edge(v, v2);
                this.addEdge_UpdateConnections(e);
            }
            foreach (Vertex vertex in this.vertices)
            {
                List<Vertex> list = new List<Vertex>();
                foreach (Edge edge2 in vertex.Connections)
                {
                    Vertex theOtherVertex = edge2.GetTheOtherVertex(vertex);
                    list.Add(theOtherVertex);
                }
                vertex.CurrentChildren = list;
            }
        }

        // Token: 0x17000055 RID: 85
        // (get) Token: 0x0600011A RID: 282 RVA: 0x0000670C File Offset: 0x0000490C
        public List<Vertex> Vertices
        {
            get
            {
                return this.vertices;
            }
        }

        // Token: 0x17000056 RID: 86
        // (get) Token: 0x0600011B RID: 283 RVA: 0x00006724 File Offset: 0x00004924
        public List<Edge> Edges
        {
            get
            {
                return this.edges;
            }
        }

        // Token: 0x0600011C RID: 284 RVA: 0x0000673C File Offset: 0x0000493C
        public bool AddVertex(Vertex v)
        {
            bool flag = !this.vertices.Contains(v);
            bool result;
            if (flag)
            {
                this.vertices.Add(v);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600011D RID: 285 RVA: 0x00006774 File Offset: 0x00004974
        public bool addEdge_UpdateConnections(Edge e)
        {
            bool flag = this.edges.Contains(e);
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                this.edges.Add(e);
                Vertex a = e.A;
                Vertex b = e.B;
                bool flag2 = false;
                bool flag3 = false;
                foreach (Edge edge in a.Connections)
                {
                    bool flag4 = edge.linkSameVertices(e);
                    if (flag4)
                    {
                        flag2 = true;
                    }
                }
                foreach (Edge edge2 in b.Connections)
                {
                    bool flag5 = edge2.linkSameVertices(e);
                    if (flag5)
                    {
                        flag3 = true;
                    }
                }
                bool flag6 = !flag2;
                if (flag6)
                {
                    a.Connections.Add(e);
                }
                bool flag7 = !flag3;
                if (flag7)
                {
                    b.Connections.Add(e);
                }
                result = true;
            }
            return result;
        }

        // Token: 0x0400008D RID: 141
        private List<Vertex> vertices;

        // Token: 0x0400008E RID: 142
        private List<Edge> edges;
    }
}

