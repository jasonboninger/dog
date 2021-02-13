namespace Assets.Scripts.ActionPlanning.Interfaces
{
	public interface IState<in TState>
	{
		void Set(TState state);

		void Reset();
	}
}
