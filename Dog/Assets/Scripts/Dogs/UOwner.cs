using BoningerWorks.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Dogs
{
	public class UOwner : MonoBehaviour
	{
		private readonly BwEvent<Vector3> _click_ = new BwEvent<Vector3>();
		public IBwEvent<Vector3> Click_ => _click_;

		private Vector2 _point;

		public void Point(InputAction.CallbackContext point)
		{
			// Set point
			_point = point.ReadValue<Vector2>();
		}

		public void Click(InputAction.CallbackContext click)
		{
			// Check if click and raycast
			if (click.performed && click.control.IsPressed() && Physics.Raycast(Camera.main.ScreenPointToRay(_point), out var hit))
			{
				// Emit click
				_click_.Invoke(hit.point);
			}
		}
	}
}
