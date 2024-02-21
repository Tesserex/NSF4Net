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
using MyNes.Core.Boards.Nintendo;
namespace MyNes.Core.Boards.Other__Mappers_
{
    [BoardName("Unknown", 196)]
    class Mapper196 : MMC3
    {
        public Mapper196() : base() { }
        public Mapper196(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        public override void Initialize()
        {
            base.Initialize();
            for (int i = 0x0000; i < 0x2000; i += 0x8)
            {
                Nes.CpuMemory.Hook(0x8000 + i, 0x8003 + i, Poke8000);
                Nes.CpuMemory.Hook(0x8004 + i, 0x8007 + i, Poke8001);
                Nes.CpuMemory.Hook(0xA000 + i, 0xA003 + i, PokeNT);
                Nes.CpuMemory.Hook(0xA004 + i, 0xA007 + i, PokeA001);
                Nes.CpuMemory.Hook(0xC000 + i, 0xC003 + i, PokeC000);
                Nes.CpuMemory.Hook(0xC004 + i, 0xC007 + i, PokeC001);
                Nes.CpuMemory.Hook(0xE000 + i, 0xE003 + i, PokeE000);
                Nes.CpuMemory.Hook(0xE004 + i, 0xE007 + i, PokeE001);
            }
        }
        private void PokeNT(int address, byte data)
        {
            Nes.PpuMemory.SwitchMirroring((data & 0x1) == 1 ? Mirroring.ModeHorz : Mirroring.ModeVert);
        }
    }
}
