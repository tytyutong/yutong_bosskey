namespace BossKeyReborn;

public sealed record WindowInfo(nint Handle, string Title, string ClassName, string ProcessName, int ProcessId);
