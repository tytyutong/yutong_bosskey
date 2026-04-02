using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BossKeyReborn;

public sealed class MainForm : Form
{
	private const string HiddenTag = "[已添加隐藏]";

	private readonly ListView _processSourceList = new ListView();

	private readonly ListBox _processRuleList = new ListBox();

	private readonly ListView _titleSourceList = new ListView();

	private readonly ListBox _titleRuleList = new ListBox();

	private readonly Label _statusLabel = new Label();

	private readonly Label _hotkeyInfoLabel = new Label();

	private readonly ComboBox _skinComboBox = new ComboBox
	{
		DropDownStyle = ComboBoxStyle.DropDownList
	};

	private readonly ImageList _processIcons = new ImageList
	{
		ColorDepth = ColorDepth.Depth32Bit,
		ImageSize = new Size(16, 16)
	};

	private readonly ImageList _titleIcons = new ImageList
	{
		ColorDepth = ColorDepth.Depth32Bit,
		ImageSize = new Size(16, 16)
	};

	private readonly CheckBox _runAtStartupCheckBox = new CheckBox
	{
		Text = "开机自动运行",
		AutoSize = true
	};

	private readonly Button _refreshProcessesButton = new Button
	{
		Text = "刷新软件名称"
	};

	private readonly Button _addProcessRuleButton = new Button
	{
		Text = "添加"
	};

	private readonly Button _removeProcessRuleButton = new Button
	{
		Text = "删除"
	};

	private readonly Button _clearProcessRuleButton = new Button
	{
		Text = "清空"
	};

	private readonly Button _refreshTitlesButton = new Button
	{
		Text = "刷新窗口标题"
	};

	private readonly Button _addTitleRuleButton = new Button
	{
		Text = "添加"
	};

	private readonly Button _removeTitleRuleButton = new Button
	{
		Text = "删除"
	};

	private readonly Button _clearTitleRuleButton = new Button
	{
		Text = "清空"
	};

	private readonly Button _settingsButton = new Button
	{
		Text = "老板键设置"
	};

	private readonly Button _importButton = new Button
	{
		Text = "导入配置"
	};

	private readonly Button _exportButton = new Button
	{
		Text = "导出配置"
	};

	private readonly Button _websiteButton = new Button
	{
		Text = "官网"
	};

	private AppConfig _currentConfig = AppConfig.CreateDefault();

	private bool _suppressSkinChange;

	public event EventHandler<AppConfig>? SaveRequested;

	public event EventHandler? ImportRequested;

	public event EventHandler? ExportRequested;

	public MainForm()
	{
		Text = "老板键重制版";
		base.StartPosition = FormStartPosition.CenterScreen;
		base.ClientSize = new Size(759, 680);
		MinimumSize = new Size(775, 719);
		MaximumSize = new Size(775, 719);
		BackColor = SystemColors.Control;
		Font = new Font("SimSun", 9f, FontStyle.Regular, GraphicsUnit.Point);
		_settingsButton.Text = "老板键设置";
		_importButton.Text = "导入配置";
		_exportButton.Text = "导出配置";
		_websiteButton.Text = "官网";
		ConfigureSkins();
		ConfigureLists();
		WireEvents();
		base.Controls.Add(BuildRoot());
	}

	public void LoadConfig(AppConfig config)
	{
		_currentConfig = config.Clone();
		_runAtStartupCheckBox.Checked = config.RunAtStartup;
		SelectSkin(config.SkinName);
		FillProcessRuleList(config.HiddenProcesses);
		FillTitleRuleList(config.HiddenTitleKeywords);
		RefreshSourceLists();
		UpdateFooterText(config);
		SetStatus("已加载配置");
	}

	public void SetStatus(string text)
	{
		_statusLabel.Text = text;
	}

	public void ApplyHotkeyFallback(AppConfig config)
	{
		_currentConfig.HotkeyModifiers = config.HotkeyModifiers;
		_currentConfig.HotkeyKey = config.HotkeyKey;
		UpdateFooterText(config);
	}

	public void RevealWindow()
	{
		if (!base.Visible)
		{
			Show();
		}
		if (base.WindowState == FormWindowState.Minimized)
		{
			base.WindowState = FormWindowState.Normal;
		}
		BringToFront();
		Activate();
		NativeMethods.SetForegroundWindow(base.Handle);
	}

