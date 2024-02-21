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
    [BoardName("15-in-1 Color Dreams", 46)]
    class _15in1ColorDreams : Board
    {
        public _15in1ColorDreams() : base() { }
        public _15in1ColorDreams(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }
        protected override void PokeSram(int address, byte data)
        {
            Switch32KPRG(((prgPage[0] >> 15) & 0x1) | (data << 1 & 0x1E));
            Switch08kCHR(((chrPage[0] >> 13) & 0x7) | (data >> 1 & 0x78));
        }
        protected override void PokePrg(int address, byte data)
        {
            Switch32KPRG((data >> 0 & 0x1) | ((prgPage[0] >> 15) & 0x1E));
            Switch08kCHR((data >> 4 & 0x7) | ((chrPage[0] >> 13) & 0x78));
        }
    }
}
