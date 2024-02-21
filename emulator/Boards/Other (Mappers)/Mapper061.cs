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
    [BoardName("20-in-1", 61)]
    class Mapper61 : Board
    {
        public Mapper61() : base() { }
        public Mapper61(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0x30)
            {
                case 0x00:
                case 0x30:

                    Switch32KPRG(address & 0xF);
                    break;

                case 0x20:
                case 0x10:

                    address = (address << 1 & 0x1E) | (address >> 4 & 0x02);
                    Switch16KPRG(address, 0x8000);
                    Switch16KPRG(address, 0xC000);
                    break;
            }
            Nes.PpuMemory.SwitchMirroring((address & 0x80) == 0x80 ? Types.Mirroring.ModeHorz : Types.Mirroring.ModeVert);
        }
    }
}
