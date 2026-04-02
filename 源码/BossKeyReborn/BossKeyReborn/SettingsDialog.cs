using System;
using System.Drawing;
using System.Windows.Forms;

namespace BossKeyReborn;

public sealed class SettingsDialog : Form
{
	private readonly CheckBox _requirePasswordCheckBox = new CheckBox
	{
		Text = "开启恢复时需要密码",
		AutoSize = true
	};

	private readonly TextBox _passwordHintTextBox = new TextBox();

	private readonly TextBox _passwordTextBox = new TextBox
	{
		UseSystemPasswordChar = true
	};

	private readonly CheckBox _mouseMiddleHideCheckBox = new CheckBox
	{
		Text = "鼠标中键按下隐藏",
		AutoSize = true
	};

	private readonly CheckBox _muteWhenHiddenCheckBox = new CheckBox
	{
		Text = "隐藏时自动静音",
		AutoSize = true
	};

	private readonly CheckBox _showTrayIconCheckBox = new CheckBox
	{
		Text = "显示自身托盘图标",
		AutoSize = true
	};

	private readonly CheckBox _runAtStartupCheckBox = new CheckBox
	{
		Text = "开机自动启动",
		AutoSize = true
	};

	private readonly ComboBox _modifierComboBox = new ComboBox
	{
		DropDownStyle = ComboBoxStyle.DropDownList
	};

	private readonly ComboBox _keyComboBox = new ComboBox
	{
		DropDownStyle = ComboBoxStyle.DropDownList
	};

	private readonly ComboBox _lockModifierComboBox = new ComboBox
	{
		DropDownStyle = ComboBoxStyle.DropDownList
	};

	private readonly ComboBox _lockKeyComboBox = new ComboBox
	{
		DropDownStyle = ComboBoxStyle.DropDownList
	};

	public KeyModifiers SelectedHotkeyModifiers { get; private set; }

	public Keys SelectedHotkeyKey { get; private set; }

	public KeyModifiers SelectedLockHotkeyModifiers { get; private set; }

	public Keys SelectedLockHotkeyKey { get; private set; }

	public bool ShowTrayIcon { get; private set; }

	public bool MouseMiddleHideEnabled { get; private set; }

	public bool MuteWhenHiddenEnabled { get; private set; }

	public bool RequirePasswordOnRestore { get; private set; }

	public string RestorePassword { get; private set; } = string.Empty;

	public string RestorePasswordHint { get; private set; } = "请输入密码：";

	public bool RunAtStartup { get; private set; }

	public SettingsDialog(AppConfig config)
	{
		Text = "软件设置";
		base.StartPosition = FormStartPosition.CenterParent;
		base.FormBorderStyle = FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.ClientSize = new Size(392, 420);
		BackColor = SystemColors.Control;
		Font = new Font("Microsoft YaHei UI", 9f, FontStyle.Regular, GraphicsUnit.Point);
		SelectedHotkeyModifiers = config.HotkeyModifiers;
		SelectedHotkeyKey = config.HotkeyKey;
		SelectedLockHotkeyModifiers = config.LockHotkeyModifiers;
		SelectedLockHotkeyKey = config.LockHotkeyKey;
		ShowTrayIcon = !config.HideTrayIcon;
		MouseMiddleHideEnabled = config.MouseMiddleHide;
		MuteWhenHiddenEnabled = config.MuteWhenHidden;
		RequirePasswordOnRestore = config.RequirePasswordOnRestore;
		RestorePassword = config.RestorePassword;
		RestorePasswordHint = config.RestorePasswordHint;
		RunAtStartup = config.RunAtStartup;
		BuildChoices();
		base.Controls.Add(BuildRoot());
		LoadState(config);
	}

	private void BuildChoices()
	{
		object[] items = new object[6] { "无", "Alt", "Ctrl", "Shift", "Ctrl+Shift", "Ctrl+Alt" };
		object[] items2 = new object[23]
		{
			"无", "1", "2", "3", "4", "5", "6", "7", "8", "9",
			"0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9",
			"F10", "F11", "F12"
		};
		_modifierComboBox.Items.AddRange(items);
		_lockModifierComboBox.Items.AddRange(items);
		_keyComboBox.Items.AddRange(items2);
		_lockKeyComboBox.Items.AddRange(items2);
	}

