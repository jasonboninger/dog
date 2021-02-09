using Assets.Scripts.Pathfinding.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
	partial class Pathfinder
	{
		private class Path : IPath
		{
			public readonly List<Vector2> positions = new List<Vector2>();
			public IReadOnlyList<Vector2> Positions => positions;
		}
	}
}
