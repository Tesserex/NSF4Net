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
    [BoardName("Pirate SMB3", 56)]
    class PirateSMB3 : Board
    {
        public PirateSMB3() : base() { }
        public PirateSMB3(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int irqCounter = 0;
        private int irqLatch = 0;
        private bool irqEnabled = false;
        private int irqControl = 0;
        private int switchControl = 0;

        public override void Initialize()
        {
            base.Initialize();
            Nes.Cpu.ClockCycle = ClockIrqTimer;
        }
        public override void HardReset()
        {
            base.HardReset();
            Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
            irqLatch = 0;
            irqCounter = 0;
            irqControl = 0;
            irqEnabled = false;
        }
        protected override void PokePrg(int address, byte data)
        {
            if (address < 0xF000)
            {
                switch (address & 0xE000)
                {
                    case 0x8000: irqLatch = (irqLatch & 0xFFF0) | (data & 0xF) << 00; break;
                    case 0x9000: irqLatch = (irqLatch & 0xFF0F) | (data & 0xF) << 04; break;
                    case 0xA000: irqLatch = (irqLatch & 0xF0FF) | (data & 0xF) << 08; break;
                    case 0xB000: irqLatch = (irqLatch & 0x0FFF) | (data & 0xF) << 12; break;

                    case 0xC000:
                        irqControl = data & 0x5;
                        irqEnabled = (data & 0x2) == 0x2;
                        if (irqEnabled) 
                            irqCounter = irqLatch;
                        Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
                        break;

                    case 0xD000: irqEnabled = (irqControl & 0x1) == 0x1; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                    case 0xE000: switchControl = data; break;
                }
            }
            else//0xF000 - 0xFFFF
            {
                //switch prg
                int offset = (switchControl & 0xF) - 1;

                if (offset < 3)
                {
                    offset <<= 13;
                    Switch08KPRG((data & 0x0F) | (prgPage[offset >> 13] & 0x10), offset);
                }
                switch (address & 0xC00)
                {
                    case 0x000:

                        address &= 0x3;

                        if (address < 3)
                        {
                            address <<= 13;
                            Switch08KPRG((data & 0x0F) | (prgPage[address >> 13] & 0x10), address);
                        }
                        break;
                    case 0x800: Nes.PpuMemory.SwitchMirroring((data & 0x1) == 0x1 ? Types.Mirroring.ModeVert : Types.Mirroring.ModeHorz); break;
                    case 0xC00: Switch01kCHR(data, (address & 0x7) << 10); break;
                }
            }
        }
        private void ClockIrqTimer()
        {
            if (irqEnabled)
            {
                if (irqCounter++ == 0xFFFF)
                {
                    irqCounter = irqLatch;
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true); 
                }
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(irqCounter);
            stream.Write(irqLatch);
            stream.Write(irqEnabled);
            stream.Write(irqControl);
            stream.Write(switchControl);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            irqCounter = stream.ReadInt32();
            irqLatch = stream.ReadInt32();
            irqEnabled = stream.ReadBoolean();
            irqControl = stream.ReadInt32();
            switchControl = stream.ReadInt32();
        }
    }
}
