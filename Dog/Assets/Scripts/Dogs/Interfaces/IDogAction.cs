using Assets.Scripts.ActionManagement.Interfaces;
using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.Models;
using Assets.Scripts.Dogs.States;

namespace Assets.Scripts.Dogs.Interfaces
{
	public interface IDogAction : IActionState, IAction<Dog>
	{
		void Initialize(Controls controls);
	}
}
