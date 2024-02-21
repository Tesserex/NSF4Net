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
using MyNes.Core.APU.VRC6;
namespace MyNes.Core.Boards.Konami
{
    [BoardName("VRC6", 0)]
    abstract class VRC6 : Board
    {
        public VRC6() : base() { }
        public VRC6(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }
        //change these values for vrc6x version
        protected int AD_8_0 = 0x8000;
        protected int AD_8_1 = 0x8001;
        protected int AD_8_2 = 0x8002;
        protected int AD_8_3 = 0x8003;

        protected int AD_9_0 = 0x9000;
        protected int AD_9_1 = 0x9001;
        protected int AD_9_2 = 0x9002;
        protected int AD_9_3 = 0x9003;

        protected int AD_A_0 = 0xA000;
        protected int AD_A_1 = 0xA001;
        protected int AD_A_2 = 0xA002;
        protected int AD_A_3 = 0xA003;

        protected int AD_B_0 = 0xB000;
        protected int AD_B_1 = 0xB001;
        protected int AD_B_2 = 0xB002;
        protected int AD_B_3 = 0xB003;

        protected int AD_C_0 = 0xC000;
        protected int AD_C_1 = 0xC001;
        protected int AD_C_2 = 0xC002;
        protected int AD_C_3 = 0xC003;

        protected int AD_D_0 = 0xD000;
        protected int AD_D_1 = 0xD001;
        protected int AD_D_2 = 0xD002;
        protected int AD_D_3 = 0xD003;

        protected int AD_E_0 = 0xE000;
        protected int AD_E_1 = 0xE001;
        protected int AD_E_2 = 0xE002;
        protected int AD_E_3 = 0xE003;

        protected int AD_F_0 = 0xF000;
        protected int AD_F_1 = 0xF001;
        protected int AD_F_2 = 0xF002;

        protected byte irqReload = 0;
        protected byte irqCounter = 0;
        protected int irqPrescaler = 0;
        protected bool irqEnable;
        protected bool irqMode;
        protected bool irqEnableOnAcknowledge;
        private VRC6ExternalSound externalSound;


        public override void Initialize()
        {
            base.Initialize();
            externalSound = new VRC6ExternalSound();
            externalSound.sndPulse1.Hook(AD_9_0, AD_9_1, AD_9_2, AD_9_3);
            externalSound.sndPulse2.Hook(AD_A_0, AD_A_1, AD_A_2, AD_9_3);
            externalSound.sndSawtooth.Hook(AD_B_0, AD_B_1, AD_B_2, AD_9_3);
            Nes.Apu.AddExternalMixer(externalSound);
        }
        public override void HardReset()
        {
            // Switch 32KB prg bank at 0x8000
            // Switch 08KB chr bank at 0x0000
            base.HardReset();

            base.Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
            Nes.Cpu.ClockCycle = TickIRQTimer;
        }
        protected override void PokePrg(int address, byte data)
        {
            if ((address == AD_8_0) ||
                (address == AD_8_1) ||
                (address == AD_8_2) ||
                (address == AD_8_3))
            {
                Switch16KPRG(data, 0x8000);
            }
            else if (address == AD_B_3)
            {
                switch ((data & 0xC) >> 2)
                {
                    case 0: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.ModeVert); break;
                    case 1: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.ModeHorz); break;
                    case 2: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.Mode1ScA); break;
                    case 3: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.Mode1ScB); break;
                }
            }
            else if ((address == AD_C_0) ||
                     (address == AD_C_1) ||
                     (address == AD_C_2) ||
                     (address == AD_C_3))
            {
                Switch08KPRG(data, 0xC000);
            }
            else if (address == AD_D_0)
            {
                Switch01kCHR(data, 0x0000);
            }
            else if (address == AD_D_1)
            {
                Switch01kCHR(data, 0x0400);
            }
            else if (address == AD_D_2)
            {
                Switch01kCHR(data, 0x0800);
            }
            else if (address == AD_D_3)
            {
                Switch01kCHR(data, 0x0C00);
            }
            else if (address == AD_E_0)
            {
                Switch01kCHR(data, 0x1000);
            }
            else if (address == AD_E_1)
            {
                Switch01kCHR(data, 0x1400);
            }
            else if (address == AD_E_2)
            {
                Switch01kCHR(data, 0x1800);
            }
            else if (address == AD_E_3)
            {
                Switch01kCHR(data, 0x1C00);
            }
            else if ((address == AD_F_0))
            {
                irqReload = data;
            }
            else if ((address == AD_F_1))
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
            else if (address == AD_F_2)
            {
                Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
                irqEnable = irqEnableOnAcknowledge;
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
