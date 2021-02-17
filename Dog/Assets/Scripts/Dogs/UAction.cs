using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.Models;
using Assets.Scripts.Dogs.States;
using Assets.Scripts.Static;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Dogs
{
	public enum EAction
	{
		Idle,
		Walk,
	}
	
	public interface IAction : IAction<Dog>
	{
		EAction Id { get; }

		void Initialize(Controls controls);

		float GetTransitionTime();

		void Enter(float transitionTime);

		IEnumerator Execute(Func<bool> cancelled);

		void Exit(float transitionTime);
	}
	
	public abstract class UAction : MonoBehaviour, IAction
	{
		public EAction Id { get; }

		protected Controls Controls { get; private set; }
		protected Dog State { get; private set; }
		protected Transform Transform { get; private set; }
		protected Animator Animator { get; private set; }
		protected Looker Looker { get; private set; }

		private Coroutine _enteringExiting;

		protected UAction(EAction id) => Id = id;

		protected abstract void Initialize();
		void IAction.Initialize(Controls controls)
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
		void IAction<Dog>.UpdateState(Dog state)
		{
			// Set action
			state.Action = Id;
			// Execute update state
			UpdateState(state);
		}

		protected abstract float GetTransitionTime();
		float IAction.GetTransitionTime() => GetTransitionTime();

		protected abstract IEnumerator Enter(float transitionTime);
		void IAction.Enter(float transitionTime)
		{
			// Stop exit
			this.StopCoroutineIfExists(_enteringExiting);
			// Set action
			State.Action = Id;
			// Execute enter
			_enteringExiting = StartCoroutine(Enter(transitionTime));
		}

		protected abstract IEnumerator Execute(Func<bool> cancelled);
		IEnumerator IAction.Execute(Func<bool> cancelled) => Execute(cancelled);

		protected abstract IEnumerator Exit(float transitionTime);
		void IAction.Exit(float transitionTime)
		{
			// Stop enter
			this.StopCoroutineIfExists(_enteringExiting);
			// Execute exit
			_enteringExiting = StartCoroutine(Exit(transitionTime));
		}
	}
}
