﻿using Assets.Scripts.Dogs.States;
using Assets.Scripts.Static;
using Assets.Scripts.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Dogs.Actions
{
	public class UMove : UDogAction
	{
		private static readonly Vector2 _zero = new Vector2(0, 0);

		[SerializeField] private float _transitionTime = default;
		[SerializeField] private float _speedMaximum = default;
		[SerializeField] private float _speedMinimum = default;
		[SerializeField] private float _speedAcceleration = default;
		[SerializeField] private float _distanceSlow = default;

		private AnimatorLayerWeight _walk;
		private AnimatorParameterFloat _speed;

		protected override void Initialize()
		{
			// Set walk
			_walk = new AnimatorLayerWeight(Animator, "Core_Walk");
			// Set speed
			_speed = new AnimatorParameterFloat(Animator, "Core_Walk---Speed");
		}

		protected override bool IsValid(Dog state) => state.LaserPointer.On;

		protected override float GetCost(Dog state) => Vector2.Distance(state.Position, state.LaserPointer.Position);

		protected override void UpdateState(Dog state) => state.LaserPointer.On = false;

		protected override float GetTransitionTime() => _transitionTime;

		protected override IEnumerator Enter(float transitionTime)
		{
			// Set speed
			_speed.Value = State.Speed.magnitude;
			// Set walk
			yield return BwEnumerator.ExecuteOverTime(weight => _walk.Weight = weight, _walk.Weight, 1, transitionTime, smooth: false);
		}

		protected override IEnumerator Execute(Func<bool> cancelled)
		{
			// Move to laser pointer while not cancelled
			while (!cancelled())
			{
				// Check if laser pointer is not on
				if (!State.LaserPointer.On)
				{
					// No laser pointer
					yield break;
				}
				// Get delta time
				var deltaTime = Time.deltaTime;
				// Get position
				var position = new Vector2(Transform.position.x, Transform.position.z);
				// Get destination
				var destination = State.LaserPointer.Position;
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
				position = Vector3.MoveTowards(position, destination, speed * deltaTime);
				// Set position
				Transform.position = new Vector3(position.x, Transform.position.y, position.y);
				// Check if direction exists
				if (!direction.normalized.Equals(_zero))
				{
					// Set rotation
					Transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, Transform.position.y, direction.y));
				}
				// Check if destination reached
				if (position.Equals(destination))
				{
					// Clear laser pointer
					State.LaserPointer.On = false;
					// Laser pointer reached
					yield break;
				}
				// Wait a frame
				yield return null;
			}
		}

		protected override IEnumerator Exit(float transitionTime)
		{
			// Set walk
			yield return BwEnumerator.ExecuteOverTime(weight => _walk.Weight = weight, _walk.Weight, 0, transitionTime, smooth: false);
		}
	}
}