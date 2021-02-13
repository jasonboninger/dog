using Assets.Scripts.Utilities;

namespace Assets.Scripts.ActionPlanning
{
	partial class ActionPlanner<TState, TAction>
	{
		private class States
		{
			private readonly Reusable<TState> _states = new Reusable<TState>(state => state.Reset());

			public TState Get(TState state = null)
			{
				// Get new state
				var stateNew = _states.GetOrCreate();
				// Check if state exists
				if (state != null)
				{
					// Set new state
					stateNew.Set(state);
				}
				// Return new state
				return stateNew;
			}

			public void Release(TState state)
			{
				// Return state to pool
				_states.ReturnToPool(state);
			}
		}
	}
}
