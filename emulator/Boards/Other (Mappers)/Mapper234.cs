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
namespace MyNes.Core.Boards.Other__Mappers_
{
    [BoardName("Unknown", 234)]
    class Mapper234 : Board
    {
        public Mapper234() : base() { }
        public Mapper234(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }
        private byte[] regs = new byte[2];
        public override void Initialize()
        {
            base.Initialize();

            Nes.CpuMemory.Hook(0xFF80, 0xFF9F, PeekFF80, PokeFF80);
            Nes.CpuMemory.Hook(0xFFE8, 0xFFF7, PeekFFE8, PokeFFE8);
        }
        public override void HardReset()
        {
            base.HardReset();

            regs = new byte[2];
        }

        private void Setup()
        {
            Switch32KPRG((regs[0] & 0xE) | (regs[regs[0] >> 6 & 0x1] & 0x1));
            Switch08kCHR((regs[0] << 2 & (regs[0] >> 4 & 0x4 ^ 0x3C)) | (regs[1] >> 4 & (regs[0] >> 4 & 0x4 | 0x3)));

        }
        private void PokeFF80(int address, byte data)
        {
            if ((regs[0] & 0x3F) == 0)
            {
                regs[0] = data;
                Nes.PpuMemory.SwitchMirroring((data & 0x80) == 0x80 ? Mirroring.ModeHorz : Mirroring.ModeVert);
                Setup();
            }
        }
        private byte PeekFF80(int address)
        {
            byte data = prg[0x6000 + address - 0xE000];
            PokeFF80(address, data);
            return data;
        }
        private void PokeFFE8(int address, byte data)
        {
            regs[1] = data;
            Setup();
        }
        private byte PeekFFE8(int address)
        {
            byte data = prg[0x6000 + address - 0xE000];
            PokeFF80(address, data);
            return data;
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(regs);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream); 
            stream.Read(regs);
        }
    }
}
