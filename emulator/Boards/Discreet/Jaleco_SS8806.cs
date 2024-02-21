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
namespace MyNes.Core.Boards.Discreet
{
    [BoardName("Jaleco SS8806", 18)]
    class Jaleco_SS8806 : Board
    {
        public Jaleco_SS8806() : base() { }
        public Jaleco_SS8806(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int[] prgs = new int[3];
        private int[] chrs = new int[8];
        private bool wramON = true;
        private int irqRelaod = 0;
        private int irqCounter = 0;
        private bool irqEnable = false;
        private int irqMask = 0;

        public override void HardReset()
        {
            base.HardReset();
            base.Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
            Nes.Cpu.ClockCycle = TickIRQTimer;
            prgs = new int[3];
            chrs = new int[8];
            wramON = true;
            irqEnable = false;
            irqMask = 0xFFFF;
            irqRelaod = 0xFFFF;
            irqCounter = 0xFFFF;
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address)
            {
                /*PRG*/
                case 0x8000: prgs[0] = (prgs[0] & 0xF0) | ((data & 0x0F) << 0); base.Switch08KPRG(prgs[0], 0x8000); break;
                case 0x8001: prgs[0] = (prgs[0] & 0x0F) | ((data & 0x0F) << 4); base.Switch08KPRG(prgs[0], 0x8000); break;
                case 0x8002: prgs[1] = (prgs[1] & 0xF0) | ((data & 0x0F) << 0); base.Switch08KPRG(prgs[1], 0xA000); break;
                case 0x8003: prgs[1] = (prgs[1] & 0x0F) | ((data & 0x0F) << 4); base.Switch08KPRG(prgs[1], 0xA000); break;
                case 0x9000: prgs[2] = (prgs[2] & 0xF0) | ((data & 0x0F) << 0); base.Switch08KPRG(prgs[2], 0xC000); break;
                case 0x9001: prgs[2] = (prgs[2] & 0x0F) | ((data & 0x0F) << 4); base.Switch08KPRG(prgs[2], 0xC000); break;
                case 0x9002: wramON = (data & 1) == 1; break;
                /*CHR*/
                case 0xA000: chrs[0] = (chrs[0] & 0xF0) | ((data & 0x0F) << 0); base.Switch01kCHR(chrs[0], 0x0000); break;
                case 0xA001: chrs[0] = (chrs[0] & 0x0F) | ((data & 0x0F) << 4); base.Switch01kCHR(chrs[0], 0x0000); break;
                case 0xA002: chrs[1] = (chrs[1] & 0xF0) | ((data & 0x0F) << 0); base.Switch01kCHR(chrs[1], 0x0400); break;
                case 0xA003: chrs[1] = (chrs[1] & 0x0F) | ((data & 0x0F) << 4); base.Switch01kCHR(chrs[1], 0x0400); break;
                case 0xB000: chrs[2] = (chrs[2] & 0xF0) | ((data & 0x0F) << 0); base.Switch01kCHR(chrs[2], 0x0800); break;
                case 0xB001: chrs[2] = (chrs[2] & 0x0F) | ((data & 0x0F) << 4); base.Switch01kCHR(chrs[2], 0x0800); break;
                case 0xB002: chrs[3] = (chrs[3] & 0xF0) | ((data & 0x0F) << 0); base.Switch01kCHR(chrs[3], 0x0C00); break;
                case 0xB003: chrs[3] = (chrs[3] & 0x0F) | ((data & 0x0F) << 4); base.Switch01kCHR(chrs[3], 0x0C00); break;
                case 0xC000: chrs[4] = (chrs[4] & 0xF0) | ((data & 0x0F) << 0); base.Switch01kCHR(chrs[4], 0x1000); break;
                case 0xC001: chrs[4] = (chrs[4] & 0x0F) | ((data & 0x0F) << 4); base.Switch01kCHR(chrs[4], 0x1000); break;
                case 0xC002: chrs[5] = (chrs[5] & 0xF0) | ((data & 0x0F) << 0); base.Switch01kCHR(chrs[5], 0x1400); break;
                case 0xC003: chrs[5] = (chrs[5] & 0x0F) | ((data & 0x0F) << 4); base.Switch01kCHR(chrs[5], 0x1400); break;
                case 0xD000: chrs[6] = (chrs[6] & 0xF0) | ((data & 0x0F) << 0); base.Switch01kCHR(chrs[6], 0x1800); break;
                case 0xD001: chrs[6] = (chrs[6] & 0x0F) | ((data & 0x0F) << 4); base.Switch01kCHR(chrs[6], 0x1800); break;
                case 0xD002: chrs[7] = (chrs[7] & 0xF0) | ((data & 0x0F) << 0); base.Switch01kCHR(chrs[7], 0x1C00); break;
                case 0xD003: chrs[7] = (chrs[7] & 0x0F) | ((data & 0x0F) << 4); base.Switch01kCHR(chrs[7], 0x1C00); break;
                /*IRQ*/
                case 0xE000: irqRelaod = (irqRelaod & 0xFFF0) | ((data & 0x0F) << 00); break;
                case 0xE001: irqRelaod = (irqRelaod & 0xFF0F) | ((data & 0x0F) << 04); break;
                case 0xE002: irqRelaod = (irqRelaod & 0xF0FF) | ((data & 0x0F) << 08); break;
                case 0xE003: irqRelaod = (irqRelaod & 0x0FFF) | ((data & 0x0F) << 12); break;

                case 0xF000: irqCounter = irqRelaod; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                case 0xF001:
                    irqEnable = (data & 1) == 1;
                    if ((data & 0x8) == 0x8)
                        irqMask = 0x000F;
                    else if ((data & 0x4) == 0x4)
                        irqMask = 0x00FF;
                    else if ((data & 0x2) == 0x2)
                        irqMask = 0x0FFF;
                    else
                        irqMask = 0xFFFF;
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                case 0xF002:
                    switch (data & 0x3)
                    {
                        case 0: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.ModeHorz); break;
                        case 1: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.ModeVert); break;
                        case 2: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.Mode1ScA); break;
                        case 3: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.Mode1ScB); break;
                    }
                    break;
            }
        }
        protected override void PokeSram(int address, byte data)
        {
            if (wramON)
                base.PokeSram(address, data);
        }
        protected override byte PeekSram(int address)
        {
            if (wramON)
                return base.PeekSram(address);
            return 0;
        }

        private void TickIRQTimer()
        {
            if (irqEnable)
            {
                if (((irqCounter & irqMask) > 0) && ((--irqCounter & irqMask) == 0))
                {
                    irqEnable = false;
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                }
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(prgs);
            stream.Write(chrs);
            stream.Write(wramON);
            stream.Write(irqRelaod);
            stream.Write(irqCounter);
            stream.Write(irqEnable);
            stream.Write(irqMask);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(prgs);
            stream.Read(chrs);
            wramON = stream.ReadBoolean();
            irqRelaod = stream.ReadInt32();
            irqCounter = stream.ReadInt32();
            irqEnable = stream.ReadBoolean();
            irqMask = stream.ReadInt32();
        }
    }
}
