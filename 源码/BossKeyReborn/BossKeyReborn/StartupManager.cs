using System.Windows.Forms;
using Microsoft.Win32;

namespace BossKeyReborn;

public sealed class StartupManager
{
	private const string RunKeyPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

	private const string ValueName = "yutong_bosskey";

	public bool IsEnabled()
	{
		using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", writable: false);
				string value = registryKey?.GetValue("yutong_bosskey") as string;
		return !string.IsNullOrWhiteSpace(value);
	}

	public void Apply(bool enabled)
	{
		using RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
		if (enabled)
		{
			string executablePath = Application.ExecutablePath;
					registryKey.SetValue("yutong_bosskey", "\"" + executablePath + "\" --startup");
		}
		else
		{
					registryKey.DeleteValue("yutong_bosskey", throwOnMissingValue: false);
		}
	}
}
