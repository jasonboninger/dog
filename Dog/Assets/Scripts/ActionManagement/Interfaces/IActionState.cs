using System;
using System.Collections;

namespace Assets.Scripts.ActionManagement.Interfaces
{
	public interface IActionState<TTransition>
	where TTransition : struct
	{
		TTransition GetTransitionIn();

		IEnumerator ExecuteAction(TTransition transitionIn, Func<TTransition?> getTransitionOut);
	}
}
