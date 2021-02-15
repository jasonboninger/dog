using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Static
{
	public static class BwEnumerator
	{
		private const float _TIME_ZERO = 0.01f;

		public static void StopCoroutineIfExists(this MonoBehaviour monoBehaviour, Coroutine coroutine)
		{
			// Check if coroutine exists
			if (coroutine != null)
			{
				// Stop coroutine
				monoBehaviour.StopCoroutine(coroutine);
			}
		}

		public static IEnumerator ExecuteInParallel
		(
			IEnumerator enumeratorOne,
			IEnumerator enumeratorTwo,
			IEnumerator enumeratorThree = null,
			IEnumerator enumeratorFour = null,
			IEnumerator enumeratorFive = null
		)
		{
			// Create complete
			bool completeOne;
			bool completeTwo;
			bool completeThree;
			bool completeFour;
			bool completeFive;
			// Loop until complete
			do
			{
				// Set complete
				completeOne = !enumeratorOne.MoveNext();
				completeTwo = !enumeratorTwo.MoveNext();
				completeThree = enumeratorThree == null || !enumeratorThree.MoveNext();
				completeFour = enumeratorFour == null || !enumeratorFour.MoveNext();
				completeFive = enumeratorFive == null || !enumeratorFive.MoveNext();
#if UNITY_EDITOR
				// Check if current is not null
				if
				(
					enumeratorOne.Current != null
					|| enumeratorTwo.Current != null
					|| (enumeratorThree != null && enumeratorThree.Current != null)
					|| (enumeratorFour != null && enumeratorFour.Current != null)
					|| (enumeratorFive != null && enumeratorFive.Current != null)
				)
				{
					// Log error
					Debug.LogError("Enumerators executing in parallel should only yield null because it is always the duration between executions.");
				}
#endif
				// Wait a frame
				yield return null;
			}
			while (!completeOne || !completeTwo || !completeThree || !completeFour || !completeFive);
		}

		public static IEnumerator ExecuteOverTime(Action<float> update, float from, float to, float time, bool smooth = true)
		{
			// Create progress
			Func<float, float, float, float> progress;
			// Check if smooth
			if (smooth)
			{
				// Set progress
				progress = (f, t, ti) => Mathf.SmoothStep(f, t, ti);
			}
			else
			{
				// Set progress
				progress = (f, t, ti) => Mathf.Lerp(f, t, ti);
			}
			// Return execute over time
			return _ExecuteOverTime(update, from, to, time, (l, r) => l == r, progress);
		}
		public static IEnumerator ExecuteOverTime(Action<Vector2> update, Vector2 from, Vector2 to, float time, bool smooth = true)
		{
			// Create progress
			Func<Vector2, Vector2, float, Vector2> progress;
			// Check if smooth
			if (smooth)
			{
				// Set progress
				progress = (f, t, ti) => new Vector2(Mathf.SmoothStep(f.x, t.x, ti), Mathf.SmoothStep(f.y, t.y, ti));
			}
			else
			{
				// Set progress
				progress = (f, t, ti) => new Vector2(Mathf.Lerp(f.x, t.x, ti), Mathf.Lerp(f.y, t.y, ti));
			}
			// Return execute over time
			return _ExecuteOverTime(update, from, to, time, (l, r) => l.Equals(r), progress);
		}
		public static IEnumerator ExecuteOverTime(Action<Vector3> update, Vector3 from, Vector3 to, float time, bool smooth = true)
		{
			// Create progress
			Func<Vector3, Vector3, float, Vector3> progress;
			// Check if smooth
			if (smooth)
			{
				// Set progress
				progress = (f, t, ti) => new Vector3(Mathf.SmoothStep(f.x, t.x, ti), Mathf.SmoothStep(f.y, t.y, ti), Mathf.SmoothStep(f.z, t.z, ti));
			}
			else
			{
				// Set progress
				progress = (f, t, ti) => new Vector3(Mathf.Lerp(f.x, t.x, ti), Mathf.Lerp(f.y, t.y, ti), Mathf.Lerp(f.z, t.z, ti));
			}
			// Return execute over time
			return _ExecuteOverTime(update, from, to, time, (l, r) => l.Equals(r), progress);
		}
		public static IEnumerator ExecuteOverTime(Action<Vector4> update, Vector4 from, Vector4 to, float time, bool smooth = true)
		{
			// Create progress
			Func<Vector4, Vector4, float, Vector4> progress;
			// Check if smooth
			if (smooth)
			{
				// Set progress
				progress = (f, t, ti) => new Vector4
					(
						Mathf.SmoothStep(f.x, t.x, ti),
						Mathf.SmoothStep(f.y, t.y, ti),
						Mathf.SmoothStep(f.z, t.z, ti),
						Mathf.SmoothStep(f.w, t.w, ti)
					);
			}
			else
			{
				// Set progress
				progress = (f, t, ti) => new Vector4
					(
						Mathf.Lerp(f.x, t.x, ti),
						Mathf.Lerp(f.y, t.y, ti),
						Mathf.Lerp(f.z, t.z, ti),
						Mathf.Lerp(f.w, t.w, ti)
					);
			}
			// Return execute over time
			return _ExecuteOverTime(update, from, to, time, (l, r) => l.Equals(r), progress);
		}
		private static IEnumerator _ExecuteOverTime<TValue>
		(
			Action<TValue> update,
			TValue from,
			TValue to,
			float time,
			Func<TValue, TValue, bool> equals,
			Func<TValue, TValue, float, TValue> progress
		)
		{
			// Check if time is zero
			if (time < _TIME_ZERO)
			{
				// Execute update
				update(to);
				// Stop execution
				yield break;
			}
			// Create current time
			var timeCurrent = 0f;
			// Execute updates
			while (true)
			{
				// Add to current time
				timeCurrent += Time.deltaTime;
				// Get normalized time
				var timeNormalized = timeCurrent / time;
				// Get value
				var value = progress(from, to, timeNormalized);
				// Execute update
				update(value);
				// Check if value equals to
				if (equals(value, to))
				{
					// Stop execution
					yield break;
				}
				// Wait a frame
				yield return null;
			}
		}
	}
}
