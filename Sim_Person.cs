using System;
using System.Collections.Generic;
using PedSimulation.Geometry;
using PedSimulation.RouteGraph;
using PedSimulation.Simulation.ForceBehaviors;
using Rhino.Geometry;
using Particle = PedSimulation.Geometry.Particle;//添加的代码
using System.Drawing;

namespace PedSimulation.Simulation
{
	public class Person : Particle
	{
		public Person(Vec2d position, Map _parentMap, Gate _destination, List<Program> _interests, List<int> _needValues, int _timeLimit, int _number, double _bodyRadius, double _mass, double _targetForce) : base(position, _mass, _parentMap.Settings.DampingRatio, _parentMap.Settings.FrictionRatio)
		{
			this.ParentMap = _parentMap;
			this.Destination = _destination;
			this.TimeLimit = _timeLimit;
			this.Interests = _interests;
			this.NeedValues = _needValues;
			this.TemplateID = _number;
			this.BodyRadius = _bodyRadius;
			this.TargetForce = _targetForce;
			this.Vision = new PanoVision();
			this.Neighbors = new HashSet<Person>();
			this.NeighboringObstacles = new HashSet<Polygon2d>();
			this.VisitedTargets = new List<Goal>();
			this.NeedSatisfied = new List<int>();
			for (int i = 0; i < this.NeedValues.Count; i++)
			{
				this.NeedSatisfied.Add(0);
			}
			this.State = PersonState.MOVING_AND_LOOKING;
			this.Trace = new PersonTrace();
			this.RecentSpeedRec = new Queue<double>();
		}
		
		public double BodyRadius { get; set; }


        public Vec2d CurrentWallDir { get; set; }

		public Gate StartGate { get; set; }


		public Gate Destination { get; set; }

		public Path CurrentPath { get; set; }

		public Goal CurrentGoal { get; set; }

		public AccessPoint CurrentAccessPoint { get; set; }

		public List<Goal> VisitedTargets { get; set; }

		public int CurrentNodeIndex { get; set; }

		public bool IsOutOfTime
		{
			get
			{
				bool flag = this.TimeLimit < 0;
				return !flag && this.PersonFrameCount >= this.TimeLimit;
			}
		}

		public Vertex CurrentNode
		{
			get
			{
				bool flag = this.CurrentPath == null || this.CurrentPath.nodes.Count == 0;
				Vertex result;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool flag2 = this.CurrentNodeIndex >= this.CurrentPath.nodes.Count;
					if (flag2)
					{
						result = null;
					}
					else
					{
						result = this.CurrentPath.nodes[this.CurrentNodeIndex];
					}
				}
				return result;
			}
		}

		public Vec2d CurrentPlanningForce { get; set; }

		public Vec2d CurrentRepulsionForce { get; set; }

		public List<Program> Interests { get; set; }

		public List<int> NeedValues { get; set; }

		public List<int> NeedSatisfied { get; set; }

		public PersonVision Vision { get; set; }

		public PersonTrace Trace { get; set; }

		public List<Polyline> FOV { get; set; }

		public List<Curve> FOVCurve { get; set; }

		public Point3d GetRhinoPoint
		{
			get
			{
				return new Point3d(base.getPosition().X, base.getPosition().Y, 0.0);
			}
		}

		public HashSet<Person> Neighbors { get; set; }

		public HashSet<Polygon2d> NeighboringObstacles { get; set; }

