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
namespace MyNes.Core.Boards.Sunsoft
{
    [BoardName("Sunsoft3", 67)]
    class Sunsoft3 : Board
    {
        public Sunsoft3() : base() { }
        public Sunsoft3(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool irqWriteFlipFlop = false;
        private int irqCounter = 0;
        private bool irqEnabled = false;

        public override void Initialize()
        {
            base.Initialize();

            Nes.Cpu.ClockCycle = TickCPU;
        }
        public override void HardReset()
        {
            base.HardReset();
            irqWriteFlipFlop = false;
            irqCounter = 0;
            irqEnabled = false;
            Switch16KPRG(prg.Length - 0x4000 >> 14, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xF800)
            {
                case 0x8800: Switch02kCHR(data, 0x0000); break;
                case 0x9800: Switch02kCHR(data, 0x0800); break;
                case 0xA800: Switch02kCHR(data, 0x1000); break;
                case 0xB800: Switch02kCHR(data, 0x1800); break;

                case 0xC800:
                    if (irqWriteFlipFlop)
                        irqCounter = (irqCounter & 0xFF00) | data;
                    else
                        irqCounter = (irqCounter & 0x00FF) | (data << 8);
                    irqWriteFlipFlop = !irqWriteFlipFlop;
                    break;
                case 0xD800: 
                    irqEnabled = (data & 0x10) == 0x10;
                    irqWriteFlipFlop = false;
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
                    break;
                case 0xE800:
                    switch (data & 0x3)
                    {
                        case 0: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeVert); break;
                        case 1: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeHorz); break;
                        case 2: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScA); break;
                        case 3: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScB); break;
                    } 
                    break;
                case 0xF800: Switch16KPRG(data, 0x8000); break;
            }
        }
        void TickCPU()
        {
            if (irqEnabled)
            {
                irqCounter--;
                if (irqCounter == 0)
                {
                    irqCounter = 0xFFFF;
                    irqEnabled = false;
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                }
            }
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(irqWriteFlipFlop);
            stream.Write(irqCounter);
            stream.Write(irqEnabled);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            irqWriteFlipFlop = stream.ReadBoolean();
            irqCounter = stream.ReadInt32();
            irqEnabled = stream.ReadBoolean();
        }
    }
}
