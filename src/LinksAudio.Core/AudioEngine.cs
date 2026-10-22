
using LinksAudio.Codecs;
using NAudio.Wave;
using NAudio.Lame;
namespace LinksAudio.Core;
public enum AudioKind { Silk, Wav, Mp3, AmrNb, AmrWb, Unknown }
public record AudioInfo(AudioKind Kind,long Size,TimeSpan? Duration,string Description);
public record ConversionOptions(string Directory,string Format="MP3",int Bitrate=192,bool Overwrite=false,string Naming="{name}",int SilkRate=24000,bool ForceLame=false);
public record ConversionResult(string Path,TimeSpan Duration,string Encoder);
public sealed class AudioEngine {
 public static AudioKind Detect(ReadOnlySpan<byte> h) {
  if(SilkDecoder.IsSilk(h)) return AudioKind.Silk;
  if(h.StartsWith("#!AMR-WB\n"u8)) return AudioKind.AmrWb;
  if(h.StartsWith("#!AMR\n"u8)) return AudioKind.AmrNb;
  if(h.Length>=12 && h[..4].SequenceEqual("RIFF"u8) && h.Slice(8,4).SequenceEqual("WAVE"u8)) return AudioKind.Wav;
  if(h.StartsWith("ID3"u8) || (h.Length>=2 && h[0]==255 && (h[1]&0xE6)==0xE2)) return AudioKind.Mp3;
  return AudioKind.Unknown;
 }
 public AudioInfo Inspect(string path) {
  using var f=File.OpenRead(path);Span<byte> h=stackalloc byte[32];int n=f.Read(h);var kind=Detect(h[..n]);TimeSpan? duration=null;
  if(f.Length==0) throw new InvalidDataException("文件为空。");
  if(kind==AudioKind.Silk) { if(f.Length>128*1024*1024) throw new InvalidDataException("SILK 文件超过 128 MB 的保护限制。");SilkDecoder.Packets(File.ReadAllBytes(path)); }
  else if(kind is AudioKind.Mp3 or AudioKind.Wav) {using var reader=OpenReader(path,kind);duration=reader.TotalTime;}
  else if(kind is AudioKind.AmrNb or AudioKind.AmrWb) throw new NotSupportedException($"已识别为真正的 {(kind==AudioKind.AmrNb?"AMR-NB":"AMR-WB")}，此版本未集成 AMR 解码后端。");
  else throw new NotSupportedException("无法识别音频内容。支持 SILK V3、WAV 和 MP3；扩展名不作为判断依据。");
  return new(kind,f.Length,duration,kind==AudioKind.Silk?"SILK V3":kind.ToString().ToUpperInvariant());
 }
 public static WaveStream OpenReader(string path,AudioKind kind) => kind switch {AudioKind.Wav=>new WaveFileReader(path),AudioKind.Mp3=>new Mp3FileReaderBase(path,wf=>new NLayer.NAudioSupport.Mp3FrameDecompressor(wf)),_=>throw new NotSupportedException("无法读取此格式。")};
 public async Task<ConversionResult> ConvertAsync(string input,ConversionOptions options,CancellationToken token=default,Action<double>? progress=null) => await Task.Run(()=>Convert(input,options,token,progress),token);
 ConversionResult Convert(string input,ConversionOptions o,CancellationToken token,Action<double>? progress) {
  token.ThrowIfCancellationRequested();var info=Inspect(input);Directory.CreateDirectory(o.Directory);
  if(o.Format is not ("MP3" or "WAV")) throw new ArgumentException("输出格式只支持 MP3 / WAV。");
  string stem=o.Naming.Replace("{name}",Path.GetFileNameWithoutExtension(input)).Replace("{date}",DateTime.Now.ToString("yyyyMMdd"));
  if(string.IsNullOrWhiteSpace(stem)||stem.IndexOfAny(Path.GetInvalidFileNameChars())>=0||stem is "." or "..") throw new ArgumentException("文件名规则包含无效字符。");
  string final=Path.Combine(o.Directory,stem+"."+o.Format.ToLowerInvariant());
  if(Path.GetFullPath(final).Equals(Path.GetFullPath(input),StringComparison.OrdinalIgnoreCase)) final=Path.Combine(o.Directory,stem+"_converted."+o.Format.ToLowerInvariant());
  if(!o.Overwrite) {int i=2;string basePath=final;while(File.Exists(final)) final=Path.Combine(o.Directory,Path.GetFileNameWithoutExtension(basePath)+$" ({i++})"+Path.GetExtension(basePath));}
  string work=Path.Combine(Path.GetTempPath(),"LinksAudio",Guid.NewGuid().ToString("N"));Directory.CreateDirectory(work);
  string stage=Path.Combine(o.Directory,".links-"+Guid.NewGuid().ToString("N")+"."+o.Format.ToLowerInvariant());
  try {
   string wav=Path.Combine(work,"decoded.wav");TimeSpan expected;
   if(info.Kind==AudioKind.Silk) {
    using(var writer=new WaveFileWriter(wav,new WaveFormat(o.SilkRate,16,1))) SilkDecoder.Decode(File.ReadAllBytes(input),writer,o.SilkRate,token,p=>progress?.Invoke(p*.45));
   } else {
    using var source=OpenReader(input,info.Kind);if(source.WaveFormat.Encoding is WaveFormatEncoding.Pcm or WaveFormatEncoding.IeeeFloat) {using var writer=new WaveFileWriter(wav,source.WaveFormat);Copy(source,writer,token,null);} else {using var pcm=WaveFormatConversionStream.CreatePcmStream(source);using var writer=new WaveFileWriter(wav,pcm.WaveFormat);Copy(pcm,writer,token,null);}
   }
   using(var reader=new WaveFileReader(wav)) expected=reader.TotalTime;
   if(expected<=TimeSpan.Zero) throw new InvalidDataException("解码结果为空。");token.ThrowIfCancellationRequested();string encoder="PCM";
   if(o.Format=="WAV") File.Copy(wav,stage);
   else {
    bool done=false;
    if(!o.ForceLame) try {
     using var source=new WaveFileReader(wav);using var resample=new MediaFoundationResampler(source,new WaveFormat(44100,16,source.WaveFormat.Channels));
     using var cancellable=new CheckedProvider(resample,token);MediaFoundationEncoder.EncodeToMp3(cancellable,stage,o.Bitrate*1000);encoder="Windows Media Foundation";done=true;
    } catch(OperationCanceledException) {throw;} catch {if(File.Exists(stage)) File.Delete(stage);}
    if(!done) {using var source=new WaveFileReader(wav);using var writer=new LameMP3FileWriter(stage,source.WaveFormat,o.Bitrate);Copy(source,writer,token,null);encoder="LAME 3.100";}
   }
   token.ThrowIfCancellationRequested();progress?.Invoke(.8);
   var duration=Validate(stage,o.Format,expected,token);token.ThrowIfCancellationRequested();
   File.Move(stage,final,o.Overwrite);progress?.Invoke(1);return new(final,duration,encoder);
  } finally {try{if(File.Exists(stage))File.Delete(stage);}catch{}try{Directory.Delete(work,true);}catch{}}
 }
 public static TimeSpan Validate(string file,string format,TimeSpan expected,CancellationToken token=default) {
  using var r=OpenReader(file,format=="WAV"?AudioKind.Wav:AudioKind.Mp3);long total=0;byte[] buffer=new byte[32768];int n;
  while((n=r.Read(buffer,0,buffer.Length))>0) {token.ThrowIfCancellationRequested();total+=n;}
  var actual=TimeSpan.FromSeconds(total/(double)r.WaveFormat.AverageBytesPerSecond);
  if(actual<=TimeSpan.Zero || Math.Abs((actual-expected).TotalSeconds)>Math.Max(.3,expected.TotalSeconds*.02)) throw new InvalidDataException("输出音频校验失败：无法完整读取或时长不一致。");return actual;
 }
 static void Copy(Stream from,Stream to,CancellationToken token,Action<double>? progress) {byte[] b=new byte[32768];int n;while((n=from.Read(b,0,b.Length))>0){token.ThrowIfCancellationRequested();to.Write(b,0,n);}}
 sealed class CheckedProvider(IWaveProvider inner,CancellationToken token):IWaveProvider,IDisposable {public WaveFormat WaveFormat=>inner.WaveFormat;public int Read(byte[] b,int o,int n){token.ThrowIfCancellationRequested();return inner.Read(b,o,n);}public void Dispose(){}}
}
