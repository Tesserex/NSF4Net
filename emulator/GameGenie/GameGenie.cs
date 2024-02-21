/* This file is part of My Nes
 * A Nintendo Entertainment System Emulator.
 *
 * Copyright © Ala Ibrahim Hadid 2009 - 2013
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System.Collections.Generic;
namespace MyNes.Core.GameGenie
{
    public class GameGenie
    {
        public string[] LettersTable = { "A", "P", "Z", "L", "G", "I", "T", "Y", "E", "O", "X", "U", "K", "S", "V", "N" };
        public byte[] HEXTable = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF };
        private List<string> lettersTable = new List<string>();

        public GameGenie()
        {
            lettersTable = new List<string>(LettersTable);
        }

        /*public void AddCheat(string code)
        {
            if (code.Length == 6)
            {
                //1 get the code in HEX
                int data = GetCodeAsHEX(code);
                //2 decode (get value and address)
                byte value = GetGGValue(data, 6);
                int address = GetGGAddress(data, 6);
                //simply replace address, no compare required
                Nes.CpuMemory[address | 0x8000]= value;
            }
            else if (code.Length == 8)
            {
                //1 get the code in HEX
                int data = GetCodeAsHEX(code);
                //2 decode (get value and address)
                byte value = GetGGValue(data, 8);
                int address = GetGGAddress(data, 8);
                int compare = GetGGCompareValue(data);
                //compare address, if found then replace
                if (Nes.CpuMemory[address | 0x8000] == compare)
                    Nes.CpuMemory[address | 0x8000] = value;
            }
        }*/
        public int GetCodeAsHEX(string code)
        {
            int data = 0;

            int shift = code.ToCharArray().Length - 1;
            foreach (char chr in code.ToCharArray())
            {
                data |= HEXTable[lettersTable.IndexOf(chr.ToString())] << (shift * 4);
                shift--;
            }

            return data;
        }
        public byte GetGGValue(int code, int length)
        {
            int bit0 = 0;
            int bit1 = 0;
            int bit2 = 0;
            int bit3 = 0;
            int bit4 = 0;
            int bit5 = 0;
            int bit6 = 0;
            int bit7 = 0;
            switch (length)
            {
                case 6:
                    /*
                     * Char # |   1   |   2   |   3   |   4   |   5   |   6   |
                     * Bit  # |3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|
                     * maps to|1|6|7|8|H|2|3|4|-|I|J|K|L|A|B|C|D|M|N|O|5|E|F|G|
                     * 
                     * The value is made of 12345678 of the maps to line.
                     */
                    bit7 = (code & 0x00800000) >> 23;//1
                    bit6 = (code & 0x00040000) >> 18;//2
                    bit5 = (code & 0x00020000) >> 17;//3
                    bit4 = (code & 0x00010000) >> 16;//4
                    bit3 = (code & 0x00000008) >> 03;//5
                    bit2 = (code & 0x00400000) >> 22;//6
                    bit1 = (code & 0x00200000) >> 21;//7
                    bit0 = (code & 0x00100000) >> 20;//8
                    break;
                case 8:
                    /*
                     * Char # |   1   |   2   |   3   |   4   |   5   |   6   |   7   |   8   |
                     * Bit  # |3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|
                     * maps to|1|6|7|8|H|2|3|4|-|I|J|K|L|A|B|C|D|M|N|O|%|E|F|G|!|^|&|*|5|@|#|$|
                     * 
                     * The value is made of 12345678 of the maps to line.
                     */
                    bit7 = (code >> 31) & 0x1;//1
                    bit6 = (code >> 27) & 0x1;//2
                    bit5 = (code >> 26) & 0x1;//3
                    bit4 = (code >> 25) & 0x1;//4
                    bit3 = (code >> 03) & 0x1;//5
                    bit2 = (code >> 30) & 0x1;//6
                    bit1 = (code >> 29) & 0x1;//7
                    bit0 = (code >> 28) & 0x1;//8
                    break;
            }
            return (byte)((bit7 << 7) | (bit6 << 6) | (bit5 << 5) | (bit4 << 4)
                        | (bit3 << 3) | (bit2 << 2) | (bit1 << 1) | (bit0 << 0));
        }
        public int GetGGAddress(int code, int length)
        {
            int bit0 = 0;
            int bit1 = 0;
            int bit2 = 0;
            int bit3 = 0;
            int bit4 = 0;
            int bit5 = 0;
            int bit6 = 0;
            int bit7 = 0;
            int bit8 = 0;
            int bit9 = 0;
            int bit10 = 0;
            int bit11 = 0;
            int bit12 = 0;
            int bit13 = 0;
            int bit14 = 0;
            switch (length)
            {
                case 6:
                    /*
                     * Char # |   1   |   2   |   3   |   4   |   5   |   6   |
                     * Bit  # |3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|
                     * maps to|1|6|7|8|H|2|3|4|-|I|J|K|L|A|B|C|D|M|N|O|5|E|F|G|
                     * 
                     * The address is made of ABCDEFGHIJKLMNO of the maps to line.
                     */
                    bit14 = (code >> 10) & 0x1;//A
                    bit13 = (code >> 09) & 0x1;//B
                    bit12 = (code >> 08) & 0x1;//C
                    bit11 = (code >> 07) & 0x1;//D
                    bit10 = (code >> 02) & 0x1;//E
                    bit9 = (code >> 01) & 0x1;//F
                    bit8 = (code >> 00) & 0x1;//G
                    bit7 = (code >> 19) & 0x1;//H
                    bit6 = (code >> 14) & 0x1;//I
                    bit5 = (code >> 13) & 0x1;//J
                    bit4 = (code >> 12) & 0x1;//K
                    bit3 = (code >> 11) & 0x1;//L
                    bit2 = (code >> 6) & 0x1;//M
                    bit1 = (code >> 5) & 0x1;//N
                    bit0 = (code >> 4) & 0x1;//O
                    break;
                case 8:
                    /*
                     * Char # |   1   |   2   |   3   |   4   |   5   |   6   |   7   |   8   |
                     * Bit  # |3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|
                     * maps to|1|6|7|8|H|2|3|4|-|I|J|K|L|A|B|C|D|M|N|O|%|E|F|G|!|^|&|*|5|@|#|$|
                     * 
                     * The address is made of ABCDEFGHIJKLMNO of the maps to line.
                     */
                    bit14 = (code >> 18) & 0x1;//A
                    bit13 = (code >> 17) & 0x1;//B
                    bit12 = (code >> 16) & 0x1;//C
                    bit11 = (code >> 15) & 0x1;//D
                    bit10 = (code >> 10) & 0x1;//E
                    bit9 = (code >> 09) & 0x1;//F
                    bit8 = (code >> 08) & 0x1;//G
                    bit7 = (code >> 25) & 0x1;//H
                    bit6 = (code >> 22) & 0x1;//I
                    bit5 = (code >> 21) & 0x1;//J
                    bit4 = (code >> 20) & 0x1;//K
                    bit3 = (code >> 19) & 0x1;//L
                    bit2 = (code >> 14) & 0x1;//M
                    bit1 = (code >> 13) & 0x1;//N
                    bit0 = (code >> 12) & 0x1;//O
                    break;
            }
            return ((bit14 << 14) | (bit13 << 13) | (bit12 << 12)
                  | (bit11 << 11) | (bit10 << 10) | (bit9 << 9) | (bit8 << 8)
                  | (bit7 << 7) | (bit6 << 6) | (bit5 << 5) | (bit4 << 4)
                  | (bit3 << 3) | (bit2 << 2) | (bit1 << 1) | (bit0 << 0));
        }
        public byte GetGGCompareValue(int code)
        {
            int bit0 = 0;
            int bit1 = 0;
            int bit2 = 0;
            int bit3 = 0;
            int bit4 = 0;
            int bit5 = 0;
            int bit6 = 0;
            int bit7 = 0;

            /*
             * Char # |   1   |   2   |   3   |   4   |   5   |   6   |   7   |   8   |
             * Bit  # |3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|
             * maps to|1|6|7|8|H|2|3|4|-|I|J|K|L|A|B|C|D|M|N|O|%|E|F|G|!|^|&|*|5|@|#|$|
             * 
             * The compare value is made of !@#$%^&* of the maps to line.
             */
            bit7 = (code >> 7) & 0x1;//!
            bit6 = (code >> 2) & 0x1;//@
            bit5 = (code >> 1) & 0x1;//#
            bit4 = (code >> 0) & 0x1;//$
            bit3 = (code >> 11) & 0x1;//%
            bit2 = (code >> 6) & 0x1;//^
            bit1 = (code >> 5) & 0x1;//&
            bit0 = (code >> 4) & 0x1;//*

            return (byte)((bit7 << 7) | (bit6 << 6) | (bit5 << 5) | (bit4 << 4)
                        | (bit3 << 3) | (bit2 << 2) | (bit1 << 1) | (bit0 << 0));
        }
    }
}
