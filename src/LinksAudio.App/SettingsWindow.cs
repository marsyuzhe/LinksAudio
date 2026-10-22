
using System.Windows;
using System.Windows.Controls;
using LinksAudio.Core;
namespace LinksAudio.App;
public sealed class SettingsWindow:Wpf.Ui.Controls.FluentWindow {
 public SettingsWindow(AppSettings settings){Title="转换设置";Width=490;Height=640;ResizeMode=ResizeMode.NoResize;WindowStartupLocation=WindowStartupLocation.CenterOwner;SetResourceReference(BackgroundProperty,"ApplicationBackgroundBrush");
  ExtendsContentIntoTitleBar=true;var panel=new StackPanel{Margin=new Thickness(26,12,26,20)};var root=new Grid();root.RowDefinitions.Add(new RowDefinition{Height=new GridLength(40)});root.RowDefinitions.Add(new RowDefinition());var bar=new Wpf.Ui.Controls.TitleBar{Title="转换设置"};root.Children.Add(bar);Grid.SetRow(panel,1);root.Children.Add(panel);Content=root;
  panel.Children.Add(new TextBlock{Text="转换设置",FontSize=22,FontWeight=FontWeights.SemiBold,Margin=new Thickness(0,0,0,18)});
  void Label(string text)=>panel.Children.Add(new TextBlock{Text=text,Margin=new Thickness(0,12,0,6)});
  Label("默认输出目录");var directory=new TextBox{Text=settings.OutputDirectory,Padding=new Thickness(8)};panel.Children.Add(directory);
  Label("默认格式");var format=new ComboBox{ItemsSource=new[]{"MP3","WAV"},SelectedItem=settings.Format};panel.Children.Add(format);
  Label("MP3 音质");var quality=new ComboBox{ItemsSource=new[]{96,128,192,256,320},SelectedItem=settings.Bitrate};panel.Children.Add(quality);
  Label("文件名规则 · {name} 原名，{date} 日期");var naming=new TextBox{Text=settings.Naming,Padding=new Thickness(8)};panel.Children.Add(naming);
  var overwrite=new CheckBox{Content="覆盖同名输出文件",IsChecked=settings.Overwrite,Margin=new Thickness(0,16,0,8)};panel.Children.Add(overwrite);
  var open=new CheckBox{Content="转换完成后打开目录",IsChecked=settings.OpenWhenDone};panel.Children.Add(open);
  var note=new TextBlock{Text="SILK 使用 SDK 重采样输出 24 kHz 单声道。WAV 是解码结果，不会恢复有损压缩前的原始音质。",TextWrapping=TextWrapping.Wrap,FontSize=11,Margin=new Thickness(0,16,0,16)};note.SetResourceReference(TextBlock.ForegroundProperty,"TextFillColorSecondaryBrush");panel.Children.Add(note);
  var save=new Wpf.Ui.Controls.Button{Content="保存设置",Appearance=Wpf.Ui.Controls.ControlAppearance.Primary,HorizontalAlignment=HorizontalAlignment.Right};panel.Children.Add(save);
  save.Click+=(s,e)=>{settings.OutputDirectory=directory.Text.Trim();settings.Format=format.SelectedItem?.ToString()??"MP3";settings.Bitrate=quality.SelectedItem is int q?q:192;settings.Naming=naming.Text;settings.Overwrite=overwrite.IsChecked==true;settings.OpenWhenDone=open.IsChecked==true;DialogResult=true;};
 }
}
public sealed class AboutWindow:Wpf.Ui.Controls.FluentWindow {
 public AboutWindow(){Title="关于 Links Audio";Width=500;Height=490;ResizeMode=ResizeMode.NoResize;WindowStartupLocation=WindowStartupLocation.CenterOwner;SetResourceReference(BackgroundProperty,"ApplicationBackgroundBrush");
 ExtendsContentIntoTitleBar=true;var p=new StackPanel{Margin=new Thickness(28,16,28,20)};var root=new Grid();root.RowDefinitions.Add(new RowDefinition{Height=new GridLength(40)});root.RowDefinitions.Add(new RowDefinition());root.Children.Add(new Wpf.Ui.Controls.TitleBar{Title="关于 Links Audio"});Grid.SetRow(p,1);root.Children.Add(p);Content=root;p.Children.Add(new TextBlock{Text="Links Audio",FontSize=30,FontWeight=FontWeights.SemiBold});p.Children.Add(new TextBlock{Text="1.0.0  ·  Links Tam",Margin=new Thickness(0,9,0,24)});p.Children.Add(new TextBlock{Text="让微信语音，回到熟悉的播放器。\n\n完全离线 · 原文件保留\n\nWPF UI / NAudio / SILK SDK / LAME\n\nSILK SDK 不授予任何专利许可；完整版权及依赖授权请参阅第三方许可证。",TextWrapping=TextWrapping.Wrap});
 var b=new Wpf.Ui.Controls.Button{Content="查看第三方许可证",Margin=new Thickness(0,22,0,0)};p.Children.Add(b);b.Click+=(s,e)=>{var file=System.IO.Path.Combine(AppContext.BaseDirectory,"THIRD_PARTY_NOTICES.md");if(System.IO.File.Exists(file))System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(file){UseShellExecute=true});};
 }
}
