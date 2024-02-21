﻿/* This file is part of My Nes
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
/*Written by Ala*/
using MyNes.Core.Types;
namespace MyNes.Core.Boards.Sunsoft
{
    [BoardName("Sunsoft4", 68)]
    class Sunsoft4 : Board
    {
        public Sunsoft4() : base() { }
        public Sunsoft4(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool ntMode = false;
        private int ntAbank = 0;
        private int ntBbank = 0;

        public override void HardReset()
        {
            base.HardReset();
            ntMode = false;
            ntAbank = 0;
            ntBbank = 0;
            Switch16KPRG(prg.Length - 0x4000 >> 14, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xF000)
            {
                case 0x8000: Switch02kCHR(data, 0x0000); break;
                case 0x9000: Switch02kCHR(data, 0x0800); break;
                case 0xA000: Switch02kCHR(data, 0x1000); break;
                case 0xB000: Switch02kCHR(data, 0x1800); break;

                case 0xC000: ntAbank = (data & 0x7F) | 0x80; break;
                case 0xD000: ntBbank = (data & 0x7F) | 0x80; break;

                case 0xE000:
                    ntMode = (data & 0x10) == 0x10;
                    break;

                case 0xF000: Switch16KPRG(data, 0x8000); break;
            }
        }
    }
}
