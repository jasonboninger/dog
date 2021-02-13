using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utilities
{
	public class Reusable<TValue>
	where TValue : class, new()
	{
		private readonly Action<TValue> _reset;

		private readonly Stack<TValue> _values = new Stack<TValue>();

		public Reusable(Action<TValue> reset) => _reset = reset;
		
		public TValue GetOrCreate()
		{
			// Check if plan exists
			if (_values.Count > 0)
			{
				// Return value
				return _values.Pop();
			}
			// Return value
			return new TValue();
		}

		public void ReturnToPool(TValue value)
		{
			// Reset value
			_reset(value);
			// Add value
			_values.Push(value);
		}
	}
}
