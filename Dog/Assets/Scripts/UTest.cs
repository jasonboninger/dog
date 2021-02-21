using Assets.Scripts.ActionPlanning;
using Assets.Scripts.ActionPlanning.Interfaces;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	public class UTest : MonoBehaviour
	{
		private enum EPosture
		{
			Standing,
			Crouching,
			Laying,
		}

		private class Enemy
		{
			public int health;

			public void Set(Enemy enemy)
			{
				health = enemy.health;
			}
		}

		private class Weapon
		{
			public int ammunition;

			public void Set(Weapon weapon)
			{
				ammunition = weapon.ammunition;
			}
		}

		private class State : IState<State>
		{
			public Enemy enemy;
			public Weapon weapon;
			public EPosture posture;

			public State()
			{
				enemy = new Enemy();
				weapon = new Weapon();
			}

			public void Set(State state)
			{
				enemy.Set(state.enemy);
				weapon.Set(state.weapon);
				posture = state.posture;
			}

			public void Reset() { /* Required for interface */ }
		}

		private abstract class Action : IAction<State>
		{
			public abstract void UpdateState(State state);
			
			public abstract float GetCost(State state);
			
			public abstract bool IsValid(State state);
		}

		private class Reload : Action
		{
			public override void UpdateState(State state)
			{
				state.weapon.ammunition += 1;
			}

			public override float GetCost(State state) => 1;

			public override bool IsValid(State state) => state.posture == EPosture.Standing;
		}

		private class Fire : Action
		{
			public override void UpdateState(State state)
			{
				state.enemy.health -= 1;
				state.weapon.ammunition -= 1;
			}

			public override float GetCost(State state) => 1;

			public override bool IsValid(State state) => state.posture == EPosture.Standing && state.weapon.ammunition > 0;
		}

		private class GetUp : Action
		{
			public override void UpdateState(State state)
			{
				state.posture = state.posture == EPosture.Laying ? EPosture.Crouching : EPosture.Standing;
			}

			public override float GetCost(State state) => 2;

			public override bool IsValid(State state) => state.posture != EPosture.Standing;
		}

		private class GetDown : Action
		{
			public override void UpdateState(State state)
			{
				state.posture = state.posture == EPosture.Standing ? EPosture.Crouching : EPosture.Laying;
			}

			public override float GetCost(State state) => 2;

			public override bool IsValid(State state) => state.posture != EPosture.Laying;
		}

		private class Goal : IGoal<State>
		{
			public bool IsAchieved(State state) => state.enemy.health <= 0;
			
			public float EstimateProximity(State state) => state.enemy.health * 3;
		}

		[SerializeField] private int _cyclesLimit = default;
		[SerializeField] private int _startingHealth = default;

		private ActionPlanner<State, Action> _actionPlanner;
		private Goal _goal;

		protected void Awake()
		{
			_actionPlanner = new ActionPlanner<State, Action>(_cyclesLimit);
			_goal = new Goal();
			_actionPlanner.AddAction(new Reload());
			_actionPlanner.AddAction(new Fire());
			_actionPlanner.AddAction(new GetUp());
			_actionPlanner.AddAction(new GetDown());
		}

		protected void Start()
		{
			_PlanActions();
		}

		private void _PlanActions()
		{
			var state = _actionPlanner.GetState();
			state.enemy.health = _startingHealth;
			state.weapon.ammunition = 0;
			state.posture = EPosture.Laying;
			
			var plan = _actionPlanner.GetPlan();
			_actionPlanner.PopulatePlan(plan, state, _goal);



			Debug.Log("Outcome: " + plan.Outcome);
			Debug.Log("Cycles: " + plan.Cycles);
			Debug.Log("Cost: " + plan.Cost);
			Debug.Log("Actions: " + string.Join(", ", plan.Steps.Select(step => step.Action.GetType().Name)));
			
			
			
			_actionPlanner.ReleasePlan(plan);
			_actionPlanner.ReleaseState(state);
		}
	}
}
