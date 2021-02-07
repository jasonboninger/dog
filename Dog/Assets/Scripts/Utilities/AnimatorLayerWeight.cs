using UnityEngine;

namespace Assets.Scripts.Utilities
{
	public class AnimatorLayerWeight
	{
		public float Weight { get => _animator.GetLayerWeight(_layer); set => _animator.SetLayerWeight(_layer, value); }

		public string Name { get; }

		private readonly Animator _animator;
		private readonly int _layer;

		public AnimatorLayerWeight(Animator animator, string name)
		{
			_animator = animator;
			_layer = animator.GetLayerIndex(name);
			Name = name;
		}
	}
}
