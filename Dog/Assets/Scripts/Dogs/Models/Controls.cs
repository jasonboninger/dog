using Assets.Scripts.Dogs.States;
using UnityEngine;

namespace Assets.Scripts.Dogs.Models
{
	public class Controls
	{
		public Dog State { get; }
		public Transform Transform { get; }
		public Animator Animator { get; }
		public Looker Looker { get; }

		public Controls(Dog state, Transform transform, Animator animator, Looker looker)
		{
			State = state;
			Transform = transform;
			Animator = animator;
			Looker = looker;
		}
	}
}
