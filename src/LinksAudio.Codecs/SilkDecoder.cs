
using System.Buffers.Binary;
using System.Runtime.InteropServices;
namespace LinksAudio.Codecs;
public static class SilkDecoder {
 [DllImport("LinksSilk", CallingConvention=CallingConvention.Cdecl)] static extern IntPtr la_create();
 [DllImport("LinksSilk", CallingConvention=CallingConvention.Cdecl)] static extern void la_destroy(IntPtr state);
 [DllImport("LinksSilk", CallingConvention=CallingConvention.Cdecl)] static extern int la_decode(IntPtr state, byte[] packet,int size,int rate,short[] output,int cap);
 public static bool IsSilk(ReadOnlySpan<byte> data) => data.StartsWith("#!SILK_V3"u8) || (data.Length>0 && data[0]==2 && data[1..].StartsWith("#!SILK_V3"u8));
 public static List<(int Offset,int Length)> Packets(ReadOnlySpan<byte> data) {
  if(!IsSilk(data)) throw new InvalidDataException("不是有效的 SILK V3 文件头。");
  int o=data[0]==2?10:9;var packets=new List<(int,int)>();
  while(o<data.Length) {
   if(o+2>data.Length) throw new InvalidDataException("SILK 帧长度被截断。");
   int n=BinaryPrimitives.ReadInt16LittleEndian(data[o..]);o+=2;
   if(n==-1) { if(o!=data.Length) throw new InvalidDataException("SILK 结束标记后存在多余数据。");break; }
   if(n<=0 || n>5120) throw new InvalidDataException("SILK 帧长度无效。");
   if(o+n>data.Length) throw new InvalidDataException("SILK 音频数据不完整，末帧被截断。");
   packets.Add((o,n));o+=n;
  }
  if(packets.Count==0) throw new InvalidDataException("SILK 文件中没有音频帧。");return packets;
 }
 // This is the requested PCM output rate, not a claim about source recording rate.
 public static void Decode(byte[] data,Stream output,int rate=24000,CancellationToken token=default,Action<double>? progress=null) {
  if(rate is not (8000 or 12000 or 16000 or 24000 or 48000)) throw new ArgumentOutOfRangeException(nameof(rate));
  var packets=Packets(data);token.ThrowIfCancellationRequested();var state=la_create();
  if(state==IntPtr.Zero) throw new InvalidOperationException("无法初始化 SILK 解码器。");
  try {
   short[] samples=new short[rate/10];int index=0;
   foreach(var (offset,length) in packets) {
    token.ThrowIfCancellationRequested();var packet=data.AsSpan(offset,length).ToArray();
    int n=la_decode(state,packet,length,rate,samples,samples.Length);
    if(n<=0) throw new InvalidDataException($"SILK 第 {index+1} 帧解码失败（{n}），请检查原文件是否损坏。");
    output.Write(MemoryMarshal.AsBytes(samples.AsSpan(0,n)));progress?.Invoke(++index/(double)packets.Count);
   }
  } finally {la_destroy(state);}
 }
}
