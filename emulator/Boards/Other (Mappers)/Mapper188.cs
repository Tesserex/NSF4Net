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
    [BoardName("Mapper188", 188)]
    class Mapper188 : Board
    {
        public Mapper188() : base() { }
        public Mapper188(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        public override void HardReset()
        {
            base.HardReset();
            base.Switch16KPRG(0, 0x8000);
            base.Switch16KPRG((prg.Length - 0x4000 >> 14), 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            Switch16KPRG((data != 0) ? (data & 0x7) | (~data >> 1 & 0x8) : (prg.Length >> 18) + 0x7, 0x8000);
        }
    }
}