	private Control BuildRoot()
	{
		Panel panel = new Panel
		{
			Dock = DockStyle.Fill,
			Padding = new Padding(16)
		};
		GroupBox groupBox = new GroupBox
		{
			Text = "恢复安全",
			Location = new Point(12, 10),
			Size = new Size(366, 100)
		};
		_requirePasswordCheckBox.Location = new Point(16, 26);
		groupBox.Controls.Add(_requirePasswordCheckBox);
		groupBox.Controls.Add(new Label
		{
			Text = "密码提示:",
			AutoSize = true,
			Location = new Point(16, 56)
		});
		_passwordHintTextBox.Location = new Point(82, 52);
		_passwordHintTextBox.Size = new Size(118, 24);
		groupBox.Controls.Add(_passwordHintTextBox);
		groupBox.Controls.Add(new Label
		{
			Text = "密码:",
			AutoSize = true,
			Location = new Point(214, 56)
		});
		_passwordTextBox.Location = new Point(254, 52);
		_passwordTextBox.Size = new Size(92, 24);
		groupBox.Controls.Add(_passwordTextBox);
		panel.Controls.Add(groupBox);
		GroupBox groupBox2 = new GroupBox
		{
			Text = "触发方式",
			Location = new Point(12, 118),
			Size = new Size(366, 106)
		};
		_mouseMiddleHideCheckBox.Location = new Point(16, 28);
		groupBox2.Controls.Add(_mouseMiddleHideCheckBox);
		_muteWhenHiddenCheckBox.Location = new Point(16, 54);
		groupBox2.Controls.Add(_muteWhenHiddenCheckBox);
		groupBox2.Controls.Add(new Label
		{
			Text = "锁定后，中键隐藏/恢复会暂时禁用。",
			AutoSize = true,
			ForeColor = Color.Blue,
			Location = new Point(16, 80)
		});
		panel.Controls.Add(groupBox2);
		GroupBox groupBox3 = new GroupBox
		{
			Text = "快捷键",
			Location = new Point(12, 232),
			Size = new Size(366, 128)
		};
		groupBox3.Controls.Add(new Label
		{
			Text = "隐藏/恢复:",
			AutoSize = true,
			Location = new Point(16, 28)
		});
		_modifierComboBox.Location = new Point(82, 24);
		_modifierComboBox.Size = new Size(102, 24);
		groupBox3.Controls.Add(_modifierComboBox);
		_keyComboBox.Location = new Point(194, 24);
		_keyComboBox.Size = new Size(62, 24);
		groupBox3.Controls.Add(_keyComboBox);
		groupBox3.Controls.Add(new Label
		{
			Text = "锁定切换:",
			AutoSize = true,
			Location = new Point(16, 58)
		});
		_lockModifierComboBox.Location = new Point(82, 54);
		_lockModifierComboBox.Size = new Size(102, 24);
		groupBox3.Controls.Add(_lockModifierComboBox);
		_lockKeyComboBox.Location = new Point(194, 54);
		_lockKeyComboBox.Size = new Size(62, 24);
		groupBox3.Controls.Add(_lockKeyComboBox);
		_showTrayIconCheckBox.Location = new Point(16, 90);
		groupBox3.Controls.Add(_showTrayIconCheckBox);
		_runAtStartupCheckBox.Location = new Point(156, 90);
		groupBox3.Controls.Add(_runAtStartupCheckBox);
		panel.Controls.Add(groupBox3);
		Button button = new Button
		{
			Text = "保存并关闭",
			Size = new Size(96, 30),
			Location = new Point(96, 372)
		};
		button.Click += delegate
		{
			ConfirmAndClose();
		};
		panel.Controls.Add(button);
		Button button2 = new Button
		{
			Text = "取消",
			Size = new Size(78, 30),
			Location = new Point(212, 372)
		};
		button2.Click += delegate
		{
			base.DialogResult = DialogResult.Cancel;
		};
		panel.Controls.Add(button2);
		return panel;
	}

