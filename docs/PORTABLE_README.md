# Links Audio 1.0.0

作者：Links Tam。Windows x64 便携版，音频只在本地处理，无需登录或网络。

完整解压后双击 LinksAudio.exe。保留 LinksSilk.dll、libmp3lame.64.dll 和 licenses/。不需要安装 .NET Runtime、Python、Node.js 或 FFmpeg。

导入文件 → 选择 MP3/WAV 与保存位置 → 开始转换。支持标准/微信前缀 SILK（包括内容为 SILK 的 .amr）、MP3 和常见 PCM WAV；支持批量转换、取消与试听。输入原件保留，重复输出默认自动编号。

真正 AMR-NB/AMR-WB 暂不解码。目标为 Windows 10/11 x64；尚未在干净 Windows 10、Windows N 等系统逐一验收。真实语音兼容性覆盖仍待扩大，建议初次公开标记 Pre-release。见 TEST_REPORT.md。

配置优先保存在程序旁 config/settings.json，不可写时回退至用户 LocalApplicationData。软件未签名；无需绕过 Windows 安全保护。

自有代码 MIT；第三方声明见 THIRD_PARTY_NOTICES.md 和 licenses/。LAME 的对应源码及配置在 licenses/sources/，DLL 可替换。build-info.json 记录实际二进制哈希。GitHub 自动生成的 Source code ZIP 是源码，不是本便携软件。
