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
    [BoardName("X in 1", 43)]
    class Mapper43Xin1 : Board
    {
        public Mapper43Xin1() : base() { }
        public Mapper43Xin1(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }
        public override void Initialize()
        {
            Nes.PpuMemory.Hook(0x0000, 0x1FFF, PeekChr, PokeChr);
            Nes.Cpu.ClockCycle = TickIRQTimer;
          
            for (int i = 0x4022; i <= 0xFFFF; i += 0x100)
            {
                switch (i & 0x71FF)
                {
                    case 0x0122: Nes.CpuMemory.Hook(i, Poke4122); break;
                    case 0x4022: Nes.CpuMemory.Hook(i, Poke4022); break;
                }
            }
            Nes.CpuMemory.Hook(0x5000, 0x5FFF, Peek5000);
            HardReset();
        }
        public override void HardReset()
        {
            Switch08kCHR(0);
            Switch08KPRG(1, 0x8000);
            Switch08KPRG(0, 0xA000); 
            Switch08KPRG(0, 0xC000);
            Switch08KPRG(9, 0xE000);
            irqEnabled = false;
            irqCounter = 0;
            title = 0xB000;
        }
        public override void SoftReset()
        {
            base.SoftReset();
            if (title == 0xB000)
            {
                title = 0xC000;
            }
            else
            {
                title = 0xB000;
            }
        }

        private byte[] banks = { 4, 3, 4, 4, 4, 7, 5, 6 };
        private bool irqEnabled = false;
        private int irqCounter = 0;
        private int title = 0x2000;

        private void Poke4022(int address, byte data)
        {
            Switch08KPRG(banks[data & 0x7], 0xC000);
        }
        private void Poke4122(int address, byte data)
        {
            irqEnabled = (data & 0x3) == 0x3;
            irqCounter = 0;
            Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
        }

        private byte Peek5000(int address)
        {
            return prg[address + title];
        }
        protected override byte PeekSram(int address)
        {
            return prg[address - (0x6000 - 0x4000)]; 
        }
        private void TickIRQTimer()
        {
            if (irqEnabled)
            {
                irqCounter = (irqCounter + 1) & 0xFFF;

                if (irqCounter == 0)
                {
                    irqEnabled = false;
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                }
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(irqEnabled);
            stream.Write(irqCounter);
            stream.Write(title);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            irqEnabled = stream.ReadBoolean();
            irqCounter = stream.ReadInt32();
            title = stream.ReadInt32();
        }
    }
}
