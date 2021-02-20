using UnityEngine;

namespace Assets.Scripts.Dogs.Interfaces
{
	public interface IDogActionMovement : IDogAction
	{
		IDogActionMovement Create(GameObject gameObject, IDogActionDestination actionDestination);
	}
}