		public bool UpdateSelf()
		{
			base.clearForce();
			this.UpdateMovementState();
			bool flag = this.State == PersonState.MOVING_AND_LOOKING;
			if (flag)
			{
				this.UpdateFOV();
				try
				{
					this.UpdateGoalAndPath();
					this.UpdateCurrentNodeIndex();
				}
				catch (NullReferenceException)
				{
					this.IsValid = false;
				}
			}
			else
			{
				bool flag2 = this.State == PersonState.STUCK;
				if (flag2)
				{
					this.SetGoalToDestination();
					this.PlanPathTowardsDestination();
				}
			}
			this.behaviors = this.ActivateBehaviors();
			this.BehaviorNames = new List<string>();
			foreach (ForceBehavior forceBehavior in this.behaviors)
			{
				this.BehaviorNames.Add(forceBehavior.ToString());
			}
			this.CurrentWallDir = null;
			List<Vec2d> list = this.RunBehaviors(this.behaviors);
			this.Forces = new List<Vector3d>();
			foreach (Vec2d vec2d in list)
			{
				base.applyForce(vec2d);
				this.Forces.Add(this.ConvertToVector3d(vec2d));
			}
			base.updateAcceleration();
			base.updateVAndP(this.ParentMap.Settings.Dt, this.CurrentWallDir);
			bool flag3 = this.PersonFrameCount % this.TraceRecordingPeriod == 0 && this.ParentMap.Settings.IsTraceEnabled;
			if (flag3)
			{
				this.RecordTraceDatum();
			}
			bool flag4 = this.State == PersonState.MOVING || this.State == PersonState.MOVING_AND_LOOKING;
			if (flag4)
			{
				this.KeepRecentSpeedRecord();
			}
			else
			{
				this.ClearRecentSpeedRecord();
			}
			this.PersonFrameCount++;
			return true;
		}

		public void UpdateNeighboringObstacles()
		{
			foreach (Polygon2d polygon2d in this.ParentMap.ObstaclePolylines)
			{
				double num = double.MaxValue;
				Vec2d position = base.getPosition();
				for (int i = 0; i < polygon2d.Points.Count; i++)
				{
					Vec2d vec2d = polygon2d.Points[i];
					double num2 = position.distance(vec2d);
					bool flag = num2 <= num;
					if (flag)
					{
						num = num2;
					}
					bool flag2 = i == polygon2d.Points.Count - 1;
					Vec2d b;
					if (flag2)
					{
						b = polygon2d.Points[0];
					}
					else
					{
						b = polygon2d.Points[i + 1];
					}
					Vec2d vec2d2;
					double num3 = Person.LineSegmentDistanceToPoint(vec2d, b, position, out vec2d2);
					bool flag3 = num3 < num;
					if (flag3)
					{
						num = num3;
					}
				}
				bool flag4 = num <= Person.ObstacleMaxDistance;
				if (flag4)
				{
					this.NeighboringObstacles.Add(polygon2d);
				}
				else
				{
					bool flag5 = this.NeighboringObstacles.Contains(polygon2d);
					if (flag5)
					{
						this.NeighboringObstacles.Remove(polygon2d);
					}
				}
			}
		}

		private static double LineSegmentDistanceToPoint(Vec2d a, Vec2d b, Vec2d pt, out Vec2d proj)
		{
			proj = Person.PointProjectionOnLine(pt, a, b);
			double num = proj.distance(a);
			double num2 = proj.distance(b);
			double num3 = a.distance(b);
			bool flag = num + num2 - num3 * 1.001 > 0.0;
			double result;
			if (flag)
			{
				double num4 = pt.distance(a);
				double num5 = pt.distance(b);
				result = ((num4 < num5) ? num4 : num5);
			}
			else
			{
				double num6 = proj.distance(pt);
				result = num6;
			}
			return result;
		}

		private static Vec2d PointProjectionOnLine(Vec2d p, Vec2d a, Vec2d b)
		{
			Vec2d vec2d = p.subtract(a);
			Vec2d vec2d2 = b.subtract(a);
			double num = vec2d.magnitude();
			double num2 = vec2d2.magnitude();
			bool flag = num == 0.0;
			Vec2d result;
			if (flag)
			{
				result = a;
			}
			else
			{
				bool flag2 = num2 == 0.0;
				if (flag2)
				{
					result = a;
				}
				else
				{
					double num3 = vec2d2.dotProduct(vec2d) / num2 / num;
					Vec2d vec2d3 = vec2d2.normalize().multiply(num * num3);
					result = vec2d3.add(a);
				}
			}
			return result;
		}

		public void RecordTraceDatum()
		{
			this.Trace.TimeInFrames.Add(this.ParentMap.FrameCount);
			this.Trace.Position.Add(this.GetRhinoPoint);
			this.Trace.Speed.Add(base.getSpeed());
			this.Trace.State.Add(this.State);
		}

