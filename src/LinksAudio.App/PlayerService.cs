
using NAudio.Wave;
namespace LinksAudio.App;
public sealed class PlayerService:IDisposable {
 WaveOutEvent? device;WaveStream? reader;
 public float Volume {get;set;}=.7f;
 public double Progress=>reader==null||reader.TotalTime.TotalSeconds==0?0:reader.CurrentTime.TotalSeconds/reader.TotalTime.TotalSeconds;
 public string TimeText=>reader==null?"":$@"{reader.CurrentTime:mm\:ss} / {reader.TotalTime:mm\:ss}";
 public void Open(string path) {Dispose();using var probe=System.IO.File.OpenRead(path);byte[] header=new byte[32];int length=probe.Read(header);reader=LinksAudio.Core.AudioEngine.OpenReader(path,LinksAudio.Core.AudioEngine.Detect(header.AsSpan(0,length)));device=new WaveOutEvent();device.Init(reader);device.Volume=Volume;device.Play();}
 public void Toggle(){if(device?.PlaybackState==PlaybackState.Playing)device.Pause();else device?.Play();}
 public void Stop(){device?.Stop();if(reader!=null)reader.CurrentTime=TimeSpan.Zero;}
 public void SetVolume(float value){Volume=value;if(device!=null)device.Volume=value;}
 public void Dispose(){device?.Stop();device?.Dispose();reader?.Dispose();device=null;reader=null;}
}

