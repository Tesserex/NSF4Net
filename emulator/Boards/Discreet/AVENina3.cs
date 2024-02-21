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
    [BoardName("AVE Nina-3", 79)]
    class AVENina3 : Board
    {
        public AVENina3() : base() { }
        public AVENina3(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        public override void Initialize()
        {
            base.Initialize();

            Nes.CpuMemory.Hook(0x4100, 0x5FFF, PokePrg);
        }
        protected override void PokePrg(int address, byte data)
        {
            if (address < 0x6000)
            {
                switch (address & 0x4100)
                {
                    case 0x4100:
                        Switch32KPRG(((data & 0x38) >> 3));
                        Switch08kCHR((data & 0x7) | ((data & 0x40) >> 3));
                        break;
                }
            }
        }
    }
}
