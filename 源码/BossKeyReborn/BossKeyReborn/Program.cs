using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace BossKeyReborn;

internal static class Program
{
	private const string MutexName = "Local\\yutong_bosskey.SingleInstance";

	[STAThread]
	private static void Main(string[] args)
	{
		bool createdNew;
		using (new Mutex(initiallyOwned: true, "Local\\yutong_bosskey.SingleInstance", out createdNew))
		{
			bool flag = args.Any((string arg) => string.Equals(arg, "--startup", StringComparison.OrdinalIgnoreCase));
			if (!createdNew)
			{
				if (!flag)
				{
					SingleInstanceActivator.TryActivateExistingWindow();
				}
			}
			else
			{
				ApplicationConfiguration.Initialize();
				Application.Run(new TrayApplicationContext(!flag));
			}
		}
	}
}
