
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace LinksAudio.App;
public sealed class Job : INotifyPropertyChanged {
 public required string Input {get;init;}
 public string Name=>System.IO.Path.GetFileName(Input);
 string state="等待",details="正在识别…",error="",duration="—";
 public string State {get=>state;set{state=value;Changed();}}
 public string Details {get=>details;set{details=value;Changed();}}
 public string Error {get=>error;set{error=value;Changed();}}
 public string DurationText {get=>duration;set{duration=value;Changed();}}
 public string? Output {get;set;}
 public bool Valid {get;set;}=true;
 public event PropertyChangedEventHandler? PropertyChanged;
 void Changed([CallerMemberName]string? name=null)=>PropertyChanged?.Invoke(this,new(name));
}
