using System;
using System.Collections.Generic;

namespace VWeaponEditor.Avalonia.MemoryUtils {
    public static class Memory {
        public const byte NULL = 0;

        public static int ToHex(char ch) {
            switch(ch) {
                case '0': return 0x0;
                case '1': return 0x1;
                case '2': return 0x2;
                case '3': return 0x3;
                case '4': return 0x4;
                case '5': return 0x5;
                case '6': return 0x6;
                case '7': return 0x7;
                case '8': return 0x8;
                case '9': return 0x9;
                case 'a': return 0xa;
                case 'A': return 0xA;
                case 'b': return 0xb;
                case 'B': return 0xB;
                case 'c': return 0xc;
                case 'C': return 0xC;
                case 'd': return 0xd;
                case 'D': return 0xD;
                case 'e': return 0xe;
                case 'E': return 0xE;
                case 'f': return 0xf;
                case 'F': return 0xF;
                default: throw new ArgumentOutOfRangeException($"char is not a valid hex code: '{ch}'");
            }
        }

        /*
            "PEP",
            "4C 8B 35 ? ? ? ? B8 ? ? ? ? 0F 57 F6 89 05 ? ? ? ? 49 63 76 10 4C 8B FE 85 F6 0F 84 ? ? ? ? 49 8B 46 08 49 FF CF FF CE 42 0F B6 0C 38",
            [](memory::handle ptr)
		    {
		       g_pointers->m_gta.m_ped_pool = ptr.add(3).rip().as<GenericPool**>();
		    }
         */

        public static void CompilePattern(string pattern, List<byte> hexList) {
            int len = pattern.Length, endIndex = len - 1;
            for (int i = 0; i < len; i++) {
                char ch = pattern[i];
                if (ch == '?') {
                    hexList.Add(NULL);
                    if (i != endIndex && pattern[i + 1] == '?') {
                        // skip 2nd question mark, just in case pattern contains "??"
                        i++;
                    }
                }
                else if (ch != ' ' && i != endIndex) {
                    int a = ToHex(ch);
                    int b = ToHex(pattern[++i]);
                    hexList.Add((byte) ((a << 4) + b));
                }
            }
        }

        public static byte[] CompilePattern(string pattern) {
            List<byte> hex = new List<byte>(pattern.Length);
            CompilePattern(pattern, hex);
            return hex.ToArray();
        }

        public static unsafe void GetBytes(byte* bytes, string mask, byte* output, int length) {
            for (int i = 0; i < length; i++) {
                output[i] = mask[i] != '?' ? bytes[i] : NULL;
            }
        }

        // From YimMenu
        public static unsafe byte* ScanPattern(byte* pattern, int patLen, byte* begin, int module_size) {
            int maxShift = patLen;
            int patEnd = patLen - 1;

            // Get wildcard index, and store max shiftable byte count
            int wild_card_idx = -1;
            for (int i = patEnd - 1; i >= 0; --i) {
                if (pattern[i] == 0) {
                    maxShift = patEnd - i;
                    wild_card_idx = i;
                    break;
                }
            }

            // Store max shiftable bytes for non wildcards.
            int[] shift = new int[256];
            for (int i = 0; i < 256; ++i) {
                shift[i] = maxShift;
            }

            // Fill shift table with sig bytes
            for (int i = wild_card_idx + 1; i != patEnd; ++i) {
                shift[pattern[i]] = patEnd - i;
            }

            // Loop data
            int moduleEnd = module_size - patLen;
            for (int i = 0; i <= moduleEnd;) {
                for (int j = patEnd; j >= 0; --j) {
                    if (pattern[j] != 0 && *(begin + i + j) != pattern[j]) {
                        i += shift[*(begin + i + patEnd)];
                        break;
                    }
                    else if (j == 0) {
                        return begin + i;
                    }
                }
            }

            return null;
        }

        public static unsafe byte* Scan(byte* begin, int module_size, byte[] pattern) {
            fixed (byte* pat = pattern) {
                return ScanPattern(pat, pattern.Length, begin, module_size);
            }
        }

        public static unsafe bool IsPatternMatch(byte* target, byte* pattern, int length) {
            for (int i = 0; i < length; i++) {
                if (pattern[i] != 0 && pattern[i] != target[i]) {
                    return false;
                }
            }

            return true;
        }

        public static unsafe void ScanAll(byte* pattern, int length, byte* begin, int module_size, List<IntPtr> list) {
            int scan_end = module_size - length;
            for (int i = 0; i < scan_end; i++) {
                if (IsPatternMatch(begin + i, pattern, length)) {
                    list.Add(new IntPtr(begin + i));
                }
            }
        }
    }
}
