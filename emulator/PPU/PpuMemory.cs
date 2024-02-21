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
using MyNes.Core.Types;
namespace MyNes.Core.PPU
{
    public class PpuMemory : Memory
    {
        public byte[] nmtBank;
        private byte[] pal;
        public byte[][] nmt;

        public PpuMemory()
            : base(0x4000)
        {
        }

        public byte PeekNmt(int addr)
        {
            return nmt[nmtBank[(addr >> 10) & 0x03]][addr & 0x03FF];
        }
        public byte PeekPal(int addr)
        {
            return pal[addr & ((addr & 0x03) == 0 ? 0x0C : 0x1F)];
        }
        public void PokeNmt(int addr, byte data)
        {
            nmt[nmtBank[(addr >> 10) & 0x03]][addr & 0x03FF] = data;
        }
        public void PokePal(int addr, byte data)
        {
            pal[addr & ((addr & 0x03) == 0 ? 0x0C : 0x1F)] = data;
        }
        public void SwitchMirroring(Mirroring value)
        {
            SwitchMirroring((byte)value);
        }
        public void SwitchMirroring(byte value)
        {
            nmtBank[0] = (byte)(value >> 6 & 0x03);
            nmtBank[1] = (byte)(value >> 4 & 0x03);
            nmtBank[2] = (byte)(value >> 2 & 0x03);
            nmtBank[3] = (byte)(value >> 0 & 0x03);
        }
        public void SwitchNmt(byte value)
        {
            nmtBank[3] = (byte)(value >> 6 & 0x03);
            nmtBank[2] = (byte)(value >> 4 & 0x03);
            nmtBank[1] = (byte)(value >> 2 & 0x03);
            nmtBank[0] = (byte)(value >> 0 & 0x03);
        }
        public override void Initialize()
        {
            base.Hook(0x2000, 0x3EFF, PeekNmt, PokeNmt);
            base.Hook(0x3F00, 0x3FFF, PeekPal, PokePal);
            HardReset();
        }
        public override void HardReset()
        {
            nmtBank = new byte[4];
            SwitchMirroring(Nes.RomInfo.Mirroring);
            pal = new byte[] // Miscellaneous, real NES loads values similar to these during power up
            {
               0x09, 0x01, 0x00, 0x01, 0x00, 0x02, 0x02, 0x0D, 0x08, 0x10, 0x08, 0x24, 0x00, 0x00, 0x04, 0x2C, // Bkg palette
               0x09, 0x01, 0x34, 0x03, 0x00, 0x04, 0x00, 0x14, 0x08, 0x3A, 0x00, 0x02, 0x00, 0x20, 0x2C, 0x08  // Spr palette
            };
            nmt = new byte[4][]
            {
               new byte[0x0400],new byte[0x0400],new byte[0x0400],new byte[0x0400]
               /*Only 2 nmt banks should be used in normal state*/
            };
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(nmtBank);
            stream.Write(pal);
            for (int i = 0; i < 4; i++)
                stream.Write(nmt[i]);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(nmtBank);
            stream.Read(pal);
            for (int i = 0; i < 4; i++)
                stream.Read(nmt[i]);
        }
    }
}