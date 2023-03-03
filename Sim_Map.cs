using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PedSimulation.Geometry;
using PedSimulation.RouteGraph;

namespace PedSimulation.Simulation
{

    public class Map
    {
        // Token: 0x17000013 RID: 19
        // (get) Token: 0x0600002B RID: 43 RVA: 0x000029A7 File Offset: 0x00000BA7
        // (set) Token: 0x0600002C RID: 44 RVA: 0x000029AF File Offset: 0x00000BAF
        public List<Polygon2d> ObstaclePolylines { get; set; }

        // Token: 0x17000014 RID: 20
        // (get) Token: 0x0600002D RID: 45 RVA: 0x000029B8 File Offset: 0x00000BB8
        // (set) Token: 0x0600002E RID: 46 RVA: 0x000029C0 File Offset: 0x00000BC0
        public List<Gate> Gates { get; set; }

        // Token: 0x17000015 RID: 21
        // (get) Token: 0x0600002F RID: 47 RVA: 0x000029C9 File Offset: 0x00000BC9
        // (set) Token: 0x06000030 RID: 48 RVA: 0x000029D1 File Offset: 0x00000BD1
        public List<Target> Targets { get; set; }

        // Token: 0x17000016 RID: 22
        // (get) Token: 0x06000031 RID: 49 RVA: 0x000029DA File Offset: 0x00000BDA
        // (set) Token: 0x06000032 RID: 50 RVA: 0x000029E2 File Offset: 0x00000BE2
        public List<Person> People { get; set; }

        // Token: 0x17000017 RID: 23
        // (get) Token: 0x06000033 RID: 51 RVA: 0x000029EB File Offset: 0x00000BEB
        // (set) Token: 0x06000034 RID: 52 RVA: 0x000029F3 File Offset: 0x00000BF3
        public List<PersonTemplate> PTemplates { get; set; }

        // Token: 0x17000018 RID: 24
        // (get) Token: 0x06000035 RID: 53 RVA: 0x000029FC File Offset: 0x00000BFC
        // (set) Token: 0x06000036 RID: 54 RVA: 0x00002A04 File Offset: 0x00000C04
        public Graph CommonGraph { get; set; }

        // Token: 0x17000019 RID: 25
        // (get) Token: 0x06000037 RID: 55 RVA: 0x00002A0D File Offset: 0x00000C0D
        // (set) Token: 0x06000038 RID: 56 RVA: 0x00002A15 File Offset: 0x00000C15
        public Dictionary<Gate, GoalSpecificGraph> GateSpecGraphs { get; set; }

        // Token: 0x1700001A RID: 26
        // (get) Token: 0x06000039 RID: 57 RVA: 0x00002A1E File Offset: 0x00000C1E
        // (set) Token: 0x0600003A RID: 58 RVA: 0x00002A26 File Offset: 0x00000C26
        public int Population { get; set; }

        // Token: 0x1700001B RID: 27
        // (get) Token: 0x0600003B RID: 59 RVA: 0x00002A2F File Offset: 0x00000C2F
        // (set) Token: 0x0600003C RID: 60 RVA: 0x00002A37 File Offset: 0x00000C37
        public int FrameCount { get; set; }

        // Token: 0x1700001C RID: 28
        // (get) Token: 0x0600003D RID: 61 RVA: 0x00002A40 File Offset: 0x00000C40
        // (set) Token: 0x0600003E RID: 62 RVA: 0x00002A48 File Offset: 0x00000C48
        public double GenerationTime { get; set; }

        // Token: 0x1700001D RID: 29
        // (get) Token: 0x0600003F RID: 63 RVA: 0x00002A51 File Offset: 0x00000C51
        // (set) Token: 0x06000040 RID: 64 RVA: 0x00002A59 File Offset: 0x00000C59
        public SystemSettings Settings { get; set; }

        // Token: 0x1700001E RID: 30
        // (get) Token: 0x06000041 RID: 65 RVA: 0x00002A62 File Offset: 0x00000C62
        // (set) Token: 0x06000042 RID: 66 RVA: 0x00002A6A File Offset: 0x00000C6A
        public List<PersonRecord> PersonRecords { get; set; }

        // Token: 0x06000043 RID: 67 RVA: 0x00002A74 File Offset: 0x00000C74
        public Map(int _population, SystemSettings _settings)
        {
            this.Gates = new List<Gate>();
            this.Targets = new List<Target>();
            this.PTemplates = new List<PersonTemplate>();
            this.People = new List<Person>();
            this.ObstaclePolylines = new List<Polygon2d>();
            this.CommonGraph = new Graph();
            this.Population = _population;
            this.FrameCount = 0;
            bool flag = _settings != null;
            if (flag)
            {
                this.Settings = _settings;
            }
            else
            {
                this.Settings = new SystemSettings();
            }
            this.GateSpecGraphs = new Dictionary<Gate, GoalSpecificGraph>();
            this.PersonRecords = new List<PersonRecord>();
        }

