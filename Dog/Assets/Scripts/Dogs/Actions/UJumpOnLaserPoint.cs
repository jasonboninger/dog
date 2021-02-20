using Assets.Scripts.Dogs.Interfaces;
using Assets.Scripts.Dogs.States;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Dogs.Actions
{
	public class UJumpOnLaserPoint : UDogAction, IDogActionDestination
	{
		[SerializeField] private float _transitionTime = 0;

		protected override void Initialize() { }

		public override bool IsValid(Dog state) => state.LaserPointer.On && state.Position.Equals(state.LaserPointer.Position);

		public override float GetCost(Dog state) => 1;

		public override void UpdateState(Dog state) => state.LaserPointer.On = false;

		public Vector2 GetPosition() => GetPosition(State);
		public Vector2 GetPosition(Dog state) => state.LaserPointer.Position;

		public bool IsReached() => IsReached(State);
		public bool IsReached(Dog state) => state.Position.Equals(state.LaserPointer.Position);

		public override float GetTransitionIn() => _transitionTime;

		public override IEnumerator ExecuteAction(float transitionIn, Func<float?> getTransitionOut)
		{
			// Log jump
			Debug.Log("JUMPED FOR IT!");
			// Set laser pointer off
			State.LaserPointer.On = false;
			// Return enumerator
			yield break;
		}
	}
}
