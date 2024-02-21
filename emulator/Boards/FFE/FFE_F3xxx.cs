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
/*Written by Ala Ibrahim Hadid*/
namespace MyNes.Core.Boards.FFE
{
    [BoardName("FFE F3xxx", 8)]
    class FFE_F3xxx : FFE
    {
        public FFE_F3xxx()
            : base()
        { }
        public FFE_F3xxx(byte[] chr, byte[] prg, byte[] trainer, bool isVram)
            : base(chr, prg, trainer, isVram)
        { }
        protected override void PokePrg(int address, byte data)
        {
            base.PokePrg(address, data); 
            if (address >= 0x8000 && address <= 0xFFFF)
            {
                base.Switch32KPRG((data >> 4) & 0x3);
                base.Switch08kCHR(data & 0x3);
            }
        }
    }
}