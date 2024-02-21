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
    [BoardName("Unknown", 225)]
    class Mapper225 : Board
    {
        public Mapper225() : base() { }
        public Mapper225(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private byte[] ram = new byte[4];

        public override void Initialize()
        {
            base.Initialize();

            Nes.CpuMemory.Hook(0x5000, 0x5FFF, Peek5000, Poke5000);
        }
        public override void HardReset()
        {
            base.HardReset();
            ram = new byte[4];
        }
        private void Poke5000(int address, byte data)
        {
            ram[address & 0x3] = (byte)(data & 0xF);
        }
        private byte Peek5000(int address)
        {
            return ram[address & 0x3];
        }
        protected override void PokePrg(int address, byte data)
        {
            Switch08kCHR(address & 0x3F);
            Nes.PpuMemory.SwitchMirroring((address & 0x2000) == 0x2000 ? Mirroring.ModeHorz : Mirroring.ModeVert);
            if ((address & 0x1000) == 0x1000)
            {
                Switch16KPRG(((address >> 6) & 0x3F) | ((address >> 6) & 1), 0x8000);
                Switch16KPRG(((address >> 6) & 0x3F) | ((address >> 6) & 1), 0xC000);
            }
            else
            {
                Switch32KPRG(address >> 7 & 0x1F);
            }
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(ram);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(ram);
        }
    }
}
