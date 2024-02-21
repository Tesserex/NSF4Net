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
    [BoardName("Unknown", 169)]
    class Mapper169 : Board
    {
        public Mapper169() : base() { }
        public Mapper169(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        public override void Initialize()
        {
            base.Initialize();

            for (int i = 0x8000; i <= 0xFFFF; i += 0x4)
            {
                Nes.CpuMemory.Hook(i + 0x0, Poke8000);
                Nes.CpuMemory.Hook(i + 0x2, Poke8002);
                Nes.CpuMemory.Hook(i + 0x3, Poke8000);
            }
        }
        private void Poke8000(int address, byte data)
        {
            Switch32KPRG(data >> 1 & 0x1F);
        }
        private void Poke8002(int address, byte data)
        {
            Switch16KPRG((data & 0x3F) + (data >> 7), 0x8000);
            Switch16KPRG((data & 0x3F) + (data >> 7), 0xC000);
        }
    }
}
