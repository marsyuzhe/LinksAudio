# 本次公开整理的改动边界

原始开发目录没有修改或批量删除。本次只在发布副本中进行：

- 新建/改写对外文档、元配置、许可证来源说明。
- 全部旧录音、派生音频、历史截图/日志排除；加入可复现的合成音调、同 SDK 基准和生成器。
- MainWindow.xaml.cs 仅调整隐藏 GUI 自检的夹具名称和截图路径显示，不修改队列/试听/转换实现。
- Release 配置关闭符号并映射源码路径；发布元数据不记录本机 SDK 路径。
- SILK DLL 使用同一原源码重新以 -s 构建；LAME 使用随附源码与公开配置构建，并确保发布时使用该 DLL。
- 新增原生与样本构建脚本；修订发布复制流程以保留对应源码。

AudioEngine.cs、AppSettings.cs、SilkDecoder.cs、PlayerService.cs、Job.cs 及 MainWindow.xaml 均保留原件内容；未重构或改写核心转换逻辑。
