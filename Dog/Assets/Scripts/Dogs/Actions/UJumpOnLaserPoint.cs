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
		[SerializeField] private bool _slowOnApproach = true;

		public bool SlowOnApproach => _slowOnApproach;

		protected override void Initialize() { }

		public override bool IsValid(Dog state) => IsTraversable(state) && state.Position.Equals(state.LaserPointer.Position);

		public override float GetCost(Dog state) => 1;

		public override void UpdateState(Dog state) => state.LaserPointer.On = false;

		public bool IsTraversable() => IsTraversable(State);
		public bool IsTraversable(Dog state) => state.LaserPointer.On && state.LaserPointer.Visible;

		public Vector2 GetPosition() => GetPosition(State);
		public Vector2 GetPosition(Dog state) => state.LaserPointer.Position;

		public bool IsReached() => IsReached(State);
		public bool IsReached(Dog state) => state.Position.Equals(state.LaserPointer.Position);

		public override float GetTransitionIn() => _transitionTime;

		public override IEnumerator ExecuteAction(float transitionIn, Func<float?> getTransitionOut)
		{
			// Log jump
			Debug.Log("JUMPED FOR IT!");
			// Wait a second
			yield return new WaitForSeconds(1);
		}
	}
}
