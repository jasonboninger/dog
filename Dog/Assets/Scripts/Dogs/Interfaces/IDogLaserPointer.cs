using UnityEngine;

namespace Assets.Scripts.Dogs.Interfaces
{
	public interface IDogLaserPointer
	{
		bool On { get; }
		Vector3? Position { get; }
	}
}