	private Control BuildRoot()
	{
		Panel panel = new Panel
		{
			Dock = DockStyle.Fill,
			BackColor = SystemColors.Control
		};
		panel.Controls.Add(BuildSkinBar());
		panel.Controls.Add(BuildProcessGroup());
		panel.Controls.Add(BuildTitleGroup());
		panel.Controls.Add(BuildBottomStrip());
		return panel;
	}

	private Control BuildSkinBar()
	{
		Panel panel = new Panel
		{
			Location = new Point(0, 0),
			Size = new Size(759, 24),
			BackColor = SystemColors.Control
		};
		panel.Controls.Add(new Label
		{
			Text = "皮肤:",
			AutoSize = true,
			Location = new Point(6, 5)
		});
		_skinComboBox.Location = new Point(42, 1);
		_skinComboBox.Size = new Size(116, 22);
		panel.Controls.Add(_skinComboBox);
		return panel;
	}

	private Control BuildProcessGroup()
	{
		GroupBox groupBox = new GroupBox
		{
			Text = "1. 按软件名称隐藏（窗口和托盘隐藏）",
			Location = new Point(8, 24),
			Size = new Size(742, 288)
		};
		_processSourceList.Location = new Point(8, 20);
		_processSourceList.Size = new Size(542, 180);
		groupBox.Controls.Add(_processSourceList);
		GroupBox groupBox2 = new GroupBox
		{
			Text = "隐藏软件名称",
			Location = new Point(564, 18),
			Size = new Size(170, 182)
		};
		_processRuleList.Location = new Point(8, 18);
		_processRuleList.Size = new Size(152, 156);
		groupBox2.Controls.Add(_processRuleList);
		groupBox.Controls.Add(groupBox2);
		Label value = new Label
		{
			Text = "刷新软件名称后，从上方列表选择软件名称（鼠标右键可加入隐藏）",
			AutoSize = true,
			Location = new Point(8, 212)
		};
		groupBox.Controls.Add(value);
		_refreshProcessesButton.Location = new Point(390, 206);
		_refreshProcessesButton.Size = new Size(96, 28);
		groupBox.Controls.Add(_refreshProcessesButton);
		_addProcessRuleButton.Location = new Point(580, 205);
		_addProcessRuleButton.Size = new Size(46, 30);
		groupBox.Controls.Add(_addProcessRuleButton);
		_removeProcessRuleButton.Location = new Point(634, 205);
		_removeProcessRuleButton.Size = new Size(46, 30);
		groupBox.Controls.Add(_removeProcessRuleButton);
		_clearProcessRuleButton.Location = new Point(688, 205);
		_clearProcessRuleButton.Size = new Size(46, 30);
		groupBox.Controls.Add(_clearProcessRuleButton);
		return groupBox;
	}

	private Control BuildTitleGroup()
	{
		GroupBox groupBox = new GroupBox
		{
			Text = "2. 按窗口关键字隐藏（窗口隐藏）",
			Location = new Point(8, 316),
			Size = new Size(742, 216)
		};
		_titleSourceList.Location = new Point(8, 20);
		_titleSourceList.Size = new Size(542, 148);
		groupBox.Controls.Add(_titleSourceList);
		GroupBox groupBox2 = new GroupBox
		{
			Text = "隐藏窗口关键字",
			Location = new Point(564, 18),
			Size = new Size(170, 152)
		};
		_titleRuleList.Location = new Point(8, 18);
		_titleRuleList.Size = new Size(152, 126);
		groupBox2.Controls.Add(_titleRuleList);
		groupBox.Controls.Add(groupBox2);
		Label value = new Label
		{
			Text = "刷新窗口标题后，从上方列表选择窗口标题（鼠标右键可加入隐藏）",
			AutoSize = true,
			Location = new Point(8, 182)
		};
		groupBox.Controls.Add(value);
		_refreshTitlesButton.Location = new Point(390, 176);
		_refreshTitlesButton.Size = new Size(96, 28);
		groupBox.Controls.Add(_refreshTitlesButton);
		_addTitleRuleButton.Location = new Point(580, 176);
		_addTitleRuleButton.Size = new Size(46, 30);
		groupBox.Controls.Add(_addTitleRuleButton);
		_removeTitleRuleButton.Location = new Point(634, 176);
		_removeTitleRuleButton.Size = new Size(46, 30);
		groupBox.Controls.Add(_removeTitleRuleButton);
		_clearTitleRuleButton.Location = new Point(688, 176);
		_clearTitleRuleButton.Size = new Size(46, 30);
		groupBox.Controls.Add(_clearTitleRuleButton);
		return groupBox;
	}

