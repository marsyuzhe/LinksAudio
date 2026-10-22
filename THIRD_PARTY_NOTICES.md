# Third-party notices / 第三方声明

Links Audio 自有代码、图标及合成样本生成器：Copyright (c) 2026 Links Tam，MIT，见 LICENSE。第三方内容不因主项目 MIT 而变更许可。

| 组件 | 采用版本与来源 | 许可 | 用途与材料 |
|---|---|---|---|
| WPF UI / WPF-UI.Abstractions | 4.3.0，https://github.com/lepoco/wpfui | MIT | Fluent 控件、主题；licenses/WPF-UI-LICENSE.txt |
| NAudio.Core / NAudio.WinMM / NAudio.Wasapi | 2.2.1，https://github.com/naudio/NAudio | MIT | 音频容器、播放与系统编码；licenses/NAudio-LICENSE.txt |
| NLayer / NLayer.NAudioSupport | 1.15.0 / 1.4.0，https://github.com/naudio/NLayer | MIT | 纯托管 MP3 解码；licenses/NLayer-LICENSE.txt |
| NAudio.Lame | 2.1.0，https://github.com/Corey-M/NAudio.Lame | MIT | C# LAME API 包装；licenses/NAudio.Lame-LICENSE.txt |
| LAME | 3.100，https://lame.sourceforge.io/ | LGPL 2.0-or-later | 本次从对应源码重建的 x64 可替换 DLL；licenses/LAME-COPYING.txt、LAME-LICENSE.txt |
| SILK SDK | 1.0.9.6，https://github.com/kn007/silk-v3-decoder/tree/507be6bca8ce1fb977a061481f1d79e8c610e309/silk | Skype BSD 风格自有许可 | 完整 SDK C 源与原版权头；licenses/SILK-SDK-LICENSE.txt |
| silk-v3-decoder 外围代码/说明 | 同一固定来源 | MIT | 原仓库说明版权；licenses/silk-v3-decoder-LICENSE.txt；不以此替代 SDK 许可 |
| .NET / WPF | 10.0.13，自包含运行库，https://github.com/dotnet/runtime | MIT 及随附第三方声明 | licenses/DOTNET-LICENSE.txt、DOTNET-THIRD-PARTY-NOTICES.txt |
| Fluent System Icons 字体 | 随 WPF UI 4.3.0 | MIT | licenses/FluentIcons-LICENSE.txt |

系统字体 Segoe UI / Microsoft YaHei UI / Segoe MDL2 Assets 由 Windows 提供，未复制分发。NuGet 锁文件还包含 .NET 标准库/平台元包的引用资产；运行库的第三方声明保持随包分发。移除了未使用的 ASIO、MIDI、WinForms 包。

## LAME redistribution

LAME 作为独立的 libmp3lame.64.dll 动态库分发，可被用户替换为 ABI 兼容的 x64 库。项目不限制为调试其修改而进行的逆向工程。没有静态链接进主 EXE。

对应源码保留所有源文件和构建材料，只排除两个不参与编译的上游音频示例。主仓库路径为 docs/lame-3.100.tar.gz、build/lame/config.h、build/build-lame.ps1；便携包中为 licenses/sources/。完整来历、上游及派生归档校验值见 [docs/LAME_SOURCE_PROVENANCE.md](docs/LAME_SOURCE_PROVENANCE.md)。发行者应继续保留源码、构建配置、完整 LGPL 文本和替换机制。LAME 网站：https://lame.sourceforge.io/ （上游 LICENSE 亦列 www.mp3dev.org）。

NAudio.Lame 的 NuGet 包自带 native DLL 未用于本次最终发布，最终包采用本次源代码构建的 DLL，其哈希在 build-info.json 中。

## SILK terms

保留 Skype Limited 2006–2012 版权、全部条款和免责声明，源代码与二进制均需保留指定声明。禁止未经许可用 Skype 或贡献者名称背书。该许可明确 **不授予任何专利许可**，不能把仓库的 MIT 简化为 SDK 的许可或宣称获得专利授权。

## Not included

未使用或分发 FFmpeg、pilk、SilkCodec.NET 或 Python/Node 运行时。原先用于本地评估的第三方语音及所有衍生文件已从公开源码与便携包排除。仓库测试数据全部为自生成音调。

## English summary

Project-owned work is MIT. Third-party rights remain separate. SILK uses its Skype BSD-style terms and explicitly grants no patent license. LAME 3.100 is LGPL 2.0-or-later, separately replaceable, with corresponding source/configuration and license text supplied. The NuGet-provided native LAME binary is replaced by a source-built binary for this distribution. Keep all notices and source materials when redistributing.
