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
    [BoardName("Tengen RAMBO-1", 64)]
    class TengenRAMBO1 : Board
    {
        public TengenRAMBO1() : base() { }
        public TengenRAMBO1(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private byte addressOf8000 = 0;
        private bool chrMode = false;
        private bool chrModeFull1k = false;
        private bool prgMode = false;
        private byte[] chrRegs = new byte[8];
        private byte[] prgRegs = new byte[3];

        private byte irqReload = 0xFF;
        private bool IrqMode = false;
        private int irqPrescaler = 0;
        private byte irqCounter = 0;
        private bool irqEnable = false;
        private bool irqClear = false;

        private int oldA12;
        private int newA12;
        private int timer;

        public override void Initialize()
        {
            base.Initialize();

            Nes.Ppu.AddressLineUpdating = this.PPU_AddressLineUpdating;
            Nes.Ppu.CycleTimer = this.TickPPU;
            Nes.Cpu.ClockCycle = this.TickCPU;
        }
        public override void HardReset()
        {
            base.HardReset();
            addressOf8000 = 0;
            chrMode = false;
            chrModeFull1k = false;
            prgMode = false;
            chrRegs = new byte[8];
            prgRegs = new byte[3];
            for (byte i = 0; i < 8; i++)
                chrRegs[i] = i;
            for (byte i = 0; i < 3; i++)
                prgRegs[i] = i;
            SetupPRG(); SetupCHR();
            Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
            irqReload = 0xFF;
            IrqMode = false;
            irqPrescaler = 0;
            irqCounter = 0;
            irqEnable = false;
            irqClear = false;
            oldA12 = 0;
            newA12 = 0;
            timer = 0;
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xE001)
            {
                case 0x8000:
                    addressOf8000 = (byte)(data & 0xF);
                    chrMode = (data & 0x80) == 0x80;
                    prgMode = (data & 0x40) == 0x40;
                    chrModeFull1k = (data & 0x20) == 0x20;
                    SetupCHR(); SetupPRG();
                    break;
                case 0x8001:
                    switch (addressOf8000)
                    {
                        case 0x0: chrRegs[0] = data; SetupCHR(); break;
                        case 0x1: chrRegs[1] = data; SetupCHR(); break;
                        case 0x2: chrRegs[2] = data; SetupCHR(); break;
                        case 0x3: chrRegs[3] = data; SetupCHR(); break;
                        case 0x4: chrRegs[4] = data; SetupCHR(); break;
                        case 0x5: chrRegs[5] = data; SetupCHR(); break;
                        case 0x6: prgRegs[0] = data; SetupPRG(); break;
                        case 0x7: prgRegs[1] = data; SetupPRG(); break;
                        case 0x8: chrRegs[6] = data; SetupCHR(); break;
                        case 0x9: chrRegs[7] = data; SetupCHR(); break;
                        case 0xF: prgRegs[2] = data; SetupPRG(); break;
                    }
                    break;
                case 0xA000: Nes.PpuMemory.SwitchMirroring((data & 1) == 1 ? Mirroring.ModeHorz : Mirroring.ModeVert); break;
                case 0xC000: irqReload = data; break;
                case 0xC001: IrqMode = (data & 0x1) == 0x1; irqClear = true; irqPrescaler = 0; break;
                case 0xE000: irqEnable = false; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                case 0xE001: irqEnable = true; break;
            }
        }
        private void SetupCHR()
        {
            if (!chrMode)
            {
                Switch01kCHR(chrRegs[2], 0x1000);
                Switch01kCHR(chrRegs[3], 0x1400);
                Switch01kCHR(chrRegs[4], 0x1800);
                Switch01kCHR(chrRegs[5], 0x1C00);
                if (!chrModeFull1k)
                {
                    Switch02kCHR(chrRegs[0] >> 1, 0x0000);
                    Switch02kCHR(chrRegs[1] >> 1, 0x0800);
                }
                else
                {
                    Switch01kCHR(chrRegs[0], 0x0000);
                    Switch01kCHR(chrRegs[6], 0x0400);
                    Switch01kCHR(chrRegs[1], 0x0800);
                    Switch01kCHR(chrRegs[7], 0x0C00);
                }
            }
            else
            {
                Switch01kCHR(chrRegs[2], 0x0000);
                Switch01kCHR(chrRegs[3], 0x0400);
                Switch01kCHR(chrRegs[4], 0x0800);
                Switch01kCHR(chrRegs[5], 0x0C00);
                if (!chrModeFull1k)
                {

                    Switch02kCHR(chrRegs[0] >> 1, 0x1000);
                    Switch02kCHR(chrRegs[1] >> 1, 0x1800);
                }
                else
                {
                    Switch01kCHR(chrRegs[0], 0x1000);
                    Switch01kCHR(chrRegs[6], 0x1400);
                    Switch01kCHR(chrRegs[1], 0x1800);
                    Switch01kCHR(chrRegs[7], 0x1C00);
                }
            }
        }
        private void SetupPRG()
        {
            if (prgMode)
            {
                Switch08KPRG(prgRegs[2], 0x8000);
                Switch08KPRG(prgRegs[0], 0xA000);
                Switch08KPRG(prgRegs[1], 0xC000);
            }
            else
            {
                Switch08KPRG(prgRegs[0], 0x8000);
                Switch08KPRG(prgRegs[1], 0xA000);
                Switch08KPRG(prgRegs[2], 0xC000);
            }
        }
        private void TickPPU()
        {
            timer++;
        }
        private void TickCPU()
        {
            if (IrqMode)
            {
                irqPrescaler++;
                if (irqPrescaler == 4)
                {
                    irqPrescaler = 0;
                    ClockIRQ();
                }
            }
        }
        private void PPU_AddressLineUpdating(int addr)
        {
            if (!IrqMode)
            {
                oldA12 = newA12;
                newA12 = addr & 0x1000;

                if (oldA12 < newA12)
                {
                    if (timer > 8)
                    {
                        ClockIRQ();
                    }

                    timer = 0;
                }
            }
        }

        private void ClockIRQ()
        {
            if (irqClear)
            {
                irqCounter = (byte)(irqReload + 1); irqClear = false;
            }
            else
            {
                if (irqCounter == 0)
                {
                    irqCounter = irqReload;
                }
                else
                {
                    if (--irqCounter == 0 && irqEnable)
                        Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                }
            }
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(addressOf8000);
            stream.Write(chrMode);
            stream.Write(chrModeFull1k);
            stream.Write(prgMode);
            stream.Write(chrRegs);
            stream.Write(prgRegs);

            stream.Write(irqReload);
            stream.Write(IrqMode);
            stream.Write(irqPrescaler);
            stream.Write(irqCounter);
            stream.Write(irqEnable);
            stream.Write(irqClear);

            stream.Write(oldA12);
            stream.Write(newA12);
            stream.Write(timer);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);

            addressOf8000 = stream.ReadByte();
            chrMode = stream.ReadBoolean();
            chrModeFull1k = stream.ReadBoolean();
            prgMode = stream.ReadBoolean();
            stream.Read(chrRegs);
            stream.Read(prgRegs);

            irqReload = stream.ReadByte();
            IrqMode = stream.ReadBoolean();
            irqPrescaler = stream.ReadInt32();
            irqCounter = stream.ReadByte();
            irqEnable = stream.ReadBoolean();
            irqClear = stream.ReadBoolean();

            oldA12 = stream.ReadInt32();
            newA12 = stream.ReadInt32();
            timer = stream.ReadInt32();
        }
    }
}
