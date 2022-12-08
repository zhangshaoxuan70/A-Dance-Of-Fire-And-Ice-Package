using System;

namespace ByteSheep.Events
{
	[Flags]
	public enum TargetFilter
	{
		Static = 0x7,
		StaticField = 0x1,
		StaticProperty = 0x2,
		StaticMethod = 0x4,
		Dynamic = 0x38,
		DynamicField = 0x8,
		DynamicProperty = 0x10,
		DynamicMethod = 0x20
	}
}
