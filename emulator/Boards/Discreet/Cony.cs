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
    [BoardName("Cony", 83)]
    class Cony : Board
    {
        public Cony() : base() { }
        public Cony(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private byte dipSwitch = 0;
        private byte pr8 = 0;
        private byte controlReg = 0;
        private byte[] prgRegs = new byte[5];
        private bool irqStep = false;
        private int irqCounter = 0;
        private bool irqEnabled = false;

        public override void Initialize()
        {
            base.Initialize();

            Nes.Cpu.ClockCycle = TickCPU;

            Nes.CpuMemory.Hook(0x5000, Peek5000);
            Nes.CpuMemory.Hook(0x5100, 0x51FF, Peek5100, Poke5100);

            for (int i = 0x8000; i < 0x9000; i += 0x400)
            {
                Nes.CpuMemory.Hook(i + 0x000, i + 0x0FF, Poke8000);
                Nes.CpuMemory.Hook(i + 0x100, i + 0x1FF, Poke8100);

                for (int j = i + 0x00, n = i + 0x100; j < n; j += 0x02)
                {
                    Nes.CpuMemory.Hook(j + 0x200, Poke8200);
                    Nes.CpuMemory.Hook(j + 0x201, Poke8201);
                }

                for (int j = i + 0x00, n = i + 0x100; j < n; j += 0x20)
                {
                    Nes.CpuMemory.Hook(j + 0x300, j + 0x30F, Poke8300);

                    if ((chr.Length / 1024) == 512)
                    {
                        Nes.CpuMemory.Hook(j + 0x310, j + 0x311, Poke8310_1);
                        Nes.CpuMemory.Hook(j + 0x316, j + 0x317, Poke8310_1);
                    }
                    else
                    {
                        Nes.CpuMemory.Hook(j + 0x310, j + 0x317, Poke8310_0);
                    }
                }
            }

            Nes.CpuMemory.Hook(0xB000, Poke8000);
            Nes.CpuMemory.Hook(0xB0FF, Poke8000);
            Nes.CpuMemory.Hook(0xB100, Poke8000);
        }
        public override void HardReset()
        {
            base.HardReset();
            Switch16KPRG((prg.Length - 0x4000 >> 14), 0xC000);
            dipSwitch = 0;
            pr8 = 0;
            controlReg = 0;
            prgRegs = new byte[5];
            irqStep = true;
            irqCounter = 0;
            irqEnabled = false;
        }
        protected override byte PeekSram(int address)
        {
            if ((controlReg & 0x20) == 0x20)
            {
                int bank = (controlReg & 0x10) == 0x10 ? 0x1F : prgRegs[3];
                return prg[(bank << 13) | (address & 0x1FFF)];
            }
            else
            {
                return (byte)(address >> 8);
            }
        }
        private void UpdatePRG()
        {
            if ((controlReg & 0x10) == 0x10)
            {
                Switch08KPRG(prgRegs[0], 0x8000);
                Switch08KPRG(prgRegs[1], 0xA000);
                Switch08KPRG(prgRegs[2], 0xC000);
            }
            else
            {
                Switch16KPRG(prgRegs[4] & 0x3F, 0x8000);
                Switch16KPRG((prgRegs[4] & 0x3F) | 0x0F, 0xC000);
            }
        }
        private byte Peek5000(int address)
        {
            return dipSwitch;
        }
        private byte Peek5100(int address)
        {
            return pr8;
        }

        private void Poke5100(int address, byte data)
        {
            pr8 = data;
        }
        private void Poke8000(int address, byte data)
        {
            prgRegs[4] = data;
            UpdatePRG();
        }
        private void Poke8100(int address, byte data)
        {
            int diff = data ^ controlReg;
            controlReg = data;

            if ((diff & 0x10) == 0x10)
                UpdatePRG();

            if ((diff & 0xC0) == 0xC0)
            {
                irqStep = ((data & 0x40) == 0x40);
            }

            if ((diff & 0x03) == 0x03)
            {
                switch (data & 0x3)
                {
                    case 0: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeVert); break;
                    case 1: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeHorz); break;
                    case 2: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScA); break;
                    case 3: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScB); break;
                }
            }
        }
        private void Poke8200(int address, byte data)
        {
            irqCounter = (irqCounter & 0xFF00) | data;
            Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
        }
        private void Poke8201(int address, byte data)
        {
            irqCounter = (irqCounter & 0x00FF) | (data << 8);
            irqEnabled = (controlReg & 0x80) == 0x80;
            Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
        }
        private void Poke8300(int address, byte data)
        {
            data &= 0x1F;

            if (prgRegs[address & 0x3] != data)
            {
                prgRegs[address & 0x3] = data;
                UpdatePRG();
            }
        }
        private void Poke8310_0(int address, byte data)
        {
            Switch01kCHR((prgRegs[4] << 4 & 0x300) | data, (address & 0x7) << 10);
        }
        private void Poke8310_1(int address, byte data)
        {
            Switch02kCHR(data, (address & 0x3) << 11);
        }

        private void TickCPU()
        {
            if (irqEnabled && irqCounter > 0)
            {
                irqCounter = (irqCounter + (irqStep ? 1 : 0)) & 0xFFFF;

                if (irqCounter == 0)
                {
                    irqEnabled = false;
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                }
            }
        }
    }
}
