using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BossKeyReborn;

public sealed class WindowPickerForm : Form
{
	private sealed record WindowListItem(WindowInfo Window)
	{
		public override string ToString()
		{
			return $"{Window.ProcessName}  |  {Window.Title}  |  {Window.ClassName}";
		}
	}

	private readonly CheckedListBox _windowList = new CheckedListBox
	{
		Dock = DockStyle.Fill,
		CheckOnClick = true,
		BorderStyle = BorderStyle.None
	};

	private readonly CheckBox _addProcessRulesCheckBox = new CheckBox
	{
		Text = "添加进程规则",
		Checked = true,
		AutoSize = true
	};

	private readonly CheckBox _addTitleRulesCheckBox = new CheckBox
	{
		Text = "添加标题关键字",
		AutoSize = true
	};

	private readonly Label _countLabel = new Label
	{
		AutoSize = true,
		ForeColor = Color.FromArgb(90, 104, 126)
	};

	private readonly Button _refreshButton = CreateSecondaryButton("刷新");

	private readonly Button _okButton = CreatePrimaryButton("添加选中项");

	private readonly Button _cancelButton = CreateSecondaryButton("取消");

	public WindowPickerForm()
	{
		Text = "当前可见窗口";
		base.StartPosition = FormStartPosition.CenterParent;
		MinimumSize = new Size(900, 560);
		BackColor = Color.FromArgb(239, 244, 250);
		Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);
		_windowList.BackColor = Color.White;
		_refreshButton.Click += delegate
		{
			LoadWindows();
		};
		_okButton.Click += delegate
		{
			base.DialogResult = DialogResult.OK;
		};
		_cancelButton.Click += delegate
		{
			base.DialogResult = DialogResult.Cancel;
		};
		base.Controls.Add(BuildLayout());
		LoadWindows();
	}

	public (IReadOnlyList<string> Processes, IReadOnlyList<string> Titles) CollectSelections()
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (WindowListItem item in _windowList.CheckedItems.Cast<WindowListItem>())
		{
			if (_addProcessRulesCheckBox.Checked)
			{
				list.Add(item.Window.ProcessName);
			}
			if (_addTitleRulesCheckBox.Checked && !string.IsNullOrWhiteSpace(item.Window.Title))
			{
				list2.Add(item.Window.Title);
			}
		}
		return (Processes: list.Distinct<string>(StringComparer.OrdinalIgnoreCase).ToArray(), Titles: list2.Distinct<string>(StringComparer.OrdinalIgnoreCase).ToArray());
	}

	private Control BuildLayout()
	{
		TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
		{
			Dock = DockStyle.Fill,
			Padding = new Padding(18),
			RowCount = 3,
			ColumnCount = 1
		};
		tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
		tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
		tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
		tableLayoutPanel.Controls.Add(BuildHeaderCard(), 0, 0);
		tableLayoutPanel.Controls.Add(BuildListCard(), 0, 1);
		tableLayoutPanel.Controls.Add(BuildFooterCard(), 0, 2);
		return tableLayoutPanel;
	}

	private Control BuildHeaderCard()
	{
		Panel panel = CreateCard(new Padding(18, 16, 18, 16));
		panel.Height = 98;
		Label value = new Label
		{
			Text = "当前可见窗口",
			Font = new Font("Segoe UI Semibold", 15f, FontStyle.Bold, GraphicsUnit.Point),
			AutoSize = true,
			Location = new Point(0, 0)
		};
		Label value2 = new Label
		{
			Text = "勾选当前桌面上的窗口，然后转换成进程规则、标题规则，或者两者都加。",
			AutoSize = true,
			Location = new Point(0, 34),
			ForeColor = Color.FromArgb(84, 96, 118)
		};
		_countLabel.Location = new Point(0, 62);
		panel.Controls.Add(value);
		panel.Controls.Add(value2);
		panel.Controls.Add(_countLabel);
		return panel;
	}

	private Control BuildListCard()
	{
		Panel panel = CreateCard(new Padding(18));
		TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
		{
			Dock = DockStyle.Fill,
			ColumnCount = 1,
			RowCount = 2
		};
		tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
		tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
		FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
		{
			Dock = DockStyle.Fill,
			AutoSize = true
		};
		flowLayoutPanel.Controls.Add(_addProcessRulesCheckBox);
		flowLayoutPanel.Controls.Add(_addTitleRulesCheckBox);
		flowLayoutPanel.Controls.Add(_refreshButton);
		Panel panel2 = new Panel
		{
			Dock = DockStyle.Fill,
			BackColor = Color.FromArgb(248, 250, 253),
			Padding = new Padding(12),
			Margin = new Padding(0, 12, 0, 0),
			BorderStyle = BorderStyle.FixedSingle
		};
		panel2.Controls.Add(_windowList);
		tableLayoutPanel.Controls.Add(flowLayoutPanel, 0, 0);
		tableLayoutPanel.Controls.Add(panel2, 0, 1);
		panel.Controls.Add(tableLayoutPanel);
		return panel;
	}

	private Control BuildFooterCard()
	{
		Panel panel = CreateCard(new Padding(18, 14, 18, 14));
		panel.Height = 72;
		FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
		{
			Dock = DockStyle.Right,
			AutoSize = true,
			FlowDirection = FlowDirection.RightToLeft,
			WrapContents = false
		};
		flowLayoutPanel.Controls.Add(_cancelButton);
		flowLayoutPanel.Controls.Add(_okButton);
		panel.Controls.Add(flowLayoutPanel);
		return panel;
	}

	private void LoadWindows()
	{
		_windowList.Items.Clear();
		foreach (WindowInfo item in NativeMethods.EnumerateVisibleWindows().OrderBy<WindowInfo, string>((WindowInfo window) => window.ProcessName, StringComparer.OrdinalIgnoreCase).ThenBy<WindowInfo, string>((WindowInfo window) => window.Title, StringComparer.OrdinalIgnoreCase))
		{
			_windowList.Items.Add(new WindowListItem(item), isChecked: false);
		}
		_countLabel.Text = $"找到 {_windowList.Items.Count} 个可见窗口";
	}

	private static Button CreatePrimaryButton(string text)
	{
		Button button = new Button
		{
			Text = text,
			AutoSize = true,
			FlatStyle = FlatStyle.Flat,
			BackColor = Color.FromArgb(38, 92, 255),
			ForeColor = Color.White,
			Padding = new Padding(14, 8, 14, 8),
			Margin = new Padding(8, 0, 0, 0)
		};
		button.FlatAppearance.BorderSize = 0;
		return button;
	}

	private static Button CreateSecondaryButton(string text)
	{
		Button button = new Button
		{
			Text = text,
			AutoSize = true,
			FlatStyle = FlatStyle.Flat,
			BackColor = Color.White,
			ForeColor = Color.FromArgb(34, 43, 58),
			Padding = new Padding(14, 8, 14, 8),
			Margin = new Padding(8, 0, 0, 0)
		};
		button.FlatAppearance.BorderColor = Color.FromArgb(208, 217, 229);
		return button;
	}

	private static Panel CreateCard(Padding padding)
	{
		return new Panel
		{
			Dock = DockStyle.Fill,
			BackColor = Color.White,
			Margin = new Padding(0, 0, 0, 14),
			Padding = padding,
			BorderStyle = BorderStyle.FixedSingle
		};
	}
}