        // Token: 0x06000044 RID: 68 RVA: 0x00002B30 File Offset: 0x00000D30
        public void Recalculate()
        {
            this.RemovePeopleReachedDestination();
            this.GeneratePeopleToFillPopulation();
            this.UpdateNeighborsMutual();
            this.UpdatePeople();
            int frameCount = this.FrameCount;
            this.FrameCount = frameCount + 1;
            this.IncrementMapObjectTimeCount();
        }

        // Token: 0x06000045 RID: 69 RVA: 0x00002B74 File Offset: 0x00000D74
        private void RemovePeopleReachedDestination()
        {
            for (int i = 0; i < this.People.Count; i++)
            {
                Person person = this.People[i];
                bool flag = person.State == PersonState.REACHED_DESTINATION;
                if (flag)
                {
                    this.RemovePerson(person);
                    i--;
                }
            }
        }

        // Token: 0x06000046 RID: 70 RVA: 0x00002BC8 File Offset: 0x00000DC8
        private void GeneratePeopleToFillPopulation()
        {
            int frameCount = this.FrameCount;
            bool flag = frameCount == 0;
            if (!flag)
            {
                int numberOfPersonMarksAtFrame = this.GetNumberOfPersonMarksAtFrame(this.FrameCount, this.GenerationTime, this.GTMark);
                this.GTMark += (double)numberOfPersonMarksAtFrame * this.GenerationTime;
                int num = 0;
                while (this.People.Count < this.Population && num < numberOfPersonMarksAtFrame)
                {
                    PersonTemplate personTemplate = this.PickFromListWithProbability(this.PTemplates);
                    bool flag2 = personTemplate == null;
                    if (flag2)
                    {
                        break;
                    }
                    Person person = this.GeneratePersonFromTemplate(personTemplate, this);
                    person.ID = this.PersonCount;
                    person.StartFrame = this.FrameCount;
                    this.People.Add(person);
                    this.PersonCount++;
                    num++;
                }
            }
        }

        // Token: 0x06000047 RID: 71 RVA: 0x00002CA4 File Offset: 0x00000EA4
        private int GetNumberOfPersonMarksAtFrame(int frameNumber, double generationTime, double startGTMark)
        {
            int num = 0;
            double num2 = startGTMark;
            while (num2 <= (double)frameNumber)
            {
                num2 += generationTime;
                num++;
            }
            return num;
        }

        // Token: 0x06000048 RID: 72 RVA: 0x00002CD4 File Offset: 0x00000ED4
        private void RemovePerson(Person p)
        {
            this.People.Remove(p);
            bool isTraceEnabled = this.Settings.IsTraceEnabled;
            if (isTraceEnabled)
            {
                int count = p.Trace.TimeInFrames.Count;
                bool flag = p.Trace.TimeInFrames[count - 1] != this.FrameCount;
                if (flag)
                {
                    p.RecordTraceDatum();
                }
                p.EndFrame = this.FrameCount;
                PersonRecord item = new PersonRecord(p);
                this.PersonRecords.Add(item);
            }
            foreach (Person person in this.People)
            {
                person.Neighbors.Remove(p);
            }
        }

        // Token: 0x06000049 RID: 73 RVA: 0x00002DB0 File Offset: 0x00000FB0
        private void UpdateNeighborsMutual()
        {
            for (int i = 0; i < this.People.Count - 1; i++)
            {
                Person person = this.People[i];
                bool flag = person.PersonFrameCount % person.NeighborUpdatingPeriod == 0;
                if (flag)
                {
                    for (int j = i + 1; j < this.People.Count; j++)
                    {
                        Person person2 = this.People[j];
                        double num = person.getPosition().distance(person2.getPosition());
                        bool flag2 = num <= Person.NeighborRadius;
                        if (flag2)
                        {
                            person.Neighbors.Add(person2);
                            person2.Neighbors.Add(person);
                        }
                        else
                        {
                            person.Neighbors.Remove(person2);
                            person2.Neighbors.Remove(person);
                        }
                    }
                }
            }
        }

        // Token: 0x0600004A RID: 74 RVA: 0x00002EA0 File Offset: 0x000010A0
        private PersonTemplate PickFromListWithProbability(List<PersonTemplate> pList)
        {
            double num = 0.0;
            for (int i = 0; i < pList.Count; i++)
            {
                bool flag = pList[i] == null;
                if (flag)
                {
                    return null;
                }
                num += pList[i].Probability;
            }
            Random random = new Random();
            double num2 = random.NextDouble() * num;
            int index = 0;
            for (int j = 0; j < pList.Count; j++)
            {
                double probability = pList[j].Probability;
                bool flag2 = num2 <= probability;
                if (flag2)
                {
                    index = j;
                    break;
                }
                num2 -= probability;
            }
            return pList[index];
        }

