using Assets.Scripts.ActionManagement.Interfaces;
using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.States;

namespace Assets.Scripts.Dogs.Interfaces
{
	public interface IDogAction : IActionState<float>, IAction<Dog> { }
}
