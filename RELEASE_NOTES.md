Links Audio 1.0.0 是一款面向 Windows 的离线音频转换工具，让 SILK 语音文件也能在常见播放器中打开。拖入文件，选择 MP3 或 WAV，即可在本机完成转换；不需要命令行、账号或额外安装运行环境。

采用简约的 Windows Fluent 界面，支持深色、浅色和跟随系统主题。所有音频都在本地处理，原始文件始终保留。

### 本次版本

- **SILK → MP3 / WAV**：识别标准 SILK V3 和微信常见 Tencent 前缀，按文件内容判断格式。
- **批量处理**：支持多文件与文件夹导入、后台转换、任务移除和取消。
- **本地试听**：播放、暂停、停止、音量调节与进度显示。
- **输出保护**：同名文件自动编号，可选覆盖；生成结果经过完整读取和时长校验。
- **开箱即用**：Windows x64 自包含便携版，无需安装 .NET Runtime、Python、Node.js 或 FFmpeg。

### 下载与使用

1. 下载附件 **LinksAudio-Portable-win-x64.zip**。
2. 完整解压，保留 EXE 旁的两个 DLL 和许可证文件夹。
3. 双击 **LinksAudio.exe**，导入音频，选择 MP3 / WAV 和保存目录，然后开始转换。

GitHub 自动生成的 **Source code (zip)** 是开发者源码，普通用户应下载上面的 **Portable-win-x64** 附件。可使用随附 `SHA256SUMS.txt` 核对下载文件。

### 支持范围与已知限制

支持 SILK V3、MP3 和常见 PCM / 浮点 WAV 输入。扩展名为 `.amr`、内容实际为 SILK 的文件可以转换；**真正的 AMR-NB / AMR-WB 暂不支持解码**。本版没有 FLAC / M4A 输出。

本轮使用无语音合成音调完成 **26 项引擎测试**，并执行了 GUI 转换、试听和 LAME 备用编码检查。目标为 Windows 10/11 x64，尚未完成干净 Windows 10 / Windows N、广泛真实录音及实际跨屏 DPI 验收；本次建议作为 **Pre-release**。程序未签名。

### 开源与许可

作者 **Links Tam**。项目自有代码采用 MIT；SILK SDK 和 LAME 保留各自许可。LAME 以可替换 DLL 分发，随包提供对应源码、配置和许可证。完整声明见软件包中的 `THIRD_PARTY_NOTICES.md`。

---

**English:** Links Audio is an offline Windows x64 audio converter with SILK V3 support, MP3/WAV output, batch processing and local playback. Extract the portable ZIP and run LinksAudio.exe; no additional runtime installation is needed. True AMR decoding is not supported. Synthetic engine tests and local GUI checks pass; broader recording and clean-system validation remain pending.
