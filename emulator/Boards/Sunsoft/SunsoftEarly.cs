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
namespace MyNes.Core.Boards.Sunsoft
{
    [BoardName("Sunsoft Early", 89)]
    class SunsoftEarly : Board
    {
        public SunsoftEarly() : base() { }
        public SunsoftEarly(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        public override void HardReset()
        {
            base.HardReset();

            Switch16KPRG((prg.Length - 0x4000) >> 14, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            Switch08kCHR((data & 0x7) | ((data & 0x80) >> 4));
            Switch16KPRG((data & 0x70) >> 4, 0x8000);
            Nes.PpuMemory.SwitchMirroring((data & 0x8 )== 0x8 ? Mirroring.Mode1ScB : Mirroring.Mode1ScA);
        }
    }
}
