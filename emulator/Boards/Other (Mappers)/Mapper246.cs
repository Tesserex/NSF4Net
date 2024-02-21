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
    [BoardName("Unknown", 246)]
    class Mapper246 : Board
    {
        public Mapper246() : base() { }
        public Mapper246(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }
        public override void HardReset()
        {
            base.HardReset();
            Switch16KPRG(0xFF, 0xE000);
        }
        protected override void PokeSram(int address, byte data)
        {
            if (address >= 0x6800)
                base.PokeSram(address, data);
            else
            {
                switch (address & 0x7)
                {
                    case 0: Switch08KPRG(data, 0x8000); break;
                    case 1: Switch08KPRG(data, 0xA000); break;
                    case 2: Switch08KPRG(data, 0xC000); break;
                    case 3: Switch08KPRG(data, 0xE000); break;
                    case 4: Switch02kCHR(data, 0x0000); break;
                    case 5: Switch02kCHR(data, 0x0800); break;
                    case 6: Switch02kCHR(data, 0x1000); break;
                    case 7: Switch02kCHR(data, 0x1800); break;
                }
            }
        }
    }
}
