using Assets.Scripts.Dogs.Interfaces;
using Assets.Scripts.Dogs.Models;
using Assets.Scripts.Dogs.States;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Dogs
{
	public abstract class UDogAction : MonoBehaviour, IDogAction
	{
		protected Controls Controls { get; private set; }
		protected Dog State { get; private set; }
		protected Transform Transform { get; private set; }
		protected Animator Animator { get; private set; }
		protected UDogOwner Owner { get; private set; }
		protected Looker Looker { get; private set; }

		protected abstract void Initialize();
		public void Initialize(Controls controls)
		{
			// Set controls
			Controls = controls;
			// Set state
			State = controls.State;
			// Set transform
			Transform = controls.Transform;
			// Set animator
			Animator = controls.Animator;
			// Set owner
			Owner = controls.Owner;
			// Set looker
			Looker = controls.Looker;
			// Execute intialize
			Initialize();
		}

		public abstract bool IsValid(Dog state);

		public abstract float GetCost(Dog state);

		public abstract void UpdateState(Dog state);

		public abstract float GetTransitionIn();
		
		public abstract IEnumerator ExecuteAction(float transitionIn, Func<float?> getTransitionOut, Action<float> setTransitionOut);
	}
}
