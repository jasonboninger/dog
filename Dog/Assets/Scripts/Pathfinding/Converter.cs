using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
	partial class Pathfinder
	{
		private static class Converter
		{
			public static Vector2Int Coordinate(in Vector2 position)
			{
				// Return coordinate
				return Vector2Int.RoundToInt(position);
			}
		}
	}
}
