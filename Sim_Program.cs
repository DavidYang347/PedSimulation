using System;
using System.Drawing;

namespace PedSimulation.Simulation
{
    public class Program
    {

        public string Name { get; set; }

        public Color DisplayColor { get; set; }

        public Program()
        {
            this.DisplayColor = Program.defaultDisplayColor;
            this.Name = "Target";
        }

        public Program(string name, Color color)
        {
            this.Name = name;
            this.DisplayColor = color;
        }

        public bool IsNullProgram()
        {
            return this.Name == "" || this.DisplayColor == Color.Empty;
        }

        private static Color defaultDisplayColor = Color.DarkGray;
    }
}

