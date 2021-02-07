using UnityEngine;

namespace Assets.Scripts.Utilities
{
	public class AnimatorParameterTrigger : AnimatorParameter
	{
		public AnimatorParameterTrigger(Animator animator, string name) : base(animator, name) { }

		public void Trigger() => animator.SetTrigger(parameter);
	}
}
