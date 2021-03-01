using Assets.Scripts.Dogs.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Dogs
{
	public class UDogOwner : MonoBehaviour
	{
		private class DogLaserPointer : IDogLaserPointer
		{
			public bool On { get; set; }
			public Vector3? Position { get; set; }
		}

		private readonly DogLaserPointer _laserPointer = new DogLaserPointer();
		public IDogLaserPointer LaserPointer => _laserPointer;

		public void Point(InputAction.CallbackContext point)
		{
			// Get position
			var position = point.ReadValue<Vector2>();
			// Get camera
			var camera = Camera.main;
			// Check if raycast hits
			if (camera != null && Physics.Raycast(camera.ScreenPointToRay(position), out var hit))
			{
				// Set position
				_laserPointer.Position = hit.point;
			}
			else
			{
				// Set no position
				_laserPointer.Position = null;
			}
		}

		public void Click(InputAction.CallbackContext click)
		{
			// Check if performed
			if (click.performed)
			{
				// Toggle laser pointer on
				_laserPointer.On ^= true;
			}
		}
	}
}
