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
using MyNes.Core.Types;
namespace MyNes.Core.Boards.Discreet
{
    [BoardName("Unknown", 96)]
    class Mapper96 : Board
    {
        public Mapper96() : base() { }
        public Mapper96(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool chrBlockSelect = false;

        public override void Initialize()
        {
            base.Initialize();

            // 32 KBytes of chr ram ...
            chr = new byte[1024 * 32];
            chrMask = chr.Length - 1;
        }
        public override void HardReset()
        {
            base.HardReset();
            chrBlockSelect = false;
        }
        protected override void PokePrg(int address, byte data)
        {
            Switch32KPRG(data & 0x3);
            chrBlockSelect = (data & 0x4) == 0x4;
        }
    }
}
