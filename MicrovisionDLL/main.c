#ifdef WIN32
#include <windows.h>
#endif
#include <stdio.h>
#include <stdlib.h>
#include <tchar.h>
#include <string.h>
#include <stdbool.h>
#include <stdint.h>

#include "calling_conventions.h"
/*
  by Luigi Auriemma
  modified by derplayer
*/
#define WIN32

static unsigned char    microvision_decompress_dump[] =
    "\x83\xec\x18\x53\x8b\x18\x55\x56\x8b\x70\x04\x33\xc0\x57\x89\x5c\x24\x20\x89\x74\x24\x24\xbf\x01\x00\x00\x00\x89\x44\x24\x14\x89\x44\x24\x18\x83\x7c\x24\x14\x00\x75\x24\x3b\xde\xc7\x44\x24\x14\x80\x00\x00\x00\x74\x10\x0f\xb6\x03\x83\xc3\x01\x89\x44\x24\x18\x89\x5c\x24\x20\xeb\x08\xc7\x44\x24\x18\xff\xff\xff\xff\x8b\x4c\x24\x14\x85\x4c\x24\x18\x74\x51\x3b\xde\x74\x10\x0f\xb6\x13\x83\xc3\x01\x89\x54\x24\x1c\x89\x5c\x24\x20\xeb\x08\xc7\x44\x24\x1c\xff\xff\xff\xff\x0f\xb6\x44\x24\x1c\x8d\x4c\x24\x13\x51\x8b\x4c\x24\x34\x88\x44\x24\x17\xe8\xbc\x00\x00\x00\x0f\xb6\x44\x24\x1c\x8b\x54\x24\x2c\x88\x04\x17\x83\xc7\x01\x81\xe7\xff\x0f\x00\x00\xd1\x7c\x24\x14\xe9\x7a\xff\xff\xff\x3b\xde\x74\x0c\x0f\xb6\x03\x83\xc3\x01\x89\x5c\x24\x20\xeb\x03\x83\xc8\xff\x3b\xde\x74\x0c\x0f\xb6\x0b\x83\xc3\x01\x89\x5c\x24\x20\xeb\x03\x83\xc9\xff\xc1\xe0\x08\x0b\xc1\x8b\xe8\xc1\xfd\x04\x74\x62\x83\xe0\x0f\x83\xc0\x01\x89\x44\x24\x1c\xbe\x00\x00\x00\x00\x78\x44\x8d\x64\x24\x00\x8b\x54\x24\x2c\x8d\x0c\x2e\x81\xe1\xff\x0f\x00\x00\x0f\xb6\x1c\x11\x8b\x4c\x24\x30\x8d\x44\x24\x13\x50\x88\x5c\x24\x17\xe8\x34\x00\x00\x00\x8b\x4c\x24\x2c\x88\x1c\x0f\x83\xc7\x01\x83\xc6\x01\x81\xe7\xff\x0f\x00\x00\x3b\x74\x24\x1c\x7e\xc4\x8b\x5c\x24\x20\x8b\x74\x24\x24\xd1\x7c\x24\x14\xe9\xe6\xfe\xff\xff\x5f\x5e\x5d\x5b\x83\xc4\x18\xc2\x08\x00\x55\x8b\xec\x83\xe4\xf8\x83\xec\x08\x56\x8b\xf1\x8b\x46\x04\x85\xc0\x57\x75\x04\x33\xd2\xeb\x05\x8b\x56\x08\x2b\xd0\x85\xc0\x74\x21\x8b\x4e\x0c\x2b\xc8\x3b\xd1\x73\x18\x8b\x46\x08\x8b\x4d\x08\x8a\x11\x88\x10\x83\xc0\x01\x89\x46\x08\x5f\x5e\x8b\xe5\x5d\xc2\x04\x00\xcc";
void (* __cdecl microvision_decompress_func)() = NULL;  // void *eax, void *esi, void *out

typedef unsigned char u8;

