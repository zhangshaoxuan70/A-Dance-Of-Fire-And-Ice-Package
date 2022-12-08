namespace MobileMenu
{
	public class MobileMenuGrabbablePlanet : MobileMenuGrabbable
	{
		public scrPlanet planet;

		public override void Ungrab()
		{
			scrSfx.instance.PlaySfx(SfxSound.PlanetRelease);
		}
	}
}
