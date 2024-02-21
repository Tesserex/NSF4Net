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
    [BoardName("Unknown", 207)]
    class Mapper207 : Board
    {
        public Mapper207() : base() { }
        public Mapper207(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int mirroring1 = 0;
        private int mirroring2 = 0;

        public override void Initialize()
        {
            base.Initialize();
            Nes.PpuMemory.Hook(0x2000, 0x3EFF, PeekNmt, PokeNmt);
        }
        public override void HardReset()
        {
            base.HardReset();

            Switch08KPRG(prg.Length - 0x2000 >> 13, 0xE000);
            mirroring1 = 0;
            mirroring2 = 0;
        }
        protected override void PokeSram(int address, byte data)
        {
            switch (address)
            {
                case 0x7EF0: mirroring1 = (data & 0x80) == 0x80 ? 1 : 0; Switch02kCHR((data & 0x7F) >> 1, 0x0000); break;
                case 0x7EF1: mirroring2 = (data & 0x80) == 0x80 ? 1 : 0; Switch02kCHR((data & 0x7F) >> 1, 0x0800); break;
                case 0x7EF2: Switch01kCHR(data, 0x1000); break;
                case 0x7EF3: Switch01kCHR(data, 0x1400); break;
                case 0x7EF4: Switch01kCHR(data, 0x1800); break;
                case 0x7EF5: Switch01kCHR(data, 0x1C00); break;
                case 0x7EFA:
                case 0x7EFB: Switch08KPRG(data, 0x8000); break;
                case 0x7EFC:
                case 0x7EFD: Switch08KPRG(data, 0xA000); break;
                case 0x7EFE:
                case 0x7EFF: Switch08KPRG(data, 0xC000); break;
            }
        }
        public byte PeekNmt(int addr)
        {
            switch ((addr >> 10) & 0x03)
            {
                case 0: return Nes.PpuMemory.nmt[mirroring1][(addr & 0x03FF)];
                case 1: return Nes.PpuMemory.nmt[mirroring1][(addr & 0x03FF)];
                case 2: return Nes.PpuMemory.nmt[mirroring2][(addr & 0x03FF)];
                case 3: return Nes.PpuMemory.nmt[mirroring2][(addr & 0x03FF)];
                default: return 0;// make compiler happy !
            }
        }
        public void PokeNmt(int addr, byte data)
        {
            switch ((addr >> 10) & 0x03)
            {
                case 0: Nes.PpuMemory.nmt[mirroring1][(addr & 0x03FF)] = data; break;
                case 1: Nes.PpuMemory.nmt[mirroring1][(addr & 0x03FF)] = data; break;
                case 2: Nes.PpuMemory.nmt[mirroring2][(addr & 0x03FF)] = data; break;
                case 3: Nes.PpuMemory.nmt[mirroring2][(addr & 0x03FF)] = data; break;
            }
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(mirroring1);
            stream.Write(mirroring2);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            mirroring1 = stream.ReadInt32();
            mirroring2 = stream.ReadInt32();
        }
    }
}
