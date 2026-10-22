# Links Audio 1.0.0

初始公开准备版本。建议勾选 **Set as a pre-release**：当前已完成本机验证，尚未覆盖所有目标环境和真实录音来源。Tag 使用 `v1.0.0`，对应 csproj 和关于页的真实版本 1.0.0；这份说明本身不表示已经发布。

## Windows 下载

下载附件 **LinksAudio-Portable-win-x64.zip**，完整解压后双击 **LinksAudio.exe**。保留同目录下的 LinksSilk.dll、libmp3lame.64.dll 和许可证目录。不需要安装 .NET Runtime、Python、Node.js 或 FFmpeg。

## 已完成

- 标准/腾讯前缀 SILK V3 → MP3 / WAV；支持内容为 SILK 的 .amr 文件。
- MP3、常见 PCM WAV 输入，批量转换与取消。
- 本地试听，深色/浅色/跟随系统主题。
- 原件保护、同名编号、输出完整读取与时长校验。
- 完全本地处理，没有登录或上传接口。

## 验证与限制

本次以无语音合成音调完成 26 项引擎测试，以及发布版 GUI 转换、试听和 LAME 备用编码测试。原生库按随附源码重建。没有在干净 Windows 10/Windows N 环境验收；实际 DPI 跨屏、广泛微信/QQ 录音及人工听音仍待验证。真正 AMR-NB/AMR-WB 只识别，不解码；不支持 FLAC/M4A 输出。程序未签名。

校验 ZIP 时比对附件 SHA256SUMS.txt。GitHub 自动生成的 Source code ZIP 仅供开发者使用，不是 Windows 程序。

## 许可

项目自有代码 MIT；SILK SDK 保留 Skype 许可及无专利授权条款；LAME LGPL 2.0-or-later 以独立可替换 DLL 和对应源码一起分发。详见随包第三方声明。

---

Windows x64 portable build. Generated-tone tests and local GUI checks pass. Real-world recording coverage and clean Windows 10 validation remain pending; this initial upload is recommended as a pre-release. True AMR decoding is not supported. Audio processing is local and offline. See the bundled notices for third-party licenses.
