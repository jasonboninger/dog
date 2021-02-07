using UnityEngine;

namespace Assets.Scripts.Utilities
{
	public class AnimatorParameterInteger : AnimatorParameter<int>
	{
		public AnimatorParameterInteger(Animator animator, string name) : base(animator, name) { }

		protected override int GetValue(Animator animator, int parameter) => animator.GetInteger(parameter);

		protected override void SetValue(Animator animator, int parameter, int value) => animator.SetInteger(parameter, value);
	}
}
