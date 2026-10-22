
using System.Text.Json;
namespace LinksAudio.Core;
public sealed class AppSettings {
 public string Theme {get;set;}="深色";
 public string OutputDirectory {get;set;}=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),"Links Audio");
 public string Format {get;set;}="MP3";
 public int Bitrate {get;set;}=192;
 public bool Overwrite {get;set;}
 public bool OpenWhenDone {get;set;}
 public string Naming {get;set;}="{name}";
 public int SilkRate {get;set;}=24000;
 public static string ConfigPath {get;private set;}="";
 public static AppSettings Load() {
  var local=Path.Combine(AppContext.BaseDirectory,"config");
  try {Directory.CreateDirectory(local);string probe=Path.Combine(local,Guid.NewGuid()+".tmp");using(File.Create(probe)){}File.Delete(probe);}catch{local=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"LinksAudio");}
  ConfigPath=Path.Combine(local,"settings.json");try {return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(ConfigPath))??new();}catch{return new();}
 }
 public bool Save() {try{Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);string temp=ConfigPath+".tmp";File.WriteAllText(temp,JsonSerializer.Serialize(this,new JsonSerializerOptions{WriteIndented=true}));File.Move(temp,ConfigPath,true);return true;}catch{return false;}}
}
