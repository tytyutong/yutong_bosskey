# yutong_bosskey

一个基于 WinForms 的老板键工具，支持：

- 按软件名称隐藏窗口
- 按窗口标题关键字隐藏窗口
- 全局热键隐藏/恢复
- 开机自启动
- 配置导入/导出
- 多皮肤切换（中文名称）

## 目录结构

- `源码/`：项目源码
- `软件/`：编译输出（可直接运行）

## 运行软件

直接运行：

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

本仓库当前仅保留软件相关源码与运行文件，已清理无关目录与临时文件。
