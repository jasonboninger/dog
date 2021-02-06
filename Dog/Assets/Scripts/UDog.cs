using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
	public class UDog : MonoBehaviour
	{
		private static readonly int _animatorSpeed = Animator.StringToHash("Speed");
		private static readonly int _animatorWalking = Animator.StringToHash("Walking");

		[SerializeField] private float _speedMaximum = default;
		[SerializeField] private float _speedMinimum = default;
		[SerializeField] private float _speedAcceleration = default;
		[SerializeField] private float _distanceSlow = default;
		[SerializeField] private Transform _look = default;

		private bool _Walking { get => _animator.GetBool(_animatorWalking); set => _animator.SetBool(_animatorWalking, value); }
		private float _Speed { get => _animator.GetFloat(_animatorSpeed); set => _animator.SetFloat(_animatorSpeed, value); }
		private float _Happy { get => _animator.GetLayerWeight(_animatorTailHappy); set => _animator.SetLayerWeight(_animatorTailHappy, value); }

		private Animator _animator;
		private int _animatorTailHappy;
		private float _speedCurrent;
		private Vector2 _point;
		private Vector3 _destination;

		void Start()
		{
			_animator = GetComponentInChildren<Animator>();
			_animatorTailHappy = _animator.GetLayerIndex("TailHappy");
		}

		void Update()
		{
			var direction = _destination - transform.position;
			var distanceRemaining = direction.magnitude;

			_Speed = Mathf.Lerp(_speedMinimum, _speedCurrent, distanceRemaining / _distanceSlow);

			_Walking = _Speed > _speedMinimum;

			var distanceTravel = _Speed * Time.deltaTime;
			var position = Vector3.MoveTowards(transform.position, _destination, distanceTravel);
			if (!direction.normalized.Equals(Vector3.zero))
			{
				var rotation = Quaternion.LookRotation(direction);
				transform.SetPositionAndRotation(position, rotation);
				_speedCurrent = Mathf.Clamp(_speedCurrent + _speedAcceleration * Time.deltaTime, _speedMinimum, _speedMaximum);
				_Look(false);
			}
			else
			{
				transform.position = position;
				_speedCurrent = _speedMinimum;
				_Look(true);
			}

			_Mood();
		}

		public void Point(InputAction.CallbackContext point) => _point = point.ReadValue<Vector2>();

		public void Click(InputAction.CallbackContext click)
		{
			if (click.performed && click.control.IsPressed())
			{
				Debug.Log("CLICKED @ " + _point);
				if (Physics.Raycast(Camera.main.ScreenPointToRay(_point), out var hit))
				{
					Debug.Log("HIT @ " + _point);
					Debug.DrawRay(hit.point, Vector3.up, Color.white, duration: 1);
					_destination = hit.point;
					_destination.y = 0;
				}
			}
		}

		private void _Look(bool look)
		{
			if (!look)
			{
				_look.transform.localRotation = Quaternion.identity;
			}
			else
			{
				var speed = 360;
				var position = _look.transform.position;
				var camera = Camera.main.transform.position;
				var direction = camera - position;
				var directionFlat = new Vector3(direction.x, 0, direction.z);
				var forward = transform.forward;
				var forwardFlat = new Vector3(forward.x, 0, forward.z);
				var up = Vector3.up;
				var angleHorizontalMaximum = 120;
				var angleHorizontalIdeal = Vector3.SignedAngle(forwardFlat, directionFlat, up);
				var angleHorizontal = Mathf.Clamp(angleHorizontalIdeal, -angleHorizontalMaximum, angleHorizontalMaximum);
				var angleHorizontalPercent = Mathf.Abs(angleHorizontal / angleHorizontalIdeal);
				var angleVertical = Vector3.Angle(directionFlat, direction) * (position.y > camera.y ? 1 : -1) * angleHorizontalPercent;
				var anglePercent = 0.9f;
				var rotation = Quaternion.Euler(angleVertical * anglePercent, angleHorizontal * anglePercent, 0);
				_look.transform.localRotation = Quaternion.RotateTowards(_look.transform.localRotation, rotation, speed * Time.deltaTime);
			}
		}

		private void _Mood()
		{
			float speed;
			float happy;
			if (_Walking)
			{
				speed = 2;
				happy = 0.2f;
			}
			else
			{
				var position = transform.position;
				position.y = 0;
				var camera = Camera.main.transform.position;
				camera.y = 0;
				var distance = (position - camera).magnitude;
				var goal = 45;
				var maximum = 75;
				speed = 2;
				happy = Mathf.Lerp(0, 1, 1 - (distance - goal) / (maximum - goal));
			}
			_Happy = Mathf.MoveTowards(_Happy, happy, speed * Time.deltaTime);
		}
	}
}
