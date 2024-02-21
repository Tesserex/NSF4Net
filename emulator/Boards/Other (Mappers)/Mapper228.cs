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
    [BoardName("Unknown", 228)]
    class Mapper228 : Board
    {
        public Mapper228() : base() { }
        public Mapper228(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private byte[] ram = new byte[4];

        public override void Initialize()
        {
            base.Initialize();

            Nes.CpuMemory.Hook(0x4020, 0x5FFF, Peek5000, Poke5000);
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
            Switch08kCHR(((address & 0xF) << 2) | (data & 0x3));
            Nes.PpuMemory.SwitchMirroring((address & 0x2000) == 0x2000 ? Mirroring.ModeHorz : Mirroring.ModeVert);
            int bank = (address >> 7 & 0x1F) + (address >> 7 & address >> 8 & 0x10);
            if ((address & 0x0020) == 0x0020)
            {
                Switch16KPRG((bank << 2) | (address >> 5 & 0x2), 0x8000);
                Switch16KPRG((bank << 2) | (address >> 5 & 0x2), 0xC000); 
            }
            else
            {
                Switch32KPRG(bank);
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
