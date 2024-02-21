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
namespace MyNes.Core.Boards.Konami
{
    [BoardName("VRC3", 73)]
    class VRC3 : Board
    {
        public VRC3() : base() { }
        public VRC3(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }
       
        private int irqCounter = 0;
        private int irqReload = 0;
        private bool irqMode = false;
        private bool irqEnabled = false;
        private bool irqEnableOnAcknowledge = false;

        public override void Initialize()
        {
            base.Initialize();

            Nes.Cpu.ClockCycle = TickCPU;
        }
        public override void HardReset()
        {
            base.HardReset();

            Switch16KPRG(prg.Length - 0x4000 >> 14, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xF000)
            {
                case 0x8000: irqReload = (irqReload & 0xFFF0) | ((data & 0xF) << 00); break;
                case 0x9000: irqReload = (irqReload & 0xFF0F) | ((data & 0xF) << 04); break;
                case 0xA000: irqReload = (irqReload & 0xF0FF) | ((data & 0xF) << 08); break;
                case 0xB000: irqReload = (irqReload & 0x0FFF) | ((data & 0xF) << 12); break;

                case 0xC000:
                    irqMode = (data & 0x4) == 0x4;
                    irqEnabled = (data & 0x2) == 0x2;
                    irqEnableOnAcknowledge = (data & 0x1) == 0x1;
                    if (irqEnabled)
                        irqCounter = irqReload;
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
                    break;
                case 0xD000: irqEnabled = irqEnableOnAcknowledge; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                case 0xF000: Switch16KPRG(data & 0xF, 0x8000); break;
            }
        }
        void TickCPU()
        {
            if (irqEnabled)
            {
                if (!irqMode)// 16-bit
                {
                    irqCounter++;
                    if (irqCounter == 0xFFFF)
                    {
                        Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                        irqCounter = irqReload;
                    }
                }
                else// 8-Bit
                {
                    irqCounter = (irqCounter & 0xFF00) | ((((irqCounter & 0xFF) + 1) & 0xFF));
                    if ((irqCounter & 0xFF) == 0xFF)
                    {
                        Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                        irqCounter = (irqCounter & 0xFF00) | (irqReload & 0x00FF);
                    }
                }
            }
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(irqCounter);
            stream.Write(irqReload);
            stream.Write(irqMode);
            stream.Write(irqEnabled);
            stream.Write(irqEnableOnAcknowledge);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            irqCounter = stream.ReadInt32();
            irqReload = stream.ReadInt32();
            irqMode = stream.ReadBoolean();
            irqEnabled = stream.ReadBoolean();
            irqEnableOnAcknowledge = stream.ReadBoolean();
        }
    }
}
