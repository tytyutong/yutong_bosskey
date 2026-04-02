using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace BossKeyReborn;

internal static class NativeMethods
{
	internal delegate bool EnumWindowsProc(nint hWnd, nint lParam);

	internal delegate nint LowLevelMouseProc(int nCode, nint wParam, nint lParam);

	internal const int WmHotKey = 786;

	internal const int SwHide = 0;

	internal const int SwRestore = 9;

	internal const int WhMouseLl = 14;

	internal const int WmMButtonDown = 519;

	[DllImport("user32.dll")]
	internal static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, nint lParam);

	[DllImport("user32.dll", SetLastError = true)]
	internal static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);

	[DllImport("user32.dll", SetLastError = true)]
	internal static extern int GetClassName(nint hWnd, StringBuilder lpClassName, int nMaxCount);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool IsWindowVisible(nint hWnd);

	[DllImport("user32.dll")]
	internal static extern uint GetWindowThreadProcessId(nint hWnd, out uint processId);

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool ShowWindow(nint hWnd, int nCmdShow);

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool SetForegroundWindow(nint hWnd);

	[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	internal static extern nint FindWindow(string? lpClassName, string? lpWindowName);

	[DllImport("user32.dll", SetLastError = true)]
	internal static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

	[DllImport("user32.dll", SetLastError = true)]
	internal static extern bool UnregisterHotKey(nint hWnd, int id);

	[DllImport("user32.dll", SetLastError = true)]
	internal static extern nint SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, nint hMod, uint dwThreadId);

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool UnhookWindowsHookEx(nint hhk);

	[DllImport("user32.dll")]
	internal static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	internal static extern nint GetModuleHandle(string? lpModuleName);

	internal static IReadOnlyList<WindowInfo> EnumerateVisibleWindows()
	{
		List<WindowInfo> windows = new List<WindowInfo>();
		EnumWindows(delegate(nint hWnd, nint _)
		{
			if (!IsWindowVisible(hWnd))
			{
				return true;
			}
			string text = ReadWindowText(hWnd);
			if (string.IsNullOrWhiteSpace(text))
			{
				return true;
			}
			string className = ReadClassName(hWnd);
			GetWindowThreadProcessId(hWnd, out var processId);
			int processId2 = (int)processId;
			string empty = string.Empty;
			try
			{
				empty = Process.GetProcessById(processId2).ProcessName;
			}
			catch
			{
				return true;
			}
			windows.Add(new WindowInfo(hWnd, text, className, empty, processId2));
			return true;
		}, IntPtr.Zero);
		return windows;
	}

	private static string ReadWindowText(nint handle)
	{
		StringBuilder stringBuilder = new StringBuilder(512);
		GetWindowText(handle, stringBuilder, stringBuilder.Capacity);
		return stringBuilder.ToString();
	}

	private static string ReadClassName(nint handle)
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		GetClassName(handle, stringBuilder, stringBuilder.Capacity);
		return stringBuilder.ToString();
	}
}
