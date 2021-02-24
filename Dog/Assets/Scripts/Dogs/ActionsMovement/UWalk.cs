using Assets.Scripts.Dogs.Interfaces;
using Assets.Scripts.Dogs.States;
using Assets.Scripts.Static;
using Assets.Scripts.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Dogs.ActionsMovement
{
	public class UWalk : UDogActionMovement
	{
		private static readonly Vector2 _zero = new Vector2(0, 0);

		[SerializeField] private float _transitionTime = default;
		[SerializeField] private float _speedMaximum = default;
		[SerializeField] private float _speedMinimum = default;
		[SerializeField] private float _speedAcceleration = default;
		[SerializeField] private float _distanceSlow = default;

		private IDogActionDestination _actionDestination;
		private AnimatorLayer _walk;
		private AnimatorParameterFloat _speed;
		private Coroutine _enteringExiting;

		protected override void Initialize()
		{
			// Set walk
			_walk = new AnimatorLayer(Animator, "Core_Walk");
			// Set speed
			_speed = new AnimatorParameterFloat(Animator, "Core_Walk---Speed");
		}

		public override IDogActionMovement Create(GameObject gameObject, IDogActionDestination actionDestination)
		{
			// Create walk
			var walk = gameObject.AddComponent<UWalk>();
			// Set serialize fields
			walk._transitionTime = _transitionTime;
			walk._speedMaximum = _speedMaximum;
			walk._speedMinimum = _speedMinimum;
			walk._speedAcceleration = _speedAcceleration;
			walk._distanceSlow = _distanceSlow;
			// Set destination action
			walk._actionDestination = actionDestination;
			// Set animators
			walk._walk = _walk;
			walk._speed = _speed;
			// Initialize walk
			walk.Initialize(Controls);
			// Return walk
			return walk;
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
				// Check if destination not traversable
				if (!_actionDestination.IsTraversable())
				{
					// No destination
					break;
				}
				// Get delta time
				var deltaTime = Time.deltaTime;
				// Get position
				var position = new Vector2(Transform.position.x, Transform.position.z);
				// Get destination
				var destination = _actionDestination.GetPosition();
				// Get direction
				var direction = destination - position;
				// Get distance
				var distance = direction.magnitude;
				// Get speed
				var speed = Mathf.Clamp(_speed.Value + _speedAcceleration * deltaTime, _speedMinimum, _speedMaximum);
				// Check if slow on approach
				if (_actionDestination.SlowOnApproach)
				{
					// Interpolate speed
					speed = Mathf.Lerp(_speedMinimum, speed, distance / _distanceSlow);
				}
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
				if (position.Equals(destination) || _actionDestination.IsReached())
				{
					// Destination reached
					break;
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
