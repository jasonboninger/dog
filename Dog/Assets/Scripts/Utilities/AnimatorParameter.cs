using UnityEngine;

namespace Assets.Scripts.Utilities
{
	public abstract class AnimatorParameter
	{
		public string Name { get; }

		protected readonly Animator animator;
		protected readonly int parameter;

		protected AnimatorParameter(Animator animator, string name)
		{
			this.animator = animator;
			parameter = Animator.StringToHash(name);
			Name = name;
		}
	}
	public abstract class AnimatorParameter<TValue> : AnimatorParameter
	where TValue : struct
	{
		public TValue Value { get => GetValue(animator, parameter); set => SetValue(animator, parameter, value); }

		protected AnimatorParameter(Animator animator, string name) : base(animator, name) { }

		protected abstract TValue GetValue(Animator animator, int parameter);
		
		protected abstract void SetValue(Animator animator, int parameter, TValue value);
	}
}
