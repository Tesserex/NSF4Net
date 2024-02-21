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
namespace MyNes.Core.Boards.Nintendo
{
    [BoardName("Namcot MMC3-Style", 95)]
    class Mapper095 : MMC3
    {
        public Mapper095() : base() { }
        public Mapper095(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        public override void Initialize()
        {
            base.Initialize();
            Nes.PpuMemory.Hook(0x2000, 0x3EFF, PeekNmt, PokeNmt);
        }
        protected override void PokeA000(int address, byte data)
        {
            // ignore the mirroring bit
        }
        public byte PeekNmt(int addr)
        {
            switch ((addr >> 10) & 0x03)
            {
                case 0: return Nes.PpuMemory.nmt[(chrRegs[chrmode ? 2 : 0] & 0x20) >> 5][(addr & 0x03FF)];
                case 1: return Nes.PpuMemory.nmt[(chrRegs[chrmode ? 3 : 0] & 0x20) >> 5][(addr & 0x03FF)];
                case 2: return Nes.PpuMemory.nmt[(chrRegs[chrmode ? 4 : 1] & 0x20) >> 5][(addr & 0x03FF)];
                case 3: return Nes.PpuMemory.nmt[(chrRegs[chrmode ? 5 : 1] & 0x20) >> 5][(addr & 0x03FF)];
                default: return 0;// make compiler happy !
            }
        }
        public void PokeNmt(int addr, byte data)
        {
            switch ((addr >> 10) & 0x03)
            {
                case 0: Nes.PpuMemory.nmt[(chrRegs[chrmode ? 2 : 0] & 0x20) >> 5][(addr & 0x03FF)] = data; break;
                case 1: Nes.PpuMemory.nmt[(chrRegs[chrmode ? 3 : 0] & 0x20) >> 5][(addr & 0x03FF)] = data; break;
                case 2: Nes.PpuMemory.nmt[(chrRegs[chrmode ? 4 : 1] & 0x20) >> 5][(addr & 0x03FF)] = data; break;
                case 3: Nes.PpuMemory.nmt[(chrRegs[chrmode ? 5 : 1] & 0x20) >> 5][(addr & 0x03FF)] = data; break;
            }
        }
    }
}
