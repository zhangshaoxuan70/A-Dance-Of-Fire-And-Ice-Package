using System;

namespace ByteSheep.Events
{
	[Flags]
	public enum TargetTypeFilter
	{
		Void = 0x1,
		String = 0x2,
		Int = 0x4,
		Float = 0x8,
		Bool = 0x10,
		Color = 0x20,
		Vector2 = 0x40,
		Vector3 = 0x80,
		Object = 0x100,
		GameObject = 0x200,
		Transform = 0x400,
		Enum = 0x800,
		Any = 0xFFF
	}
}
