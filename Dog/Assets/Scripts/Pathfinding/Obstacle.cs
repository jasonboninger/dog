using Assets.Scripts.Pathfinding.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
	partial class Pathfinder
	{
		private class Obstacle : IObstacle
		{
			private readonly List<Vector2Int> _coordinates = new List<Vector2Int>();
			public IReadOnlyList<Vector2Int> Coordinates => _coordinates;

			public Obstacle(Vector2 position, float radius)
			{
				// Get offset radius
				var radiusOffset = radius + 0.5f;
				// Get offset radius squared
				var radiusOffsetSquared = radiusOffset * radiusOffset;
				// Create ring radius
				var radiusRing = 0;
				// Get ring coordinate
				var coordinateRing = Converter.Coordinate(position);
				// Add coordinates
				while (true)
				{
					// Create found
					var found = false;
					// Get ring
					var ring = Rings.Get(radiusRing++);
					// Run through ring
					for (int i = 0; i < ring.Count; i++)
					{
						var coordinate = ring.GetCoordinate(coordinateRing, i);
						// Get distance squared
						var distanceSquared = (coordinate - position).sqrMagnitude;
						// Check if distance is within offset radius
						if (distanceSquared < radiusOffsetSquared)
						{
							// Add coordinate
							_coordinates.Add(coordinate);
							// Set found
							found = true;
						}
					}
					// Check if not found
					if (!found)
					{
						// Stop loop
						break;
					}
				}
			}
		}
	}
}
