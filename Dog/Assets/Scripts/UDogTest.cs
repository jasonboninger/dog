using Assets.Scripts.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
	[RequireComponent(typeof(Animator))]
	public class UDogTest : MonoBehaviour
	{
		[SerializeField] private float _speedMaximum = default;
		[SerializeField] private float _speedMinimum = default;
		[SerializeField] private float _speedAcceleration = default;
		[SerializeField] private float _distanceSlow = default;
		[SerializeField] private Transform _look = default;

		private Animator _animator;
		private AnimatorParameterBool _walking;
		private AnimatorParameterFloat _speed;
		private AnimatorParameterTrigger _ready;
		private AnimatorLayer _tailHappy;
		private int _base;
		private float _speedCurrent;
		private Vector2 _point;
		private Vector3 _destination;
		private Coroutine _readyWaiter;

		protected void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		protected void Start()
		{
			_walking = new AnimatorParameterBool(_animator, "Walking");
			_speed = new AnimatorParameterFloat(_animator, "Speed");
			_ready = new AnimatorParameterTrigger(_animator, "Ready");
			_tailHappy = new AnimatorLayer(_animator, "TailHappy");
			_base = _animator.GetLayerIndex("Base");
		}

		protected void Update()
		{
			var state = _animator.GetCurrentAnimatorStateInfo(_base);
			var transition = _animator.GetAnimatorTransitionInfo(_base);
			var idle = !transition.IsName("Idle -> Ready") && state.IsName("Idle");
			var walk = transition.IsName("Idle -> Walk") || state.IsName("Walk");

			_walking.Value = !_destination.Equals(transform.position);

			if (_walking.Value && (idle || walk))
			{
				_Walk(_destination);
			}

			_Mood();

			_Look(!walk);
			_Ready(idle);
		}

		public void Point(InputAction.CallbackContext point)
		{
			_point = point.ReadValue<Vector2>();
		}

		public void Click(InputAction.CallbackContext click)
		{
			if (click.performed && click.control.IsPressed() && Physics.Raycast(Camera.main.ScreenPointToRay(_point), out var hit))
			{
				_destination = hit.point;
				_destination.y = 0;
			}
		}

		private void _Walk(Vector3 destination)
		{
			var direction = destination - transform.position;
			var distanceRemaining = direction.magnitude;

			_speed.Value = Mathf.Lerp(_speedMinimum, _speedCurrent, distanceRemaining / _distanceSlow);

			var distanceTravel = _speed.Value * Time.deltaTime;
			var position = Vector3.MoveTowards(transform.position, destination, distanceTravel);
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
			if (_walking.Value)
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
			_tailHappy.Weight = Mathf.MoveTowards(_tailHappy.Weight, happy, speed * Time.deltaTime);
		}

		private void _Ready(bool ready)
		{
			if (!ready)
			{
				if (_readyWaiter != null)
				{
					StopCoroutine(_readyWaiter);
				}
				_readyWaiter = null;
			}
			else
			{
				if (_readyWaiter == null)
				{
					IEnumerator waitForReady()
					{
						yield return new WaitForSeconds(Random.Range(5, 10));
						_ready.Trigger();
						_readyWaiter = null;
					}
					_readyWaiter = StartCoroutine(waitForReady());
				}
			}
		}
	}
}
