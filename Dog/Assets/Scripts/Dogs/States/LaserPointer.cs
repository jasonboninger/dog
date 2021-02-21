using Assets.Scripts.ActionPlanning.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Dogs.States
{
	public class LaserPointer : IState<LaserPointer>
	{
		public bool On { get; set; }
		public bool Visible { get; set; }
		public Vector2 Position { get; set; }

		public void Reset()
		{
			On = false;
			Visible = false;
			Position = default;
		}

		public void Set(LaserPointer state)
		{
			On = state.On;
			Visible = state.Visible;
			Position = state.Position;
		}
	}
}
