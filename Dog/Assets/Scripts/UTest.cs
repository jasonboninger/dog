using Assets.Scripts.ActionPlanning;
using Assets.Scripts.ActionPlanning.Interfaces;
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
			public void Set(int health)
			{
				this.health = health;
			}
		}

		private class Weapon
		{
			public int ammunition;

			public void Set(Weapon weapon)
			{
				ammunition = weapon.ammunition;
			}
			public void Set(int ammunition)
			{
				this.ammunition = ammunition;
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
			public void Set(int enemyHealth, int weaponAmmunition, EPosture posture)
			{
				enemy.Set(enemyHealth);
				weapon.Set(weaponAmmunition);
				this.posture = posture;
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
			public float EstimateProximity(State state) => state.enemy.health * 3;

			public bool IsAchieved(State state) => state.enemy.health <= 0;
		}

		private readonly ActionPlanner<State, IAction<State>> _actionPlanner = new ActionPlanner<State, IAction<State>>();
		private readonly Goal _goal = new Goal();

		protected void Awake()
		{
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
			state.Set
				(
					enemyHealth: 10,
					weaponAmmunition: 0,
					posture: EPosture.Laying
				);
			var plan = _actionPlanner.GetPlan();
			_actionPlanner.PopulatePlan(plan, state, _goal);
			
			
			
			Debug.Log(plan.Cycles);
			foreach (var step in plan.Steps)
			{
				Debug.Log(step.Action.GetType().Name);
			}
			
			
			
			_actionPlanner.ReleasePlan(plan);
			_actionPlanner.ReleaseState(state);
		}
	}
}