	private Control BuildBottomStrip()
	{
		Panel panel = new Panel
		{
			Location = new Point(8, 536),
			Size = new Size(742, 120)
		};
		_settingsButton.Location = new Point(328, 30);
		_settingsButton.Size = new Size(96, 30);
		panel.Controls.Add(_settingsButton);
		_websiteButton.Location = new Point(442, 30);
		_websiteButton.Size = new Size(42, 30);
		panel.Controls.Add(_websiteButton);
		_runAtStartupCheckBox.Location = new Point(292, 82);
		panel.Controls.Add(_runAtStartupCheckBox);
		_importButton.Location = new Point(404, 76);
		_importButton.Size = new Size(72, 28);
		panel.Controls.Add(_importButton);
		_exportButton.Location = new Point(482, 76);
		_exportButton.Size = new Size(72, 28);
		panel.Controls.Add(_exportButton);
		GroupBox groupBox = new GroupBox
		{
			Location = new Point(560, 10),
			Size = new Size(176, 108)
		};
		_hotkeyInfoLabel.Location = new Point(8, 18);
		_hotkeyInfoLabel.AutoSize = false;
		_hotkeyInfoLabel.Size = new Size(160, 66);
		_hotkeyInfoLabel.Font = new Font("SimSun", 8.5f, FontStyle.Regular, GraphicsUnit.Point);
		groupBox.Controls.Add(_hotkeyInfoLabel);
		_statusLabel.Location = new Point(8, 86);
		_statusLabel.AutoSize = false;
		_statusLabel.Size = new Size(160, 16);
		_statusLabel.Font = new Font("SimSun", 8.5f, FontStyle.Regular, GraphicsUnit.Point);
		groupBox.Controls.Add(_statusLabel);
		panel.Controls.Add(groupBox);
		return panel;
	}

	private void ConfigureLists()
	{
		ConfigureSourceList(_processSourceList, "软件名称", "窗口标题");
		ConfigureSourceList(_titleSourceList, "窗口标题", null);
		_processSourceList.SmallImageList = _processIcons;
		_titleSourceList.SmallImageList = _titleIcons;
		_processIcons.Images.Add("default", SystemIcons.Application.ToBitmap());
		_titleIcons.Images.Add("default", SystemIcons.Application.ToBitmap());
		_processRuleList.HorizontalScrollbar = true;
		_titleRuleList.HorizontalScrollbar = true;
	}

	private void ConfigureSkins()
	{
		_skinComboBox.DisplayMember = "DisplayName";
		_skinComboBox.ValueMember = "Name";
		foreach (SkinPalette item in SkinManager.GetAll())
		{
			_skinComboBox.Items.Add(item);
		}
	}

	private static void ConfigureSourceList(ListView view, string col1, string? col2)
	{
		view.View = View.Details;
		view.FullRowSelect = true;
		view.GridLines = true;
		view.HideSelection = false;
		view.MultiSelect = false;
		view.Columns.Clear();
		view.Columns.Add(col1, 170);
		if (!string.IsNullOrEmpty(col2))
		{
			view.Columns.Add(col2, 350);
		}
	}

