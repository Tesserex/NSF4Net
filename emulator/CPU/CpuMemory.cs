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
namespace MyNes.Core.CPU
{
    public class CpuMemory : Memory
    {
        private byte[] ram = new byte[2048];

        public CpuMemory()
            : base(0x10000)
        {
            Hook(0x0000, 0x1FFF, PeekRam, PokeRam);
        }

        private byte PeekRam(int address)
        {
            return ram[address & 0x7FF];
        }
        private void PokeRam(int address, byte data)
        {
            ram[address & 0x7FF] = data;
        }

        public override void Initialize()
        {
            base.Hook(0x0000, 0x1FFF, PeekRam, PokeRam);
            HardReset();
        }
        public override void HardReset()
        {
            ram = new byte[2048];
            ram[0x0008] = 0xF7;
            ram[0x0008] = 0xF7;
            ram[0x0009] = 0xEF;
            ram[0x000A] = 0xDF;
            ram[0x000F] = 0xBF;
        }
        public override void SaveState(Core.Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(ram);
        }
        public override void LoadState(Core.Types.StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(ram);
        }
    }
}