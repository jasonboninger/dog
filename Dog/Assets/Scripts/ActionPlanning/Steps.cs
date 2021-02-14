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
					// Get from
					var from = step.from;
					// Check if from exists
					if (from != null)
					{
						// Release from
						states.Release(from);
						// Clear from
						step.from = null;
					}
					// Get to
					var to = step.to;
					// Check if to exists
					if (to != null)
					{
						// Release to
						states.Release(to);
						// Clear to
						step.to = null;
					}
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
