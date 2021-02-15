using UnityEngine;

namespace Assets.Scripts.Dogs.Models
{
	public class Dog
	{
		public Transform Transform { get; }
		public Animator Animator { get; }
		public Looker Looker { get; }
		public State State { get; }

		public Dog(Transform transform, Animator animator, Looker looker, State state)
		{
			Transform = transform;
			Animator = animator;
			Looker = looker;
			State = state;
		}
	}
}
