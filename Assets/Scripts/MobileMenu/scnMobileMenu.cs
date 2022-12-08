namespace MobileMenu
{
	public class scnMobileMenu : ADOBase
	{
		public MobileMenuController menuController;

		public MobileMenuTutorial tutorialController;

		public MobileMenuReviewPrompt reviewPromptController;

		private void Awake()
		{
			menuController.LoadMap("main");
		}

		private void Start()
		{
			bool num = tutorialController.TryRunTutorial();
			bool flag = reviewPromptController.TryRunReviewPrompt();
			if (!num && !flag)
			{
				menuController.JumpToMenuEntrance();
			}
		}

		private void LateUpdate()
		{
		}
	}
}
