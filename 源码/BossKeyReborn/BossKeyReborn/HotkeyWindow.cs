using System;
using System.Windows.Forms;

namespace BossKeyReborn;

public sealed class HotkeyWindow : NativeWindow, IDisposable
{
	private const int ToggleHotkeyId = 1;

	private const int LockHotkeyId = 2;

	private readonly Action _onToggleHotkeyPressed;

	private readonly Action _onLockHotkeyPressed;

	public bool IsLockHotkeyRegistered { get; private set; }

	public HotkeyWindow(Action onToggleHotkeyPressed, Action onLockHotkeyPressed)
	{
		_onToggleHotkeyPressed = onToggleHotkeyPressed;
		_onLockHotkeyPressed = onLockHotkeyPressed;
		CreateHandle(new CreateParams());
	}

	public void Register(AppConfig config)
	{
		NativeMethods.UnregisterHotKey(base.Handle, 1);
		NativeMethods.UnregisterHotKey(base.Handle, 2);
		if (!NativeMethods.RegisterHotKey(base.Handle, 1, (uint)config.HotkeyModifiers, (uint)config.HotkeyKey))
		{
			throw new InvalidOperationException("隐藏/恢复热键注册失败，可能已被其他软件占用。");
		}
		IsLockHotkeyRegistered = false;
		if (config.LockHotkeyKey != Keys.None)
		{
			IsLockHotkeyRegistered = NativeMethods.RegisterHotKey(base.Handle, 2, (uint)config.LockHotkeyModifiers, (uint)config.LockHotkeyKey);
		}
	}

	protected override void WndProc(ref Message m)
	{
		if (m.Msg == 786 && m.WParam == 1)
		{
			_onToggleHotkeyPressed();
		}
		else if (m.Msg == 786 && m.WParam == 2)
		{
			_onLockHotkeyPressed();
		}
		base.WndProc(ref m);
	}

	public void Dispose()
	{
		NativeMethods.UnregisterHotKey(base.Handle, 1);
		NativeMethods.UnregisterHotKey(base.Handle, 2);
		DestroyHandle();
	}
}
