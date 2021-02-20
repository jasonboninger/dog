using Assets.Scripts.Dogs.States;
using UnityEngine;

namespace Assets.Scripts.Dogs.Interfaces
{
	public interface IDogActionDestination
	{
		Vector2 GetPosition();
		Vector2 GetPosition(Dog state);

		bool IsReached();
		bool IsReached(Dog state);
	}
}
