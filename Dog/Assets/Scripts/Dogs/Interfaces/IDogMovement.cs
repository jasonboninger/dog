using Assets.Scripts.Dogs.States;
using UnityEngine;

namespace Assets.Scripts.Dogs.Interfaces
{
	public interface IDogMovement
	{
		Vector2 GetDestination();
		Vector2 GetDestination(Dog state);

		bool ReachedDestination();
		bool ReachedDestination(Dog state);
	}
}
