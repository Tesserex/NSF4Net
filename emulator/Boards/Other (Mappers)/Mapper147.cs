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
    [BoardName("Unknown", 147)]
    class Mapper147 : Board
    {
        public Mapper147() : base() { }
        public Mapper147(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        public override void Initialize()
        {
            base.Initialize();

            for (int i = 0x4100; i <= 0xFFFF; ++i)
            {
                if ((i & 0x103) == 0x102)
                    Nes.CpuMemory.Hook(i, Poke4100);
            }
        }

        private void Poke4100(int address, byte data)
        {
            Switch32KPRG((data >> 6 & 0x2) | (data >> 2 & 0x1));
            Switch08kCHR(data >> 3);
        }
    }
}
