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
using System.IO;
namespace MyNes.Core.Boards.Discreet
{
    [BoardName("Taito TC190V/TC0350", 33)]
    class TaitoTC190V : Board
    {
        public TaitoTC190V() : base() { }
        public TaitoTC190V(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool MODE = false;//true = mapper 33 mode[TC0350], false = mapper 48 mode[TC190V]
        private byte irqReload = 0xFF;
        private byte irqCounter = 0;
        private bool IrqEnable = false;
        private bool mmc3_alt_behavior = false;//true=rev A, false= rev B
        private bool clear = false;
        private int oldA12;
        private int newA12;
        private int timer;

        public override void Initialize()
        {
            base.Initialize();
            Nes.Ppu.AddressLineUpdating = this.PPU_AddressLineUpdating;
            Nes.Ppu.CycleTimer = this.TickPPU; 
            MODE = false;
            //since some game function like 48 but assigned as 33, we need to add some cases here
            if (Path.GetFileNameWithoutExtension(Nes.RomInfo.Path).Contains("Don Doko Don") &&
                !Path.GetFileNameWithoutExtension(Nes.RomInfo.Path).Contains("Don Doko Don 2"))
            {
                MODE = true;
            } 
            if (Path.GetFileNameWithoutExtension(Nes.RomInfo.Path).Contains("Insector X"))
            {
                MODE = true;
            }
            if (Path.GetFileNameWithoutExtension(Nes.RomInfo.Path).Contains("Takeshi no Sengoku Fuuunji"))
            {
                MODE = true;
            }
        }
        public override void HardReset()
        {
            // Switch 32KB prg bank at 0x8000
            // Switch 08KB chr bank at 0x0000
            base.HardReset();

            Switch16KPRG((prg.Length - 0x4000) >> 14, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            if (MODE)//normal mapper 33
            {
                switch (address & 0xA003)
                {
                    case 0x8000:
                        Switch08KPRG(data & 0x3F, 0x8000);
                        Nes.PpuMemory.SwitchMirroring((data & 0x40) == 0x40 ? Types.Mirroring.ModeHorz : Types.Mirroring.ModeVert);
                        break;
                    case 0x8001: Switch08KPRG(data & 0x3F, 0xA000); break;
                    case 0x8002: Switch02kCHR(data, 0x0000); break;
                    case 0x8003: Switch02kCHR(data, 0x0800); break;
                    case 0xA000: Switch01kCHR(data, 0x1000); break;
                    case 0xA001: Switch01kCHR(data, 0x1400); break;
                    case 0xA002: Switch01kCHR(data, 0x1800); break;
                    case 0xA003: Switch01kCHR(data, 0x1C00); break;
                }
            }
            else//mapper 48
            {
                switch (address & 0xE003)
                {
                    case 0x8000:
                        Switch08KPRG(data, 0x8000);
                        //
                        break;
                    case 0x8001: Switch08KPRG(data, 0xA000); break;
                    case 0x8002: Switch02kCHR(data, 0x0000); break;
                    case 0x8003: Switch02kCHR(data, 0x0800); break;
                    case 0xA000: Switch01kCHR(data, 0x1000); break;
                    case 0xA001: Switch01kCHR(data, 0x1400); break;
                    case 0xA002: Switch01kCHR(data, 0x1800); break;
                    case 0xA003: Switch01kCHR(data, 0x1C00); break;

                    case 0xE000: Nes.PpuMemory.SwitchMirroring((data & 0x40) == 0x40 ? Types.Mirroring.ModeHorz : Types.Mirroring.ModeVert); break;

                    case 0xC000: irqReload = (byte)(data ^ 0xFF); break;
                    case 0xC001: if (mmc3_alt_behavior) clear = true; irqCounter = 0; break;
                    case 0xC002: IrqEnable = true; break;
                    case 0xC003: IrqEnable = false; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                }
            }
        }
        private void TickPPU()
        {
            if (!MODE)//no irqs in mapper 33
                timer++;
        }
        private void PPU_AddressLineUpdating(int addr)
        {
            if (!MODE)//no irqs in mapper 33
            {
                oldA12 = newA12;
                newA12 = addr & 0x1000;

                if (oldA12 < newA12)
                {
                    if (timer > 8)
                    {
                        int old = irqCounter;

                        if (irqCounter == 0 || clear)
                            irqCounter = irqReload;
                        else
                            irqCounter = (byte)(irqCounter - 1);

                        if ((!mmc3_alt_behavior || old != 0 || clear) && irqCounter == 0 && IrqEnable)
                            Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);

                        clear = false;
                    }

                    timer = 0;
                }
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(irqReload);
            stream.Write(irqCounter);
            stream.Write(IrqEnable);
            stream.Write(clear);
            stream.Write(oldA12);
            stream.Write(newA12);
            stream.Write(timer);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            irqReload = stream.ReadByte();
            irqCounter = stream.ReadByte();
            IrqEnable = stream.ReadBoolean();
            clear = stream.ReadBoolean();
            oldA12 = stream.ReadInt32();
            newA12 = stream.ReadInt32();
            timer = stream.ReadInt32();
        }
    }
}
