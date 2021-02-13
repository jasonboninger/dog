using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
	partial class Pathfinder
	{
		private class Searcher
		{
			private class Waypoint
			{
				public readonly Waypoint previous;
				public readonly Vector2Int coordinate;
				public readonly float costKnown;
				public readonly float costEstimated;

				public Waypoint(Waypoint previous, Vector2Int coordinate, float costKnown, float costEstimated)
				{
					this.previous = previous;
					this.coordinate = coordinate;
					this.costKnown = costKnown;
					this.costEstimated = costEstimated;
				}
			}

			public void PopulatePath(Path path, Agent agent, Vector2 start, Vector2 end)
			{
				// Clear positions
				path.positions.Clear();
				// Get start coordinate
				var coordinateStart = Converter.Coordinate(start);
				// Get end coordinate
				var coordinateEnd = Converter.Coordinate(end);
				// Create waypoints
				var waypoints = new List<Waypoint> { new Waypoint(previous: null, coordinateStart, costKnown: 0, estimateCost(coordinateStart)) };
				// Search squares
				while (waypoints.Count > 0)
				{
					// Get waypoint
					var waypoint = waypoints[0];
					// Remove waypoint
					waypoints.RemoveAt(0);
					// Check if end coordinate
					if (waypoint.coordinate == coordinateEnd)
					{
						// Set path
						setPath(waypoint);
						// Path populated
						return;
					}
					// Add waypoints
					addWaypoint(waypoint, waypoint.coordinate + new Vector2Int(1, 0));
					addWaypoint(waypoint, waypoint.coordinate + new Vector2Int(-1, 0));
					addWaypoint(waypoint, waypoint.coordinate + new Vector2Int(0, 1));
					addWaypoint(waypoint, waypoint.coordinate + new Vector2Int(0, -1));
					addWaypoint(waypoint, waypoint.coordinate + new Vector2Int(1, 1));
					addWaypoint(waypoint, waypoint.coordinate + new Vector2Int(1, -1));
					addWaypoint(waypoint, waypoint.coordinate + new Vector2Int(-1, 1));
					addWaypoint(waypoint, waypoint.coordinate + new Vector2Int(-1, -1));
				}

				float estimateCost(Vector2Int coordinateCurrent)
				{
					return Vector2.Distance(coordinateCurrent, coordinateEnd);
				}

				void addWaypoint(Waypoint previous, Vector2Int coordinateNew)
				{
					if (agent.GetSquare(coordinateNew).blocked != 0)
					{
						return;
					}
					var costKnown = previous.costKnown + Vector2Int.Distance(previous.coordinate, coordinateNew);
					var costEstimated = costKnown + estimateCost(coordinateNew);
					if (waypoints.Find(waypoint => waypoint.coordinate == coordinateNew && waypoint.costKnown <= costKnown) != null)
					{
						return;
					}
					waypoints.Add(new Waypoint(previous, coordinateNew, costKnown, costEstimated));
					waypoints.Sort((l, r) => l.costEstimated.CompareTo(r.costEstimated));
				}

				void setPath(Waypoint waypoint)
				{
					do
					{
						path.positions.Add(waypoint.coordinate);
						waypoint = waypoint.previous;
					}
					while (waypoint != null);
					path.positions.Reverse();
				}
			}
		}
	}
}
