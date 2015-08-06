using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Player
{
	#region classes
	class Point
	{
		public int X { get; set; }
		public int Y { get; set; }
	}

	class Drone
	{
		public Drone(int zoneCount, int droneId)
		{
			TargetZones = new List<TargetZone>(zoneCount);
			Linked = false;
			DroneId = droneId;
			Wait = 0;
		}
		public Point Position { get; set; }
		public List<TargetZone> TargetZones { get; set; }
		public bool Linked { get; set; }
		public int LinkNo { get; set; }
		public Zone Target { get; set; }
		public int DroneId { get; set; }
		public double BestDistance { get; set; }

		public int DroneClass { get; set; }

		public int Wait { get; set; }
	}

	class TargetZone
	{
		public Zone Zone { get; set; }
		public double Distance { get; set; }
	}

	class TargetDrone
	{
		public Drone Drone { get; set; }
		public double Distance { get; set; }
	}

	class LinkedDrones
	{
		public Drone Dron { get; set; }
		public double Distance { get; set; }
	}

	class Zone
	{
		public const int Radius = 100;

		public Zone(int droneCount)
		{
			TargetDrone = new List<TargetDrone>(droneCount);
		}

		public Point Center { get; set; }
		public int OwnerId { get; set; } // -1 if no owner

		public List<TargetDrone> TargetDrone { get; set; }

		public int MaxDrones { get; set; }
		public int MaxTreat { get; set; }
		public int MyDrones { get; set; }
		public int ZoneId { get; set; }

		public int ZoneClass { get; set; }

		public double BestDistance { get; set; }


	}

	class Team
	{
		public Team(int droneCount, int zoneCount, int teamId)
		{
			Drones = new List<Drone>(droneCount);
			TeamId = teamId;
			for (var droneId = 0; droneId < droneCount; droneId++)
				Drones.Add(new Drone(zoneCount, droneId));
		}

		public IList<Drone> Drones { get; set; }
		public int TeamId { get; set; }
	}

	#endregion

	class Game
	{
		#region consts

		List<Zone> _zones; // all game zones
		List<Team> _teams; // all the team of drones. Array index = team's ID
		int _myTeamId; // index of my team in the array of teams
		public static int LinkId = -1;
		public static TargetZone[] LinkedTargetZone = new TargetZone[10000];
		public static int ZoneCount = 0;
		public static int DroneCount = 0;

		#endregion
		// read initial games data (one time at the beginning of the game: P I D Z...)
		public void Init()
		{
			var pidz = ReadIntegers();
			ZoneCount = pidz[3];
			DroneCount = pidz[2];
			_myTeamId = pidz[1];
			_zones = ReadZones(pidz[3], pidz[2]).ToList();
			_teams = ReadTeams(pidz[0], pidz[2], pidz[3]).ToList();

		}

		IEnumerable<Zone> ReadZones(int zoneCount, int dronesPerTeam)
		{
			for (var areaId = 0; areaId < zoneCount; areaId++)
				yield return new Zone(dronesPerTeam) { Center = ReadPoint(), ZoneId = areaId};
		}

		IEnumerable<Team> ReadTeams(int teamCount, int dronesPerTeam, int zoneCount)
		{
			for (var teamId = 0; teamId < teamCount; teamId++)
				yield return new Team(dronesPerTeam, zoneCount, teamId);
		}

		public void ReadContext()
		{
			foreach (var zone in _zones)
			{
				zone.OwnerId = ReadIntegers()[0];
			}

			foreach (var team in _teams)
				foreach (var drone in team.Drones)
					drone.Position = ReadPoint();
		}

		// Compute logic here. This method is called for each game round. 
		public void Play()
		{
			var myDrones = _teams[_myTeamId].Drones;

			#region Zones loop
			foreach (var zone in _zones)
			{
				zone.MaxDrones = 0;
				zone.MyDrones = 0;
				zone.MaxTreat = 0;
				zone.TargetDrone.Clear();
				foreach (var team in _teams)
				{
					int count = team.Drones.Count(a => ProximityDetection(zone.Center, a.Position));
					if (count > 0)
					{
						if (team.TeamId == _myTeamId)
						{
							zone.MyDrones = count;
						}
						else
						{
							zone.MaxDrones = count > zone.MaxDrones ? count : zone.MaxDrones;
						}
					}

					count = team.Drones.Count(a => TreatDetection(zone.Center, a.Position));
					if (count > 0)
					{
						if (team.TeamId != _myTeamId)
						{
							zone.MaxTreat = count > zone.MaxTreat ? count : zone.MaxTreat;
						}
					}

				}
				zone.TargetDrone = new List<TargetDrone>(myDrones.Count());
				zone.TargetDrone.AddRange(myDrones.Select(myDrone => new TargetDrone
				{
					Distance = GetDistance(myDrone.Position.X, myDrone.Position.Y, zone.Center.X, zone.Center.Y),
					Drone = myDrone
				}));
				zone.BestDistance = zone.TargetDrone.OrderBy(o => o.Distance).Select(a => a.Distance).First();

				if (zone.OwnerId == _myTeamId && zone.MaxTreat > 0)
				{
					zone.ZoneClass = 0;
				}
				else if (zone.OwnerId != _myTeamId && zone.MaxDrones == 0)
				{
					zone.ZoneClass = 1;
				}
				else if (zone.OwnerId != _myTeamId && zone.MaxTreat < 2)
				{
					zone.ZoneClass = 2;
				}
				else if (zone.OwnerId != _myTeamId && zone.MaxTreat > 1)
				{
					zone.ZoneClass = 3;
				}
				else if (zone.OwnerId == _myTeamId && zone.MaxTreat == 0)
				{
					zone.ZoneClass = 4;
				}

			}
			#endregion

			#region drones loop
			foreach (var drone in myDrones)
			{

				drone.TargetZones.Clear();
				drone.Target = null;
				foreach (var zone in _zones)
				{
					TargetZone tZ = new TargetZone { Zone = zone, Distance = GetDistance(drone.Position.X, drone.Position.Y, zone.Center.X, zone.Center.Y) };
					drone.TargetZones.Add(tZ);
				}

				if (drone.TargetZones.Count(z => z.Distance < 100 && z.Zone.MaxTreat > 0 && z.Zone.OwnerId == _myTeamId) > 0)
				{
					drone.DroneClass = 0;
				}
				else if (drone.TargetZones.Count(z => z.Distance < 300 && z.Zone.OwnerId != _myTeamId) > 0)
				{
					drone.DroneClass = 1;
				}
				else
				{
					drone.DroneClass = 2;
				}

			}

			#endregion

			GetTargetZone(_zones, myDrones);

			foreach (Drone drone in myDrones)
			{
				string cords;
				if (drone.Target == null)
				{
					TargetZone tz = drone.TargetZones.OrderBy(o => o.Distance).FirstOrDefault(f => f.Zone.OwnerId == _myTeamId);
					if (tz == null)
					{
						Point wheretogo = GetGravityPoint(_zones);
						cords = (wheretogo.X) + " " + (wheretogo.Y);
					}
					else
					{
						cords = (tz.Zone.Center.X) + " " + (tz.Zone.Center.Y);
					}
					
				}
				else
				{
					cords = (drone.Target.Center.X) + " " + (drone.Target.Center.Y);
				}
				Console.WriteLine(cords);
			}

			//debugger part
			//foreach (var zone in _zones.OrderBy(z => z.ZoneClass).ThenBy(z => z.BestDistance))
			//{
				
			//		Console.Error.WriteLine("zona " + zone.ZoneId + " klasa " + zone.ZoneClass);
				

			//}
			//Console.Error.WriteLine("===========================");
			//foreach (var drone in myDrones)
			//{
			//	Console.Error.WriteLine("dron " + drone.DroneClass);
			//}
		}

		#region helpers

		static int[] ReadIntegers()
		{
			// ReSharper disable once PossibleNullReferenceException
			return Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
		}

		static Point ReadPoint()
		{
			var xy = ReadIntegers();
			return new Point { X = xy[0], Y = xy[1] };
		}

		static double GetDistance(int dronex, int droney, int zonex, int zoney)
		{
			int y = zoney - droney;
			int x = zonex - dronex;
			double dist = Math.Sqrt(x * x + y * y);
			return dist;
		}

		static bool ProximityDetection(Point center, Point position)
		{
			if (GetDistance(position.X, position.Y, center.X, center.Y) < 100)
			{
				return true;
			}
			return false;
		}

		static bool TreatDetection(Point center, Point position)
		{
			if (GetDistance(position.X, position.Y, center.X, center.Y) <= 400)
			{
				return true;
			}
			return false;
		}

		private static Point GetGravityPoint(IList<Zone> zones)
		{
			int x = 0, y = 0;
			foreach (Zone zone in zones.OrderByDescending(o=> o.Center.X).Take(3))
			{
				x = x + zone.Center.X;
				y = y + zone.Center.Y;
			}
			// ReSharper disable once PossibleLossOfFraction
			double centx = x/3;
			// ReSharper disable once PossibleLossOfFraction
			double centy = y/3;
			var tocka = new Point {X = (int) Math.Ceiling(centx), Y = (int) Math.Ceiling(centy)};
			return tocka;
		}

		#endregion

		#region getTarget

		private static void GetTargetZone(IList<Zone> zones, IList<Drone> myDrones)
		{

			foreach (Zone zone in zones.OrderBy(z => z.ZoneClass).ThenBy(z => z.BestDistance))
			{
				if (zone.ZoneClass == 0)
				{
					if (zone.MaxTreat - zone.MyDrones > 0)
					{
						List<Drone> drones =
							zone.TargetDrone.OrderBy(o => o.Distance)
								.ThenByDescending(d => d.Drone.DroneClass)
								.Where(w => w.Drone.Target == null).Select(s => s.Drone).Take(zone.MaxTreat - zone.MyDrones).ToList();
						foreach (Drone drone in drones)
						{
							drone.Target = zone;
						}
					}
					else 
					{
						List<Drone> drones =
							zone.TargetDrone.OrderBy(o => o.Distance)
								.ThenByDescending(d => d.Drone.DroneClass)
								.Where(w => w.Drone.Target == null).Select(s => s.Drone).Take(zone.MaxTreat).ToList();
						foreach (Drone drone in drones)
						{
							drone.Target = zone;
						}
					}
				}
				if (zone.ZoneClass == 1)
				{
						List<Drone> drones =
							zone.TargetDrone.OrderBy(o => o.Distance)
								.ThenByDescending(d => d.Drone.DroneClass)
								.Where(w => w.Drone.Target == null).Select(s => s.Drone).Take(1).ToList();
					
						foreach (Drone drone in drones)
						{
							drone.Target = zone;
						}
				}
				if (zone.ZoneClass == 2)
				{
						List<Drone> drones =
							zone.TargetDrone.OrderBy(o => o.Distance)
								.ThenByDescending(d => d.Drone.DroneClass)
								.Where(w => w.Drone.Target == null).Select(s => s.Drone).Take(zone.MaxDrones+1).ToList();
						foreach (Drone drone in drones)
						{
							drone.Target = zone;
						}
				}
				if (zone.ZoneClass == 3)
				{
					if (zone.MaxDrones < myDrones.Count(c => c.Target == null) + 1)
					{
						List<Drone> drones =
							zone.TargetDrone.OrderBy(o => o.Distance)
								.ThenByDescending(d => d.Drone.DroneClass)
								.Where(w => w.Drone.DroneClass > 0 && w.Drone.Target == null).Select(s => s.Drone).Take(zone.MaxDrones + 1).ToList();
						foreach (Drone drone in drones)
						{
							drone.Target = zone;
						}
					}
				}
				
			}

		}

		#endregion

	}

	static class Program
	{
		static void Main()
		{
			var game = new Game();


			game.Init();

			while (true)
			{
				game.ReadContext();
				game.Play();
			}
			// ReSharper disable once FunctionNeverReturns
		}
	}
}
