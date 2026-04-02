using System;

namespace BossKeyReborn;

[Flags]
public enum KeyModifiers
{
	None = 0,
	Alt = 1,
	Control = 2,
	Shift = 4,
	Win = 8
}
