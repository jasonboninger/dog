namespace Assets.Scripts.ActionPlanning.Interfaces
{
	public interface IStep<out TState, out TAction>
	{
		TState From { get; }
		TState To { get; }
		TAction Action { get; }
	}
}
