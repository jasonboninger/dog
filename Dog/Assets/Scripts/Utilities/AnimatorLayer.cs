using UnityEngine;

namespace Assets.Scripts.Utilities
{
	public class AnimatorLayer
	{
		public float Weight { get => _animator.GetLayerWeight(_layer); set => _animator.SetLayerWeight(_layer, value); }

		public string Name { get; }

		private readonly Animator _animator;
		private readonly int _layer;

		public AnimatorLayer(Animator animator, string name)
		{
			_animator = animator;
			_layer = animator.GetLayerIndex(name);
			Name = name;
		}

		public AnimatorStateInfo GetCurrentAnimatorStateInfo() => _animator.GetCurrentAnimatorStateInfo(_layer);

		public void Play(string stateName, float normalizedTime) => _animator.Play(stateName, _layer, normalizedTime);
	}
}
