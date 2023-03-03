using System;

namespace PedSimulation.RouteGraph
{
    // Token: 0x02000019 RID: 25
    public class GoalSpecificGraph : Graph
    {
        // Token: 0x17000054 RID: 84
        // (get) Token: 0x06000115 RID: 277 RVA: 0x000064FC File Offset: 0x000046FC
        // (set) Token: 0x06000116 RID: 278 RVA: 0x00006504 File Offset: 0x00004704
        public Vertex GoalVertex { get; set; }

        // Token: 0x06000117 RID: 279 RVA: 0x0000650D File Offset: 0x0000470D
        public GoalSpecificGraph(Graph g, Vertex _goalVertex) : base(g)
        {
            this.GoalVertex = _goalVertex;
        }
    }
}

