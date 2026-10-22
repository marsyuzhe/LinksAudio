
using System.IO;
using System.Windows;
namespace LinksAudio.App;
public partial class App:Application {
 protected override void OnStartup(StartupEventArgs e) {
  DispatcherUnhandledException+=(s,a)=>{MessageBox.Show(a.Exception.Message,"Links Audio",MessageBoxButton.OK,MessageBoxImage.Error);a.Handled=true;};
  base.OnStartup(e);
 }
}
