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
namespace MyNes.Core.Boards.Konami
{
    [BoardName("VRC4", 0)]
    abstract class VRC4 : Board
    {
        public VRC4() : base() { }
        public VRC4(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        //change these values for vrc4x version
        protected int AD_8_0 = 0x8000;
        protected int AD_8_1 = 0x8002;
        protected int AD_8_2 = 0x8004;
        protected int AD_8_3 = 0x8006;
        protected int AD_8_1_1 = 0x8040;
        protected int AD_8_2_2 = 0x8080;
        protected int AD_8_3_3 = 0x80C0;

        protected int AD_9_0 = 0x9000;
        protected int AD_9_1 = 0x9002;
        protected int AD_9_2 = 0x9004;
        protected int AD_9_3 = 0x9006;
        protected int AD_9_1_1 = 0x9040;
        protected int AD_9_2_2 = 0x9080;
        protected int AD_9_3_3 = 0x90C0;

        protected int AD_A_0 = 0xA000;
        protected int AD_A_1 = 0xA002;
        protected int AD_A_2 = 0xA004;
        protected int AD_A_3 = 0xA006;
        protected int AD_A_1_1 = 0xA040;
        protected int AD_A_2_2 = 0xA080;
        protected int AD_A_3_3 = 0xA0C0;

        protected int AD_B_0 = 0xB000;
        protected int AD_B_1 = 0xB002;
        protected int AD_B_2 = 0xB004;
        protected int AD_B_3 = 0xB006;
        protected int AD_B_1_1 = 0xB040;
        protected int AD_B_2_2 = 0xB080;
        protected int AD_B_3_3 = 0xB0C0;

        protected int AD_C_0 = 0xC000;
        protected int AD_C_1 = 0xC002;
        protected int AD_C_2 = 0xC004;
        protected int AD_C_3 = 0xC006;
        protected int AD_C_1_1 = 0xC040;
        protected int AD_C_2_2 = 0xC080;
        protected int AD_C_3_3 = 0xC0C0;

        protected int AD_D_0 = 0xD000;
        protected int AD_D_1 = 0xD002;
        protected int AD_D_2 = 0xD004;
        protected int AD_D_3 = 0xD006;
        protected int AD_D_1_1 = 0xD040;
        protected int AD_D_2_2 = 0xD080;
        protected int AD_D_3_3 = 0xD0C0;

        protected int AD_E_0 = 0xE000;
        protected int AD_E_1 = 0xE002;
        protected int AD_E_2 = 0xE004;
        protected int AD_E_3 = 0xE006;
        protected int AD_E_1_1 = 0xE040;
        protected int AD_E_2_2 = 0xE080;
        protected int AD_E_3_3 = 0xE0C0;

        protected int AD_F_0 = 0xF000;
        protected int AD_F_1 = 0xF002;
        protected int AD_F_2 = 0xF004;
        protected int AD_F_3 = 0xF006;
        protected int AD_F_1_1 = 0xF040;
        protected int AD_F_2_2 = 0xF080;
        protected int AD_F_3_3 = 0xF0C0;

        protected bool prgMode;
        protected byte[] prgRegs = new byte[2];
        protected byte[] chrRegs = new byte[8];

        protected byte irqReload = 0;
        protected byte irqCounter = 0;
        protected int irqPrescaler = 0;
        protected bool irqEnable;
        protected bool irqMode;
        protected bool irqEnableOnAcknowledge;

        public override void HardReset()
        {
            // Switch 32KB prg bank at 0x8000
            // Switch 08KB chr bank at 0x0000
            base.HardReset();
            base.Switch16KPRG((prg.Length - 0x4000) >> 14, 0xC000);
            Nes.Cpu.ClockCycle = TickIRQTimer;
        }
        protected override void PokePrg(int address, byte data)
        {
            // Since we can't use switch 'cause we can't use const 'cause consts can't be overriden in C#,
            // we must do it in this stupid way !
            if ((address == AD_8_0) ||
                (address == AD_8_1) ||
                (address == AD_8_1_1) ||
                (address == AD_8_2) ||
                (address == AD_8_2_2) ||
                (address == AD_8_3) ||
                (address == AD_8_3_3))
            {
                prgRegs[0] = (byte)(data & 0x1F); SetupPRG();
            }
            else if ((address == AD_9_0) || (address == AD_9_1) || (address == AD_9_1_1))
            {
                switch (data & 0x3)
                {
                    case 0: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.ModeVert); break;
                    case 1: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.ModeHorz); break;
                    case 2: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.Mode1ScA); break;
                    case 3: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.Mode1ScB); break;
                }
            }
            else if ((address == AD_9_2) || (address == AD_9_2_2) || (address == AD_9_3) || (address == AD_9_3_3))
            {
                prgMode = (data & 0x2) == 0x2;
            }
            else if ((address == AD_A_0) ||
               (address == AD_A_1) ||
               (address == AD_A_1_1) ||
               (address == AD_A_2) ||
               (address == AD_A_2_2) ||
               (address == AD_A_3) ||
               (address == AD_A_3_3))
            {
                prgRegs[1] = (byte)(data & 0x1F); SetupPRG();
            }
            else if ((address == AD_B_0))
            {
                chrRegs[0] = (byte)((chrRegs[0] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[0], 0x0000);
            }
            else if ((address == AD_B_1) || (address == AD_B_1_1))
            {
                chrRegs[0] = (byte)((chrRegs[0] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[0], 0x0000);
            }
            else if ((address == AD_B_2) || (address == AD_B_2_2))
            {
                chrRegs[1] = (byte)((chrRegs[1] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[1], 0x0400);
            }
            else if ((address == AD_B_3) || (address == AD_B_3_3))
            {
                chrRegs[1] = (byte)((chrRegs[1] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[1], 0x0400);
            }
            else if ((address == AD_C_0))
            {
                chrRegs[2] = (byte)((chrRegs[2] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[2], 0x0800);
            }
            else if ((address == AD_C_1) || (address == AD_C_1_1))
            {
                chrRegs[2] = (byte)((chrRegs[2] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[2], 0x0800);
            }
            else if ((address == AD_C_2) || (address == AD_C_2_2))
            {
                chrRegs[3] = (byte)((chrRegs[3] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[3], 0x0C00);
            }
            else if ((address == AD_C_3) || (address == AD_C_3_3))
            {
                chrRegs[3] = (byte)((chrRegs[3] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[3], 0x0C00);
            }
            else if ((address == AD_D_0))
            {
                chrRegs[4] = (byte)((chrRegs[4] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[4], 0x1000);
            }
            else if ((address == AD_D_1) || (address == AD_D_1_1))
            {
                chrRegs[4] = (byte)((chrRegs[4] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[4], 0x1000);
            }
            else if ((address == AD_D_2) || (address == AD_D_2_2))
            {
                chrRegs[5] = (byte)((chrRegs[5] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[5], 0x1400);
            }
            else if ((address == AD_D_3) || (address == AD_D_3_3))
            {
                chrRegs[5] = (byte)((chrRegs[5] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[5], 0x1400);
            }
            else if ((address == AD_E_0))
            {
                chrRegs[6] = (byte)((chrRegs[6] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[6], 0x1800);
            }
            else if ((address == AD_E_1) || (address == AD_E_1_1))
            {
                chrRegs[6] = (byte)((chrRegs[6] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[6], 0x1800);
            }
            else if ((address == AD_E_2) || (address == AD_E_2_2))
            {
                chrRegs[7] = (byte)((chrRegs[7] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[7], 0x1C00);
            }
            else if ((address == AD_E_3) || (address == AD_E_3_3))
            {
                chrRegs[7] = (byte)((chrRegs[7] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[7], 0x1C00);
            }
            else if ((address == AD_F_0))
            {
                irqReload = (byte)((irqReload & 0xF0) | (data & 0x0F) << 0);
            }
            else if ((address == AD_F_1) || (address == AD_F_1_1))
            {
                irqReload = (byte)((irqReload & 0x0F) | (data & 0x0F) << 4);
            }
            else if ((address == AD_F_2) || (address == AD_F_2_2))
            {
                irqMode = (data & 0x4) == 0x4;
                irqEnable = (data & 0x2) == 0x2;
                irqEnableOnAcknowledge = (data & 0x1) == 0x1;
                if (irqEnable)
                {
                    irqCounter = irqReload;
                    irqPrescaler = 341;
                }
                Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
            }
            else if ((address == AD_F_3) || (address == AD_F_3_3))
            {
                Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
                irqEnable = irqEnableOnAcknowledge;
            }
        }
        protected void SetupPRG()
        {
            if (!prgMode)
            {
                base.Switch08KPRG(prgRegs[0], 0x8000);
                base.Switch08KPRG(prgRegs[1], 0xA000);
                base.Switch08KPRG((prg.Length - 0x4000) >> 13, 0xC000);
                base.Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
            }
            else
            {
                base.Switch08KPRG((prg.Length - 0x4000) >> 13, 0x8000);
                base.Switch08KPRG(prgRegs[1], 0xA000);
                base.Switch08KPRG(prgRegs[0], 0xC000);
                base.Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
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
            stream.Write(prgMode);
            stream.Write(prgRegs);
            stream.Write(chrRegs);
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
            prgMode = stream.ReadBoolean();
            stream.Read(prgRegs);
            stream.Read(chrRegs);
            irqReload = stream.ReadByte();
            irqCounter = stream.ReadByte();
            irqPrescaler = stream.ReadInt32();
            irqEnable = stream.ReadBoolean();
            irqMode = stream.ReadBoolean();
            irqEnableOnAcknowledge = stream.ReadBoolean();
        }
    }
}
