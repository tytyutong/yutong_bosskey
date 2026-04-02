using System;
using System.Diagnostics;

namespace BossKeyReborn;

public sealed class MouseHookService : IDisposable
{
	private readonly Action _onMiddleButtonDown;

	private readonly Func<bool> _isEnabled;

	private readonly NativeMethods.LowLevelMouseProc _hookProc;

	private nint _hookHandle;

	private long _lastTriggerTicks;

	public MouseHookService(Action onMiddleButtonDown, Func<bool> isEnabled)
	{
		_onMiddleButtonDown = onMiddleButtonDown;
		_isEnabled = isEnabled;
		_hookProc = HookCallback;
		Install();
	}

	private void Install()
	{
		using Process process = Process.GetCurrentProcess();
		using ProcessModule processModule = process.MainModule;
		nint moduleHandle = NativeMethods.GetModuleHandle(processModule?.ModuleName);
		_hookHandle = NativeMethods.SetWindowsHookEx(14, _hookProc, moduleHandle, 0u);
		if (_hookHandle == IntPtr.Zero)
		{
			throw new InvalidOperationException("鼠标钩子注册失败。");
		}
	}

	private nint HookCallback(int nCode, nint wParam, nint lParam)
	{
		if (nCode >= 0 && wParam == 519 && _isEnabled())
		{
			long tickCount = Environment.TickCount64;
			if (tickCount - _lastTriggerTicks > 300)
			{
				_lastTriggerTicks = tickCount;
				_onMiddleButtonDown();
			}
		}
		return NativeMethods.CallNextHookEx(_hookHandle, nCode, wParam, lParam);
	}

	public void Dispose()
	{
		if (_hookHandle != IntPtr.Zero)
		{
			NativeMethods.UnhookWindowsHookEx(_hookHandle);
			_hookHandle = IntPtr.Zero;
		}
	}
}