        // Token: 0x0600004B RID: 75 RVA: 0x00002F64 File Offset: 0x00001164
        private Person GeneratePersonFromTemplate(PersonTemplate pTemp, Map parentMap)
        {
            Gate startGate = pTemp.StartGate;
            Gate destinationGate = pTemp.DestinationGate;
            List<Program> interests = pTemp.Interests;
            List<int> needValues = pTemp.NeedValues;
            Person person = new Person(startGate.GetPosition(), parentMap, destinationGate, interests, needValues, pTemp.TimeLimit, pTemp.Number, pTemp.BodyRadius, pTemp.Mass, pTemp.TargetForce);
            bool flag = !(pTemp.Vision is PanoVision);
            if (flag)
            {
                person.Vision = pTemp.Vision;
            }
            person.StartGate = startGate;
            person.SetGoalToDestination();
            person.CurrentPath = pTemp.InitialPath;
            person.CurrentNodeIndex = 0;
            return person;
        }

        // Token: 0x0600004C RID: 76 RVA: 0x00003014 File Offset: 0x00001214
        private void UpdatePeople()
        {
            bool flag = this.People.Count == 0;
            if (!flag)
            {
                this.UpdatePeopleNeighboringObstacles_Parallel();
                this.UpdatePeopleBehaviorAndRemoveInvalid_Parallel();
                this.UpdatePeopleWaitingAtGoals();
            }
        }

        // Token: 0x0600004D RID: 77 RVA: 0x0000304B File Offset: 0x0000124B
        private void UpdatePeopleNeighboringObstacles_Parallel()
        {
            Parallel.ForEach<Person>(this.People, delegate (Person p)
            {
                bool flag = p.PersonFrameCount % p.NeighboringObstacleUpdatingPeriod == 0;
                if (flag)
                {
                    p.UpdateNeighboringObstacles();
                }
            });
        }

        // Token: 0x0600004E RID: 78 RVA: 0x0000307C File Offset: 0x0000127C
        private void UpdatePeopleBehaviorAndRemoveInvalid_Parallel()
        {
            Parallel.ForEach<Person>(this.People, delegate (Person p)
            {
                p.UpdateSelf();
            });
            for (int i = 0; i < this.People.Count; i++)
            {
                Person person = this.People[i];
                bool flag = person.State == PersonState.REACHED_DESTINATION;
                if (flag)
                {
                    this.RemovePerson(person);
                    i--;
                }
                else
                {
                    bool flag2 = !person.IsValid;
                    if (flag2)
                    {
                        this.RemovePerson(person);
                        i--;
                    }
                }
            }
        }

        // Token: 0x0600004F RID: 79 RVA: 0x00003118 File Offset: 0x00001318
        private void ClearForces()
        {
            for (int i = 0; i < this.People.Count; i++)
            {
                Person person = this.People[i];
                person.clearForce();
            }
        }

        // Token: 0x06000050 RID: 80 RVA: 0x00003158 File Offset: 0x00001358
        private void UpdatePeopleBehaviorAndRemoveInvalid()
        {
            for (int i = 0; i < this.People.Count; i++)
            {
                Person person = this.People[i];
                bool flag = person.State == PersonState.REACHED_DESTINATION;
                if (flag)
                {
                    this.RemovePerson(person);
                    i--;
                }
                else
                {
                    bool flag2 = !person.UpdateSelf();
                    if (flag2)
                    {
                        this.RemovePerson(person);
                        i--;
                    }
                }
            }
        }

        // Token: 0x06000051 RID: 81 RVA: 0x000031CC File Offset: 0x000013CC
        private void UpdatePeopleWaitingAtGoals()
        {
            foreach (Target target in this.Targets)
            {
                foreach (AccessPoint accessPoint in target.AccessPoints)
                {
                    bool flag = accessPoint.Visitors.Count > 0;
                    if (flag)
                    {
                        Person person = accessPoint.Visitors.Peek();
                        person.State = PersonState.SHOPPING;
                    }
                }
            }
        }

        // Token: 0x06000052 RID: 82 RVA: 0x00003288 File Offset: 0x00001488
        private double GetRepulsionForce_Exp(double dist, double a, double b, double max, double threshold)
        {
            double num = 0.0;
            bool flag = dist == 0.0;
            if (flag)
            {
                num = 0.0;
            }
            else
            {
                bool flag2 = dist < threshold;
                if (flag2)
                {
                    num = a * Math.Exp(-b * dist);
                }
            }
            bool flag3 = num > max;
            if (flag3)
            {
                num = max;
            }
            return num;
        }

        // Token: 0x06000053 RID: 83 RVA: 0x000032E8 File Offset: 0x000014E8
        private void IncrementMapObjectTimeCount()
        {
            foreach (Gate gate in this.Gates)
            {
                gate.IncrementTimeCounts();
            }
        }

        // Token: 0x04000014 RID: 20
        private double GTMark = 0.0;

        // Token: 0x04000015 RID: 21
        private int PersonCount = 0;
    }
}

