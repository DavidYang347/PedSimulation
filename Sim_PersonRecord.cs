using System;

namespace PedSimulation.Simulation
{

    public class PersonRecord
    {

        public PersonRecord(Person p)
        {
            this.ID = p.ID;
            this.TemplateID = p.TemplateID;
            this.StartFrame = p.StartFrame;
            this.EndFrame = p.EndFrame;
            this.Trace = p.Trace;
        }


        public PersonTrace Trace { get; set; }

        public int ID;

        public int TemplateID;

        public int StartFrame;

        public int EndFrame;
    }
}