		private void KeepRecentSpeedRecord()
		{
			bool flag = this.RecentSpeedRec.Count >= this.RecentSpeedRecLength;
			if (flag)
			{
				this.RecentSpeedRec.Dequeue();
			}
			this.RecentSpeedRec.Enqueue(base.getSpeed());
		}

		private void ClearRecentSpeedRecord()
		{
			this.RecentSpeedRec.Clear();
		}

		private void UpdateGoalAndPath()
		{
			bool flag = this.CurrentGoal == null;
			if (flag)
			{
				throw new NullReferenceException("UpdateGoalAndPlanPath: CurrentGoal cannot be determined.");
			}
			bool flag2 = !this.IsOutOfTime && this.CurrentGoal is Gate;
			if (flag2)
			{
				List<Target> leastOccupiedVisibleInterestingTargets = this.GetLeastOccupiedVisibleInterestingTargets(this.ParentMap);
				bool flag3 = leastOccupiedVisibleInterestingTargets.Count > 0;
				if (flag3)
				{
					this.CurrentGoal = this.GetRandomItem<Target>(leastOccupiedVisibleInterestingTargets);
					this.CurrentAccessPoint = this.ChooseAccessPoint(this.CurrentGoal);
					this.CurrentPath = this.CreateLinePath(base.getPosition(), this.CurrentAccessPoint.Position);
					this.CurrentNodeIndex = 0;
				}
			}
			bool isOutOfTime = this.IsOutOfTime;
			if (isOutOfTime)
			{
				this.SetGoalToDestination();
				this.PlanPathTowardsDestination();
			}
		}

		private void UpdateMovementState()
		{
			bool flag = this.State == PersonState.SHOPPING;
			if (flag)
			{
				this.ShoppingFrameCount++;
				bool flag2 = (double)this.ShoppingFrameCount >= this.CurrentGoal.VisitDuration;
				if (flag2)
				{
					this.CurrentAccessPoint.DequeueVisitor();
					this.CurrentGoal.AddVisitCount();
					this.FinishShoppingUpdateSelfState();
				}
			}
			else
			{
				bool flag3 = this.State == PersonState.MOVING || this.State == PersonState.MOVING_AND_LOOKING;
				if (flag3)
				{
					bool flag4 = this.HasReachedGoal();
					if (flag4)
					{
						bool flag5 = this.CurrentGoal == this.Destination;
						if (flag5)
						{
							Gate gate = (Gate)this.CurrentGoal;
							bool flag6 = gate.CheckExitTime();
							if (flag6)
							{
								this.State = PersonState.REACHED_DESTINATION;
								gate.AddVisitCount();
								gate.StartExitCounter();
							}
							else
							{
								this.State = PersonState.WAITING;
							}
						}
						else
						{
							Target target = (Target)this.CurrentGoal;
							bool flag7 = this.CurrentAccessPoint.ParentGoal != target;
							if (flag7)
							{
								throw new Exception("UpdateMovementState(): CurrentAccessPoint and currentTarget do not match.");
							}
							bool flag8 = this.CurrentAccessPoint.QueueLength < 1;
							if (flag8)
							{
								this.State = PersonState.SHOPPING;
							}
							else
							{
								this.State = PersonState.WAITING;
							}
							this.CurrentAccessPoint.AddVisitor(this);
						}
					}
					else
					{
						bool flag9 = this.IsStuck();
						if (flag9)
						{
							this.State = PersonState.STUCK;
						}
						else
						{
							bool flag10 = this.PersonFrameCount % this.LookingPeriod == 0;
							if (flag10)
							{
								this.State = PersonState.MOVING_AND_LOOKING;
							}
							else
							{
								this.State = PersonState.MOVING;
							}
						}
					}
				}
				else
				{
					bool flag11 = this.State == PersonState.WAITING;
					if (flag11)
					{
						bool flag12 = this.HasReachedGoal();
						if (flag12)
						{
							bool flag13 = this.CurrentGoal == this.Destination;
							if (flag13)
							{
								Gate gate2 = (Gate)this.CurrentGoal;
								bool flag14 = gate2.CheckExitTime();
								if (flag14)
								{
									this.State = PersonState.REACHED_DESTINATION;
									gate2.AddVisitCount();
									gate2.StartExitCounter();
								}
							}
							else
							{
								Target target2 = (Target)this.CurrentGoal;
								bool flag15 = this.CurrentAccessPoint.ParentGoal != target2;
								if (flag15)
								{
									throw new Exception("UpdateMovementState(): CurrentAccessPoint and currentTarget do not match.");
								}
								bool flag16 = this.CurrentAccessPoint.QueueLength <= 1;
								if (flag16)
								{
									this.State = PersonState.SHOPPING;
								}
							}
						}
					}
					else
					{
						bool flag17 = this.State == PersonState.STUCK;
						if (flag17)
						{
							bool flag18 = this.PersonFrameCount % this.LookingPeriod == 0;
							if (flag18)
							{
								this.State = PersonState.MOVING_AND_LOOKING;
							}
							else
							{
								this.State = PersonState.MOVING;
							}
						}
					}
				}
			}
		}

