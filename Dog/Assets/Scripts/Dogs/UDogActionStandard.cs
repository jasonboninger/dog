namespace Assets.Scripts.Dogs
{
	public abstract class UDogActionStandard : UDogAction
	{
		public bool Global { get; }

		protected UDogActionStandard(bool global) => Global = global;
	}
}
