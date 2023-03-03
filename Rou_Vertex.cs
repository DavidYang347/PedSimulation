using System;
using System.Collections.Generic;
using PedSimulation.Geometry;

namespace PedSimulation.RouteGraph
{
    // Token: 0x0200001D RID: 29
    public class Vertex
    {
        // Token: 0x17000057 RID: 87
        // (get) Token: 0x06000125 RID: 293 RVA: 0x00006F12 File Offset: 0x00005112
        // (set) Token: 0x06000126 RID: 294 RVA: 0x00006F1A File Offset: 0x0000511A
        public Vec2d Position { get; set; }

        // Token: 0x17000058 RID: 88
        // (get) Token: 0x06000127 RID: 295 RVA: 0x00006F23 File Offset: 0x00005123
        // (set) Token: 0x06000128 RID: 296 RVA: 0x00006F2B File Offset: 0x0000512B
        public int Depth { get; set; }

        // Token: 0x17000059 RID: 89
        // (get) Token: 0x06000129 RID: 297 RVA: 0x00006F34 File Offset: 0x00005134
        // (set) Token: 0x0600012A RID: 298 RVA: 0x00006F3C File Offset: 0x0000513C
        public double GValue { get; set; }

        // Token: 0x1700005A RID: 90
        // (get) Token: 0x0600012B RID: 299 RVA: 0x00006F45 File Offset: 0x00005145
        // (set) Token: 0x0600012C RID: 300 RVA: 0x00006F4D File Offset: 0x0000514D
        public bool IsOptimal { get; set; }

        // Token: 0x0600012D RID: 301 RVA: 0x00006F56 File Offset: 0x00005156
        public Vertex(Vec2d _position, Vertex _parent, int _depth)
        {
            this.Position = _position;
            this.Parent = _parent;
            this.Depth = _depth;
            this.Connections = new List<Edge>();
            this.CurrentChildren = new List<Vertex>();
            this.IsOptimal = false;
        }

        // Token: 0x0600012E RID: 302 RVA: 0x00006F98 File Offset: 0x00005198
        public Vertex(Vertex u)
        {
            this.Position = u.Position;
            this.Parent = null;
            this.Depth = 0;
            this.Connections = new List<Edge>();
            this.CurrentChildren = new List<Vertex>();
            this.IsOptimal = false;
        }

        // Token: 0x0600012F RID: 303 RVA: 0x00006FE8 File Offset: 0x000051E8
        public Path TracePath()
        {
            Vertex vertex = this;
            List<Vertex> list = new List<Vertex>();
            list.Add(vertex);
            while (vertex.Parent != null)
            {
                Vertex parent = vertex.Parent;
                list.Add(parent);
                vertex = parent;
            }
            return new Path(list);
        }

        // Token: 0x04000090 RID: 144
        public List<Edge> Connections;

        // Token: 0x04000093 RID: 147
        public Vertex Parent;

        // Token: 0x04000094 RID: 148
        public List<Vertex> CurrentChildren;
    }
}

