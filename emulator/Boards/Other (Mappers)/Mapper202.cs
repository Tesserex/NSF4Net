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
namespace MyNes.Core.Boards.Other__Mappers_
{
    [BoardName("Unknown", 202)]
    class Mapper202 : Board
    {
        public Mapper202() : base() { }
        public Mapper202(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        public override void Initialize()
        {
            base.Initialize();

            Nes.CpuMemory.Hook(0x4020, 0xFFFF, PokePrg);
        }
        protected override void PokePrg(int address, byte data)
        {
            int bank = address >> 1 & 0x7;
            Switch16KPRG(bank, 0x8000);
            Switch16KPRG(bank + (address & 0xC) == 0xC ? 1 : 0, 0xC000);
            Switch08kCHR(bank);
        }
    }
}
