# 贡献指南 / Contributing

欢迎提交可复现的问题和小范围改进。请先阅读 BUILD.md 与 TEST_REPORT.md。

- 问题报告注明软件版本、Windows 版本、实际文件头/格式、操作步骤、错误提示与预期结果。路径请脱敏。
- 不要公开微信聊天、私人录音、密钥、配置文件或完整崩溃转储。尽量用数学合成音调复现；涉及真实语音时先确认授权，并通过你与维护者另行约定的私有渠道处理。
- 修改保持 UI、Core、Codecs 的分层。格式检测依赖内容，不以扩展名替代校验。不得改写输入原件。
- 音频相关变更运行 build/test.ps1；UI 变更检查深浅主题、最小窗口及缩放。没有执行的测试明确写未验证。
- 新依赖说明版本、来源、许可、分发义务和必要性。保留第三方版权；不要把 LGPL/GPL 代码直接标成 MIT。
- 提交贡献时确认自己拥有贡献权利，并同意项目自有代码采用 MIT；第三方内容保留原许可。

Please provide reproducible steps, keep private audio and credentials out of issues, run the relevant tests, and document the license of every new dependency. Do not claim validation that was not performed.
