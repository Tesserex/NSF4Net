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
    [BoardName("Irem H-3001", 65)]
    class IremH3001 : Board
    {
        public IremH3001() : base() { }
        public IremH3001(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool irqEnabled;
        private int irqCounter = 0;
        private int irqReload = 0;

        public override void Initialize()
        {
            base.Initialize();
            Nes.Cpu.ClockCycle = TickCPU;
        }
        public override void HardReset()
        {
            base.HardReset();
            Switch08KPRG(0, 0x8000);
            Switch08KPRG(1, 0xA000);
            Switch08KPRG(0xFE, 0xC000);
            Switch08KPRG(prg.Length - 0x2000 >> 13, 0xE000);
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address)
            {
                case 0x8000:
                case 0xA000:
                case 0xC000: Switch08KPRG(data, address); break;
                case 0xB000: Switch01kCHR(data, 0x0000); break;
                case 0xB001: Switch01kCHR(data, 0x0400); break;
                case 0xB002: Switch01kCHR(data, 0x0800); break;
                case 0xB003: Switch01kCHR(data, 0x0C00); break;
                case 0xB004: Switch01kCHR(data, 0x1000); break;
                case 0xB005: Switch01kCHR(data, 0x1400); break;
                case 0xB006: Switch01kCHR(data, 0x1800); break;
                case 0xB007: Switch01kCHR(data, 0x1C00); break;
                case 0x9001: Nes.PpuMemory.SwitchMirroring((data & 0x80) == 0x80 ? Mirroring.ModeHorz : Mirroring.ModeVert); break;
                case 0x9003: irqEnabled = (data & 0x80) == 0x80; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                case 0x9004: irqCounter = irqReload; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                case 0x9005: irqReload = (irqReload & 0x00FF) | (data << 8); break;
                case 0x9006: irqReload = (irqReload & 0xFF00) | (data << 0); break;
            }
        }
        void TickCPU()
        {
            if (irqEnabled)
            {
                irqCounter--;
                if (irqCounter == 0)
                {
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                }
            }
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(irqEnabled);
            stream.Write(irqCounter);
            stream.Write(irqReload);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            irqEnabled = stream.ReadBoolean();
            irqCounter = stream.ReadInt32();
            irqReload = stream.ReadInt32();
        }
    }
}
