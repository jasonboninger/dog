using Assets.Scripts.Utilities;

namespace Assets.Scripts.ActionPlanning
{
	partial class ActionPlanner<TState, TAction>
	{
		private class Steps
		{
			private readonly Reusable<Step> _steps;
			
			public Steps(States states)
			{
				// Set steps
				_steps = new Reusable<Step>(step =>
				{
					// Release states
					states.Release(step.from);
					states.Release(step.to);
					// Clear action
					step.action = default;
				});
			}

			public Step Get(TState from, TState to, TAction action)
			{
				// Get step
				var step = _steps.GetOrCreate();
				// Set step
				step.from = from;
				step.to = to;
				step.action = action;
				// Return step
				return step;
			}

			public void Release(Step step)
			{
				// Return step to pool
				_steps.ReturnToPool(step);
			}
		}
	}
}
