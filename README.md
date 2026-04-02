# yutong_bosskey

一个基于 WinForms 的老板键工具，支持：

- 按软件名称隐藏窗口
- 按窗口标题关键字隐藏窗口
- 全局热键隐藏/恢复
- 开机自启动
- 配置导入/导出
- 多皮肤切换（中文名称）

## 目录结构

- `源码/`：项目源码（已上传）
- `软件/`：本地编译输出（已加入忽略，不上传）

## 运行软件（本地）

先从源码编译，再运行：

`软件/yutong_bosskey.exe`

## 从源码编译

要求：

- .NET 8 SDK
- Windows

命令：

```powershell
dotnet build "源码\BossKeyReborn\BossKeyReborn.csproj" -c Debug -o "软件"
```

编译输出文件名为 `yutong_bosskey.exe`。

## 说明

GitHub 仓库仅跟踪源码，不包含 `软件/` 下的可执行文件与运行时产物。