		private bool IsStuck()
		{
			bool flag = this.RecentSpeedRec.Count < this.RecentSpeedRecLength;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				double num = this.AverageNumber(this.RecentSpeedRec);
				result = (num < this.StuckSpeed);
			}
			return result;
		}

		private double AverageNumber(Queue<double> numbers)
		{
			bool flag = numbers.Count == 0;
			double result;
			if (flag)
			{
				result = 0.0;
			}
			else
			{
				double num = 0.0;
				foreach (double num2 in numbers)
				{
					num += num2;
				}
				num /= (double)numbers.Count;
				result = num;
			}
			return result;
		}

		private List<ForceBehavior> ActivateBehaviors()
		{
			List<ForceBehavior> list = new List<ForceBehavior>();
			bool flag = this.State == PersonState.MOVING || this.State == PersonState.MOVING_AND_LOOKING;
			if (flag)
			{
				list.Add(new TargetForceBehavior(this, this.ParentMap));
				list.Add(new ObstacleCollisionAvoidance(this, this.ParentMap));
				list.Add(new AnticipatoryCollisionAvoidance(this, this.ParentMap));
				list.Add(new PassiveCollisionAvoidance(this, this.ParentMap));
			}
			else
			{
				bool flag2 = this.State == PersonState.SHOPPING || this.State == PersonState.WAITING;
				if (flag2)
				{
					list.Add(new MaintainClosenessToGoalBehavior(this, this.ParentMap));
					list.Add(new ObstacleCollisionAvoidance(this, this.ParentMap));
					list.Add(new PassiveCollisionAvoidance(this, this.ParentMap));
				}
			}
			return list;
		}

		private List<Vec2d> RunBehaviors(List<ForceBehavior> behaviors)
		{
			List<Vec2d> list = new List<Vec2d>();
			foreach (ForceBehavior forceBehavior in behaviors)
			{
				list.Add(forceBehavior.CalculateForce());
			}
			return list;
		}

		public void UpdateCurrentNodeIndex()
		{
			bool flag = this.CurrentNodeIndex == this.CurrentPath.nodes.Count - 1;
			if (!flag)
			{
				Vertex vertex = null;
				bool flag2 = this.CurrentNodeIndex < this.CurrentPath.nodes.Count - 1;
				if (flag2)
				{
					vertex = this.CurrentPath.nodes[this.CurrentNodeIndex + 1];
				}
				bool flag3 = vertex == null;
				if (flag3)
				{
					throw new Exception("next node is null");
				}
				bool flag4 = base.getPosition().distance(this.CurrentNode.Position) < this.nodePositionError || (vertex != null && RhinoGeoMethods.IsPositionVisible(base.getPosition(), vertex.Position, this.ParentMap.ObstaclePolylines));
				if (flag4)
				{
					int currentNodeIndex = this.CurrentNodeIndex;
					this.CurrentNodeIndex = currentNodeIndex + 1;
				}
			}
		}

		public void FinishShoppingUpdateSelfState()
		{
			Target target = (Target)this.CurrentGoal;
			this.ShoppingFrameCount = 0;
			this.VisitedTargets.Add(this.CurrentGoal);
			this.State = PersonState.MOVING_AND_LOOKING;
			int num = this.Interests.IndexOf(target.TargetProgram);
			List<int> needSatisfied = this.NeedSatisfied;
			int index = num;
			int num2 = needSatisfied[index];
			needSatisfied[index] = num2 + 1;
			this.SetGoalToDestination();
			this.PlanPathTowardsDestination();
		}

		public List<Target> GetLeastOccupiedVisibleInterestingTargets(Map map)
		{
			List<Target> list = new List<Target>();
			int num = int.MaxValue;
			Point3d getRhinoPoint = this.GetRhinoPoint;
			foreach (Target target in map.Targets)
			{
				bool flag = this.VisitedTargets.Contains(target);
				if (!flag)
				{
					bool flag2 = !this.Interests.Contains(target.TargetProgram);
					if (!flag2)
					{
						bool flag3 = this.IsInterestatisfied(target.TargetProgram);
						if (!flag3)
						{
							Point3d tagPosition = target.TagPosition;
							bool flag4 = this.FOV == null || this.FOV.Count == 0;
							if (flag4)
							{
								bool flag5 = RhinoGeoMethods.IsPositionVisible(getRhinoPoint, tagPosition, map.ObstaclePolylines) && this.Interests.Contains(target.TargetProgram);
								if (flag5)
								{
									int minQueueLength = target.MinQueueLength;
									bool flag6 = minQueueLength < num;
									if (flag6)
									{
										num = minQueueLength;
										list.Clear();
										list.Add(target);
									}
									else
									{
										bool flag7 = minQueueLength == num;
										if (flag7)
										{
											list.Add(target);
										}
									}
								}
							}
							else
							{
								bool flag8 = this.Interests.Contains(target.TargetProgram);
								if (flag8)
								{
									bool flag9 = false;
									foreach (Curve curve in this.FOVCurve)
									{
										PointContainment pointContainment = curve.Contains(tagPosition);
										bool flag10 = ((int)pointContainment == 1);
                                        //bool flag10 = (pointContainment == 1);
                                        if (flag10)
										{
											flag9 = true;
											break;
										}
									}
									bool flag11 = flag9;
									if (flag11)
									{
										int minQueueLength2 = target.MinQueueLength;
										bool flag12 = minQueueLength2 < num;
										if (flag12)
										{
											num = minQueueLength2;
											list.Clear();
											list.Add(target);
										}
										else
										{
											bool flag13 = minQueueLength2 == num;
											if (flag13)
											{
												list.Add(target);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return list;
		}

		private bool IsInterestatisfied(Program interest)
		{
			int num = this.Interests.IndexOf(interest);
			bool flag = num < 0;
			return !flag && this.NeedSatisfied[num] >= this.NeedValues[num];
		}

		public string InterestsToString()
		{
			string text = "";
			for (int i = 0; i < this.Interests.Count; i++)
			{
				text = string.Concat(new object[]
				{
					text,
					this.Interests[i].Name,
					" ",
					this.NeedSatisfied[i],
					"/",
					this.NeedValues[i],
					"\n"
				});
			}
			return text;
		}

		public void SetGoalToDestination()
		{
			this.CurrentGoal = this.Destination;
			this.CurrentAccessPoint = this.Destination.DefaultAP;
		}

		private AccessPoint ChooseAccessPoint(Goal goal)
		{
			bool flag = goal == null;
			AccessPoint result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Target target = goal as Target;
				bool flag2 = target != null;
				if (flag2)
				{
					List<AccessPoint> leastOccupiedAP = target.GetLeastOccupiedAP();
					result = this.GetRandomItem<AccessPoint>(leastOccupiedAP);
				}
				else
				{
					Gate gate = (Gate)goal;
					result = gate.DefaultAP;
				}
			}
			return result;
		}

		public T GetRandomItem<T>(List<T> list)
		{
			bool flag = list.Count == 0;
			T result;
			if (flag)
			{
                result = default(T);
               // result = default (!!0);
				
			}
			else
			{
				Random random = new Random();
				int index = random.Next(list.Count);
				result = list[index];
			}
			return result;
		}

		public void UpdateFOV()
		{
			this.FOV = this.Vision.GetFOVPolylines(this, this.ParentMap);
			this.FOVCurve = new List<Curve>();
			bool flag = this.FOV == null || this.FOV.Count == 0;
			if (!flag)
			{
				foreach (Polyline polyline in this.FOV)
				{
					PolylineCurve item = new PolylineCurve(polyline);
					this.FOVCurve.Add(item);
				}
			}
		}

		public bool HasReachedGoal()
		{
			bool flag = this.CurrentGoal == null || this.CurrentAccessPoint == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				double num = this.CurrentGoal.AccessRadius + this.BodyRadius;
				double num2 = base.getPosition().distance(this.CurrentPath.nodes[this.CurrentPath.getCount() - 1].Position);
				result = (num2 < num);
			}
			return result;
		}

		private void PlanPathTowardsDestination()
		{
			GoalSpecificGraph goalSpecificGraph = this.ParentMap.GateSpecGraphs[this.Destination];
			Vertex goalVertex = goalSpecificGraph.GoalVertex;
			Path currentPath = this.PlanPathForVertex(base.getPosition(), goalVertex, this.ParentMap, goalSpecificGraph);
			this.CurrentPath = currentPath;
			this.CurrentNodeIndex = 0;
		}

		private Path PlanPathForVertex(Vec2d a, Vertex gv, Map map, GoalSpecificGraph g)
		{
			Vertex vertex = new Vertex(a, null, 0);
			List<Vertex> list = new List<Vertex>
			{
				vertex
			};
			Vec2d position = gv.Position;
			bool flag = RhinoGeoMethods.IsPositionVisible(a, position, map.ObstaclePolylines);
			Path result;
			if (flag)
			{
				list.Add(gv);
				Path path = new Path(list);
				result = path;
			}
			else
			{
				bool flag2 = g == null || g.Vertices.Count == 0;
				if (flag2)
				{
					throw new Exception("PlanPathForVertex: The Graph is null and there are obstacles in the way.");
				}
				List<Vertex> list2 = new List<Vertex>();
				foreach (Vertex vertex2 in g.Vertices)
				{
					bool flag3 = RhinoGeoMethods.IsPositionVisible(a, vertex2.Position, map.ObstaclePolylines);
					if (flag3)
					{
						list2.Add(vertex2);
					}
				}
				bool flag4 = list2.Count == 0;
				if (flag4)
				{
					throw new Exception("PlanPathForVertex: There is no visible start vertex.");
				}
				Vertex parent = list2[0];
				double num = double.MaxValue;
				foreach (Vertex vertex3 in list2)
				{
					double num2 = vertex3.Position.distance(a);
					bool flag5 = num2 + vertex3.GValue < num;
					if (flag5)
					{
						num = num2 + vertex3.GValue;
						parent = vertex3;
					}
				}
				vertex.Parent = parent;
				Path path = vertex.TracePath();
				result = path;
			}
			return result;
		}

		private Path CreateLinePath(Vec2d a, Vec2d b)
		{
			Vertex vertex = new Vertex(a, null, 0);
			Vertex item = new Vertex(b, vertex, 1);
			List<Vertex> nodes = new List<Vertex>
			{
				vertex,
				item
			};
			return new Path(nodes);
		}

		private Vector3d ConvertToVector3d(Vec2d v)
		{
			return new Vector3d(v.X, v.Y, 0.0);
		}

		public static double NeighborRadius = 5.0;

		public static double ObstacleMaxDistance = 10.0;

		public int NeighborUpdatingPeriod = 10;

		public int LookingPeriod = 10;

		public int TraceRecordingPeriod = 10;

		public int NeighboringObstacleUpdatingPeriod = 10;

		private readonly double nodePositionError = 0.1;

		public double TargetForce;

		public Map ParentMap;

		private List<ForceBehavior> behaviors;

		public List<string> BehaviorNames;

		public List<Vector3d> Forces;

		public PersonState State;

		public int TimeLimit;

		public int ShoppingFrameCount = 0;

		public int PersonFrameCount = 0;

		public int TemplateID;

		public int ID;

		public int StartFrame;

		public int EndFrame;

		private readonly Queue<double> RecentSpeedRec;

		private readonly int RecentSpeedRecLength = 20;

		private readonly double StuckSpeed = 0.3;

		public bool IsValid = true;
	}
}

