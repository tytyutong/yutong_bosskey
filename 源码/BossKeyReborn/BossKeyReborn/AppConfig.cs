using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace BossKeyReborn;

public sealed class AppConfig
{
	public bool HideBrowser { get; set; }

	public bool HidePlayer { get; set; }

	public bool HideChat { get; set; }

	public bool HideDownloader { get; set; }

	public bool HideStock { get; set; }

	public bool HideGame { get; set; }

	public bool RunAtStartup { get; set; }

	public bool HideTrayIcon { get; set; }

	public bool CameraAutoclose { get; set; }

	public bool MouseMiddleHide { get; set; }

	public bool MuteWhenHidden { get; set; }

	public bool RequirePasswordOnRestore { get; set; }

	public string RestorePassword { get; set; } = string.Empty;

	public string RestorePasswordHint { get; set; } = "请输入密码：";

	public string SkinName { get; set; } = "classic-blue";

	public Keys HotkeyKey { get; set; } = Keys.D1;

	public KeyModifiers HotkeyModifiers { get; set; } = KeyModifiers.Alt;

	public Keys LockHotkeyKey { get; set; } = Keys.D2;

	public KeyModifiers LockHotkeyModifiers { get; set; } = KeyModifiers.Alt;

	public List<string> HiddenProcesses { get; } = new List<string>();

	public List<string> HiddenTitleKeywords { get; } = new List<string>();

	public static AppConfig CreateDefault()
	{
		return new AppConfig
		{
			HideBrowser = false,
			HidePlayer = false,
			HideChat = false,
			HideDownloader = false,
			HideStock = false,
			HideGame = false,
			RunAtStartup = false,
			HideTrayIcon = false,
			CameraAutoclose = false,
			MouseMiddleHide = false,
			MuteWhenHidden = false,
			RequirePasswordOnRestore = false,
			RestorePassword = string.Empty,
			RestorePasswordHint = "请输入密码：",
			SkinName = "classic-blue"
		};
	}

	public string SerializeProcesses()
	{
		return string.Join(",", HiddenProcesses);
	}

	public string SerializeTitles()
	{
		return string.Join(",", HiddenTitleKeywords);
	}

	public string SerializeHotkey()
	{
		return $"{HotkeyModifiers}|{HotkeyKey}";
	}

	public string SerializeLockHotkey()
	{
		return $"{LockHotkeyModifiers}|{LockHotkeyKey}";
	}

	public static void ParseHotkey(string? raw, AppConfig config)
	{
		if (string.IsNullOrWhiteSpace(raw))
		{
			return;
		}
		string[] array = raw.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (array.Length == 2)
		{
			if (int.TryParse(array[0], out var result))
			{
				config.HotkeyModifiers = (KeyModifiers)result;
			}
			if (int.TryParse(array[1], out var result2))
			{
				config.HotkeyKey = (Keys)result2;
			}
		}
	}

	public static void ParseLockHotkey(string? raw, AppConfig config)
	{
		if (string.IsNullOrWhiteSpace(raw))
		{
			return;
		}
		string[] array = raw.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (array.Length == 2)
		{
			if (int.TryParse(array[0], out var result))
			{
				config.LockHotkeyModifiers = (KeyModifiers)result;
			}
			if (int.TryParse(array[1], out var result2))
			{
				config.LockHotkeyKey = (Keys)result2;
			}
		}
	}

	public AppConfig Clone()
	{
		AppConfig appConfig = CreateDefault();
		appConfig.HideBrowser = HideBrowser;
		appConfig.HidePlayer = HidePlayer;
		appConfig.HideChat = HideChat;
		appConfig.HideDownloader = HideDownloader;
		appConfig.HideStock = HideStock;
		appConfig.HideGame = HideGame;
		appConfig.RunAtStartup = RunAtStartup;
		appConfig.HideTrayIcon = HideTrayIcon;
		appConfig.CameraAutoclose = CameraAutoclose;
		appConfig.MouseMiddleHide = MouseMiddleHide;
		appConfig.MuteWhenHidden = MuteWhenHidden;
		appConfig.RequirePasswordOnRestore = RequirePasswordOnRestore;
		appConfig.RestorePassword = RestorePassword;
		appConfig.RestorePasswordHint = RestorePasswordHint;
		appConfig.SkinName = SkinName;
		appConfig.HotkeyKey = HotkeyKey;
		appConfig.HotkeyModifiers = HotkeyModifiers;
		appConfig.LockHotkeyKey = LockHotkeyKey;
		appConfig.LockHotkeyModifiers = LockHotkeyModifiers;
		appConfig.HiddenProcesses.AddRange(HiddenProcesses);
		appConfig.HiddenTitleKeywords.AddRange(HiddenTitleKeywords);
		return appConfig;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder3 = stringBuilder2;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(9, 2, stringBuilder2);
		handler.AppendLiteral("Hotkey: ");
		handler.AppendFormatted(HotkeyModifiers);
		handler.AppendLiteral("+");
		handler.AppendFormatted(HotkeyKey);
		stringBuilder3.AppendLine(ref handler);
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder4 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(27, 3, stringBuilder2);
		handler.AppendLiteral("Browser: ");
		handler.AppendFormatted(HideBrowser);
		handler.AppendLiteral(", Player: ");
		handler.AppendFormatted(HidePlayer);
		handler.AppendLiteral(", Chat: ");
		handler.AppendFormatted(HideChat);
		stringBuilder4.AppendLine(ref handler);
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder5 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(29, 3, stringBuilder2);
		handler.AppendLiteral("Downloader: ");
		handler.AppendFormatted(HideDownloader);
		handler.AppendLiteral(", Stock: ");
		handler.AppendFormatted(HideStock);
		handler.AppendLiteral(", Game: ");
		handler.AppendFormatted(HideGame);
		stringBuilder5.AppendLine(ref handler);
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder6 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder2);
		handler.AppendLiteral("Startup: ");
		handler.AppendFormatted(RunAtStartup);
		stringBuilder6.AppendLine(ref handler);
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder7 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(11, 1, stringBuilder2);
		handler.AppendLiteral("Processes: ");
		handler.AppendFormatted(SerializeProcesses());
		stringBuilder7.AppendLine(ref handler);
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder8 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(8, 1, stringBuilder2);
		handler.AppendLiteral("Titles: ");
		handler.AppendFormatted(SerializeTitles());
		stringBuilder8.AppendLine(ref handler);
		return stringBuilder.ToString();
	}
}
