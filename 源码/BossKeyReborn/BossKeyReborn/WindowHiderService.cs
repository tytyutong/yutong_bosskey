using System;
using System.Collections.Generic;
using System.Linq;

namespace BossKeyReborn;

public sealed class WindowHiderService
{
	private readonly int _ownProcessId;

	private readonly Dictionary<nint, WindowInfo> _hiddenWindows = new Dictionary<nint, WindowInfo>();

	private AppConfig _config;

	private static readonly HashSet<string> BrowserProcesses = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "chrome", "msedge", "firefox", "iexplore", "qqbrowser", "360se", "360chrome", "brave", "opera" };

	private static readonly string[] BrowserClassMarkers = new string[6] { "Chrome_WidgetWin", "Mozilla", "IEFrame", "ApplicationFrameWindow", "Maxthon", "OperaWindowClass" };

	private static readonly string[] BrowserTitleMarkers = new string[7] { "Microsoft Edge", "Google Chrome", "Mozilla Firefox", "QQBrowser", "Brave", "Opera", "360" };

	private static readonly HashSet<string> PlayerProcesses = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "potplayermini64", "potplayer", "vlc", "qmplayer", "mpc-hc", "mpc-be", "kmplayer" };

	private static readonly HashSet<string> ChatProcesses = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "wechat", "qq", "tim", "telegram", "slack", "feishu", "lark", "dingtalk" };

	private static readonly HashSet<string> DownloaderProcesses = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "thunder", "idman", "motrix", "qbittorrent", "transmission-qt" };

	private static readonly HashSet<string> StockProcesses = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "hexin", "tonghuashun", "eastmoney", "winstock" };

	private static readonly HashSet<string> GameProcesses = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "game", "steam", "wegame", "leagueclientux", "dota2", "cs2" };

	public bool HasHiddenWindows => _hiddenWindows.Count > 0;

	public WindowHiderService(AppConfig config)
	{
		_config = config.Clone();
		_ownProcessId = Environment.ProcessId;
	}

	public void UpdateConfig(AppConfig config)
	{
		_config = config.Clone();
	}

	public void ToggleHide()
	{
		if (HasHiddenWindows)
		{
			RestoreAll();
		}
		else
		{
			HideAll();
		}
	}

	public void HideAll()
	{
		foreach (WindowInfo item in NativeMethods.EnumerateVisibleWindows())
		{
			if (ShouldHide(item) && !_hiddenWindows.ContainsKey(item.Handle) && NativeMethods.ShowWindow(item.Handle, 0))
			{
				_hiddenWindows[item.Handle] = item;
			}
		}
	}

	public void RestoreAll()
	{
		KeyValuePair<nint, WindowInfo>[] array = _hiddenWindows.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<nint, WindowInfo> keyValuePair = array[i];
			nint num2 = keyValuePair.Key;
			NativeMethods.ShowWindow(num2, 9);
			_hiddenWindows.Remove(num2);
		}
	}

	private bool ShouldHide(WindowInfo window)
	{
		if (window.ProcessId == _ownProcessId)
		{
			return false;
		}
		string processName = window.ProcessName.Trim();
		string title = window.Title.Trim();
		if (_config.HiddenProcesses.Any((string item) => processName.Equals(item, StringComparison.OrdinalIgnoreCase) || (processName + ".exe").Equals(item, StringComparison.OrdinalIgnoreCase)))
		{
			return true;
		}
		if (_config.HiddenTitleKeywords.Any((string item) => title.Contains(item, StringComparison.OrdinalIgnoreCase)))
		{
			return true;
		}
		if (_config.HideBrowser && BrowserProcesses.Contains(processName))
		{
			return true;
		}
		if (_config.HideBrowser && BrowserClassMarkers.Any((string marker) => window.ClassName.Contains(marker, StringComparison.OrdinalIgnoreCase)))
		{
			return true;
		}
		if (_config.HideBrowser && BrowserTitleMarkers.Any((string marker) => title.Contains(marker, StringComparison.OrdinalIgnoreCase)))
		{
			return true;
		}
		if (_config.HidePlayer && PlayerProcesses.Contains(processName))
		{
			return true;
		}
		if (_config.HideChat && ChatProcesses.Contains(processName))
		{
			return true;
		}
		if (_config.HideDownloader && DownloaderProcesses.Contains(processName))
		{
			return true;
		}
		if (_config.HideStock && StockProcesses.Contains(processName))
		{
			return true;
		}
		if (_config.HideGame && GameProcesses.Contains(processName))
		{
			return true;
		}
		return false;
	}
}
