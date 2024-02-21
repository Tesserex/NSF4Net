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
namespace MyNes.Core.Boards.Discreet
{
    [BoardName("100-in-1 Contra Function 16", 15)]
    class Mapper015 : Board
    {
        public Mapper015()
            : base()
        { }
        public Mapper015(byte[] chr, byte[] prg, byte[] trainer, bool isVram)
            : base(chr, prg, trainer, isVram)
        { }
        public override void Initialize()
        {
            base.Initialize();
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0x3)
            {
                case 0:
                    Switch16KPRG(data, 0x8000);
                    Switch16KPRG(data ^ 1, 0xC000);
                    break;
                case 1:
                    Switch16KPRG(data, 0x8000);
                    Switch16KPRG(prg.Length - 0x4000 >> 14, 0xC000);
                    break;
                case 2:
                    data <<= 1;
                    data = (byte)((data & 0x7E) | ((data >> 7) & 1));
                    base.Switch08KPRG(data, 0x8000);
                    base.Switch08KPRG(data, 0xA000);
                    base.Switch08KPRG(data, 0xC000);
                    base.Switch08KPRG(data, 0xE000);
                    break;

                case 3:
                    Switch16KPRG(data, 0x8000);
                    Switch16KPRG(data, 0xC000);
                    break;

            }
            Nes.PpuMemory.SwitchMirroring((data & 0x40) == 0x40 ? Types.Mirroring.ModeHorz : Types.Mirroring.ModeVert);
        }
    }
}