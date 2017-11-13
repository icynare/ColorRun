/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/10/17 14:06:03
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

/*
 * Copyright (c) 2005 Oren J. Maurice <oymaurice@hazorea.org.il>
 * 
 * Redistribution and use in source and binary forms, with or without modifica-
 * tion, are permitted provided that the following conditions are met:
 * 
 *   1.  Redistributions of source code must retain the above copyright notice,
 *       this list of conditions and the following disclaimer.
 * 
 *   2.  Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 * 
 *   3.  The name of the author may not be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MER-
 * CHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO
 * EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPE-
 * CIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTH-
 * ERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * Alternatively, the contents of this file may be used under the terms of
 * the GNU General Public License version 2 (the "GPL"), in which case the
 * provisions of the GPL are applicable instead of the above. If you wish to
 * allow the use of your version of this file only under the terms of the
 * GPL and not to allow others to use your version of this file under the
 * BSD license, indicate your decision by deleting the provisions above and
 * replace them with the notice and other provisions required by the GPL. If
 * you do not delete the provisions above, a recipient may use your version
 * of this file under either the BSD or the GPL.
 */
using System;

namespace LZF.NET {
    public class CRC {
        // CRC32 data & function
        uint[] crc_32_tab = new uint[256]
        {
            0x00000000, 0x77073096, 0xee0e612c, 0x990951ba, 0x076dc419,
            0x706af48f, 0xe963a535, 0x9e6495a3, 0x0edb8832, 0x79dcb8a4,
            0xe0d5e91e, 0x97d2d988, 0x09b64c2b, 0x7eb17cbd, 0xe7b82d07,
            0x90bf1d91, 0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de,
            0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7, 0x136c9856,
            0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9,
            0xfa0f3d63, 0x8d080df5, 0x3b6e20c8, 0x4c69105e, 0xd56041e4,
            0xa2677172, 0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b,
            0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940, 0x32d86ce3,
            0x45df5c75, 0xdcd60dcf, 0xabd13d59, 0x26d930ac, 0x51de003a,
            0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423, 0xcfba9599,
            0xb8bda50f, 0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924,
            0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d, 0x76dc4190,
            0x01db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x06b6b51f,
            0x9fbfe4a5, 0xe8b8d433, 0x7807c9a2, 0x0f00f934, 0x9609a88e,
            0xe10e9818, 0x7f6a0dbb, 0x086d3d2d, 0x91646c97, 0xe6635c01,
            0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e, 0x6c0695ed,
            0x1b01a57b, 0x8208f4c1, 0xf50fc457, 0x65b0d9c6, 0x12b7e950,
            0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3,
            0xfbd44c65, 0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2,
            0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb, 0x4369e96a,
            0x346ed9fc, 0xad678846, 0xda60b8d0, 0x44042d73, 0x33031de5,
            0xaa0a4c5f, 0xdd0d7cc9, 0x5005713c, 0x270241aa, 0xbe0b1010,
            0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,
            0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17,
            0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad, 0xedb88320, 0x9abfb3b6,
            0x03b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x04db2615,
            0x73dc1683, 0xe3630b12, 0x94643b84, 0x0d6d6a3e, 0x7a6a5aa8,
            0xe40ecf0b, 0x9309ff9d, 0x0a00ae27, 0x7d079eb1, 0xf00f9344,
            0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb,
            0x196c3671, 0x6e6b06e7, 0xfed41b76, 0x89d32be0, 0x10da7a5a,
            0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5,
            0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252, 0xd1bb67f1,
            0xa6bc5767, 0x3fb506dd, 0x48b2364b, 0xd80d2bda, 0xaf0a1b4c,
            0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef,
            0x4669be79, 0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236,
            0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f, 0xc5ba3bbe,
            0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31,
            0x2cd99e8b, 0x5bdeae1d, 0x9b64c2b0, 0xec63f226, 0x756aa39c,
            0x026d930a, 0x9c0906a9, 0xeb0e363f, 0x72076785, 0x05005713,
            0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0x0cb61b38, 0x92d28e9b,
            0xe5d5be0d, 0x7cdcefb7, 0x0bdbdf21, 0x86d3d2d4, 0xf1d4e242,
            0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1,
            0x18b74777, 0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c,
            0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45, 0xa00ae278,
            0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7,
            0x4969474d, 0x3e6e77db, 0xaed16a4a, 0xd9d65adc, 0x40df0b66,
            0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
            0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605,
            0xcdd70693, 0x54de5729, 0x23d967bf, 0xb3667a2e, 0xc4614ab8,
            0x5d681b02, 0x2a6f2b94, 0xb40bbe37, 0xc30c8ea1, 0x5a05df1b,
            0x2d02ef8d
        };

