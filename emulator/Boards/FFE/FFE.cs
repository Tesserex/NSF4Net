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
namespace MyNes.Core.Boards.FFE
{
    //This is not a board, it's a master class for FFE boards.
    abstract class FFE : Board
    {
        public FFE()
            : base()
        { }
        public FFE(byte[] chr, byte[] prg, byte[] trainer, bool isVram)
            : base(chr, prg, trainer, isVram)
        { }
        protected bool IRQEnabled = false;
        protected int irq_counter = 0;

        public override void Initialize()
        {
            base.Initialize();
            Nes.CpuMemory.Hook(0x4018, 0x5FFF, PeekPrg, PokePrg);
        }
        public override void HardReset()
        {
            base.HardReset();
            sram = new byte[0x2000];
            //trainer loaded into address 0x7000
            trainer.CopyTo(sram, 0x1000);
            Nes.Cpu.ClockCycle = TickIRQTimer;
        }
        protected override byte PeekPrg(int address)
        {
            return base.PeekPrg(address);
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address)
            {
                case 0x42FE:
                    if ((data & 0x10) != 0)
                        Nes.PpuMemory.SwitchMirroring(Types.Mirroring.Mode1ScB);
                    else
                        Nes.PpuMemory.SwitchMirroring(Types.Mirroring.Mode1ScA);
                    break;
                case 0x42FF:
                    if ((data & 0x10) != 0)
                        Nes.PpuMemory.SwitchMirroring(Types.Mirroring.ModeHorz);
                    else
                        Nes.PpuMemory.SwitchMirroring(Types.Mirroring.ModeVert);
                    break;
                case 0x4501: IRQEnabled = false; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                case 0x4502: irq_counter = (short)((irq_counter & 0xFF00) | data); break;
                case 0x4503: irq_counter = (short)((data << 8) | (irq_counter & 0x00FF)); IRQEnabled = true;
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
            }
        }
        private void TickIRQTimer()
        {
            if (IRQEnabled)
            {
                irq_counter++;
                if (irq_counter >= 0xFFFF)
                {
                    irq_counter = 0;
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                }
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(IRQEnabled);
            stream.Write(irq_counter);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            IRQEnabled = stream.ReadBoolean();
            irq_counter = stream.ReadInt32();
        }
    }
}
