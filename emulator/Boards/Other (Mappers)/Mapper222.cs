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
    [BoardName("Unknown", 222)]
    class Mapper222 : Board
    {
        public Mapper222() : base() { }
        public Mapper222(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }
        public override void HardReset()
        {
            base.HardReset();
            Switch16KPRG(prg.Length - 0x4000 >> 14, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xF003)
            {
                case 0x8000:
                    Switch08KPRG(data,0x8000);
                    break;
                case 0x9000: Nes.PpuMemory.SwitchMirroring((data & 1) == 1 ? Mirroring.ModeHorz : Mirroring.ModeVert); break;
                case 0xA000:
                    Switch08KPRG( data, 0xA000);
                    break;
                case 0xB000:
                    Switch01kCHR(data, 0x0000);
                    break;
                case 0xB002:
                    Switch01kCHR(data, 0x0400);
                    break;
                case 0xC000:
                    Switch01kCHR(data, 0x0800);
                    break;
                case 0xC002:
                    Switch01kCHR(data, 0x0C00);
                    break;
                case 0xD000:
                    Switch01kCHR(data, 0x1000);
                    break;
                case 0xD002:
                    Switch01kCHR(data, 0x1400);
                    break;
                case 0xE000:
                    Switch01kCHR(data, 0x1800);
                    break;
                case 0xE002:
                    Switch01kCHR(data, 0x1C00);
                    break;
            }
        }
    }
}
