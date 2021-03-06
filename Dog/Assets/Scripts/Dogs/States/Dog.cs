﻿using Assets.Scripts.ActionPlanning.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Dogs.States
{
	public class Dog : IState<Dog>
	{
		public Vector2 Position { get; set; }
		public Vector2 Speed { get; set; }
		public bool Chilled { get; set; }

		public Owner Owner { get; } = new Owner();
		public LaserPointer LaserPointer { get; } = new LaserPointer();

		public void Reset()
		{
			Position = default;
			Speed = default;
			Chilled = false;
			Owner.Reset();
			LaserPointer.Reset();
		}

		public void Set(Dog state)
		{
			Position = state.Position;
			Speed = state.Speed;
			Chilled = state.Chilled;
			Owner.Set(state.Owner);
			LaserPointer.Set(state.LaserPointer);
		}
	}
}
