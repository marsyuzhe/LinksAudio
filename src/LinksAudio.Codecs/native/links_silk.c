
#include <stdlib.h>
#include "SKP_Silk_SDK_API.h"
__declspec(dllexport) void* la_create(void) {
    SKP_int32 n=0; if(SKP_Silk_SDK_Get_Decoder_Size(&n)!=0) return NULL;
    void *s=calloc(1,n); if(s && SKP_Silk_SDK_InitDecoder(s)!=0) {free(s);return NULL;} return s;
}
__declspec(dllexport) void la_destroy(void *s) {free(s);}
__declspec(dllexport) int la_decode(void *s,const unsigned char *p,int size,int rate,short *out,int cap) {
    SKP_SILK_SDK_DecControlStruct c={0};c.API_sampleRate=rate;
    int total=0,frames=0;
    do {
        SKP_int16 n=0;
        if(cap-total < rate/50 || ++frames>5) return -100;
        int r=SKP_Silk_SDK_Decode(s,&c,0,p,size,out+total,&n);
        if(r!=0) return r;
        if(n<=0 || n>cap-total) return -101;
        total+=n;
    } while(c.moreInternalDecoderFrames);
    return total;
}