        uint _value = 0xFFFFFFFF;

        public void Init() { _value = 0xFFFFFFFF; }

        public void CalcCrc32(byte NewData) {
            _value = crc_32_tab[(_value & 0xff) ^ NewData] ^ (_value >> 8);
        }

        public void CalcCrc32(byte[] data, uint offset, uint size) {
            for (uint i = 0; i < size; i++) {
                _value = crc_32_tab[(_value & 0xff) ^ data[offset + i]] ^ (_value >> 8);
            }
        }

        public uint GetDigest() { return _value ^ 0xFFFFFFFF; }

        public static uint CalculateDigest(byte[] data, uint offset, uint size) {
            CRC crc = new CRC();
            crc.Init();
            crc.CalcCrc32(data, offset, size);
            return crc.GetDigest();
        }

        public static uint CalculateDigest(string path)
        {
            byte[] data = System.IO.File.ReadAllBytes(path);
            return CalculateDigest(data, 0, (uint)data.Length);
        }
    }

    /// <summary>
    /// Summary description for CLZF.
    /// </summary>
    public class CLZF {
        /// <summary>
        /// LZF Compressor
        /// </summary>
        UInt32 HLOG = 14;
        UInt32 HSIZE = (1 << 14);
        /*
		* don't play with this unless you benchmark!
		* decompression is not dependent on the hash function
		* the hashing function might seem strange, just believe me
		* it works ;)
		*/
        UInt32 MAX_LIT = (1 << 5);
        UInt32 MAX_OFF = (1 << 13);
        UInt32 MAX_REF = ((1 << 8) + (1 << 3));
        UInt32 FRST(byte[] Array, UInt32 ptr)
        {
            return (UInt32)(((Array[ptr]) << 8) | Array[ptr + 1]);
        }

        UInt32 NEXT(UInt32 v, byte[] Array, UInt32 ptr)
        {
            return ((v) << 8) | Array[ptr + 2];
        }

        UInt32 IDX(UInt32 h)
        {
            return ((((h ^ (h << 5)) >> (int)(3 * 8 - HLOG)) - h * 5) & (HSIZE - 1));
        }