	private void WireEvents()
	{
		_refreshProcessesButton.Click += delegate
		{
			RefreshProcessSourceList();
		};
		_addProcessRuleButton.Click += delegate
		{
			AddSelectedProcessRule();
		};
		_refreshTitlesButton.Click += delegate
		{
			RefreshTitleSourceList();
		};
		_removeProcessRuleButton.Click += delegate
		{
			RemoveSelectedListItem(_processRuleList, refreshProcessList: true);
		};
		_clearProcessRuleButton.Click += delegate
		{
			ClearRuleList(_processRuleList, refreshProcessList: true);
		};
		_addTitleRuleButton.Click += delegate
		{
			AddSelectedTitleRule();
		};
		_removeTitleRuleButton.Click += delegate
		{
			RemoveSelectedListItem(_titleRuleList, refreshProcessList: false);
		};
		_clearTitleRuleButton.Click += delegate
		{
			ClearRuleList(_titleRuleList, refreshProcessList: false);
		};
		_processSourceList.DoubleClick += delegate
		{
			AddSelectedProcessRule();
		};
		_titleSourceList.DoubleClick += delegate
		{
			AddSelectedTitleRule();
		};
		_processSourceList.MouseUp += OnProcessSourceListMouseUp;
		_titleSourceList.MouseUp += OnTitleSourceListMouseUp;
		_settingsButton.Click += delegate
		{
			ShowSettingsDialog();
		};
		_skinComboBox.SelectedIndexChanged += delegate
		{
			OnSkinChanged();
		};
		_importButton.Click += delegate
		{
			this.ImportRequested?.Invoke(this, EventArgs.Empty);
		};
		_exportButton.Click += delegate
		{
			this.ExportRequested?.Invoke(this, EventArgs.Empty);
		};
		_websiteButton.Click += delegate
		{
			SetStatus("官网功能待补充");
		};
		_runAtStartupCheckBox.CheckedChanged += delegate
		{
			OnRunAtStartupChanged();
		};
		base.FormClosing += OnFormClosing;
	}

	private void RefreshSourceLists()
	{
		RefreshProcessSourceList();
		RefreshTitleSourceList();
	}

	private void RefreshProcessSourceList()
	{
		HashSet<string> hashSet = _processRuleList.Items.Cast<string>().Select(NormalizeProcessName).OfType<string>()
			.ToHashSet<string>(StringComparer.OrdinalIgnoreCase);
		_processSourceList.BeginUpdate();
		_processSourceList.Items.Clear();
		foreach (WindowInfo item in NativeMethods.EnumerateVisibleWindows().OrderBy<WindowInfo, string>((WindowInfo w) => w.ProcessName, StringComparer.OrdinalIgnoreCase))
		{
			string text = item.ProcessName + ".exe";
			string imageKey = EnsureProcessIcon(item.ProcessName, item.ProcessId, _processIcons);
			bool flag = hashSet.Contains(item.ProcessName);
			string text2 = (flag ? (text + " " + HiddenTag) : text);
			ListViewItem listViewItem = new ListViewItem(text2)
			{
				ImageKey = imageKey,
				ForeColor = (flag ? Color.DarkBlue : SystemColors.WindowText)
			};
			listViewItem.SubItems.Add(item.Title);
			_processSourceList.Items.Add(listViewItem);
		}
		_processSourceList.EndUpdate();
		SetStatus("已刷新软件名称");
	}

	private void RefreshTitleSourceList()
	{
		_titleSourceList.BeginUpdate();
		_titleSourceList.Items.Clear();
		foreach (WindowInfo item in NativeMethods.EnumerateVisibleWindows().OrderBy<WindowInfo, string>((WindowInfo w) => w.Title, StringComparer.OrdinalIgnoreCase))
		{
			string imageKey = EnsureProcessIcon(item.ProcessName, item.ProcessId, _titleIcons);
			_titleSourceList.Items.Add(new ListViewItem(item.Title)
			{
				ImageKey = imageKey
			});
		}
		_titleSourceList.EndUpdate();
		SetStatus("已刷新窗口标题");
	}

	private static string EnsureProcessIcon(string processName, int processId, ImageList imageList)
	{
		string text = processName.ToLowerInvariant();
		if (imageList.Images.ContainsKey(text))
		{
			return text;
		}
		Icon icon = null;
		try
		{
			using Process process = Process.GetProcessById(processId);
			string text2 = null;
			try
			{
				text2 = process.MainModule?.FileName;
			}
			catch
			{
				text2 = null;
			}
			if (!string.IsNullOrWhiteSpace(text2) && File.Exists(text2))
			{
				icon = System.Drawing.Icon.ExtractAssociatedIcon(text2);
			}
		}
		catch
		{
			icon = null;
		}
		if (icon == null)
		{
			imageList.Images.Add(text, imageList.Images["default"]);
			return text;
		}
		using (icon)
		{
			imageList.Images.Add(text, icon.ToBitmap());
		}
		return text;
	}

