using UnityEngine;

namespace MobileMenu
{
	public class MobileMenuColorScreen : MobileMenuScreen
	{
		public override void Instantiate()
		{
			transform = Object.Instantiate(RDConstants.data.prefab_colorsScreen).transform;
		}
	}
}
