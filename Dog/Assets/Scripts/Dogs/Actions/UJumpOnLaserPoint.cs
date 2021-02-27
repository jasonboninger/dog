using Assets.Scripts.Dogs.Interfaces;
using Assets.Scripts.Dogs.States;
using Assets.Scripts.Static;
using Assets.Scripts.Utilities;
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

		private AnimatorLayer _jumpOn;
		private Coroutine _enteringExiting;

		protected override void Initialize()
		{
			// Set jump on
			_jumpOn = new AnimatorLayer(Animator, "Core_JumpOn");
		}

		public override bool IsValid(Dog state) => IsTraversable(state) && state.Position.Equals(state.LaserPointer.Position);

		public override float GetCost(Dog state) => 1;

		public override void UpdateState(Dog state) => state.LaserPointer.Caught = true;

		public bool IsTraversable() => IsTraversable(State);
		public bool IsTraversable(Dog state) => state.LaserPointer.On && state.LaserPointer.Visible;

		public Vector2 GetPosition() => GetPosition(State);
		public Vector2 GetPosition(Dog state) => state.LaserPointer.Position;

		public bool IsReached() => IsReached(State);
		public bool IsReached(Dog state) => state.Position.Equals(state.LaserPointer.Position);

		public override float GetTransitionIn() => _transitionTime;

		public override IEnumerator ExecuteAction(float transitionIn, Func<float?> getTransitionOut, Action<float> setTransitionOut)
		{
			// Execute transition
			this.StopCoroutineIfExistsAndReplace
				(
					ref _enteringExiting,
					StartCoroutine(BwEnumerator.ExecuteOverTime(weight => _jumpOn.Weight = weight, _jumpOn.Weight, 1, transitionIn, smooth: false))
				);
			// Play state
			_jumpOn.Play("JumpOn", 0.2f);
			// Loop until done
			while (true)
			{
				// Wait a frame
				yield return null;
				// Get state
				var state = _jumpOn.GetCurrentAnimatorStateInfo();
				// Check if complete
				if (state.normalizedTime * state.length >= state.length - 0.1f)
				{
					// Stop loop
					break;
				}
			}
			// Execute transition
			this.StopCoroutineIfExistsAndReplace
				(
					ref _enteringExiting,
					StartCoroutine(BwEnumerator.ExecuteOverTime(weight => _jumpOn.Weight = weight, _jumpOn.Weight, 0, 0.1f, smooth: false))
				);
		}
	}
}
