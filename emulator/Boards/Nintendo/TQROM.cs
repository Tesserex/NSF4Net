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
/*Written by Ala Ibrahim Hadid*/
using MyNes.Core.Types;
namespace MyNes.Core.Boards.Nintendo
{
    [BoardName("TQROM", 119)]
    class TQROM : MMC3
    {
        public TQROM() : base() { }
        public TQROM(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private byte[] chrRam = new byte[0x2000];// 8 KBytes
        public override void HardReset()
        {
            base.HardReset();
            chrRam = new byte[0x2000];// 8 KBytes
        }
        // we need to do some tricks here for chr ram
        protected override byte PeekChr(int address)
        {
            if (address < 0x0400)
            {
                if (chrPage[0] < chr.Length)
                    return chr[((address & 0x03FF) | chrPage[0]) & (chr.Length - 1)];
                else
                    return chrRam[((address & 0x03FF) | (chrPage[0] - chr.Length)) & 0x1FFF];
            }
            if (address < 0x0800)
            {
                if (chrPage[1] < chr.Length)
                    return chr[((address & 0x03FF) | chrPage[1]) & (chr.Length - 1)];
                else
                    return chrRam[((address & 0x03FF) | (chrPage[1] - chr.Length)) & 0x1FFF];
            }
            if (address < 0x0C00)
            {
                if (chrPage[2] < chr.Length)
                    return chr[((address & 0x03FF) | chrPage[2]) & (chr.Length - 1)];
                else
                    return chrRam[((address & 0x03FF) | (chrPage[2] - chr.Length)) & 0x1FFF];
            }
            if (address < 0x1000)
            {
                if (chrPage[3] < chr.Length)
                    return chr[((address & 0x03FF) | chrPage[3]) & (chr.Length - 1)];
                else
                    return chrRam[((address & 0x03FF) | (chrPage[3] - chr.Length)) & 0x1FFF];
            }
            if (address < 0x1400)
            {
                if (chrPage[4] < chr.Length)
                    return chr[((address & 0x03FF) | chrPage[4]) & (chr.Length - 1)];
                else
                    return chrRam[((address & 0x03FF) | (chrPage[4] - chr.Length)) & 0x1FFF];
            }
            if (address < 0x1800)
            {
                if (chrPage[5] < chr.Length)
                    return chr[((address & 0x03FF) | chrPage[5]) & (chr.Length - 1)];
                else
                    return chrRam[((address & 0x03FF) | (chrPage[5] - chr.Length)) & 0x1FFF];
            }
            if (address < 0x1C00)
            {
                if (chrPage[6] < chr.Length)
                    return chr[((address & 0x03FF) | chrPage[6]) & (chr.Length - 1)];
                else
                    return chrRam[((address & 0x03FF) | (chrPage[6] - chr.Length)) & 0x1FFF];
            }
            else
            {
                if (chrPage[7] < chr.Length)
                    return chr[((address & 0x03FF) | chrPage[7]) & (chr.Length - 1)];
                else
                    return chrRam[((address & 0x03FF) | (chrPage[7] - chr.Length)) & 0x1FFF];
            }
        }
        protected override void PokeChr(int address, byte data)
        {
            if (address < 0x0400)
            {
                if (chrPage[0] > chr.Length)
                    chrRam[((address & 0x03FF) | (chrPage[0] - chr.Length)) & 0x1FFF] = data;
            }
            if (address < 0x0800)
            {
                if (chrPage[1] > chr.Length)
                    chrRam[((address & 0x03FF) | (chrPage[1] - chr.Length)) & 0x1FFF] = data;
            }
            if (address < 0x0C00)
            {
                if (chrPage[2] > chr.Length)
                    chrRam[((address & 0x03FF) | (chrPage[2] - chr.Length)) & 0x1FFF] = data;
            }
            if (address < 0x1000)
            {
                if (chrPage[3] > chr.Length)
                    chrRam[((address & 0x03FF) | (chrPage[3] - chr.Length)) & 0x1FFF] = data;
            }
            if (address < 0x1400)
            {
                if (chrPage[4] > chr.Length)
                    chrRam[((address & 0x03FF) | (chrPage[4] - chr.Length)) & 0x1FFF] = data;
            }
            if (address < 0x1800)
            {
                if (chrPage[5] > chr.Length)
                    chrRam[((address & 0x03FF) | (chrPage[5] - chr.Length)) & 0x1FFF] = data;
            }
            if (address < 0x1C00)
            {
                if (chrPage[6] > chr.Length)
                    chrRam[((address & 0x03FF) | (chrPage[6] - chr.Length)) & 0x1FFF] = data;
            }
            else
            {
                if (chrPage[7] > chr.Length)
                    chrRam[((address & 0x03FF) | (chrPage[7] - chr.Length)) & 0x1FFF] = data;
            }
        }
        protected override void SetupCHR()
        {
            if (!chrmode)
            {
                chrPage[0] = ((chrRegs[0] & 0x40) == 0x40) ? (((chrRegs[0] + 0) << 10) + chr.Length) : ((chrRegs[0] + 0) << 10);
                chrPage[1] = ((chrRegs[0] & 0x40) == 0x40) ? (((chrRegs[0] + 1) << 10) + chr.Length) : ((chrRegs[0] + 1) << 10);
                chrPage[2] = ((chrRegs[1] & 0x40) == 0x40) ? (((chrRegs[1] + 0) << 10) + chr.Length) : ((chrRegs[1] + 0) << 10);
                chrPage[3] = ((chrRegs[1] & 0x40) == 0x40) ? (((chrRegs[1] + 1) << 10) + chr.Length) : ((chrRegs[1] + 1) << 10);
                chrPage[4] = ((chrRegs[2] & 0x40) == 0x40) ? ((chrRegs[2] << 10) + chr.Length) : (chrRegs[2] << 10);
                chrPage[5] = ((chrRegs[3] & 0x40) == 0x40) ? ((chrRegs[3] << 10) + chr.Length) : (chrRegs[3] << 10);
                chrPage[6] = ((chrRegs[4] & 0x40) == 0x40) ? ((chrRegs[4] << 10) + chr.Length) : (chrRegs[4] << 10);
                chrPage[7] = ((chrRegs[5] & 0x40) == 0x40) ? ((chrRegs[5] << 10) + chr.Length) : (chrRegs[5] << 10);
            }
            else
            {
                chrPage[0] = ((chrRegs[2] & 0x40) == 0x40) ? ((chrRegs[2] << 10) + chr.Length) : (chrRegs[2] << 10);
                chrPage[1] = ((chrRegs[3] & 0x40) == 0x40) ? ((chrRegs[3] << 10) + chr.Length) : (chrRegs[3] << 10);
                chrPage[2] = ((chrRegs[4] & 0x40) == 0x40) ? ((chrRegs[4] << 10) + chr.Length) : (chrRegs[4] << 10);
                chrPage[3] = ((chrRegs[5] & 0x40) == 0x40) ? ((chrRegs[5] << 10) + chr.Length) : (chrRegs[5] << 10);
                chrPage[4] = ((chrRegs[0] & 0x40) == 0x40) ? (((chrRegs[0] + 0) << 10) + chr.Length) : ((chrRegs[0] + 0) << 10);
                chrPage[5] = ((chrRegs[0] & 0x40) == 0x40) ? (((chrRegs[0] + 1) << 10) + chr.Length) : ((chrRegs[0] + 1) << 10);
                chrPage[6] = ((chrRegs[1] & 0x40) == 0x40) ? (((chrRegs[1] + 0) << 10) + chr.Length) : ((chrRegs[1] + 0) << 10);
                chrPage[7] = ((chrRegs[1] & 0x40) == 0x40) ? (((chrRegs[1] + 1) << 10) + chr.Length) : ((chrRegs[1] + 1) << 10);
            }
        }
    }
}
