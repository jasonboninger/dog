using Assets.Scripts.Dogs.Models;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Dogs.Actions
{
	public class UIdle : UAction
	{
		[SerializeField] private float _transitionTime = default;
		
		public UIdle() : base(EAction.Idle) { }

		protected override void Initialize() { }

		protected override bool IsValid(State state) => true;

		protected override float GetCost(State state) => 1000000;

		protected override void UpdateState(State state) { }

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
