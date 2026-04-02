using System;

namespace BossKeyReborn;

internal static class SingleInstanceActivator
{
	public static void TryActivateExistingWindow()
	{
		nint num = NativeMethods.FindWindow(null, "老板键重制版");
		if (num != IntPtr.Zero)
		{
			NativeMethods.ShowWindow(num, 9);
			NativeMethods.SetForegroundWindow(num);
		}
	}
}
