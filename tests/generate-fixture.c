/* MIT, Copyright (c) 2026 Links Tam. Synthetic tones only; no recorded audio. */
#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include "SKP_Silk_SDK_API.h"
int main(int argc,char **argv){
 if(argc!=3)return 1; FILE *silk=fopen(argv[1],"wb"),*ref=fopen(argv[2],"wb");if(!silk||!ref)return 2;
 SKP_int32 es=0,ds=0;SKP_Silk_SDK_Get_Encoder_Size(&es);SKP_Silk_SDK_Get_Decoder_Size(&ds);void* e=calloc(1,es);void* d=calloc(1,ds);
 SKP_SILK_SDK_EncControlStruct c={0};SKP_Silk_SDK_InitEncoder(e,&c);SKP_Silk_SDK_InitDecoder(d);
 c.API_sampleRate=24000;c.maxInternalSampleRate=24000;c.packetSize=480;c.bitRate=24000;c.complexity=2;c.packetLossPercentage=0;c.useInBandFEC=0;c.useDTX=0;
 fputc(2,silk);fwrite("#!SILK_V3",1,9,silk);
 for(int frame=0;frame<328;frame++){
  short pcm[480];for(int j=0;j<480;j++){double t=(frame*480+j)/24000.0;double env=fmin(1.0,fmin(t/.05,(6.56-t)/.05));pcm[j]=(short)(env*(5000*sin(2*3.141592653589793*440*t)+2000*sin(2*3.141592653589793*660*t)));}
  unsigned char packet[5120];SKP_int16 length=5120;int rc=SKP_Silk_SDK_Encode(e,&c,pcm,480,packet,&length);if(rc||length<=0)return 3;
  fputc(length&255,silk);fputc((length>>8)&255,silk);fwrite(packet,1,length,silk);
  SKP_SILK_SDK_DecControlStruct dc={0};dc.API_sampleRate=24000;
  do{short out[2400];SKP_int16 n=0;if(SKP_Silk_SDK_Decode(d,&dc,0,packet,length,out,&n)!=0)return 4;fwrite(out,2,n,ref);}while(dc.moreInternalDecoderFrames);
 }
 fclose(silk);fclose(ref);free(e);free(d);return 0;
}
