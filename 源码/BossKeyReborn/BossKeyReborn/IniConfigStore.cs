using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BossKeyReborn;

public sealed class IniConfigStore
{
	private const string SectionName = "Bosskey";

	private readonly string _configPath;

	public string ConfigPath => _configPath;

	public IniConfigStore(string configPath)
	{
		_configPath = configPath;
	}

	public AppConfig Load()
	{
		Directory.CreateDirectory(Path.GetDirectoryName(_configPath));
		if (!File.Exists(_configPath))
		{
			AppConfig appConfig = AppConfig.CreateDefault();
			Save(appConfig);
			return appConfig;
		}
		Dictionary<string, string> values = ParseIni(File.ReadAllLines(_configPath, Encoding.UTF8));
		AppConfig appConfig2 = AppConfig.CreateDefault();
		appConfig2.HideBrowser = GetBool(values, "HideBrowser");
		appConfig2.HidePlayer = GetBool(values, "HidePlayer");
		appConfig2.HideChat = GetBool(values, "HideChat");
		appConfig2.HideDownloader = GetBool(values, "HideDownloader");
		appConfig2.HideStock = GetBool(values, "HideStock");
		appConfig2.HideGame = GetBool(values, "HideGame");
		appConfig2.RunAtStartup = GetBool(values, "RunAtStartup");
		appConfig2.HideTrayIcon = GetBool(values, "HideRightIco");
		appConfig2.CameraAutoclose = GetBool(values, "CameraAutoclose");
		appConfig2.MouseMiddleHide = GetBool(values, "MouseMiddleHide");
		appConfig2.MuteWhenHidden = GetBool(values, "MuteWhenHidden");
		appConfig2.RequirePasswordOnRestore = GetBool(values, "NeedPasswordOnRestore");
		appConfig2.RestorePassword = GetValue(values, "RestorePassword");
		appConfig2.RestorePasswordHint = GetValue(values, "PasswordHint");
		if (string.IsNullOrWhiteSpace(appConfig2.RestorePasswordHint))
		{
			appConfig2.RestorePasswordHint = "请输入密码：";
		}
		appConfig2.SkinName = GetValue(values, "SkinName");
		if (string.IsNullOrWhiteSpace(appConfig2.SkinName))
		{
			appConfig2.SkinName = "classic-blue";
		}
		AppConfig.ParseHotkey(GetValue(values, "HideShowSoftKey"), appConfig2);
		AppConfig.ParseLockHotkey(GetValue(values, "LockToggleKey"), appConfig2);
		appConfig2.HiddenProcesses.AddRange(ParseList(GetValue(values, "hidesoft")));
		appConfig2.HiddenTitleKeywords.AddRange(ParseList(GetValue(values, "hidewindow")));
		return appConfig2;
	}

	public void Save(AppConfig config)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(_configPath));
		int num = 20;
		List<string> list = new List<string>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<string> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = "[Bosskey]";
		num2++;
		span[num2] = "HideBrowser=" + ToBool(config.HideBrowser);
		num2++;
		span[num2] = "HidePlayer=" + ToBool(config.HidePlayer);
		num2++;
		span[num2] = "HideChat=" + ToBool(config.HideChat);
		num2++;
		span[num2] = "HideDownloader=" + ToBool(config.HideDownloader);
		num2++;
		span[num2] = "HideStock=" + ToBool(config.HideStock);
		num2++;
		span[num2] = "HideGame=" + ToBool(config.HideGame);
		num2++;
		span[num2] = "RunAtStartup=" + ToBool(config.RunAtStartup);
		num2++;
		span[num2] = "HideRightIco=" + ToBool(config.HideTrayIcon);
		num2++;
		span[num2] = "CameraAutoclose=" + ToBool(config.CameraAutoclose);
		num2++;
		span[num2] = "MouseMiddleHide=" + ToBool(config.MouseMiddleHide);
		num2++;
		span[num2] = "MuteWhenHidden=" + ToBool(config.MuteWhenHidden);
		num2++;
		span[num2] = "NeedPasswordOnRestore=" + ToBool(config.RequirePasswordOnRestore);
		num2++;
		span[num2] = "RestorePassword=" + config.RestorePassword;
		num2++;
		span[num2] = "PasswordHint=" + config.RestorePasswordHint;
		num2++;
		span[num2] = "SkinName=" + config.SkinName;
		num2++;
		span[num2] = "HideShowSoftKey=" + config.SerializeHotkey();
		num2++;
		span[num2] = "LockToggleKey=" + config.SerializeLockHotkey();
		num2++;
		span[num2] = "hidesoft=" + config.SerializeProcesses();
		num2++;
		span[num2] = "hidewindow=" + config.SerializeTitles();
		num2++;
		List<string> contents = list;
		File.WriteAllLines(_configPath, contents, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
	}

	private static Dictionary<string, string> ParseIni(IEnumerable<string> lines)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		string a = null;
		foreach (string line in lines)
		{
			string text = line.Trim();
			if (string.IsNullOrWhiteSpace(text) || text.StartsWith(';') || text.StartsWith('#'))
			{
				continue;
			}
			if (text.StartsWith('[') && text.EndsWith(']'))
			{
				string text2 = text;
				a = text2.Substring(1, text2.Length - 1 - 1);
			}
			else if (string.Equals(a, "Bosskey", StringComparison.OrdinalIgnoreCase))
			{
				int num = text.IndexOf('=');
				if (num > 0)
				{
					string key = text.Substring(0, num).Trim();
					string text2 = text;
					int num2 = num + 1;
					dictionary[key] = text2.Substring(num2, text2.Length - num2).Trim();
				}
			}
		}
		return dictionary;
	}

	private static string GetValue(IReadOnlyDictionary<string, string> values, string key)
	{
		string value;
		return values.TryGetValue(key, out value) ? value : string.Empty;
	}

	private static bool GetBool(IReadOnlyDictionary<string, string> values, string key)
	{
		string value;
		return values.TryGetValue(key, out value) && (value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase));
	}

	private static IEnumerable<string> ParseList(string? raw)
	{
		IEnumerable<string> result;
		if (!string.IsNullOrWhiteSpace(raw))
		{
			result = raw.Split(new char[3] { ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct<string>(StringComparer.OrdinalIgnoreCase);
		}
		else
		{
			IEnumerable<string> enumerable = Array.Empty<string>();
			result = enumerable;
		}
		return result;
	}

	private static string ToBool(bool value)
	{
		return value ? "1" : "0";
	}
}
