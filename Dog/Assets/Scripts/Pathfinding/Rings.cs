using BoningerWorks.Grid;
using System.Collections.Generic;

namespace Assets.Scripts.Pathfinding
{
	partial class Pathfinder
	{
		private static class Rings
		{
			private static readonly List<BwRelativeRing> _rings = new List<BwRelativeRing>();

			public static BwRelativeRing Get(in int radius)
			{
				// Create rings
				while (radius >= _rings.Count)
				{
					// Create ring
					var ring = new BwRelativeRing(_rings.Count);
					// Add ring
					_rings.Add(ring);
				}
				// Return ring
				return _rings[radius];
			}
		}
	}
}
