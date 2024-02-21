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
    [BoardName("Super 700-in-1", 62)]
    class Mapper62 : Board
    {
        public Mapper62() : base() { }
        public Mapper62(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        protected override void PokePrg(int address, byte data)
        {
            //switch prg
            int page = ((address & 0x3F00) >> 8) | (address & 0x40);
            if ((address & 0x20) == 0x20)
            {
                Switch16KPRG(page, 0x8000);
                Switch16KPRG(page, 0xC000);
            }
            else
            {
                Switch32KPRG(page >> 1);
            }
            //switch chr
            page = ((address & 0x001F) << 2) | (data & 0x03);
            Switch08kCHR(page);

            //mirroring
            Nes.PpuMemory.SwitchMirroring((address & 0x80) == 0x80 ? Types.Mirroring.ModeHorz : Types.Mirroring.ModeVert);
        }
    }
}
