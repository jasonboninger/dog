using Assets.Scripts.Dogs.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Dogs
{
	public abstract class UDogActionMovement : UDogAction, IDogActionMovement
	{
		public abstract IDogActionMovement Create(GameObject gameObject, IDogActionDestination actionDestination);
	}
}
