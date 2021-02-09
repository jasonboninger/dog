using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pathfinding.Interfaces
{
	public interface IPath
	{
		IReadOnlyList<Vector2> Positions { get; }
	}
}
