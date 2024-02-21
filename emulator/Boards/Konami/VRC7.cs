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
namespace MyNes.Core.Boards.Konami
{
    [BoardName("VRC7", 85)]
    class VRC7 : Board
    {
        public VRC7() : base() { }
        public VRC7(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private byte irqReload = 0;
        private byte irqCounter = 0;
        private int irqPrescaler = 0;
        private bool irqEnable;
        private bool irqMode;
        private bool irqEnableOnAcknowledge;

        public override void Initialize()
        {
            base.Initialize();

            Nes.Cpu.ClockCycle = TickIRQTimer;
        }
        public override void HardReset()
        {
            base.HardReset();

            base.Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address)
            {
                case 0x8000: Switch08KPRG(data, 0x8000); break;

                case 0x8008:
                case 0x8010: Switch08KPRG(data, 0xA000); break;

                case 0x9000: Switch08KPRG(data, 0xC000); break;

                case 0xA000: Switch01kCHR(data, 0x0000); break;
                case 0xA008:
                case 0xA010: Switch01kCHR(data, 0x0400); break;
                case 0xB000: Switch01kCHR(data, 0x0800); break;
                case 0xB008:
                case 0xB010: Switch01kCHR(data, 0x0C00); break;
                case 0xC000: Switch01kCHR(data, 0x1000); break;
                case 0xC008:
                case 0xC010: Switch01kCHR(data, 0x1400); break;
                case 0xD000: Switch01kCHR(data, 0x1800); break;
                case 0xD008:
                case 0xD010: Switch01kCHR(data, 0x1C00); break;

                case 0xE000:
                    switch (data & 0x3)
                    {
                        case 0: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.ModeVert); break;
                        case 1: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.ModeHorz); break;
                        case 2: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.Mode1ScA); break;
                        case 3: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.Mode1ScB); break;
                    }
                    break;
                case 0xE008:
                case 0xE010: irqReload = data; break;
                case 0xF000:
                    irqMode = (data & 0x4) == 0x4;
                    irqEnable = (data & 0x2) == 0x2;
                    irqEnableOnAcknowledge = (data & 0x1) == 0x1;
                    if (irqEnable)
                    {
                        irqCounter = irqReload;
                        irqPrescaler = 341;
                    }
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
                    break;
                case 0xF008:
                case 0xF010: Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
                    irqEnable = irqEnableOnAcknowledge;
                    break;
            }
        }
        private void TickIRQTimer()
        {
            if (irqEnable)
            {
                if (!irqMode)
                {
                    if (irqPrescaler > 0)
                        irqPrescaler -= 3;
                    else
                    {
                        irqPrescaler = 341;
                        irqCounter++;
                        if (irqCounter == 0xFF)
                        {
                            irqCounter = irqReload;
                            Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                        }
                    }
                }
                else
                {
                    irqCounter++;
                    if (irqCounter == 0xFF)
                    {
                        irqCounter = irqReload;
                        Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                    }
                }
            }
        }
        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(irqReload);
            stream.Write(irqCounter);
            stream.Write(irqPrescaler);
            stream.Write(irqEnable);
            stream.Write(irqMode);
            stream.Write(irqEnableOnAcknowledge);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            irqReload = stream.ReadByte();
            irqCounter = stream.ReadByte();
            irqPrescaler = stream.ReadInt32();
            irqEnable = stream.ReadBoolean();
            irqMode = stream.ReadBoolean();
            irqEnableOnAcknowledge = stream.ReadBoolean();
        }
    }
}
