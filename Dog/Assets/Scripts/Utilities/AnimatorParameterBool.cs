using UnityEngine;

namespace Assets.Scripts.Utilities
{
	public class AnimatorParameterBool : AnimatorParameter<bool>
	{
		public AnimatorParameterBool(Animator animator, string name) : base(animator, name) { }

		protected override bool GetValue(Animator animator, int parameter) => animator.GetBool(parameter);

		protected override void SetValue(Animator animator, int parameter, bool value) => animator.SetBool(parameter, value);
	}
}
