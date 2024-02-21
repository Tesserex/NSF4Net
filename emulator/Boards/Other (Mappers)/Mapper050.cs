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
    [BoardName("FDS-Port - Alt. Levels", 50)]
    class Mapper50AltLevels : Board
    {
        public Mapper50AltLevels() : base() { }
        public Mapper50AltLevels(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private ushort irqCounter = 0;
        private bool irqEnable = false;

        public override void Initialize()
        {
            // Maps prg writes to 0x8000 - 0xFFFF. Maps sram reads and writes to 0x6000 - 0x8000.
            // Then do a hard reset.
            base.Initialize();

            Nes.CpuMemory.Hook(0x4020, 0x5FFF, PeekPrg, PokePrg); 
            Nes.Cpu.ClockCycle = TickIRQTimer;
        }
        public override void HardReset()
        {
            // Switch 32KB prg bank at 0x8000
            // Switch 08KB chr bank at 0x0000
            base.HardReset();

            Switch08KPRG(8, 0x8000);
            Switch08KPRG(9, 0xA000); 
            Switch08KPRG(0, 0xC000); 
            Switch08KPRG(0xB, 0xE000);
        }
        protected override byte PeekSram(int address)
        {
            return prg[(address - 0x6000) + (0xF << 13)];
        }
        protected override void PokePrg(int address, byte data)
        {
            if (address < 0x6000)
                switch (address & 0x4120)
                {
                    case 0x4020: int page = (data << 0 & 0x8) | (data << 2 & 0x4) | (data >> 1 & 0x3);
                        Switch08KPRG(page, 0xC000); break;

                    case 0x4120: irqEnable = (data & 1) == 1;
                        if (!irqEnable)
                        {
                            Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
                            irqCounter = 0;
                        }
                        break;
                }
        }
        private void TickIRQTimer()
        {
            if (irqEnable)
            {
                if (++irqCounter == 0x1000)
                {
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                }
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(irqCounter);
            stream.Write(irqEnable);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            irqCounter = stream.ReadUshort();
            irqEnable = stream.ReadBoolean();
        }
    }
}
