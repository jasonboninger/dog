using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.Models;
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
	
	public interface IAction : IAction<State>
	{
		EAction Id { get; }

		void Initialize(Dog dog);

		float GetTransitionTime();

		void Enter(float transitionTime);

		IEnumerator Execute(Func<bool> cancelled);

		void Exit(float transitionTime);
	}
	
	public abstract class UAction : MonoBehaviour, IAction
	{
		public EAction Id { get; }

		protected Dog Dog { get; private set; }
		protected Transform Transform { get; private set; }
		protected Animator Animator { get; private set; }
		protected Looker Looker { get; private set; }
		protected State State { get; private set; }

		private Coroutine _enteringExiting;

		protected UAction(EAction id) => Id = id;

		protected abstract void Initialize();
		void IAction.Initialize(Dog dog)
		{
			// Set dog
			Dog = dog;
			// Set transform
			Transform = dog.Transform;
			// Set animator
			Animator = dog.Animator;
			// Set looker
			Looker = dog.Looker;
			// Set state
			State = dog.State;
			// Execute intialize
			Initialize();
		}

		protected abstract bool IsValid(State state);
		bool IAction<State>.IsValid(State state) => IsValid(state);

		protected abstract float GetCost(State state);
		float IAction<State>.GetCost(State state) => GetCost(state);

		protected abstract void UpdateState(State state);
		void IAction<State>.UpdateState(State state)
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
			Dog.State.Action = Id;
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
