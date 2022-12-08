using UnityEngine;

namespace MobileMenu
{
	public static class MoveDirectionHelper
	{
		public static MoveDirection Invert(this MoveDirection dir)
		{
			return 3 - dir;
		}

		public static Vector2Int GetVector(this MoveDirection dir)
		{
			int x;
			switch (dir)
			{
			default:
				x = 0;
				break;
			case MoveDirection.Left:
				x = -1;
				break;
			case MoveDirection.Right:
				x = 1;
				break;
			}
			int y;
			switch (dir)
			{
			default:
				y = 0;
				break;
			case MoveDirection.Down:
				y = -1;
				break;
			case MoveDirection.Up:
				y = 1;
				break;
			}
			return new Vector2Int(x, y);
		}
	}
}
