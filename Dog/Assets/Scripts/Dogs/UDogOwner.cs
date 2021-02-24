using BoningerWorks.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Dogs
{
	public class UDogOwner : MonoBehaviour
	{
		private readonly BwEvent _click_ = new BwEvent();
		public IBwEvent Click_ => _click_;

		private readonly BwEvent<Vector3?> _point_ = new BwEvent<Vector3?>();
		public IBwEvent<Vector3?> Point_ => _point_;

		public void Point(InputAction.CallbackContext point)
		{
			// Get position
			var position = point.ReadValue<Vector2>();
			// Get camera
			var camera = Camera.main;
			// Check if raycast hits
			if (camera != null && Physics.Raycast(camera.ScreenPointToRay(position), out var hit))
			{
				// Emit point
				_point_.Invoke(hit.point);
			}
			else
			{
				// Emit no point
				_point_.Invoke(null);
			}
		}

		public void Click(InputAction.CallbackContext click)
		{
			// Check if pressed
			if (click.control.IsPressed())
			{
				// Emit click
				_click_.Invoke();
			}
		}
	}
}
