using UnityEngine;

namespace Assets.Scripts.Utilities
{
	public class AnimatorParameterFloat : AnimatorParameter<float>
	{
		public AnimatorParameterFloat(Animator animator, string name) : base(animator, name) { }

		protected override float GetValue(Animator animator, int parameter) => animator.GetFloat(parameter);

		protected override void SetValue(Animator animator, int parameter, float value) => animator.SetFloat(parameter, value);
	}
}
