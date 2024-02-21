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
    [BoardName("Unknown", 230)]
    class Mapper230 : Board
    {
        public Mapper230() : base() { }
        public Mapper230(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool contraMode = false;

        public override void HardReset()
        {
            base.HardReset();

            //set contra mode
            contraMode = true;
            Switch16KPRG(0, 0x8000);
            Switch16KPRG(7, 0xC000);
        }
        public override void SoftReset()
        {
            base.SoftReset();
            contraMode = !contraMode;
            if (contraMode)
            {
                Switch16KPRG(0, 0x8000);
                Switch16KPRG(7, 0xC000);
                Nes.PpuMemory.SwitchMirroring(Mirroring.ModeVert);
            }
            else
            {
                //Nes.PpuMemory.SwitchMirroring(Mirroring.ModeHorz);
                Switch16KPRG(8, 0x8000);
                Switch16KPRG(39, 0xC000);

            }
        }
        protected override void PokePrg(int address, byte data)
        {
            if (contraMode)
            {
                Switch16KPRG(data & 0x7, 0x8000);
                Switch16KPRG(7, 0xC000);
                Nes.PpuMemory.SwitchMirroring(Mirroring.ModeVert);
            }
            else
            {
                Nes.PpuMemory.SwitchMirroring((data & 0x40) == 0x40 ? Mirroring.ModeVert : Mirroring.ModeHorz);

                Switch16KPRG((data & 0x1F) + 8, 0x8000);
                Switch16KPRG((data & 0x1F) + 8 | (~data >> 5 & 0x1), 0xC000);

            }
        }
    }
}
