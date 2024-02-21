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
    [BoardName("NES-EVENT", 105)]
    class NESEVENT : Board
    {
        public NESEVENT() : base() { }
        public NESEVENT(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        public override void Initialize()
        {
            base.Initialize();
            Nes.Cpu.ClockCycle = ClockCpu;
        }
        public override void HardReset()
        {
            base.HardReset();

            reg = new byte[4];
            reg[0] = 0x0C;
            reg[1] = reg[2] = reg[3] = 0;

            wramON = true;
            shift = 0;
            buffer = 0;

            IRQcontrol = true;
            IRQcounter = 0;
            DipSwitchTargetIrqCounter = 0x28000000;
            DipSwitchNumber = 0;
        }
        public override void SoftReset()
        {
            base.SoftReset();
            DipSwitchNumber = (DipSwitchNumber + 1) & 0xF;
            DipSwitchTargetIrqCounter = 0x20000000 | (DipSwitchNumber << 22);
            IRQcontrol = true;
            IRQcounter = 0;
        }
        private byte[] reg = new byte[4];
        private byte shift = 0;
        private byte buffer = 0;
        private bool wramON = true;//enable sram, not sure about this but some docs mansion it

        private bool IRQcontrol = false; 
        private int IRQcounter = 0;
        private int DipSwitchTargetIrqCounter = 0;
        private int DipSwitchNumber = 0;
       
        protected override void PokePrg(int address, byte data)
        {
            if ((data & 0x80) == 0x80)
            {
                reg[0] |= 0x0C;
                shift = buffer = 0;
                return;
            }
            if ((data & 0x01) == 0x01)
                buffer |= (byte)(1 << shift);
            if (++shift < 5)
                return;

            address = (ushort)((address & 0x7FFF) >> 13);
            reg[address] = buffer;

            shift = buffer = 0;
            switch (address)
            {
                case 0://8000-9FFF
                    if ((reg[0] & 0x02) == 0x02)
                    {
                        if ((reg[0] & 0x01) != 0)
                            Nes.PpuMemory.SwitchMirroring(Mirroring.ModeHorz);
                        else
                            Nes.PpuMemory.SwitchMirroring(Mirroring.ModeVert);
                    }
                    else
                    {
                        if ((reg[0] & 0x01) != 0)
                            Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScB);
                        else
                            Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScA);
                    }
                    break;

                case 1://A000-BFFF [...I OAA.]
                    IRQcontrol = (reg[1] & 0x10) == 0x10;//I
                    if (IRQcontrol)
                    { 
                        IRQcounter = 0;
                        Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
                    }
                    if (!IRQcontrol)// I=0
                    {
                        if ((reg[1] & 0x8) == 0)// O=0
                        {
                            Switch32KPRG((reg[1] >> 1) & 3);
                        }
                    }
                    else// I=1
                    {
                        Switch32KPRG(0);
                    }
                    break;

                case 3://E000-FFFF
                    wramON = (reg[3] & 0x10) == 0;
                    if (!IRQcontrol)// I=0
                    {
                        if ((reg[1] & 0x8) == 0x8)// O=1
                        {
                            if ((reg[0] & 0x08) == 0)//P=0
                            {
                                Switch32KPRG(((reg[3] >> 1)& 0x3) + 4);// use second 128K
                            }
                            else//P=1
                            {
                                if ((reg[0] & 0x04) == 0x04)//S=1
                                {
                                    base.Switch16KPRG(8 + (reg[3] & 0x7), 0x8000);
                                    base.Switch16KPRG(15, 0xC000);
                                }
                                else//S=0
                                {

                                    base.Switch16KPRG(8, 0x8000);
                                    base.Switch16KPRG(8 + (reg[3] & 0x7), 0xC000);
                                }
                            }
                        }
                    }
                    else// I=1
                    {
                        Switch32KPRG(0);
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

        private void ClockCpu()
        {
            if (!IRQcontrol)
            {
                IRQcounter++;
                if (IRQcounter == DipSwitchTargetIrqCounter)
                {
                    //IRQcounter = 0;
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                }
            }
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(reg);
            stream.Write(sram);
            stream.Write(shift);
            stream.Write(buffer);
            stream.Write(wramON);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(reg);
            stream.Read(sram);
            shift = stream.ReadByte();
            buffer = stream.ReadByte();
            wramON = stream.ReadBoolean();
        }
    }
}
