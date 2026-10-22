# Test report / 实测范围

公开准备日期：2026-10-22；本轮引擎运行时间：2026-10-22T20:35:53.7713625+08:00。环境：Windows NT 10.0.26300 x64、SDK 10.0.402、运行库 10.0.13。

本轮使用自生成 440/660 Hz 音调，不含录音。没有把原先的真实语音评估文件加入公开目录。没有用合成数据冒充真实微信/QQ 录音验证。

## Engine: 26 passed / 0 failed

| 检查项 | 结果 | 实际信息 |
|---|---|---|
| native_vs_original_reference | PASS | PCM byte identical, 314880 bytes, 6.560 s |
| standard.silk_to_WAV | PASS | 6.560 s; PCM |
| standard.silk_to_MP3 | PASS | 6.609 s; Windows Media Foundation |
| 合成测试.amr_to_WAV | PASS | 6.560 s; PCM |
| 合成测试.amr_to_MP3 | PASS | 6.609 s; Windows Media Foundation |
| forced_LAME_fallback | PASS | LAME 3.100; 6.624 s |
| WAV_input_to_MP3 | PASS | Output produced and fully decoded; local path omitted. |
| MP3_input_to_WAV | PASS | Output produced and fully decoded; local path omitted. |
| empty | PASS | rejected |
| truncated | PASS | rejected |
| damaged_header | PASS | rejected |
| corrupt_payload | PASS | rejected |
| amrnb_detected | PASS | recognized; explicitly unsupported backend (header-only fixture) |
| amrwb_detected | PASS | recognized; explicitly unsupported backend (header-only fixture) |
| unicode_paths | PASS | Output produced and fully decoded; local path omitted. |
| long_paths | PASS | path chars 352 |
| duplicate_names | PASS | standard (2).mp3 |
| overwrite_preserves_input | PASS | voice_converted.wav |
| cancel_before_start | PASS | cancelled |
| cancel_during_decode | PASS | cancelled; no output |
| cancel_after_encoding | PASS | staged MP3 removed before commit |
| unwritable_output | PASS | rejected |
| locked_input | PASS | rejected |
| batch_partial_success | PASS | 2 success, 1 rejected |
| sample_rate_resampling | PASS | 8/12/16/24/48 kHz, duration invariant 6.560 s |
| source_unchanged | PASS | byte identical |

reference.pcm 来自同一版本原始 SDK 对新生成数据的解码，是回归基准，不是独立的多来源兼容性证明。MP3 输出经过实际完整解码，MF 为 6.609 秒、LAME 为 6.624 秒；原始音调 6.560 秒，差异处于编码填充容差内。

## GUI and portable build

实际新构建自包含 EXE 已运行：文件/文件夹导入、混合成功/无效队列、转换、路径复制、清空、深浅主题、播放进度、暂停、停止、音量调用以及强制 LAME 编码通过。日志和机器结果在 docs/validation/。

截图来自运行中实际应用，使用合成任务和通用路径。96/120/144 DPI 图像已渲染；这不是实际系统缩放切换或跨屏操作的验收。

## Not verified / 未验证

- 广泛真实微信/QQ 录音、人工听音，以及用户原先提到的录音：本轮未验证。
- 真正 AMR-NB/AMR-WB 解码：未实现；头部夹具只证明正确识别并拒绝。
- 干净 Windows 10、Windows N、Server 与 ARM64：未验证。
- 实际鼠标拖拽、操作系统文件选择器、打开目录的端到端手势：未自动执行，相关事件/API 已连接。
- 实际跨屏 DPI、千级队列压力、真实 ACL 写入拒绝和不可写程序目录配置回退：未强制模拟。
- 代码签名：未签名。

发布建议：初次上传以 Pre-release 公开，并保留上述范围说明。
