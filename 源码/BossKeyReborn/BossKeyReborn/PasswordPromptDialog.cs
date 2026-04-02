using System.Drawing;
using System.Windows.Forms;

namespace BossKeyReborn;

public sealed class PasswordPromptDialog : Form
{
	private readonly TextBox _passwordTextBox = new TextBox
	{
		UseSystemPasswordChar = true
	};

	public string EnteredPassword => _passwordTextBox.Text;

	public PasswordPromptDialog(string hint)
	{
		Text = "输入密码";
		base.StartPosition = FormStartPosition.CenterParent;
		base.FormBorderStyle = FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.ClientSize = new Size(300, 138);
		Font = new Font("Microsoft YaHei UI", 9f, FontStyle.Regular, GraphicsUnit.Point);
		base.Controls.Add(new Label
		{
			Text = (string.IsNullOrWhiteSpace(hint) ? "请输入密码：" : hint),
			AutoSize = true,
			Location = new Point(18, 20)
		});
		_passwordTextBox.Location = new Point(20, 48);
		_passwordTextBox.Size = new Size(256, 24);
		base.Controls.Add(_passwordTextBox);
		Button button = new Button
		{
			Text = "确定",
			Size = new Size(74, 28),
			Location = new Point(72, 92)
		};
		button.Click += delegate
		{
			base.DialogResult = DialogResult.OK;
		};
		base.Controls.Add(button);
		Button button2 = new Button
		{
			Text = "取消",
			Size = new Size(74, 28),
			Location = new Point(156, 92)
		};
		button2.Click += delegate
		{
			base.DialogResult = DialogResult.Cancel;
		};
		base.Controls.Add(button2);
		base.AcceptButton = button;
		base.CancelButton = button2;
	}
}
