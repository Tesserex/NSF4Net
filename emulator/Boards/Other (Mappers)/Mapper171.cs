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
    [BoardName("Unknow", 171)]
    class Mapper171 : Board
    {
        public Mapper171() : base() { }
        public Mapper171(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        public override void Initialize()
        {
            base.Initialize();

            for (int i = 0xF000; i <= 0xFFFF; i += 0x100)
            {
                Nes.CpuMemory.Hook(i + 0x00, i + 0x7F, Poke1);
                Nes.CpuMemory.Hook(i + 0x80, i + 0xFF, Poke2);
            }
        }
        private void Poke1(int address, byte data)
        {
            Switch04kCHR(data, 0x0000);
        }
        private void Poke2(int address, byte data)
        {
            Switch04kCHR(data, 0x1000);
        }
    }
}
