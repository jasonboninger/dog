namespace Assets.Scripts.Pathfinding
{
	partial class Pathfinder
	{
		private struct SSquare
		{
			public const int STATE_NONE = 0;
			public const int STATE_BLOCKED = 1 << 1;

			public int state;
			public byte blocked;
		}
	}
}
