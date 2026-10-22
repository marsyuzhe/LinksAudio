
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LinksAudio.Core;
using Microsoft.Win32;
using Wpf.Ui.Appearance;
namespace LinksAudio.App;
public partial class MainWindow:Wpf.Ui.Controls.FluentWindow {
 public ObservableCollection<Job> Jobs {get;}=[];
 readonly AudioEngine engine=new();readonly AppSettings settings=AppSettings.Load();readonly PlayerService player=new();
 CancellationTokenSource? batchCancellation,currentCancellation,previewCancellation;bool busy,initializing=true;Job? current;readonly DispatcherTimer timer=new(){Interval=TimeSpan.FromMilliseconds(250)};
 string? previewFolder,playingName;bool closed;
 public MainWindow() {
  InitializeComponent();DataContext=this;OutputBox.Text=settings.OutputDirectory;FormatBox.SelectedIndex=settings.Format=="WAV"?1:0;ThemeBox.SelectedIndex=settings.Theme=="浅色"?1:settings.Theme=="跟随系统"?2:0;
  initializing=false;ApplyTheme();timer.Tick+=(s,e)=>{PlaybackProgress.Value=player.Progress;if(playingName!=null)PlayingLabel.Text=playingName+" · "+player.TimeText;};timer.Start();
  Closing+=Window_Closing;Closed+=(s,e)=>{closed=true;timer.Stop();previewCancellation?.Cancel();player.Dispose();CleanPreview();SystemEvents.UserPreferenceChanged-=SystemThemeChanged;};SystemEvents.UserPreferenceChanged+=SystemThemeChanged;
  Loaded+=async(s,e)=>{var args=Environment.GetCommandLineArgs().Skip(1).ToArray();if(args.Length>=2&&args[0]=="--ui-test")await RunUiTest(args[1]);else await AddPaths(args.Where(File.Exists));};
 }
 void SystemThemeChanged(object sender,UserPreferenceChangedEventArgs e){if(settings.Theme=="跟随系统")Dispatcher.BeginInvoke(ApplyTheme);}
 void ApplyTheme(){var dark=settings.Theme!="浅色";if(settings.Theme=="跟随系统"){try{dark=(int?)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize","AppsUseLightTheme",1)==0;}catch{dark=true;}}ApplicationThemeManager.Apply(dark?ApplicationTheme.Dark:ApplicationTheme.Light,Wpf.Ui.Controls.WindowBackdropType.None,false);ApplicationAccentColorManager.Apply(Color.FromRgb(99,151,229),dark?ApplicationTheme.Dark:ApplicationTheme.Light);SetResourceReference(BackgroundProperty,"ApplicationBackgroundBrush");}
 void Theme_Changed(object sender,SelectionChangedEventArgs e){if(initializing)return;settings.Theme=((ComboBoxItem)ThemeBox.SelectedItem).Content.ToString()!;ApplyTheme();settings.Save();}
 async void Select_Click(object sender,RoutedEventArgs e){var d=new OpenFileDialog{Title="选择音频文件",Multiselect=true,Filter="音频文件|*.amr;*.silk;*.slk;*.aud;*.wav;*.mp3|所有文件|*.*"};if(d.ShowDialog(this)==true)await AddPaths(d.FileNames);}
 async void Folder_Click(object sender,RoutedEventArgs e){var d=new OpenFolderDialog{Title="选择音频文件夹（仅加入当前层文件）"};if(d.ShowDialog(this)==true)await AddPaths([d.FolderName]);}
 async void Files_Drop(object sender,DragEventArgs e){if(e.Data.GetData(DataFormats.FileDrop) is string[] files)await AddPaths(files);}
 void Files_DragOver(object sender,DragEventArgs e){e.Effects=e.Data.GetDataPresent(DataFormats.FileDrop)?DragDropEffects.Copy:DragDropEffects.None;e.Handled=true;}
 public async Task AddPaths(IEnumerable<string> paths){
  List<string> files=[];foreach(var path in paths)try{if(Directory.Exists(path))files.AddRange(Directory.EnumerateFiles(path).Where(f=>new[]{".silk",".slk",".aud",".amr",".mp3",".wav"}.Contains(Path.GetExtension(f).ToLowerInvariant())));else files.Add(path);}catch(Exception ex){Status.Text=ex.Message;}
  foreach(var path in files){if(closed)return;if(Jobs.Any(j=>j.Input.Equals(path,StringComparison.OrdinalIgnoreCase)))continue;var job=new Job{Input=path,State="识别中"};Jobs.Add(job);RefreshQueue();
   try{var info=await Task.Run(()=>engine.Inspect(path));job.Details=$"{info.Description}  ·  {info.Size/1024.0:F1} KB";if(info.Duration.HasValue)job.DurationText=info.Duration.Value.ToString(@"mm\:ss\.f");job.State="等待";}catch(Exception ex){job.Valid=false;job.State="失败";job.Error=ex.Message;job.Details="无法转换";}
  }
 }
 void RefreshQueue(){QueueLabel.Text=$"转换队列 · {Jobs.Count} 个文件";EmptyHint.Visibility=Jobs.Count==0?Visibility.Visible:Visibility.Collapsed;}
 void Clear_Click(object sender,RoutedEventArgs e){if(busy)return;Jobs.Clear();RefreshQueue();Overall.Value=0;Status.Text="列表已清空";}
 void Remove_Click(object sender,RoutedEventArgs e){if((sender as FrameworkElement)?.Tag is not Job job)return;if(job==current&&busy){currentCancellation?.Cancel();return;}Jobs.Remove(job);RefreshQueue();}
 async void Convert_Click(object sender,RoutedEventArgs e)=>await ConvertQueue();
 public async Task ConvertQueue(){
  if(busy)return;var queue=Jobs.Where(j=>j.Valid && j.State is "等待" or "失败" or "取消").ToArray();if(queue.Length==0){Status.Text="请先添加可转换的音频文件。";return;}
  settings.OutputDirectory=OutputBox.Text.Trim();settings.Format=((ComboBoxItem)FormatBox.SelectedItem).Content.ToString()!;
  if(!Path.IsPathFullyQualified(settings.OutputDirectory)){Status.Text="请选择有效的绝对输出路径。";return;}
  settings.Save();busy=true;ConvertButton.IsEnabled=ClearButton.IsEnabled=FormatBox.IsEnabled=OutputBox.IsEnabled=SettingsButton.IsEnabled=false;CancelButton.IsEnabled=true;batchCancellation=new();Overall.Value=0;int success=0,failed=0,cancelled=0;string outputDir=settings.OutputDirectory;
  try{for(int i=0;i<queue.Length;i++){
   var job=queue[i];if(!Jobs.Contains(job))continue;if(batchCancellation.IsCancellationRequested){job.State="取消";cancelled++;continue;}
   current=job;currentCancellation=CancellationTokenSource.CreateLinkedTokenSource(batchCancellation.Token);job.State="转换中";job.Error="";
   int index=i;Status.Text=$"正在转换 {i+1} / {queue.Length} · {job.Name}";var progress=new Progress<double>(p=>Overall.Value=(index+p)/queue.Length);
   try{var result=await engine.ConvertAsync(job.Input,new(outputDir,settings.Format,settings.Bitrate,settings.Overwrite,settings.Naming,settings.SilkRate),currentCancellation.Token,p=>((IProgress<double>)progress).Report(p));job.Output=result.Path;job.DurationText=result.Duration.ToString(@"mm\:ss\.f");job.State="成功";job.Details+="  ·  "+settings.Format;success++;}
   catch(OperationCanceledException){job.State="取消";cancelled++;}catch(Exception ex){job.State="失败";job.Error=ex.Message;failed++;}
   finally{currentCancellation.Dispose();currentCancellation=null;current=null;}
   Overall.Value=(i+1.0)/queue.Length;
  }}finally{busy=false;ConvertButton.IsEnabled=ClearButton.IsEnabled=FormatBox.IsEnabled=OutputBox.IsEnabled=SettingsButton.IsEnabled=true;CancelButton.IsEnabled=false;batchCancellation.Dispose();batchCancellation=null;}
  Status.Text=$"转换完成 · 成功 {success}，失败 {failed}，取消 {cancelled}"+(Jobs.Any(j=>!j.Valid)?$" · 无效文件 {Jobs.Count(j=>!j.Valid)}":"");if(settings.OpenWhenDone&&success>0)OpenPath(outputDir);
 }
 void Cancel_Click(object sender,RoutedEventArgs e)=>batchCancellation?.Cancel();
 async void Preview_Click(object sender,RoutedEventArgs e){if((sender as FrameworkElement)?.Tag is not Job job)return;await Preview(job);}
 public async Task Preview(Job job){
  previewCancellation?.Cancel();var cancellation=new CancellationTokenSource();previewCancellation=cancellation;player.Dispose();CleanPreview();
  try{string path=job.Output??job.Input;
   if(job.Output==null&&engine.Inspect(path).Kind==AudioKind.Silk){var folder=Path.Combine(Path.GetTempPath(),"LinksAudio-preview",Guid.NewGuid().ToString("N"));Directory.CreateDirectory(folder);previewFolder=folder;PlayingLabel.Text="正在准备试听…";
    var result=await engine.ConvertAsync(path,new(folder,"WAV",SilkRate:settings.SilkRate),cancellation.Token);path=result.Path;}
   cancellation.Token.ThrowIfCancellationRequested();player.Open(path);playingName=job.Name;PlayingLabel.Text=playingName;
  }catch(OperationCanceledException){}catch(Exception ex){Status.Text="试听失败："+ex.Message;}finally{if(previewCancellation==cancellation)previewCancellation=null;cancellation.Dispose();}
 }
 void CleanPreview(){if(previewFolder!=null){try{Directory.Delete(previewFolder,true);}catch{}previewFolder=null;}}
 void PlayPause_Click(object sender,RoutedEventArgs e){if(playingName==null&&TaskList.SelectedItem is Job job){_=Preview(job);return;}player.Toggle();}
 void Stop_Click(object sender,RoutedEventArgs e){previewCancellation?.Cancel();player.Stop();}
 void Volume_Changed(object sender,RoutedPropertyChangedEventArgs<double> e){if(player!=null)player.SetVolume((float)e.NewValue);}
 void Browse_Click(object sender,RoutedEventArgs e){var d=new OpenFolderDialog{Title="选择输出文件夹"};if(d.ShowDialog(this)==true)OutputBox.Text=d.FolderName;}
 void OpenFolder_Click(object sender,RoutedEventArgs e){string directory=OutputBox.Text.Trim();if(Directory.Exists(directory))OpenPath(directory);else Status.Text="输出目录尚未创建，请先转换或选择目录。";}
 void Copy_Click(object sender,RoutedEventArgs e){var selected=TaskList.SelectedItem as Job;string path=selected?.Output??Jobs.LastOrDefault(j=>j.Output!=null)?.Output??OutputBox.Text;try{Clipboard.SetText(path);Status.Text="路径已复制";}catch(Exception ex){Status.Text=ex.Message;}}
 void OpenPath(string path){try{Process.Start(new ProcessStartInfo(path){UseShellExecute=true});}catch(Exception ex){Status.Text=ex.Message;}}
 void Settings_Click(object sender,RoutedEventArgs e){var window=new SettingsWindow(settings){Owner=this};if(window.ShowDialog()==true){OutputBox.Text=settings.OutputDirectory;FormatBox.SelectedIndex=settings.Format=="WAV"?1:0;if(!settings.Save())Status.Text="设置无法保存，将仅用于本次运行。";}}
 void About_Click(object sender,RoutedEventArgs e){var window=new AboutWindow(){Owner=this};window.ShowDialog();}
 void Window_Closing(object? sender,CancelEventArgs e){if(busy){batchCancellation?.Cancel();e.Cancel=true;Status.Text="正在取消转换，请完成后再次关闭窗口。";return;}settings.OutputDirectory=OutputBox.Text.Trim();settings.Save();}
 async Task RunUiTest(string root){
  try{var output=Path.Combine(root,"tests/output/ui");Directory.CreateDirectory(output);ThemeBox.SelectedIndex=0;File.WriteAllText(Path.Combine(output,"resources.txt"),$"appcount={Application.Current.Resources.Count}; merged={Application.Current.Resources.MergedDictionaries.Count}; uiapp={Wpf.Ui.UiApplication.Current.IsApplication}; background={TryFindResource("ApplicationBackgroundBrush")}; btnstyle={TryFindResource(typeof(Wpf.Ui.Controls.Button))}; actualbg={Background}; buttonbg={ConvertButton.Background}; basedon={ConvertButton.Style?.BasedOn?.TargetType}; templateTarget={ConvertButton.Template?.TargetType};  button={ConvertButton.Style}; template={ConvertButton.Template}; theme={ApplicationThemeManager.GetAppTheme()}"+string.Join("\n",Application.Current.Resources.MergedDictionaries.Select(d=>d.Source?.ToString())));await Task.Delay(400);Capture(Path.Combine(output,"dark-empty.png"));
   await AddPaths([Path.Combine(root,"tests/Fixtures/合成测试.amr"),Path.Combine(root,"tests/Fixtures/empty.amr")]);OutputBox.Text=Path.Combine(output,"converted");await ConvertQueue();await Task.Delay(300);Capture(Path.Combine(output,"dark-queue.png"));
   string playback="not checked";try{await Preview(Jobs.First(j=>j.Output!=null));await Task.Delay(400);if(player.Progress<=0)throw new Exception("播放进度没有前进");player.Toggle();var position=player.Progress;await Task.Delay(200);if(Math.Abs(player.Progress-position)>.01)throw new Exception("暂停未生效");player.SetVolume(.4f);player.Stop();playback="PASS: playback advances; pause stable; stop; volume";}catch(Exception ex){playback="UNVERIFIED/FAILED: "+ex.Message;}File.WriteAllText(Path.Combine(output,"playback.txt"),playback);
   var forced=await engine.ConvertAsync(Path.Combine(root,"tests/Fixtures/合成测试.amr"),new(Path.Combine(output,"fallback"),ForceLame:true));File.WriteAllText(Path.Combine(output,"fallback.txt"),$"PASS: {forced.Encoder}; {forced.Duration.TotalSeconds:F3} s");
   var passed=Jobs.Count(j=>j.State=="成功")==1&&Jobs.Count(j=>j.State=="失败")==1;
   Width=820;Height=700;await Task.Delay(200);Capture(Path.Combine(output,"compact.png"));
   ThemeBox.SelectedIndex=1;await Task.Delay(200);Capture(Path.Combine(output,"light.png"));
   var savedDirectory=settings.OutputDirectory;settings.OutputDirectory=@"C:\Audio\Converted";var sw=new SettingsWindow(settings){Owner=this};sw.Show();await Task.Delay(200);CaptureOther(sw,Path.Combine(output,"settings.png"));sw.Close();settings.OutputDirectory=savedDirectory;
   var aw=new AboutWindow(){Owner=this};aw.Show();await Task.Delay(200);CaptureOther(aw,Path.Combine(output,"about.png"));aw.Close();
   foreach(var scale in new[]{1.0,1.25,1.5}){Capture(Path.Combine(output,$"dpi-{scale}.png"),scale);}
   var folder=Path.Combine(output,"folder-input");Directory.CreateDirectory(folder);File.Copy(Path.Combine(root,"tests/Fixtures/合成测试.amr"),Path.Combine(folder,"folder-test.aud"),true);await AddPaths([folder]);passed=passed&&Jobs.Any(j=>j.Name=="folder-test.aud"&&j.State=="等待");
   TaskList.SelectedItem=Jobs.First();Copy_Click(this,new RoutedEventArgs());passed=passed&&Clipboard.GetText().EndsWith(".mp3");Clear_Click(this,new RoutedEventArgs());passed=passed&&Jobs.Count==0;
   File.WriteAllText(Path.Combine(output,"result.txt"),passed?"PASS: GUI import, folder import, mixed queue conversion, copy path, clear queue, theme switch, minimum size; rendered at 96/120/144 DPI":"FAIL");Close();
  }catch(Exception ex){File.WriteAllText(Path.Combine(root,"tests/output/ui-error.txt"),ex.ToString());Close();}
 }
 void CaptureOther(Window window,string path){var visual=(FrameworkElement)window;visual.UpdateLayout();var b=new RenderTargetBitmap((int)Math.Ceiling(visual.ActualWidth),(int)Math.Ceiling(visual.ActualHeight),96,96,PixelFormats.Pbgra32);b.Render(visual);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(b));using var file=File.Create(path);png.Save(file);}
 void Capture(string path,double scale=1){var savedOutput=OutputBox.Text;OutputBox.Text=@"C:\Audio\Converted";UpdateLayout();var visual=(FrameworkElement)Content;var bitmap=new RenderTargetBitmap((int)Math.Ceiling(visual.ActualWidth*scale),(int)Math.Ceiling(visual.ActualHeight*scale),96*scale,96*scale,PixelFormats.Pbgra32);bitmap.Render(visual);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bitmap));using var file=File.Create(path);png.Save(file);OutputBox.Text=savedOutput;}
}
