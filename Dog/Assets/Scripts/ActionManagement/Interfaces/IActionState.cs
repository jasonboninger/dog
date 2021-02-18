using System;
using System.Collections;

namespace Assets.Scripts.ActionManagement.Interfaces
{
	public interface IActionState
	{
		float GetTransitionTime();

		void Enter(float transitionTime);

		IEnumerator Execute(Func<bool> cancelled);

		void Exit(float transitionTime);
	}
}
