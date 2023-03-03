using System;
using System.Collections.Generic;

namespace PedSimulation.RouteGraph
{

    public class Path
    {

        public Path()
        {
            this.nodes = new List<Vertex>();
        }


        public Path(List<Vertex> _nodes)
        {
            this.nodes = _nodes;
        }

        public int getCount()
        {
            bool flag = this.nodes == null;
            int result;
            if (flag)
            {
                result = 0;
            }
            else
            {
                result = this.nodes.Count;
            }
            return result;
        }

        public double getLength()
        {
            double num = 0.0;
            bool flag = this.getCount() <= 1;
            double result;
            if (flag)
            {
                result = 0.0;
            }
            else
            {
                for (int i = 1; i < this.getCount(); i++)
                {
                    double num2 = this.nodes[i].Position.distance(this.nodes[i - 1].Position);
                    num += num2;
                }
                result = num;
            }
            return result;
        }

        public List<Vertex> nodes;
    }
}

