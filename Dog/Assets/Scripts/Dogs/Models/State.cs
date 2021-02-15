using Assets.Scripts.ActionPlanning.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Dogs.Models
{
	public class State : IState<State>
	{
		public EAction Action { get; set; }
		public Vector2 Position { get; set; }
		public Vector2 Speed { get; set; }
		public Vector2? Destination { get; set; }

		public void Reset()
		{
			Set(default, default, default, null);
		}

		public void Set(State state)
		{
			Set(state.Action, state.Position, state.Speed, state.Destination);
		}
		public void Set(EAction action, Vector2 position, Vector2 speed, Vector2? destination)
		{
			Action = action;
			Position = position;
			Speed = speed;
			Destination = destination;
		}
	}
}