// Address range: 0x41a000 - 0x41a052
__declspec(dllexport) int32_t sub_41A000(int32_t a1) {
    int32_t v1; // eax
    int32_t result; // ecx
    uint64_t v2 = 0x78787879 * (int64_t)(v1 - result); // 0x41a00c
    int32_t v3 = v2 / 0x100000000; // 0x41a00c
    if ((int32_t)(v3 < -31) + v3 / 32 < 1) {
        // 0x41a04e
        return result;
    }
    int32_t v4 = (int32_t)(v3 < -31) + v3 / 32; // 0x41a016
    int32_t result2; // 0x41a0322
    while (true) {
        int32_t v5 = (v4 - (v4 >> 31)) / 2; // 0x41a029
        int32_t v6 = 68 * v5 + result; // 0x41a032
        int32_t v7;
        if (*(int16_t *)v6 < *(int16_t *)a1) {
            // 0x41a03b
            result2 = v6 + 68;
            v7 = v4 - 1 - v5;
        } else {
            result2 = result;
            v7 = v5;
        }
        // 0x41a049
        if (v7 >= 0 != v7 != 0) {
            // break -> 0x41a04d
            break;
        }
        result = result2;
        v4 = v7;
    }
    // 0x41a04e
    return result2;
}

// anti DEP limitation! if you apply VirtualAlloc on a static char
// it will cover also the rest of the page included other variables!
void *microvision_decompress_alloc(u8 *dump, int dumpsz) {
    int     pagesz;
    void    *ret;

    pagesz = (dumpsz + 4095) & (~4095); // useful for pages? mah

#ifdef WIN32

    ret = VirtualAlloc(
        NULL,
        pagesz,
        MEM_COMMIT | MEM_RESERVE,
        PAGE_EXECUTE_READWRITE);    // write for memcpy

#else
    ret = malloc(pagesz);
    mprotect(
        ret,
        pagesz,
        PROT_EXEC | PROT_WRITE);    // write for memcpy
#endif
    memcpy(ret, dump, dumpsz);
    return(ret);
}

void alloc(unsigned char **data, int size) {
    *data = (unsigned char *) malloc( size );
    if ( *data ) memset( *data, 0, size );
}

__declspec(dllexport) unsigned char* microvision_decompress(unsigned char* in, int osize, int insz)
{
    unsigned char * out = NULL;
    alloc(&out, osize);

    unsigned char   eax[12],
                    esi[16];
    int             tmpsz;
    unsigned char   *tmp;

    if(!microvision_decompress_func) {
        microvision_decompress_func = microvision_decompress_alloc(microvision_decompress_dump, sizeof(microvision_decompress_dump));
    }

    tmpsz = osize;
    tmp = calloc(tmpsz, 1);
    if(!tmp) return -1;

    *(void **)(eax+0)  = in;
    *(void **)(eax+4)  = in + insz;
    *(int *)  (eax+8)  = insz;

    *(void **)(esi+0)  = 0;
    *(void **)(esi+4)  = tmp;
    *(void **)(esi+8)  = tmp;
    *(void **)(esi+12) = tmp + tmpsz;

    usercall_call(microvision_decompress_func, 9, 0, eax, 0, 0, 0, 0, 0, out, esi);
    free(tmp);
    //free(in);

    //return *(void **)(esi+8) - *(void **)(esi+4);
    return out;
}

__declspec(dllexport) void microvision_free(unsigned char *buffer)
{
  free(buffer);
}

int main()
{
    printf("Microvision Decompressor - CLI Mode\n");

    // Reading size of file
    FILE * file = fopen("dump.bin", "r+");
    if (file == NULL) return;
    fseek(file, 0, SEEK_END);
    int insz = ftell(file);
    fclose(file);

    // Reading data to array of unsigned chars
    file = fopen("dump.bin", "r+");
    unsigned char * in = (unsigned char *) malloc(insz);
    int bytes_read = fread(in, sizeof(unsigned char), insz, file);
    fclose(file);

    auto tmpData = microvision_decompress(in, 65535, insz);
    microvision_free(tmpData);

    return 1;
}
