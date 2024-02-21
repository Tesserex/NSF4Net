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
    [BoardName("Unknown", 142)]
    class Mapper142 : Board
    {
        public Mapper142() : base() { }
        public Mapper142(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int irqCount = 0;
        private bool irqEnabled = false;
        private byte control = 0;
        private int prgBankAt6000 = 0;
        public override void Initialize()
        {
            base.Initialize();
            Nes.Cpu.ClockCycle = TickCPU;
        }
        public override void HardReset()
        {
            base.HardReset();

            irqCount = 0;
            irqEnabled = false;
            control = 0;
            prgBankAt6000 = 0;
        }
        protected override byte PeekSram(int address)
        {
            return prg[(prgBankAt6000 << 13) | (address & 0x1FFF)];
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xF000)
            {
                case 0x8000: irqCount = (irqCount & 0xFFF0) | (data & 0xF) << 0; break;
                case 0x9000: irqCount = (irqCount & 0xFF0F) | (data & 0xF) << 4; break;
                case 0xA000: irqCount = (irqCount & 0xF0FF) | (data & 0xF) << 8; break;
                case 0xB000: irqCount = (irqCount & 0x0FFF) | (data & 0xF) << 12; break;
                case 0xC000: irqEnabled = (data & 0xF) == 0xF; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                case 0xE000: control = data; break;
                case 0xF000:
                    address = (control & 0xF) - 1;

                    if (address < 3)
                    {
                        Switch08KPRG(data, address << 13);
                    }
                    else if (address < 4)
                    {
                        prgBankAt6000 = data;
                    }
                    break;
            }
        }
        private void TickCPU()
        {
            if (irqEnabled && irqCount++ == 0xFFFF)
            {
                irqEnabled = false;
                irqCount = 0;
                Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
            }
        }
    }
}
