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
    [BoardName("Unknown", 212)]
    class Mapper212 : Board
    {
        public Mapper212() : base() { }
        public Mapper212(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        public override void Initialize()
        {
            base.Initialize();

      		Nes.CpuMemory.Hook( 0x8000, 0xBFFF, Poke8000 );
			Nes.CpuMemory.Hook( 0xC000, 0xFFFF, PokeC000 ); 
        }
        private void Poke8000(int address, byte data)
        {
            Switch16KPRG(address, 0x8000);
            Switch16KPRG(address, 0xC000);
            Nes.PpuMemory.SwitchMirroring((address & 0x8) == 0x8 ? Mirroring.ModeHorz : Mirroring.ModeVert);
            Switch08kCHR(address);
        }
        private void PokeC000(int address, byte data)
        {
            Switch32KPRG(address >> 1);
            Nes.PpuMemory.SwitchMirroring((address & 0x8) == 0x8 ? Mirroring.ModeHorz : Mirroring.ModeVert);
            Switch08kCHR(address);
        }
    }
}
