using Assets.Scripts.Dogs.States;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Dogs.Actions
{
	public class UIdle : UDogAction
	{
		[SerializeField] private float _transitionTime = default;
		
		protected override void Initialize() { }

		protected override bool IsValid(Dog state) => false;

		protected override float GetCost(Dog state) => 0;

		protected override void UpdateState(Dog state) { }

		protected override float GetTransitionTime() => _transitionTime;

		protected override IEnumerator Enter(float transitionTime) { yield break; }

		protected override IEnumerator Execute(Func<bool> cancelled)
		{
			// Loop until cancelled
			while (!cancelled())
			{
				// Wait a frame
				yield return null;
			}
		}

		protected override IEnumerator Exit(float transitionTime) { yield break; }
	}
}