	private void LoadState(AppConfig config)
	{
		_requirePasswordCheckBox.Checked = config.RequirePasswordOnRestore;
		_passwordHintTextBox.Text = (string.IsNullOrWhiteSpace(config.RestorePasswordHint) ? "请输入密码：" : config.RestorePasswordHint);
		_passwordTextBox.Text = config.RestorePassword;
		_mouseMiddleHideCheckBox.Checked = config.MouseMiddleHide;
		_muteWhenHiddenCheckBox.Checked = config.MuteWhenHidden;
		_showTrayIconCheckBox.Checked = !config.HideTrayIcon;
		_runAtStartupCheckBox.Checked = config.RunAtStartup;
		_modifierComboBox.SelectedItem = ModifierToText(config.HotkeyModifiers);
		_keyComboBox.SelectedItem = KeyToText(config.HotkeyKey);
		_lockModifierComboBox.SelectedItem = ModifierToText(config.LockHotkeyModifiers);
		_lockKeyComboBox.SelectedItem = KeyToText(config.LockHotkeyKey);
		if (_modifierComboBox.SelectedIndex < 0)
		{
			_modifierComboBox.SelectedItem = "Alt";
		}
		if (_keyComboBox.SelectedIndex < 0)
		{
			_keyComboBox.SelectedItem = "1";
		}
		if (_lockModifierComboBox.SelectedIndex < 0)
		{
			_lockModifierComboBox.SelectedItem = "Alt";
		}
		if (_lockKeyComboBox.SelectedIndex < 0)
		{
			_lockKeyComboBox.SelectedItem = "2";
		}
	}

	private void ConfirmAndClose()
	{
		SelectedHotkeyModifiers = TextToModifier(_modifierComboBox.SelectedItem?.ToString());
		SelectedHotkeyKey = TextToKey(_keyComboBox.SelectedItem?.ToString());
		SelectedLockHotkeyModifiers = TextToModifier(_lockModifierComboBox.SelectedItem?.ToString());
		SelectedLockHotkeyKey = TextToKey(_lockKeyComboBox.SelectedItem?.ToString());
		ShowTrayIcon = _showTrayIconCheckBox.Checked;
		MouseMiddleHideEnabled = _mouseMiddleHideCheckBox.Checked;
		MuteWhenHiddenEnabled = _muteWhenHiddenCheckBox.Checked;
		RequirePasswordOnRestore = _requirePasswordCheckBox.Checked;
		RestorePassword = _passwordTextBox.Text;
		RestorePasswordHint = (string.IsNullOrWhiteSpace(_passwordHintTextBox.Text) ? "请输入密码：" : _passwordHintTextBox.Text.Trim());
		RunAtStartup = _runAtStartupCheckBox.Checked;
		base.DialogResult = DialogResult.OK;
	}

	private static string ModifierToText(KeyModifiers modifiers)
	{
		if (1 == 0)
		{
		}
		string result = modifiers switch
		{
			KeyModifiers.Alt => "Alt", 
			KeyModifiers.Control => "Ctrl", 
			KeyModifiers.Shift => "Shift", 
			KeyModifiers.Control | KeyModifiers.Shift => "Ctrl+Shift", 
			KeyModifiers.Alt | KeyModifiers.Control => "Ctrl+Alt", 
			_ => "无", 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private static KeyModifiers TextToModifier(string? text)
	{
		if (1 == 0)
		{
		}
		KeyModifiers result = text switch
		{
			"Alt" => KeyModifiers.Alt, 
			"Ctrl" => KeyModifiers.Control, 
			"Shift" => KeyModifiers.Shift, 
			"Ctrl+Shift" => KeyModifiers.Control | KeyModifiers.Shift, 
			"Ctrl+Alt" => KeyModifiers.Alt | KeyModifiers.Control, 
			_ => KeyModifiers.None, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private static string KeyToText(Keys key)
	{
		if (key >= Keys.D0 && key <= Keys.D9)
		{
			return ((char)(48 + (key - 48))).ToString();
		}
		return (key >= Keys.F1 && key <= Keys.F12) ? key.ToString() : "无";
	}

	private static Keys TextToKey(string? text)
	{
		if (string.IsNullOrWhiteSpace(text) || text == "无")
		{
			return Keys.None;
		}
		if (text.Length == 1 && char.IsDigit(text[0]))
		{
			return (Keys)(48 + (text[0] - 48));
		}
		Keys result;
		return Enum.TryParse<Keys>(text, out result) ? result : Keys.D1;
	}
}
