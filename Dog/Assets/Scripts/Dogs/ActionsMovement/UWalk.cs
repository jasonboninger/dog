using Assets.Scripts.Dogs.Interfaces;
using Assets.Scripts.Dogs.States;
using Assets.Scripts.Static;
using Assets.Scripts.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Dogs.ActionsMovement
{
	public class UWalk : UDogAction
	{
		private static readonly Vector2 _zero = new Vector2(0, 0);

		public IDogMovement Movement { get; set; }

		[SerializeField] private float _transitionTime = default;
		[SerializeField] private float _speedMaximum = default;
		[SerializeField] private float _speedMinimum = default;
		[SerializeField] private float _speedAcceleration = default;
		[SerializeField] private float _distanceSlow = default;

		private Coroutine _enteringExiting;
		private AnimatorLayerWeight _walk;
		private AnimatorParameterFloat _speed;

		protected override void Initialize()
		{
			// Set walk
			_walk = new AnimatorLayerWeight(Animator, "Core_Walk");
			// Set speed
			_speed = new AnimatorParameterFloat(Animator, "Core_Walk---Speed");
		}

		public override bool IsValid(Dog state) => throw new NotImplementedException();

		public override float GetCost(Dog state) => throw new NotImplementedException();

		public override void UpdateState(Dog state) => throw new NotImplementedException();

		public override float GetTransitionIn() => _transitionTime;

		public override IEnumerator ExecuteAction(float transitionIn, Func<float?> getTransitionOut)
		{
			// Execute enter
			this.StopCoroutineIfExistsAndReplace(ref _enteringExiting, StartCoroutine(_Enter(transitionIn)));
			// Create out transition
			float? transitionOut;
			// Loop forever
			while (true)
			{
				// Set out transition
				transitionOut = getTransitionOut();
				// Check if out transition exists
				if (transitionOut.HasValue)
				{
					// Stop loop
					break;
				}
				// Get delta time
				var deltaTime = Time.deltaTime;
				// Get position
				var position = new Vector2(Transform.position.x, Transform.position.z);
				// Get destination
				var destination = Movement.GetDestination();
				// Get direction
				var direction = destination - position;
				// Get distance
				var distance = direction.magnitude;
				// Get speed
				var speed = Mathf.Clamp(_speed.Value + _speedAcceleration * deltaTime, _speedMinimum, _speedMaximum);
				// Interpolate speed
				speed = Mathf.Lerp(_speedMinimum, speed, distance / _distanceSlow);
				// Set speed
				_speed.Value = speed;
				// Move position
				position = Vector2.MoveTowards(position, destination, speed * deltaTime);
				// Set position
				Transform.position = new Vector3(position.x, Transform.position.y, position.y);
				// Check if direction exists
				if (!direction.normalized.Equals(_zero))
				{
					// Set rotation
					Transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, Transform.position.y, direction.y));
				}
				// Check if at destination or destination reached
				if (position.Equals(destination) || Movement.ReachedDestination())
				{
					// Destination reached
					yield break;
				}
				// Wait a frame
				yield return null;
			}
			// Execute exit
			this.StopCoroutineIfExistsAndReplace(ref _enteringExiting, StartCoroutine(_Exit(transitionOut ?? 0.1f)));
		}

		private IEnumerator _Enter(float transitionTime)
		{
			// Set speed
			_speed.Value = State.Speed.magnitude;
			// Set walk
			yield return BwEnumerator.ExecuteOverTime(weight => _walk.Weight = weight, _walk.Weight, 1, transitionTime, smooth: false);
		}

		private IEnumerator _Exit(float transitionTime)
		{
			// Set walk
			yield return BwEnumerator.ExecuteOverTime(weight => _walk.Weight = weight, _walk.Weight, 0, transitionTime, smooth: false);
		}
	}
}
