using Assets.Scripts.Dogs.Interfaces;
using Assets.Scripts.Dogs.States;
using System;
using System.Collections;

namespace Assets.Scripts.Dogs.ActionsSpecial
{
	public class Idle : IDogAction
	{
		public bool IsValid(Dog state) => false;

		public float GetCost(Dog state) => 0;

		public void UpdateState(Dog state) { /* Idle does not do anything */ }

		public float GetTransitionIn() => 0.1f;

		public IEnumerator ExecuteAction(float transitionIn, Func<float?> getTransitionOut)
		{
			// Loop while out transition does not exist
			while (!getTransitionOut().HasValue)
			{
				// Wait a frame
				yield return null;
			}
		}
	}
}
