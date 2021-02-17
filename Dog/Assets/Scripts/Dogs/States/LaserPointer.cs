using Assets.Scripts.ActionPlanning.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Dogs.States
{
	public class LaserPointer : IState<LaserPointer>
	{
		public bool InHand { get; set; }
		public bool On { get; set; }
		public Vector2 Position { get; set; }

		public void Reset()
		{
			InHand = false;
			On = false;
			Position = default;
		}

		public void Set(LaserPointer state)
		{
			InHand = state.InHand;
			On = state.On;
			Position = state.Position;
		}
	}
}
