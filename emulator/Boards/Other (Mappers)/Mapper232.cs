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
    [BoardName("Another Camerica/Codemasters", 232)]
    class Mapper232 : Board
    {
        public Mapper232() : base() { }
        public Mapper232(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool mode = false;

        public override void HardReset()
        {
            base.HardReset();

            Switch16KPRG(3, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            if (address >= 0x8000 & address < 0xC000)
            {
                if (mode)
                    SetupPRG(data >> 1 & 0xC);
                else
                    SetupPRG((data & 0x8) | (data >> 2 & 0x4));
            }
            else
            {
                Switch16KPRG(((prgPage[0] >> 14) & 0xC) | (data & 0x3), 0x8000);
            }
        }
        private void SetupPRG(int bs)
        {
            Switch16KPRG(bs | ((prgPage[0] >> 14) & 0x3), 0x8000);
            Switch16KPRG(bs | 0x3, 0xC000);
        }
    }
}
