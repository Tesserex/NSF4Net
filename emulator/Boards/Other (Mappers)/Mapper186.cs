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
    [BoardName("Unknown", 186)]
    class Mapper186 : Board
    {
        public Mapper186() : base() { }
        public Mapper186(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private byte[] ram = new byte[0xB00];
        private int sramPRG = 0;

        public override void Initialize()
        {
            base.Initialize();

            Nes.CpuMemory.Hook(0x4200, 0x4201, Peek4200);
            Nes.CpuMemory.Hook(0x4202, Peek4202);
            Nes.CpuMemory.Hook(0x4203, Peek4200);
            Nes.CpuMemory.Hook(0x4204, 0x43FF, Peek4204);

            for (int i = 0x4200; i < 0x4400; i += 0x2)
            {
                Nes.CpuMemory.Hook(i + 0x0, Poke4200);
                Nes.CpuMemory.Hook(i + 0x1, PRGSwitch);
            }

            Nes.CpuMemory.Hook(0x4400, 0x4EFF, Peek4400, Poke4400);
        }
        public override void HardReset()
        {
            base.HardReset();
            sramPRG = 0;
            ram = new byte[0xB00];

            Switch16KPRG(prg.Length - 0x4000 >> 4, 0xC000);
        }
        protected override byte PeekSram(int address)
        {
            return prg[(sramPRG << 13) | (address & 0x1FFF)];
        }
        private void PRGSwitch(int address, byte data)
        {
            Switch16KPRG(data, 0x8000);
        }
        private byte Peek4200(int address)
        {
            return 0x00;
        }
        private byte Peek4202(int address)
        {
            return 0x40;
        }
        private byte Peek4204(int address)
        {
            return 0xFF;
        }
        private void Poke4200(int address, byte data)
        {
            sramPRG = data >> 6 & 0x3;
        }
        private byte Peek4400(int address)
        {
            return ram[address - 0x4400];
        }
        private void Poke4400(int address, byte data)
        {
            ram[address - 0x4400] = data;
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(ram);
            stream.Write(sramPRG);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(ram);
            sramPRG = stream.ReadInt32();
        }
    }
}
