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
    [BoardName("Pirate MMC5-Style", 90)]
    class Mapper090 : Board
    {
        /* TODO:
         * irq Funky Mode Reg not emulated yet
         * Only 2 irq counter modes are emulated, the cpu cycle and ppu's A12
         * Dipswitch is unknown and not emulated well here
         */
        public Mapper090() : base() { }
        public Mapper090(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int[] prgRegs = new int[4];
        private int[] chrRegs = new int[8];
        private int[] ntRegs = new int[4];
        private int chrSetup = 0;
        private bool putPRGin6000 = false;
        private int prgSelectMode = 0;
        private int prgPageIn6000 = 0;
        private bool mirrorCHR = false;
        private bool chrBlockMode = false;
        private int chrBLOCK = 0;
        private bool EnableAdvancedMirroring = false;
        private bool disableNTRAM = false;
        private bool NTRAM = false;
        private byte Dipswitch;
        private int irqCounter = 0;
        private bool IrqEnable = false;
        private int oldA12;
        private int newA12;
        private bool irqCountDownMode = false;
        private bool irqCountUpMode = false;
        private bool irqFunkyMode = false;
        private bool irqPrescalerSize = false;
        private int irqSource = 0;
        private int irqPrescaler = 0;
        private int irqPrescalerXOR = 0;
        private byte irqFunkyModeReg = 0;

        private byte multiA = 0;
        private byte multiB = 0;
        private byte RAM5803 = 0;

        public override void Initialize()
        {
            base.Initialize();
            Nes.Cpu.ClockCycle = TickCPU;
            Nes.Ppu.AddressLineUpdating = this.PPU_AddressLineUpdating;
            Nes.CpuMemory.Hook(0x5000, Peek5000);
            Nes.CpuMemory.Hook(0x5800, Peek5800, Poke5800);
            Nes.CpuMemory.Hook(0x5801, Peek5801, Poke5801);
            Nes.CpuMemory.Hook(0x5803, Peek5803, Poke5803);
        }
        // Call this at initialize to enable the advanced mirroring stuff for mapper 209.
        protected void ApplyAdvancedMirroring()
        {
            Nes.PpuMemory.Hook(0x2000, 0x3EFF, PeekNmt, PokeNmt);
        }
        public override void HardReset()
        {
            base.HardReset();

            Switch32KPRG((prg.Length - 0x8000) >> 15);

            prgRegs = new int[4];
            chrRegs = new int[8];
            ntRegs = new int[4];
            chrSetup = 0;
            putPRGin6000 = false;
            prgSelectMode = 0;
            prgPageIn6000 = 0;
            mirrorCHR = false;
            chrBlockMode = false;
            chrBLOCK = 0;
            EnableAdvancedMirroring = false;
            disableNTRAM = false;
            NTRAM = false;
            Dipswitch = 0;// ??
            irqCounter = 0;
            IrqEnable = false;
            oldA12 = 0;
            newA12 = 0;
            irqCountDownMode = false;
            irqCountUpMode = false;
            irqFunkyMode = false;
            irqPrescalerSize = false;
            irqSource = 0;
            irqPrescaler = 0;
            irqPrescalerXOR = 0;
            irqFunkyModeReg = 0;

            multiA = 0;
            multiB = 0;
            RAM5803 = 0;
        }
        public override void SoftReset()
        {
            base.SoftReset();
            if (Dipswitch == 0)
                Dipswitch = 0xFF;
            else
                Dipswitch = 0;
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xF007)
            {
                case 0x8000:
                case 0x8004: prgRegs[0] = data & 0x3F; SetupPRG(); break;
                case 0x8001:
                case 0x8005: prgRegs[1] = data & 0x3F; SetupPRG(); break;
                case 0x8002:
                case 0x8006: prgRegs[2] = data & 0x3F; SetupPRG(); break;
                case 0x8003:
                case 0x8007: prgRegs[3] = data & 0x3F; SetupPRG(); break;

                case 0x9000: chrRegs[0] = (chrRegs[0] & 0xFF00) | (data << 0); SetupCHR(); break;
                case 0xA000: chrRegs[0] = (chrRegs[0] & 0x00FF) | (data << 8); SetupCHR(); break;
                case 0x9001: chrRegs[1] = (chrRegs[1] & 0xFF00) | (data << 0); SetupCHR(); break;
                case 0xA001: chrRegs[1] = (chrRegs[1] & 0x00FF) | (data << 8); SetupCHR(); break;
                case 0x9002: chrRegs[2] = (chrRegs[2] & 0xFF00) | (data << 0); SetupCHR(); break;
                case 0xA002: chrRegs[2] = (chrRegs[2] & 0x00FF) | (data << 8); SetupCHR(); break;
                case 0x9003: chrRegs[3] = (chrRegs[3] & 0xFF00) | (data << 0); SetupCHR(); break;
                case 0xA003: chrRegs[3] = (chrRegs[3] & 0x00FF) | (data << 8); SetupCHR(); break;
                case 0x9004: chrRegs[4] = (chrRegs[4] & 0xFF00) | (data << 0); SetupCHR(); break;
                case 0xA004: chrRegs[4] = (chrRegs[4] & 0x00FF) | (data << 8); SetupCHR(); break;
                case 0x9005: chrRegs[5] = (chrRegs[5] & 0xFF00) | (data << 0); SetupCHR(); break;
                case 0xA005: chrRegs[5] = (chrRegs[5] & 0x00FF) | (data << 8); SetupCHR(); break;
                case 0x9006: chrRegs[6] = (chrRegs[6] & 0xFF00) | (data << 0); SetupCHR(); break;
                case 0xA006: chrRegs[6] = (chrRegs[6] & 0x00FF) | (data << 8); SetupCHR(); break;
                case 0x9007: chrRegs[7] = (chrRegs[7] & 0xFF00) | (data << 0); SetupCHR(); break;
                case 0xA007: chrRegs[7] = (chrRegs[7] & 0x00FF) | (data << 8); SetupCHR(); break;

                case 0xB000: ntRegs[0] = (ntRegs[0] & 0xFF00) | (data << 0); break;
                case 0xB004: ntRegs[0] = (ntRegs[0] & 0x00FF) | (data << 8); break;
                case 0xB001: ntRegs[1] = (ntRegs[1] & 0xFF00) | (data << 0); break;
                case 0xB005: ntRegs[1] = (ntRegs[1] & 0x00FF) | (data << 8); break;
                case 0xB002: ntRegs[2] = (ntRegs[2] & 0xFF00) | (data << 0); break;
                case 0xB006: ntRegs[2] = (ntRegs[2] & 0x00FF) | (data << 8); break;
                case 0xB003: ntRegs[3] = (ntRegs[3] & 0xFF00) | (data << 0); break;
                case 0xB007: ntRegs[3] = (ntRegs[3] & 0x00FF) | (data << 8); break;

                case 0xC000:
                    if ((data & 1) == 1) { IrqEnable = true; }
                    else { IrqEnable = false; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); } break;
                case 0xC001:
                    irqCountDownMode = (data & 0x80) == 0x80;
                    irqCountUpMode = (data & 0x40) == 0x40;
                    irqFunkyMode = (data & 0x8) == 0x8;
                    irqPrescalerSize = (data & 0x4) == 0x4;
                    irqSource = data & 3;
                    break;
                case 0xC002: IrqEnable = false; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                case 0xC003: IrqEnable = true; break;
                case 0xC004: irqPrescaler = data ^ irqPrescalerXOR; break;
                case 0xC005: irqCounter = data ^ irqPrescalerXOR; break;
                case 0xC006: irqPrescalerXOR = data; break;
                case 0xC007: irqFunkyModeReg = data; break;
                case 0xD000:
                    chrSetup = (data >> 3) & 0x3;

                    EnableAdvancedMirroring = (data & 0x20) == 0x20;
                    disableNTRAM = (data & 0x40) == 0x40;

                    putPRGin6000 = (data & 0x80) == 0x80;
                    prgSelectMode = data & 0x7;

                    SetupPRG();
                    SetupCHR();
                    break;
                case 0xD001:
                    switch (data & 0x3)
                    {
                        case 0: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeVert); break;
                        case 1: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeHorz); break;
                        case 2: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScA); break;
                        case 3: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScB); break;
                    }
                    break;
                case 0xD002: NTRAM = (data & 0x80) == 0x80; break;
                case 0xD003:
                    mirrorCHR = (data & 0x80) == 0x80;
                    chrBlockMode = (data & 0x20) == 0x20;
                    chrBLOCK = (data & 0x1F) << 8;
                    SetupCHR();
                    break;
            }
        }
        protected override byte PeekSram(int address)
        {
            if (putPRGin6000)
            {
                return prg[(prgPageIn6000 << 13) | (address & 0x1FFF)];
            }
            return 0;
        }

        private byte Peek5000(int address)
        {
            return Dipswitch;
        }
        private void Poke5800(int address, byte data)
        {
            multiA = data;
        }
        private void Poke5801(int address, byte data)
        {
            multiB = data;
        }
        private byte Peek5800(int address)
        {
            return (byte)((multiA * multiB) & 0xFF);
        }
        private byte Peek5801(int address)
        {
            return (byte)(((multiA * multiB) & 0xFF00) >> 8);
        }
        private void Poke5803(int address, byte data)
        {
            RAM5803 = data;
        }
        private byte Peek5803(int address)
        {
            return RAM5803;
        }

        private void SetupPRG()
        {
            switch (prgSelectMode)
            {
                case 0: prgPageIn6000 = (prgRegs[3] * 4) + 3; Switch32KPRG((prg.Length - 0x8000) >> 15); break;
                case 1:
                    prgPageIn6000 = (prgRegs[3] * 2) + 1;
                    Switch16KPRG(prgRegs[1], 0x8000);
                    Switch16KPRG((prg.Length - 0x4000) >> 14, 0xC000);
                    break;
                case 2:
                    prgPageIn6000 = prgRegs[3];
                    Switch08KPRG(prgRegs[0], 0x8000);
                    Switch08KPRG(prgRegs[1], 0xA000);
                    Switch08KPRG(prgRegs[2], 0xC000);
                    Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
                    break;
                case 3:
                    prgPageIn6000 = ReverseByte(prgRegs[3]);
                    Switch08KPRG(ReverseByte(prgRegs[0]), 0x8000);
                    Switch08KPRG(ReverseByte(prgRegs[1]), 0xA000);
                    Switch08KPRG(ReverseByte(prgRegs[2]), 0xC000);
                    Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
                    break;
                case 4: prgPageIn6000 = (prgRegs[3] * 4) + 3; Switch32KPRG(prgRegs[3]); break;
                case 5:
                    prgPageIn6000 = (prgRegs[3] * 2) + 1;
                    Switch16KPRG(prgRegs[1], 0x8000);
                    Switch16KPRG(prgRegs[3], 0xC000);
                    break;
                case 6:
                    prgPageIn6000 = prgRegs[3];
                    Switch08KPRG(prgRegs[0], 0x8000);
                    Switch08KPRG(prgRegs[1], 0xA000);
                    Switch08KPRG(prgRegs[2], 0xC000);
                    Switch08KPRG(prgRegs[3], 0xE000);
                    break;
                case 7:
                    prgPageIn6000 = ReverseByte(prgRegs[3]);
                    Switch08KPRG(ReverseByte(prgRegs[0]), 0x8000);
                    Switch08KPRG(ReverseByte(prgRegs[1]), 0xA000);
                    Switch08KPRG(ReverseByte(prgRegs[2]), 0xC000);
                    Switch08KPRG(ReverseByte(prgRegs[3]), 0xE000);
                    break;
            }
        }
        private void SetupCHR()
        {
            switch (chrSetup)
            {
                case 0:
                    if (chrBlockMode)
                        Switch08kCHR(chrRegs[0]);
                    else
                        Switch08kCHR((chrRegs[0] & 0xFF) | chrBLOCK);
                    break;
                case 1:
                    if (chrBlockMode)
                    {
                        Switch04kCHR(chrRegs[0], 0x0000);
                        Switch04kCHR(chrRegs[4], 0x1000);
                    }
                    else
                    {
                        Switch04kCHR((chrRegs[0] & 0xFF) | chrBLOCK, 0x0000);
                        Switch04kCHR((chrRegs[4] & 0xFF) | chrBLOCK, 0x1000);
                    }
                    break;
                case 2:
                    if (chrBlockMode)
                    {
                        if (!mirrorCHR)
                        {
                            Switch02kCHR(chrRegs[0], 0x0000);
                            Switch02kCHR(chrRegs[2], 0x0800);
                            Switch02kCHR(chrRegs[4], 0x1000);
                            Switch02kCHR(chrRegs[6], 0x1800);
                        }
                        else
                        {
                            Switch02kCHR(chrRegs[0], 0x0000);
                            Switch02kCHR(chrRegs[0], 0x0800);
                            Switch02kCHR(chrRegs[4], 0x1000);
                            Switch02kCHR(chrRegs[6], 0x1800);
                        }
                    }
                    else
                    {
                        if (!mirrorCHR)
                        {
                            Switch02kCHR((chrRegs[0] & 0xFF) | chrBLOCK, 0x0000);
                            Switch02kCHR((chrRegs[2] & 0xFF) | chrBLOCK, 0x0800);
                            Switch02kCHR((chrRegs[4] & 0xFF) | chrBLOCK, 0x1000);
                            Switch02kCHR((chrRegs[6] & 0xFF) | chrBLOCK, 0x1800);
                        }
                        else
                        {
                            Switch02kCHR((chrRegs[0] & 0xFF) | chrBLOCK, 0x0000);
                            Switch02kCHR((chrRegs[0] & 0xFF) | chrBLOCK, 0x0800);
                            Switch02kCHR((chrRegs[4] & 0xFF) | chrBLOCK, 0x1000);
                            Switch02kCHR((chrRegs[6] & 0xFF) | chrBLOCK, 0x1800);
                        }
                    }
                    break;
                case 3:
                    if (chrBlockMode)
                    {
                        if (!mirrorCHR)
                        {
                            Switch01kCHR(chrRegs[0], 0x0000);
                            Switch01kCHR(chrRegs[1], 0x0400);
                            Switch01kCHR(chrRegs[2], 0x0800);
                            Switch01kCHR(chrRegs[3], 0x0C00);
                            Switch01kCHR(chrRegs[4], 0x1000);
                            Switch01kCHR(chrRegs[5], 0x1400);
                            Switch01kCHR(chrRegs[6], 0x1800);
                            Switch01kCHR(chrRegs[7], 0x1C00);
                        }
                        else
                        {
                            Switch01kCHR(chrRegs[0], 0x0000);
                            Switch01kCHR(chrRegs[1], 0x0400);
                            Switch01kCHR(chrRegs[0], 0x0800);
                            Switch01kCHR(chrRegs[1], 0x0C00);
                            Switch01kCHR(chrRegs[4], 0x1000);
                            Switch01kCHR(chrRegs[5], 0x1400);
                            Switch01kCHR(chrRegs[6], 0x1800);
                            Switch01kCHR(chrRegs[7], 0x1C00);
                        }
                    }
                    else
                    {
                        if (!mirrorCHR)
                        {
                            Switch01kCHR((chrRegs[0] & 0xFF) | chrBLOCK, 0x0000);
                            Switch01kCHR((chrRegs[1] & 0xFF) | chrBLOCK, 0x0400);
                            Switch01kCHR((chrRegs[2] & 0xFF) | chrBLOCK, 0x0800);
                            Switch01kCHR((chrRegs[3] & 0xFF) | chrBLOCK, 0x0C00);
                            Switch01kCHR((chrRegs[4] & 0xFF) | chrBLOCK, 0x1000);
                            Switch01kCHR((chrRegs[5] & 0xFF) | chrBLOCK, 0x1400);
                            Switch01kCHR((chrRegs[6] & 0xFF) | chrBLOCK, 0x1800);
                            Switch01kCHR((chrRegs[7] & 0xFF) | chrBLOCK, 0x1C00);
                        }
                        else
                        {
                            Switch01kCHR((chrRegs[0] & 0xFF) | chrBLOCK, 0x0000);
                            Switch01kCHR((chrRegs[1] & 0xFF) | chrBLOCK, 0x0400);
                            Switch01kCHR((chrRegs[0] & 0xFF) | chrBLOCK, 0x0800);
                            Switch01kCHR((chrRegs[1] & 0xFF) | chrBLOCK, 0x0C00);
                            Switch01kCHR((chrRegs[4] & 0xFF) | chrBLOCK, 0x1000);
                            Switch01kCHR((chrRegs[5] & 0xFF) | chrBLOCK, 0x1400);
                            Switch01kCHR((chrRegs[6] & 0xFF) | chrBLOCK, 0x1800);
                            Switch01kCHR((chrRegs[7] & 0xFF) | chrBLOCK, 0x1C00);
                        }
                    }
                    break;
            }
        }
        private byte ReverseByte(int value)
        {
            byte data = 0;
            data = (byte)(((value & 0x40) >> 6) | ((value & 0x20) >> 4) | ((value & 0x10) >> 2)
                | ((value & 0x8)) | ((value & 0x4) << 2) | ((value & 0x2) << 4) | ((value & 0x1) << 6));
            return data;
        }

        public byte PeekNmt(int addr)
        {
            if (!EnableAdvancedMirroring)
            {
                return Nes.PpuMemory.nmt[Nes.PpuMemory.nmtBank[(addr >> 10) & 0x03]][addr & 0x03FF];
            }
            else
            {
                if (disableNTRAM)
                {
                    return chr[(ntRegs[(addr >> 10) & 0x03] << 10) | (addr & 0x03FF)];
                }
                else
                {
                    if ((ntRegs[(addr >> 10) & 0x03] & 0x80) == 0x80 && NTRAM)
                        return Nes.PpuMemory.nmt[ntRegs[(addr >> 10) & 0x03] & 1][addr & 0x03FF];
                    else
                        return chr[(ntRegs[(addr >> 10) & 0x03] << 10) | (addr & 0x03FF)];
                }
            }
        }
        public void PokeNmt(int addr, byte data)
        {
           // Nes.PpuMemory.nmt[Nes.PpuMemory.nmtBank[(addr >> 10) & 0x03]][addr & 0x03FF] = data;
            if (!EnableAdvancedMirroring)
            {
                Nes.PpuMemory.nmt[Nes.PpuMemory.nmtBank[(addr >> 10) & 0x03]][addr & 0x03FF] = data;
            }
            else
            {
                if (disableNTRAM)
                {
                    //return chr[(ntRegs[(addr >> 10) & 0x03] << 10) | (addr & 0x03FF)];// what do suppose to do ? lol
                }
                else
                {
                    if ((ntRegs[(addr >> 10) & 0x03] & 0x80) == 0x80 && NTRAM)
                        Nes.PpuMemory.nmt[ntRegs[(addr >> 10) & 0x03] & 1][addr & 0x03FF] = data;
                }
            }
        }
        private void TickCPU()
        {
            if (irqSource == 0)
            {
                if (irqPrescalerSize)//3-bits
                {
                    irqPrescaler = (irqPrescaler & 0xF8) | (((irqPrescaler & 0x7) + 1) & 0x7);
                    if ((irqPrescaler & 0x7) == 0x7)
                    {
                        ClockIRQCounter();
                    }
                }
                else//8-bits
                {
                    irqPrescaler = (byte)(irqPrescaler + 1);
                    if (irqPrescaler == 0xFF)
                    {
                        ClockIRQCounter();
                    }
                }
            }
        }
        private void PPU_AddressLineUpdating(int addr)
        {
            if (irqSource == 1)
            {
                oldA12 = newA12;
                newA12 = addr & 0x1000;

                if (oldA12 < newA12)
                {
                    if (irqPrescalerSize)//3-bits
                    {
                        irqPrescaler = (irqPrescaler & 0xF8) | (((irqPrescaler & 0x7) + 1) & 0x7);
                        if ((irqPrescaler & 0x7) == 0x7)
                        {
                            ClockIRQCounter();
                        }
                    }
                    else//8-bits
                    {
                        irqPrescaler = (byte)(irqPrescaler + 1);
                        if (irqPrescaler == 0xFF)
                        {
                            ClockIRQCounter();
                        }
                    }
                }
            }
        }
        private void ClockIRQCounter()
        {
            if (irqCountDownMode && irqCountUpMode)
                return;
            if (irqCountDownMode)
            {
                irqCounter--;
                if (irqCounter == 0)
                {
                    irqCounter = 0xFF;
                    if (IrqEnable)
                        Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                }
            }
            else if (irqCountUpMode)
            {
                irqCounter = (byte)(irqCounter + 1);
                if (irqCounter == 0xFF)
                {
                    irqCounter = 0;
                    if (IrqEnable)
                        Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                }
            }
            /*int old = irqCounter;

            if (irqCounter == 0 || clear)
                irqCounter = irqReload;
            else
                irqCounter = (byte)(irqCounter - 1);

            if ((old != 0 || clear) && irqCounter == 0 && IrqEnable)
                Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);

            clear = false;*/
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(prgRegs);
            stream.Write(chrRegs);
            stream.Write(ntRegs);
            stream.Write(chrSetup);
            stream.Write(putPRGin6000);
            stream.Write(prgSelectMode);
            stream.Write(prgPageIn6000);
            stream.Write(mirrorCHR);
            stream.Write(chrBlockMode);
            stream.Write(chrBLOCK);
            stream.Write(EnableAdvancedMirroring);
            stream.Write(disableNTRAM);
            stream.Write(NTRAM);
            stream.Write(Dipswitch);
            stream.Write(irqCounter);
            stream.Write(IrqEnable);
            stream.Write(oldA12);
            stream.Write(newA12);
            stream.Write(irqCountDownMode);
            stream.Write(irqCountUpMode);
            stream.Write(irqFunkyMode);
            stream.Write(irqPrescalerSize);
            stream.Write(irqSource);
            stream.Write(irqPrescaler);
            stream.Write(irqPrescalerXOR);
            stream.Write(irqFunkyModeReg);
            stream.Write(multiA);
            stream.Write(multiB);
            stream.Write(RAM5803);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(prgRegs);
            stream.Read(chrRegs);
            stream.Read(ntRegs);
            chrSetup = stream.ReadInt32();
            putPRGin6000 = stream.ReadBoolean();
            prgSelectMode = stream.ReadInt32();
            prgPageIn6000 = stream.ReadInt32();
            mirrorCHR = stream.ReadBoolean();
            chrBlockMode = stream.ReadBoolean();
            chrBLOCK = stream.ReadInt32();
            EnableAdvancedMirroring = stream.ReadBoolean();
            disableNTRAM = stream.ReadBoolean();
            NTRAM = stream.ReadBoolean();
            Dipswitch = stream.ReadByte();
            irqCounter = stream.ReadInt32();
            IrqEnable = stream.ReadBoolean();
            oldA12 = stream.ReadInt32();
            newA12 = stream.ReadInt32();
            irqCountDownMode = stream.ReadBoolean();
            irqCountUpMode = stream.ReadBoolean();
            irqFunkyMode = stream.ReadBoolean();
            irqPrescalerSize = stream.ReadBoolean();
            irqSource = stream.ReadInt32();
            irqPrescaler = stream.ReadInt32();
            irqPrescalerXOR = stream.ReadInt32();
            irqFunkyModeReg = stream.ReadByte();

            multiA = stream.ReadByte();
            multiB = stream.ReadByte();
            RAM5803 = stream.ReadByte();
        }
    }
}
