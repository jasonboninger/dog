using Assets.Scripts.ActionPlanning.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Dogs.States
{
	public class Owner : IState<Owner>
	{
		public Vector2 Position { get; set; }

		public void Reset()
		{
			Position = default;
		}

		public void Set(Owner state)
		{
			Position = state.Position;
		}
	}
}
