using System;
using System.Collections.Generic;
using PedSimulation.Geometry;

namespace PedSimulation.RouteGraph
{

    public static class GraphGenerator
    {

        public static Graph GenerateEmptyGraph()
        {
            return new Graph();
        }


        public static Graph GenerateGraphFromObstacles(List<Polygon2d> obstacles, double obsOffset)
        {
            List<Polygon2d> list = new List<Polygon2d>();
            for (int i = 0; i < obstacles.Count; i++)
            {
                Polygon2d polygon2d = obstacles[i];
                Polygon2d item = polygon2d.Offset(obsOffset);
                list.Add(item);
            }
            List<Vertex> list2 = new List<Vertex>();
            for (int j = 0; j < list.Count; j++)
            {
                Polygon2d polygon2d2 = list[j];
                for (int k = 0; k < polygon2d2.Points.Count; k++)
                {
                    Vertex item2 = new Vertex(polygon2d2.Points[k], null, 0);
                    list2.Add(item2);
                }
            }
            Graph graph = new Graph();
            foreach (Vertex v in list2)
            {
                graph.AddVertex(v);
            }
            int count = list2.Count;
            for (int l = 0; l < count - 1; l++)
            {
                Vertex vertex = list2[l];
                for (int m = l + 1; m < count; m++)
                {
                    Vertex vertex2 = list2[m];
                    bool flag = RhinoGeoMethods.IsPositionVisible(vertex.Position, vertex2.Position, obstacles);
                    if (flag)
                    {
                        Edge e = new Edge(vertex, vertex2);
                        graph.addEdge_UpdateConnections(e);
                    }
                }
            }
            foreach (Vertex vertex3 in graph.Vertices)
            {
                List<Vertex> list3 = new List<Vertex>();
                foreach (Edge edge in vertex3.Connections)
                {
                    Vertex theOtherVertex = edge.GetTheOtherVertex(vertex3);
                    list3.Add(theOtherVertex);
                }
                vertex3.CurrentChildren = list3;
            }
            return graph;
        }

        public static GoalSpecificGraph GenerateGoalSpecificGraph(Graph graph, Vec2d goalPos, List<Polygon2d> obstacles)
        {
            Queue<Vertex> queue = new Queue<Vertex>();
            HashSet<Vertex> hashSet = new HashSet<Vertex>();
            Vertex vertex = new Vertex(goalPos, null, 0);
            vertex.GValue = 0.0;
            GoalSpecificGraph goalSpecificGraph = new GoalSpecificGraph(graph, vertex);
            goalSpecificGraph.AddVertex(vertex);
            vertex.IsOptimal = true;
            for (int i = 0; i < goalSpecificGraph.Vertices.Count - 1; i++)
            {
                Vertex vertex2 = goalSpecificGraph.Vertices[i];
                vertex2.GValue = double.MaxValue;
                bool flag = RhinoGeoMethods.IsPositionVisible(goalPos, vertex2.Position, obstacles);
                if (flag)
                {
                    Edge e = new Edge(vertex, vertex2);
                    goalSpecificGraph.addEdge_UpdateConnections(e);
                    vertex2.Parent = vertex;
                    vertex.CurrentChildren.Add(vertex2);
                    queue.Enqueue(vertex2);
                    vertex2.GValue = vertex2.Position.distance(vertex.Position);
                    vertex2.IsOptimal = true;
                }
            }
            while (queue.Count > 0)
            {
                Vertex vertex3 = queue.Dequeue();
                hashSet.Add(vertex3);
                foreach (Vertex vertex4 in vertex3.CurrentChildren)
                {
                    bool flag2 = !hashSet.Contains(vertex4);
                    if (flag2)
                    {
                        double num = vertex3.Position.distance(vertex4.Position);
                        bool flag3 = vertex3.GValue + num < vertex4.GValue;
                        if (flag3)
                        {
                            vertex4.Parent = vertex3;
                            vertex4.GValue = vertex3.GValue + num;
                            vertex4.Depth = vertex3.Depth + 1;
                            queue.Enqueue(vertex4);
                        }
                    }
                }
            }
            int num2 = 10000;
            int j = 0;
            while (j < num2)
            {
                bool flag4 = true;
                foreach (Vertex vertex5 in goalSpecificGraph.Vertices)
                {
                    bool isOptimal = vertex5.IsOptimal;
                    if (!isOptimal)
                    {
                        bool flag5 = false;
                        foreach (Vertex vertex6 in vertex5.CurrentChildren)
                        {
                            double num3 = vertex5.Position.distance(vertex6.Position);
                            bool flag6 = vertex5.GValue > vertex6.GValue + num3;
                            if (flag6)
                            {
                                vertex5.Parent = vertex6;
                                vertex5.GValue = vertex6.GValue + num3;
                                vertex5.Depth = vertex6.Depth + 1;
                                flag5 = true;
                            }
                            j++;
                        }
                        bool flag7 = !flag5;
                        if (flag7)
                        {
                            vertex5.IsOptimal = true;
                        }
                        bool flag8 = !vertex5.IsOptimal;
                        if (flag8)
                        {
                            flag4 = false;
                        }
                    }
                }
                bool flag9 = flag4;
                if (flag9)
                {
                    break;
                }
            }
            return goalSpecificGraph;
        }
    }
}