        /*
		* compressed format
		*
		* 000LLLLL <L+1>    ; literal
		* LLLOOOOO oooooooo ; backref L
		* 111OOOOO LLLLLLLL oooooooo ; backref L+7
		*
		*/
        public int lzf_compress(byte[] in_data, int in_len, byte[] out_data, int out_len)
        {
            int c;
            long[] htab = new long[1 << 14];
            for (c = 0; c < 1 << 14; c++)
            {
                htab[c] = 0;
            }

            long hslot;
            UInt32 iidx = 0;
            UInt32 oidx = 0;
            //byte *in_end  = ip + in_len;
            //byte *out_end = op + out_len;
            long reference;
            UInt32 hval = FRST(in_data, iidx);
            long off;
            int lit = 0;
            for (;;)
            {
                if (iidx < in_len - 2)
                {
                    hval = NEXT(hval, in_data, iidx);
                    hslot = IDX(hval);
                    reference = htab[hslot];
                    htab[hslot] = (long)iidx;
                    if ((off = iidx - reference - 1) < MAX_OFF && iidx + 4 < in_len && reference > 0 && in_data[reference + 0] == in_data[iidx + 0] && in_data[reference + 1] == in_data[iidx + 1] && in_data[reference + 2] == in_data[iidx + 2])
                    {
                        /* match found at *reference++ */
                        UInt32 len = 2;
                        UInt32 maxlen = (UInt32)in_len - iidx - len;
                        maxlen = maxlen > MAX_REF ? MAX_REF : maxlen;
                        if (oidx + lit + 1 + 3 >= out_len)
                            return 0;
                        do len++;
                        while (len < maxlen && in_data[reference + len] == in_data[iidx + len]);
                        if (lit != 0)
                        {
                            out_data[oidx++] = (byte)(lit - 1);
                            lit = -lit;
                            do out_data[oidx++] = in_data[iidx + lit];
                            while ((++lit) != 0);
                        }

                        len -= 2;
                        iidx++;
                        if (len < 7)
                        {
                            out_data[oidx++] = (byte)((off >> 8) + (len << 5));
                        }
                        else
                        {
                            out_data[oidx++] = (byte)((off >> 8) + (7 << 5));
                            out_data[oidx++] = (byte)(len - 7);
                        }

                        out_data[oidx++] = (byte)off;
                        iidx += len - 1;
                        hval = FRST(in_data, iidx);
                        hval = NEXT(hval, in_data, iidx);
                        htab[IDX(hval)] = iidx;
                        iidx++;
                        hval = NEXT(hval, in_data, iidx);
                        htab[IDX(hval)] = iidx;
                        iidx++;
                        continue;
                    }
                }
                else if (iidx == in_len)
                    break;
                /* one more literal byte we must copy */
                lit++;
                iidx++;
                if (lit == MAX_LIT)
                {
                    if (oidx + 1 + MAX_LIT >= out_len)
                        return 0;
                    out_data[oidx++] = (byte)(MAX_LIT - 1);
                    lit = -lit;
                    do out_data[oidx++] = in_data[iidx + lit];
                    while ((++lit) != 0);
                }
            }

            if (lit != 0)
            {
                if (oidx + lit + 1 >= out_len)
                    return 0;
                out_data[oidx++] = (byte)(lit - 1);
                lit = -lit;
                do out_data[oidx++] = in_data[iidx + lit];
                while ((++lit) != 0);
            }

            return (int)oidx;
        }

        /// <summary>
        /// LZF Decompressor
        /// </summary>
        public int lzf_decompress(byte[] in_data, int offset, int in_len, byte[] out_data, int out_len)
        {
            UInt32 iidx = (UInt32)offset;
            UInt32 oidx = 0;
            UInt32 eidx = (UInt32)(offset + in_len);
            do
            {
                UInt32 ctrl = in_data[iidx++];
                if (ctrl < (1 << 5)) /* literal run */
                {
                    ctrl++;
                    if (oidx + ctrl > out_len)
                    {
                        //SET_ERRNO (E2BIG);
                        return 0;
                    }

                    do out_data[oidx++] = in_data[iidx++];
                    while ((--ctrl) != 0);
                }
                else /* back reference */
                {
                    UInt32 len = ctrl >> 5;
                    int reference = (int)(oidx - ((ctrl & 0x1f) << 8) - 1);
                    if (len == 7)
                        len += in_data[iidx++];
                    reference -= in_data[iidx++];
                    if (oidx + len + 2 > out_len)
                    {
                        //SET_ERRNO (E2BIG);
                        return 0;
                    }

                    if (reference < 0)
                    {
                        //SET_ERRNO (EINVAL);
                        return 0;
                    }

                    out_data[oidx++] = out_data[reference++];
                    out_data[oidx++] = out_data[reference++];
                    do out_data[oidx++] = out_data[reference++];
                    while ((--len) != 0);
                }
            }
            while (iidx < eidx);
            return (int)oidx;
        }

        public CLZF()
        {
        //
        // TODO: Add ructor logic here
        //
        }
    }
}