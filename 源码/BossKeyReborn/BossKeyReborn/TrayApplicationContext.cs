using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BossKeyReborn;

public sealed class TrayApplicationContext : ApplicationContext
{
	private readonly IniConfigStore _configStore;

	private readonly StartupManager _startupManager;

	private readonly MainForm _mainForm;

	private readonly NotifyIcon _notifyIcon;

	private readonly WindowHiderService _windowHider;

	private readonly HotkeyWindow _hotkeyWindow;

	private readonly MouseHookService _mouseHookService;

	private AppConfig _config;

	private readonly bool _showMainWindowOnLaunch;

	private bool _mouseMiddleLocked;

	public TrayApplicationContext(bool showMainWindow)
	{
		_showMainWindowOnLaunch = showMainWindow;
		string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Abosskey", "newconfig.ini");
		_configStore = new IniConfigStore(configPath);
		_startupManager = new StartupManager();
		_config = _configStore.Load();
		_config.RunAtStartup = _startupManager.IsEnabled();
		_windowHider = new WindowHiderService(_config);
		_mainForm = new MainForm();
		_mainForm.LoadConfig(_config);
		_mainForm.SaveRequested += OnSaveRequested;
		_mainForm.ImportRequested += OnImportRequested;
		_mainForm.ExportRequested += OnExportRequested;
		_hotkeyWindow = new HotkeyWindow(ToggleHideWindows, ToggleMouseMiddleLock);
		_mouseHookService = new MouseHookService(ToggleHideWindows, () => _config.MouseMiddleHide && !_mouseMiddleLocked);
		TryRegisterHotkey(showMessageBoxOnFailure: false);
		ContextMenuStrip contextMenuStrip = new ContextMenuStrip
		{
			Items = 
			{
				{
					"打开设置",
					(Image?)null,
					(EventHandler?)delegate
					{
						ShowSettings();
					}
				},
				{
					"立即切换",
					(Image?)null,
					(EventHandler?)delegate
					{
						ToggleHideWindows();
					}
				},
				{
					"中键锁定切换",
					(Image?)null,
					(EventHandler?)delegate
					{
						ToggleMouseMiddleLock();
					}
				},
				{
					"恢复所有窗口",
					(Image?)null,
					(EventHandler?)delegate
					{
						RestoreWindows();
					}
				},
				(ToolStripItem)new ToolStripSeparator(),
				{
					"退出",
					(Image?)null,
					(EventHandler?)delegate
					{
						ExitThread();
					}
				}
			}
		};
		_notifyIcon = new NotifyIcon
		{
			Text = "老板键重制版",
			Visible = true,
			Icon = SystemIcons.Shield,
			ContextMenuStrip = contextMenuStrip
		};
		_notifyIcon.DoubleClick += delegate
		{
			ShowSettings();
		};
		if (_showMainWindowOnLaunch)
		{
			ShowSettings();
		}
	}

	protected override void ExitThreadCore()
	{
		_windowHider.RestoreAll();
		_notifyIcon.Visible = false;
		_notifyIcon.Dispose();
		_hotkeyWindow.Dispose();
		_mouseHookService.Dispose();
		_mainForm.Dispose();
		base.ExitThreadCore();
	}

	private void OnSaveRequested(object? sender, AppConfig config)
	{
		_config = config.Clone();
		_startupManager.Apply(_config.RunAtStartup);
		_configStore.Save(_config);
		_windowHider.UpdateConfig(_config);
		bool flag = TryRegisterHotkey(showMessageBoxOnFailure: true);
		_mainForm.LoadConfig(_config);
		_mainForm.SetStatus(flag ? "已保存并应用配置。" : "配置已保存，但当前热键不可用。");
	}

