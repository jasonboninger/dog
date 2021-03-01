using Assets.Scripts.ActionPlanning.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Dogs.States
{
	public class LaserPointer : IState<LaserPointer>
	{
		public bool Never { get; set; }
		public Vector2 Position { get; set; }

		public void Reset()
		{
			Never = false;
			Position = default;
		}

		public void Set(LaserPointer state)
		{
			Never = state.Never;
			Position = state.Position;
		}
	}
}
