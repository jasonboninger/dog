using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
	partial class Pathfinder
	{
		private class Agent
		{
			public readonly int size;

			private readonly Dictionary<Vector2Int, SSquare> _coordinateToSquareMappings = new Dictionary<Vector2Int, SSquare>();
			private readonly HashSet<Obstacle> _obstacles = new HashSet<Obstacle>();

			public Agent(int size) => this.size = size;

			public SSquare GetSquare(in Vector2Int coordinate)
			{
				// Get square
				_coordinateToSquareMappings.TryGetValue(coordinate, out var square);
				// Return square
				return square;
			}

			public void AddObstacle(Obstacle obstacle)
			{
				// Add obstacle
				_AddRemoveObstacle(obstacle, add: true);
			}

			public void RemoveObstacle(Obstacle obstacle)
			{
				// Remove obstacle
				_AddRemoveObstacle(obstacle, add: false);
			}

			private void _AddRemoveObstacle(Obstacle obstacle, bool add)
			{
				// Add or remove obstacle
				var addRemove = add ? _obstacles.Add(obstacle) : _obstacles.Remove(obstacle);
				// Check if not added or removed
				if (!addRemove)
				{
					// Already added or removed
					return;
				}
				// Get coordinates
				var coordinates = obstacle.Coordinates;
				// Run through coordinates
				for (int i = 0; i < coordinates.Count; i++)
				{
					var coordinate = coordinates[i];
					var x = coordinate.x;
					var y = coordinate.y;
					// Run through size
					for (int xOffset = 0; xOffset < size; xOffset++)
					{
						for (int yOffset = 0; yOffset < size; yOffset++)
						{
							// Add or remove block
							_AddRemoveBlocked(new Vector2Int(x - xOffset, y - yOffset), add);
						}
					}
				}
			}

			private void _AddRemoveBlocked(Vector2Int coordinate, bool add)
			{
				// Get square
				var square = GetSquare(coordinate);
				// Check if add
				if (add)
				{
					// Add blocked
					square.state |= SSquare.STATE_BLOCKED;
					// Add blocked
					square.blocked++;
				}
				else
				{
					// Remove blocked
					if (--square.blocked == 0)
					{
						// Remove blocked
						square.state &= ~SSquare.STATE_BLOCKED;
					}
				}
				// Update square
				_UpdateSquare(coordinate, square);
			}

			private void _UpdateSquare(in Vector2Int coordinate, in SSquare square)
			{
				// Check if no state
				if (square.state == SSquare.STATE_NONE)
				{
					// Remove square
					_coordinateToSquareMappings.Remove(coordinate);
				}
				else
				{
					// Set square
					_coordinateToSquareMappings[coordinate] = square;
				}
			}
		}
	}
}
