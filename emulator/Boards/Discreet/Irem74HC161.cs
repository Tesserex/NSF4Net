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
namespace MyNes.Core.Boards.Discreet
{
    [BoardName("Irem 74HC161", 78)]
    class Irem74HC161 : Board
    {
        public Irem74HC161() : base() { }
        public Irem74HC161(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool mode = false;
        public override void Initialize()
        {
            base.Initialize();
            /* from 078.txt doc:
             * "This mapper number covers two seperate mappers which are *almost* identical... however the mirroring control
             * on each is different (making them incompatible).  You'll probably have to do a CRC or Hash check to figure
             * out which mirroring setup to use."
             */
            if (Nes.RomInfo.SHA1.ToUpper() == "BC6F5A884FD31FE6B4439E83AD6C2A29D038E545")//Holy Diver
            {
                mode = true;
            }
        }
        public override void HardReset()
        {
            base.HardReset();

            Switch16KPRG(prg.Length - 0x4000 >> 14, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            Switch16KPRG(data & 0x7, 0x8000);
            Switch08kCHR((data & 0xF0) >> 4);

            if (mode)
            {
                Nes.PpuMemory.SwitchMirroring((data & 0x8) == 0x8 ? Mirroring.ModeVert : Mirroring.ModeHorz);
            }
            else
            {
                Nes.PpuMemory.SwitchMirroring((data & 0x8) == 0x8 ? Mirroring.Mode1ScA : Mirroring.Mode1ScB);
            }
        }
    }
}