	private void OnImportRequested(object? sender, EventArgs e)
	{
		try
		{
			using OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "导入配置",
				Filter = "配置文件 (*.ini)|*.ini|所有文件 (*.*)|*.*",
				CheckFileExists = true,
				Multiselect = false
			};
			if (openFileDialog.ShowDialog(_mainForm) != DialogResult.OK)
			{
				return;
			}
			AppConfig appConfig = new IniConfigStore(openFileDialog.FileName).Load();
			_config = appConfig.Clone();
			_startupManager.Apply(_config.RunAtStartup);
			_configStore.Save(_config);
			_windowHider.UpdateConfig(_config);
			bool flag = TryRegisterHotkey(showMessageBoxOnFailure: true);
			_mainForm.LoadConfig(_config);
			_mainForm.SetStatus(flag ? "已导入并应用配置。" : "已导入配置，但当前热键不可用。");
		}
		catch (Exception ex)
		{
			_mainForm.SetStatus("导入失败: " + ex.Message);
			MessageBox.Show("导入配置失败：\r\n" + ex.Message, "导入失败", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	private void OnExportRequested(object? sender, EventArgs e)
	{
		try
		{
			_configStore.Save(_config);
			using SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Title = "导出配置",
				Filter = "配置文件 (*.ini)|*.ini|所有文件 (*.*)|*.*",
				FileName = "bosskey-config.ini",
				OverwritePrompt = true
			};
			if (saveFileDialog.ShowDialog(_mainForm) != DialogResult.OK)
			{
				return;
			}
			File.Copy(_configStore.ConfigPath, saveFileDialog.FileName, overwrite: true);
			_mainForm.SetStatus("已导出配置: " + saveFileDialog.FileName);
		}
		catch (Exception ex)
		{
			_mainForm.SetStatus("导出失败: " + ex.Message);
			MessageBox.Show("导出配置失败：\r\n" + ex.Message, "导出失败", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	private void ToggleHideWindows()
	{
		if (_windowHider.HasHiddenWindows)
		{
			if (!CanRestoreHiddenWindows())
			{
				_mainForm.SetStatus("密码错误，未恢复窗口。");
				return;
			}
			_windowHider.RestoreAll();
			_mainForm.SetStatus("窗口已恢复。");
		}
		else
		{
			_windowHider.HideAll();
			_mainForm.SetStatus(_windowHider.HasHiddenWindows ? "窗口已隐藏。" : "没有匹配到可隐藏窗口。");
		}
	}

	private void RestoreWindows()
	{
		if (_windowHider.HasHiddenWindows && !CanRestoreHiddenWindows())
		{
			_mainForm.SetStatus("密码错误，未恢复窗口。");
			return;
		}
		_windowHider.RestoreAll();
		_mainForm.SetStatus("窗口已恢复。");
	}

	private void ToggleMouseMiddleLock()
	{
		_mouseMiddleLocked = !_mouseMiddleLocked;
		string status = (_mouseMiddleLocked ? "已锁定中键" : "已解除中键锁定");
		_mainForm.SetStatus(status);
	}

	private void ShowSettings()
	{
		_mainForm.LoadConfig(_config);
		_mainForm.RevealWindow();
	}

	private bool TryRegisterHotkey(bool showMessageBoxOnFailure)
	{
		try
		{
			_hotkeyWindow.Register(_config);
			if (!_hotkeyWindow.IsLockHotkeyRegistered && _config.LockHotkeyKey != Keys.None)
			{
				_mainForm.SetStatus("锁定快捷键注册失败，请更换组合键。");
			}
			return true;
		}
		catch (InvalidOperationException)
		{
			_config.HotkeyModifiers = KeyModifiers.Control | KeyModifiers.Shift;
			_config.HotkeyKey = Keys.D1;
			try
			{
				_hotkeyWindow.Register(_config);
				_mainForm.ApplyHotkeyFallback(_config);
				if (showMessageBoxOnFailure)
				{
					MessageBox.Show("你选择的隐藏/恢复热键已被占用，程序已临时切换为 Ctrl+Shift+1。", "热键冲突", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
				return true;
			}
			catch (InvalidOperationException)
			{
				_mainForm.ApplyHotkeyFallback(_config);
				if (showMessageBoxOnFailure)
				{
					MessageBox.Show("你选择的隐藏/恢复热键已被占用，备用热键也不可用，请到设置里更换热键。", "热键不可用", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				return false;
			}
		}
	}

	private bool CanRestoreHiddenWindows()
	{
		if (!_config.RequirePasswordOnRestore)
		{
			return true;
		}
		using PasswordPromptDialog passwordPromptDialog = new PasswordPromptDialog(_config.RestorePasswordHint);
		SkinManager.ApplyTheme(passwordPromptDialog, _config.SkinName);
		return passwordPromptDialog.ShowDialog() == DialogResult.OK && string.Equals(passwordPromptDialog.EnteredPassword, _config.RestorePassword, StringComparison.Ordinal);
	}
}
