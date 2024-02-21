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
    [BoardName("Mario Baby", 42)]
    class MarioBaby : Board
    {
        public MarioBaby() : base() { }
        public MarioBaby(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int SRAM_PRG_Page = 0;
        private bool irqEnable = false;
        private int irqCounter = 0;

        public override void Initialize()
        {
            // Maps prg writes to 0x8000 - 0xFFFF. Maps sram reads and writes to 0x6000 - 0x8000.
            // Then do a hard reset.
            base.Initialize();
            Nes.Cpu.ClockCycle = TickIRQTimer;
        }
        public override void HardReset()
        {
            // Switch 32KB prg bank at 0x8000
            // Switch 08KB chr bank at 0x0000
            base.HardReset();

            Switch32KPRG((prg.Length - 0x8000) >> 15);
        }
        protected override void PokePrg(int address, byte data)
        {
            if (address == 0x8000)
            {
                Switch08kCHR(data);
            }
            else if (address == 0xF000)
            {
                SRAM_PRG_Page = data << 13;
            }
            else
                switch (address & 0xE003)
                {
                    case 0xE000:
                        SRAM_PRG_Page = data << 13;
                        break;

                    case 0xE001:
                        if ((data & 0x8) == 0x8)
                            Nes.PpuMemory.SwitchMirroring(Types.Mirroring.ModeHorz);
                        else
                            Nes.PpuMemory.SwitchMirroring(Types.Mirroring.ModeVert);
                        break;

                    case 0xE002:
                        irqEnable = (data & 2) == 2;
                        if (!irqEnable)
                            irqCounter = 0;
                        Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
                        break;
                }

        }
        protected override byte PeekSram(int address)
        {
            return prg[(address & 0x1FFF) | SRAM_PRG_Page];
        }
        private void TickIRQTimer()
        {
            if (irqEnable)
            {
                int prev = irqCounter++;

                if ((irqCounter & 0x6000) != (prev & 0x6000))
                {
                    if ((irqCounter & 0x6000) == 0x6000)
                        Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                    else
                        Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
                }
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(SRAM_PRG_Page);
            stream.Write(irqEnable); 
            stream.Write(irqCounter);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            SRAM_PRG_Page = stream.ReadInt32();
            irqEnable = stream.ReadBoolean();
            irqCounter = stream.ReadInt32();
        }
    }
}
