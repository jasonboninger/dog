using Assets.Scripts.Pathfinding.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
	public partial class Pathfinder
	{
		private readonly float _precision;
		private readonly Searcher _searcher = new Searcher();
		private readonly List<Agent> _agents = new List<Agent>();
		private readonly List<Obstacle> _obstacles = new List<Obstacle>();
		private readonly Stack<Path> _paths = new Stack<Path>();
		private readonly Dictionary<int, Agent> _sizeToAgentMappings = new Dictionary<int, Agent>();

		public Pathfinder(float precision = 1) => _precision = precision;

		public IObstacle AddObstacle(Vector2 position, float radius)
		{
			// Get scaled position
			var positionScaled = position * _precision;
			// Get scaled radius
			var radiusScaled = radius * _precision;
			// Create obstacle
			var obstacle = new Obstacle(positionScaled, radiusScaled);
			// Add obstacle
			_obstacles.Add(obstacle);
			// Run through agents
			for (int i = 0; i < _agents.Count; i++)
			{
				// Add obstacle
				_agents[i].AddObstacle(obstacle);
			}
			// Return obstacle
			return obstacle;
		}

		public void RemoveObstacle(IObstacle obstacle)
		{
			// Get converted obstacle
			var obstacleConverted = (Obstacle)obstacle;
			// Remove obstacle
			_obstacles.Remove(obstacleConverted);
			// Run through agents
			for (int i = 0; i < _agents.Count; i++)
			{
				// Remove obstacle
				_agents[i].RemoveObstacle(obstacleConverted);
			}
		}

		public IPath CreatePath()
		{
			// Check if path exists
			if (_paths.Count > 0)
			{
				// Return path
				return _paths.Pop();
			}
			// Return path
			return new Path();
		}

		public void PopulatePath(IPath path, Vector2 start, Vector2 end, float radius)
		{
			// Get converted path
			var pathConverted = (Path)path;
			// Get scaled start
			var startScaled = start * _precision;
			// Get scaled end
			var endScaled = end * _precision;
			// Get or create agent
			var agent = _GetOrCreateAgent(radius);
			// Populate path
			_searcher.PopulatePath(pathConverted, agent, startScaled, endScaled);
			// Get positions
			var positions = pathConverted.positions;
			// Run through positions
			for (int i = 0; i < positions.Count; i++)
			{
				// Scale position
				positions[i] /= _precision;
			}
		}

		public void ReleasePath(IPath path)
		{
			// Get converted path
			var pathConverted = (Path)path;
			// Clear positions
			pathConverted.positions.Clear();
			// Add path
			_paths.Push(pathConverted);
		}

		private Agent _GetOrCreateAgent(float radius)
		{
			// Get size
			var size = Mathf.CeilToInt(radius * 2 * _precision);
			// Try to get agent
			if (!_sizeToAgentMappings.TryGetValue(size, out var agent))
			{
				// Create agent
				agent = new Agent(size);
				// Run through obstacles
				for (int i = 0; i < _obstacles.Count; i++)
				{
					// Add obstacle
					agent.AddObstacle(_obstacles[i]);
				}
				// Add agent
				_sizeToAgentMappings.Add(size, agent);
				_agents.Add(agent);
			}
			// Return agent
			return agent;
		}
	}
}
