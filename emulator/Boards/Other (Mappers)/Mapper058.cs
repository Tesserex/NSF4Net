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
namespace MyNes.Core.Boards.Discreet
{
    [BoardName("X-in-1", 58)]
    class Mapper58Xin1 : Board
    {
        public Mapper58Xin1() : base() { }
        public Mapper58Xin1(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        protected override void PokePrg(int address, byte data)
        {
            if ((address & 0x40) == 0x40)
            {
                Switch16KPRG(address & 0x7, 0x8000);
                Switch16KPRG(address & 0x7, 0xC000);
            }
            else
            {
                Switch32KPRG(address & 0x7);
            }
            Switch08kCHR((address & 0x38) >> 3);
            Nes.PpuMemory.SwitchMirroring((address & 0x80) == 0x80 ? Types.Mirroring.ModeHorz : Types.Mirroring.ModeVert);
        }
    }
}
