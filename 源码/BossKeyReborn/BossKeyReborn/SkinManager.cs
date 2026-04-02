using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BossKeyReborn;

public static class SkinManager
{
    public static readonly SkinPalette ClassicBlue = new(
        "classic-blue", "经典蓝",
        Color.FromArgb(244, 247, 252),
        Color.FromArgb(236, 241, 249),
        Color.White,
        Color.FromArgb(49, 108, 214),
        Color.FromArgb(219, 230, 250),
        Color.FromArgb(40, 48, 64),
        Color.White,
        Color.FromArgb(234, 240, 251),
        Color.FromArgb(36, 47, 70),
        Color.FromArgb(160, 184, 223));

    public static readonly SkinPalette MintGreen = new(
        "mint-green", "薄荷绿",
        Color.FromArgb(242, 249, 246),
        Color.FromArgb(233, 245, 239),
        Color.White,
        Color.FromArgb(53, 142, 108),
        Color.FromArgb(211, 237, 225),
        Color.FromArgb(34, 61, 51),
        Color.White,
        Color.FromArgb(227, 242, 235),
        Color.FromArgb(35, 74, 59),
        Color.FromArgb(154, 199, 181));

    public static readonly SkinPalette WarmSand = new(
        "warm-sand", "暖砂色",
        Color.FromArgb(250, 245, 238),
        Color.FromArgb(245, 237, 225),
        Color.White,
        Color.FromArgb(180, 116, 64),
        Color.FromArgb(241, 223, 203),
        Color.FromArgb(68, 53, 41),
        Color.White,
        Color.FromArgb(248, 238, 223),
        Color.FromArgb(89, 63, 42),
        Color.FromArgb(210, 184, 149));

    public static readonly SkinPalette CrimsonRed = new(
        "crimson-red", "绯红",
        Color.FromArgb(252, 242, 244),
        Color.FromArgb(248, 232, 236),
        Color.White,
        Color.FromArgb(196, 51, 76),
        Color.FromArgb(248, 213, 221),
        Color.FromArgb(72, 30, 40),
        Color.White,
        Color.FromArgb(250, 226, 232),
        Color.FromArgb(90, 32, 45),
        Color.FromArgb(223, 162, 176));

    private static readonly IReadOnlyList<SkinPalette> All =
        new _003C_003Ez__ReadOnlyArray<SkinPalette>(new[] { ClassicBlue, MintGreen, WarmSand, CrimsonRed });

    public static IReadOnlyList<SkinPalette> GetAll() => All;

    public static SkinPalette Resolve(string? name) =>
        All.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase)) ?? ClassicBlue;

    public static void ApplyTheme(Control root, string? skinName) => ApplyTheme(root, Resolve(skinName));

    public static void ApplyTheme(Control root, SkinPalette skin)
    {
        ApplyRecursive(root, skin, isRoot: true);
        if (root is Form form)
        {
            form.BackColor = skin.FormBack;
            form.ForeColor = skin.Fore;
        }
    }

    private static void ApplyRecursive(Control control, SkinPalette skin, bool isRoot = false)
    {
        switch (control)
        {
            case Form form:
                form.BackColor = skin.FormBack;
                form.ForeColor = skin.Fore;
                break;
            case GroupBox groupBox:
                groupBox.BackColor = skin.GroupBack;
                groupBox.ForeColor = skin.Fore;
                break;
            case Panel panel:
                panel.BackColor = isRoot ? skin.FormBack : skin.PanelBack;
                panel.ForeColor = skin.Fore;
                break;
            case Button button:
                button.BackColor = skin.ButtonBack;
                button.ForeColor = skin.ButtonFore;
                button.FlatStyle = FlatStyle.Standard;
                break;
            case CheckBox checkBox:
                checkBox.BackColor = Color.Transparent;
                checkBox.ForeColor = skin.Fore;
                break;
            case Label label:
                label.ForeColor = label.ForeColor == Color.Blue ? skin.Accent : skin.Fore;
                if (label.Parent is not null && label.Parent is not GroupBox)
                {
                    label.BackColor = Color.Transparent;
                }
                break;
            case TextBox textBox:
                textBox.BackColor = skin.InputBack;
                textBox.ForeColor = skin.Fore;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                break;
            case ComboBox comboBox:
                comboBox.BackColor = skin.InputBack;
                comboBox.ForeColor = skin.Fore;
                comboBox.FlatStyle = FlatStyle.Standard;
                break;
            case ListBox listBox:
                listBox.BackColor = skin.InputBack;
                listBox.ForeColor = skin.Fore;
                listBox.BorderStyle = BorderStyle.FixedSingle;
                break;
            case ListView listView:
                listView.BackColor = skin.InputBack;
                listView.ForeColor = skin.Fore;
                listView.BorderStyle = BorderStyle.FixedSingle;
                break;
        }

        foreach (Control child in control.Controls)
        {
            ApplyRecursive(child, skin);
        }
    }
}
