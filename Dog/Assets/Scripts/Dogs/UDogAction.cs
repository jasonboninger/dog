using Assets.Scripts.ActionManagement.Interfaces;
using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.Interfaces;
using Assets.Scripts.Dogs.Models;
using Assets.Scripts.Dogs.States;
using Assets.Scripts.Static;
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
		protected Looker Looker { get; private set; }

		private Coroutine _enteringExiting;

		protected abstract void Initialize();
		void IDogAction.Initialize(Controls controls)
		{
			// Set controls
			Controls = controls;
			// Set state
			State = controls.State;
			// Set transform
			Transform = controls.Transform;
			// Set animator
			Animator = controls.Animator;
			// Set looker
			Looker = controls.Looker;
			// Execute intialize
			Initialize();
		}

		protected abstract bool IsValid(Dog state);
		bool IAction<Dog>.IsValid(Dog state) => IsValid(state);

		protected abstract float GetCost(Dog state);
		float IAction<Dog>.GetCost(Dog state) => GetCost(state);

		protected abstract void UpdateState(Dog state);
		void IAction<Dog>.UpdateState(Dog state) => UpdateState(state);

		protected abstract float GetTransitionTime();
		float IActionState.GetTransitionTime() => GetTransitionTime();

		protected abstract IEnumerator Enter(float transitionTime);
		void IActionState.Enter(float transitionTime)
		{
			// Stop exit
			this.StopCoroutineIfExists(_enteringExiting);
			// Execute enter
			_enteringExiting = StartCoroutine(Enter(transitionTime));
		}

		protected abstract IEnumerator Execute(Func<bool> cancelled);
		IEnumerator IActionState.Execute(Func<bool> cancelled) => Execute(cancelled);

		protected abstract IEnumerator Exit(float transitionTime);
		void IActionState.Exit(float transitionTime)
		{
			// Stop enter
			this.StopCoroutineIfExists(_enteringExiting);
			// Execute exit
			_enteringExiting = StartCoroutine(Exit(transitionTime));
		}
	}
}
