# Build / 开发构建

普通用户只需要 Releases 中的便携 ZIP。本页只适用于开发者。

## 工具

- Windows x64，.NET SDK 10.0.402。global.json 允许相同主版本中兼容的更高 feature band。
- 可访问 NuGet 的构建网络。依赖版本及内容哈希保存在各项目 packages.lock.json。
- 可选 Zig 0.13.0，用于重建原生 SILK/LAME DLL 和合成夹具。Windows 自带 tar.exe 用于展开源码包。

没有 Git 命令要求。源码根目录直接包含解决方案、src、tests、build 和 vendor；不要在源码 ZIP 内直接构建，先完整解压。

## 发布 Windows 应用

在源码根目录打开 PowerShell：

```powershell
./build/build.ps1
```

如 dotnet 不在 PATH，可传入自己的 SDK 路径：`./build/build.ps1 -DotnetPath <dotnet.exe路径>`。不需要、更不应为本项目关闭系统安全保护。

脚本以 Release 配置生成 win-x64 自包含压缩 EXE，复制两个外置原生 DLL、中文/英文说明和许可材料至 `dist/LinksAudio-Portable-win-x64/`。首次构建会恢复 NuGet 包；可先运行 `dotnet restore LinksAudio.slnx --locked-mode` 核对锁定依赖。

所有项目的 Release 调试信息关闭，并用 PathMap 去除开发机器源路径。build-info.json 只记录版本、运行库和二进制哈希，不记录 SDK 安装目录。

项目包含可使用的原生 DLL，也包含其对应源码：

```powershell
./build/build-native.ps1 -ZigPath <zig.exe路径>
./build/build-lame.ps1 -ZigPath <zig.exe路径>
./build/generate-fixtures.ps1 -ZigPath <zig.exe路径>
```

构建时将尖括号占位符替换成自己的工具路径。不要原样复制占位符。生成器使用数学正弦波，不录音。

SILK SDK 源码在 `vendor/silk-v3-decoder/silk/`；桥接层在 `src/LinksAudio.Codecs/native/links_silk.c`。LAME 完整对应源代码在 `docs/lame-3.100.tar.gz`，只排除了两份不参与编译的上游音频示例；配置在 `build/lame/config.h`，详见 [来源记录](docs/LAME_SOURCE_PROVENANCE.md)。LAME 解码器未启用，应用的 MP3 解码由 NLayer 完成。

## 测试

```powershell
./build/test.ps1
```

引擎测试先执行；如存在已发布的 EXE，再运行 GUI 自检。试听测试需要可用音频设备。生成数据仅写入 `tests/output/` 及测试夹具的派生文件。基础合成载荷和 reference.pcm 可重新生成，不包含语音。

没有重建原生 DLL 时，测试使用源码树中已提供的 DLL。发布过程强制使用源码构建的 LAME，而不是 NuGet 包随附的旧 native DLL。对于新依赖和平台，先重新测试再更新支持声明。

## 发布前复查

从新发布目录中移除本机 config、缓存、PDB、旧 ZIP 和测试输出，再打包整个便携目录；保留 EXE、两个 DLL 与许可证。检查输出校验、完整解压及 SHA-256。开发者自行产生的 bin/obj/dist 不能上传普通源码仓库；网页上传不会根据 .gitignore 自动过滤。

## English

Use Windows x64 and the SDK specified by global.json. Run build/build.ps1, then build/test.ps1. The published folder is self-contained. Native rebuild and synthetic fixture generation require Zig 0.13.0. All relevant native source and configuration are included. Build artifacts and local data are excluded from repository uploads. Do not claim clean-system or real-recording compatibility based only on synthetic tests.
