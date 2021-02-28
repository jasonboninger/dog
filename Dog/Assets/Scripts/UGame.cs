using Assets.Scripts.Dogs;
using UnityEngine;

namespace Assets.Scripts
{
	public class UGame : MonoBehaviour
	{
		[SerializeField] private UDogOwner _prefabDogOwner = default;
		[SerializeField] private UDog _prefabDog = default;

		protected void Awake()
		{
			// Create dog owner
			var dogOwwner = Instantiate(_prefabDogOwner);
			// Create dog
			Instantiate(_prefabDog).Initialize(dogOwwner);
		}
	}
}
