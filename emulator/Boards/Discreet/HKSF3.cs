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
    [BoardName("HK-SF3 ", 91)]
    class HKSF3 : Board
    {
        public HKSF3() : base() { }
        public HKSF3(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        protected byte irqReload = 0xFF;
        protected byte irqCounter = 0;
        protected bool IrqEnable = false;
        protected int oldA12;
        protected int newA12;
        protected int timer;

        public override void Initialize()
        {
            base.Initialize();

            Nes.Ppu.AddressLineUpdating = this.PPU_AddressLineUpdating;
            Nes.Ppu.CycleTimer = this.TickPPU;
        }
        public override void HardReset()
        {
            base.HardReset();

            Switch16KPRG(prg.Length - 0x4000 >> 14, 0xC000);
            irqReload = 0xFF;
            irqCounter = 0;
            IrqEnable = false;
            oldA12 = 0;
            newA12 = 0;
            timer = 0;
        }
        protected override void PokeSram(int address, byte data)
        {
            switch (address & 0x7003)
            {
                case 0x6000: Switch02kCHR(data, 0x0000); break;
                case 0x6001: Switch02kCHR(data, 0x0800); break;
                case 0x6002: Switch02kCHR(data, 0x1000); break;
                case 0x6003: Switch02kCHR(data, 0x1800); break;

                case 0x7000: Switch08KPRG(data & 0xF, 0x8000); break;
                case 0x7001: Switch08KPRG(data & 0xF, 0xA000); break;
                case 0x7002: IrqEnable = false; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                case 0x7003: irqReload = 7; irqCounter = 0; IrqEnable = true; break;
            }
        }
        private void TickPPU()
        {
            timer++;
        }
        private void PPU_AddressLineUpdating(int addr)
        {
            oldA12 = newA12;
            newA12 = addr & 0x1000;

            if (oldA12 < newA12)
            {
                if (timer > 8)
                {
                    int old = irqCounter;

                    if (irqCounter == 0)
                        irqCounter = irqReload;
                    else
                        irqCounter = (byte)(irqCounter - 1);

                    if ((old != 0) && irqCounter == 0 && IrqEnable)
                        Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                }
                timer = 0;
            }
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(irqReload);
            stream.Write(irqCounter);
            stream.Write(IrqEnable);
            stream.Write(oldA12);
            stream.Write(newA12);
            stream.Write(timer);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            irqReload = stream.ReadByte();
            irqCounter = stream.ReadByte();
            IrqEnable = stream.ReadBoolean();
            oldA12 = stream.ReadInt32();
            newA12 = stream.ReadInt32();
            timer = stream.ReadInt32();
        }
    }
}