	private void OnProcessSourceListMouseUp(object? sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			ListViewItem itemAt = _processSourceList.GetItemAt(e.X, e.Y);
			if (itemAt != null)
			{
				itemAt.Selected = true;
				AddSelectedProcessRule();
			}
		}
	}

	private void OnTitleSourceListMouseUp(object? sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			ListViewItem itemAt = _titleSourceList.GetItemAt(e.X, e.Y);
			if (itemAt != null)
			{
				itemAt.Selected = true;
				AddSelectedTitleRule();
			}
		}
	}

	private void AddSelectedProcessRule()
	{
		if (_processSourceList.SelectedItems.Count == 0)
		{
			SetStatus("请先选择软件名称");
			return;
		}
		string text = NormalizeProcessName(_processSourceList.SelectedItems[0].Text) ?? string.Empty;
		if (!AddRuleIfMissing(_processRuleList, text))
		{
			SetStatus("已在隐藏列表中: " + text);
			return;
		}
		this.SaveRequested?.Invoke(this, ReadConfigFromInputs());
		RefreshProcessSourceList();
		UpdateFooterText(ReadConfigFromInputs());
		SetStatus("已添加: " + text);
	}

	private void AddSelectedTitleRule()
	{
		if (_titleSourceList.SelectedItems.Count == 0)
		{
			SetStatus("请先选择窗口标题");
			return;
		}
		string text = _titleSourceList.SelectedItems[0].Text.Trim();
		if (!AddRuleIfMissing(_titleRuleList, text))
		{
			SetStatus("已在隐藏列表中: " + text);
			return;
		}
		this.SaveRequested?.Invoke(this, ReadConfigFromInputs());
		UpdateFooterText(ReadConfigFromInputs());
		SetStatus("已添加: " + text);
	}

	private void RemoveSelectedListItem(ListBox listBox, bool refreshProcessList)
	{
		if (listBox.SelectedItem != null)
		{
			listBox.Items.Remove(listBox.SelectedItem);
			this.SaveRequested?.Invoke(this, ReadConfigFromInputs());
			if (refreshProcessList)
			{
				RefreshProcessSourceList();
			}
			UpdateFooterText(ReadConfigFromInputs());
			SetStatus("已删除");
		}
	}

	private void ClearRuleList(ListBox listBox, bool refreshProcessList)
	{
		listBox.Items.Clear();
		this.SaveRequested?.Invoke(this, ReadConfigFromInputs());
		if (refreshProcessList)
		{
			RefreshProcessSourceList();
		}
		UpdateFooterText(ReadConfigFromInputs());
		SetStatus("已清空");
	}

	private void FillProcessRuleList(IEnumerable<string> rules)
	{
		_processRuleList.Items.Clear();
		foreach (string item in rules.Select(NormalizeProcessName).OfType<string>().Distinct<string>(StringComparer.OrdinalIgnoreCase))
		{
			_processRuleList.Items.Add(item);
		}
	}

	private void FillTitleRuleList(IEnumerable<string> rules)
	{
		_titleRuleList.Items.Clear();
		foreach (string item in rules.Where((string r) => !string.IsNullOrWhiteSpace(r)).Distinct<string>(StringComparer.OrdinalIgnoreCase))
		{
			_titleRuleList.Items.Add(item);
		}
	}

	private static bool AddRuleIfMissing(ListBox listBox, string value)
	{
		if (listBox.Items.Cast<object>().Any((object item) => string.Equals(item.ToString(), value, StringComparison.OrdinalIgnoreCase)))
		{
			return false;
		}
		listBox.Items.Add(value);
		return true;
	}

	private AppConfig ReadConfigFromInputs()
	{
		AppConfig appConfig = _currentConfig.Clone();
		appConfig.RunAtStartup = _runAtStartupCheckBox.Checked;
		if (_skinComboBox.SelectedItem is SkinPalette skinPalette)
		{
			appConfig.SkinName = skinPalette.Name;
		}
		appConfig.HiddenProcesses.Clear();
		appConfig.HiddenTitleKeywords.Clear();
		appConfig.HiddenProcesses.AddRange(_processRuleList.Items.Cast<string>());
		appConfig.HiddenTitleKeywords.AddRange(_titleRuleList.Items.Cast<string>());
		return appConfig;
	}

	private void UpdateFooterText(AppConfig config)
	{
		_hotkeyInfoLabel.Text = $"热键: {config.HotkeyModifiers}+{config.HotkeyKey}\r\n锁定: {config.LockHotkeyModifiers}+{config.LockHotkeyKey}\r\n中键: {(config.MouseMiddleHide ? "开" : "关")}\r\n静音: {(config.MuteWhenHidden ? "开" : "关")}";
	}

	private void ShowSettingsDialog()
	{
		using SettingsDialog settingsDialog = new SettingsDialog(_currentConfig);
		SkinManager.ApplyTheme(settingsDialog, _currentConfig.SkinName);
		if (settingsDialog.ShowDialog(this) == DialogResult.OK)
		{
			_currentConfig.HotkeyModifiers = settingsDialog.SelectedHotkeyModifiers;
			_currentConfig.HotkeyKey = settingsDialog.SelectedHotkeyKey;
			_currentConfig.LockHotkeyModifiers = settingsDialog.SelectedLockHotkeyModifiers;
			_currentConfig.LockHotkeyKey = settingsDialog.SelectedLockHotkeyKey;
			_currentConfig.HideTrayIcon = !settingsDialog.ShowTrayIcon;
			_currentConfig.MouseMiddleHide = settingsDialog.MouseMiddleHideEnabled;
			_currentConfig.MuteWhenHidden = settingsDialog.MuteWhenHiddenEnabled;
			_currentConfig.RequirePasswordOnRestore = settingsDialog.RequirePasswordOnRestore;
			_currentConfig.RestorePassword = settingsDialog.RestorePassword;
			_currentConfig.RestorePasswordHint = settingsDialog.RestorePasswordHint;
			_currentConfig.RunAtStartup = settingsDialog.RunAtStartup;
			_runAtStartupCheckBox.Checked = _currentConfig.RunAtStartup;
			this.SaveRequested?.Invoke(this, ReadConfigFromInputs());
			UpdateFooterText(ReadConfigFromInputs());
			SetStatus("已更新设置");
		}
	}

	private void OnRunAtStartupChanged()
	{
		if (_runAtStartupCheckBox.Checked != _currentConfig.RunAtStartup)
		{
			_currentConfig.RunAtStartup = _runAtStartupCheckBox.Checked;
			this.SaveRequested?.Invoke(this, ReadConfigFromInputs());
			UpdateFooterText(ReadConfigFromInputs());
			SetStatus(_runAtStartupCheckBox.Checked ? "已开启开机自动运行" : "已关闭开机自动运行");
		}
	}

	private void OnSkinChanged()
	{
		if (!_suppressSkinChange && _skinComboBox.SelectedItem is SkinPalette skinPalette)
		{
			_currentConfig.SkinName = skinPalette.Name;
			SkinManager.ApplyTheme(this, skinPalette);
			this.SaveRequested?.Invoke(this, ReadConfigFromInputs());
			SetStatus("已切换皮肤: " + skinPalette.DisplayName);
		}
	}

	private void SelectSkin(string? skinName)
	{
		SkinPalette skinPalette = SkinManager.Resolve(skinName);
		_suppressSkinChange = true;
		for (int i = 0; i < _skinComboBox.Items.Count; i++)
		{
			if (_skinComboBox.Items[i] is SkinPalette skinPalette2 && skinPalette2.Name == skinPalette.Name)
			{
				_skinComboBox.SelectedIndex = i;
				break;
			}
		}
		_suppressSkinChange = false;
		SkinManager.ApplyTheme(this, skinPalette);
	}

	private static string? NormalizeProcessName(string? raw)
	{
		if (string.IsNullOrWhiteSpace(raw))
		{
			return null;
		}
		string text = raw.Trim();
		text = text.Replace(HiddenTag, string.Empty, StringComparison.OrdinalIgnoreCase);
		text = text.Trim();
		if (text.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
		{
			string text2 = text;
			text = text2.Substring(0, text2.Length - 4);
		}
		return text.Trim();
	}

	private void OnFormClosing(object? sender, FormClosingEventArgs e)
	{
		if (e.CloseReason == CloseReason.UserClosing)
		{
			e.Cancel = true;
			Hide();
		}
	}
}



