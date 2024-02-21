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
using MyNes.Core.APU.Namco163;
namespace MyNes.Core.Boards.Discreet
{
    [BoardName("Namcot 106", 19)]
    class Namcot106 : Board
    {
        public Namcot106() : base() { }
        public Namcot106(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }
        private Namco163ExternalSound externalSound = new Namco163ExternalSound();
        private ushort irqCounter = 0;
        private bool irqEnabled = false;
        private bool chrH = false;
        private bool chrL = false;
        private byte[] CRAM = new byte[0x8000];//32 KB

        protected bool EnableAdvancedMirroring = false;// mapper 19 only
        public override void Initialize()
        {
            // Maps prg writes to 0x8000 - 0xFFFF. Maps sram reads and writes to 0x6000 - 0x8000.
            // Then do a hard reset.
            Nes.CpuMemory.Hook(0x4018, 0x5FFF, PeekPrg, PokePrg);
            Nes.PpuMemory.Hook(0x2000, 0x3EFF, PeekNmt, PokeNmt);
            Nes.Cpu.ClockCycle = TickIRQTimer;
            base.Initialize();
            EnableAdvancedMirroring = true;
            externalSound = new Namco163ExternalSound();
            Nes.Apu.AddExternalMixer(externalSound);
        }
        public override void HardReset()
        {
            // Switch 32KB prg bank at 0x8000
            // Switch 08KB chr bank at 0x0000
            base.HardReset();
            Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
            CRAM = new byte[0x8000];//32 KB
            //we must set nametables
            if (EnableAdvancedMirroring)
            {
                switch (Nes.RomInfo.Mirroring)
                {
                    case Types.Mirroring.ModeVert:
                        Nes.PpuMemory.nmtBank[0] = 0xE0;
                        Nes.PpuMemory.nmtBank[1] = 0xE1;
                        Nes.PpuMemory.nmtBank[2] = 0xE0;
                        Nes.PpuMemory.nmtBank[3] = 0xE1;
                        break;
                    case Types.Mirroring.ModeHorz:
                        Nes.PpuMemory.nmtBank[0] = 0xE0;
                        Nes.PpuMemory.nmtBank[1] = 0xE0;
                        Nes.PpuMemory.nmtBank[2] = 0xE1;
                        Nes.PpuMemory.nmtBank[3] = 0xE1;
                        break;
                }
            }
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xF800)
            {
                case 0x4800: externalSound.Poke4800(address, data); break;
                /*IRQs*/
                case 0x5000: irqCounter = (ushort)((irqCounter & 0x7F00) | (data << 0)); Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                case 0x5800: irqCounter = (ushort)((irqCounter & 0x00FF) | (data << 8)); irqEnabled = (data & 0x80) == 0x80; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                /*chr*/
                case 0x8000:
                    if ((data < 0xE0) || chrL)
                        Switch01kCHR(data, 0x0000);
                    else
                        Switch01kCHR((data & 0x1F) + (chr.Length >> 10), 0x0000);
                    break;
                case 0x8800:
                    if ((data < 0xE0) || chrL)
                        Switch01kCHR(data, 0x0400);
                    else
                        Switch01kCHR((data & 0x1F) + (chr.Length >> 10), 0x0400);
                    break;
                case 0x9000:
                    if ((data < 0xE0) || chrL)
                        Switch01kCHR(data, 0x0800);
                    else
                        Switch01kCHR((data & 0x1F) + (chr.Length >> 10), 0x0800);
                    break;
                case 0x9800:
                    if ((data < 0xE0) || chrL)
                        Switch01kCHR(data, 0x0C00);
                    else
                        Switch01kCHR((data & 0x1F) + (chr.Length >> 10), 0x0C00);
                    break;
                case 0xA000:
                    if ((data < 0xE0) || chrH)
                        Switch01kCHR(data, 0x1000);
                    else
                        Switch01kCHR((data & 0x1F) + (chr.Length >> 10), 0x1000);
                    break;
                case 0xA800:
                    if ((data < 0xE0) || chrH)
                        Switch01kCHR(data, 0x1400);
                    else
                        Switch01kCHR((data & 0x1F) + (chr.Length >> 10), 0x1400);
                    break;
                case 0xB000:
                    if ((data < 0xE0) || chrH)
                        Switch01kCHR(data, 0x1800);
                    else
                        Switch01kCHR((data & 0x1F) + (chr.Length >> 10), 0x1800);
                    break;
                case 0xB800:
                    if ((data < 0xE0) || chrH)
                        Switch01kCHR(data, 0x1C00);
                    else
                        Switch01kCHR((data & 0x1F) + (chr.Length >> 10), 0x1C00);
                    break;
                /*Mirroring*/
                case 0xC000: if (EnableAdvancedMirroring) Nes.PpuMemory.nmtBank[0] = data; break;
                case 0xC800: if (EnableAdvancedMirroring) Nes.PpuMemory.nmtBank[1] = data; break;
                case 0xD000: if (EnableAdvancedMirroring) Nes.PpuMemory.nmtBank[2] = data; break;
                case 0xD800: if (EnableAdvancedMirroring) Nes.PpuMemory.nmtBank[3] = data; break;
                /*prg*/
                case 0xE000: Switch08KPRG(data & 0x3F, 0x8000); break;
                case 0xE800:
                    Switch08KPRG(data & 0x3F, 0xA000);
                    chrL = (data & 0x40) != 0;
                    chrH = (data & 0x80) != 0;
                    break;
                case 0xF000: Switch08KPRG(data & 0x3F, 0xC000); break;
                /*Sound control reg*/
                case 0xF800: externalSound.PokeF800(address, data); break;
            }
        }
        protected override byte PeekPrg(int address)
        {
            switch (address & 0xF800)
            {
                case 0x4800: return externalSound.Peek4800(address);
                case 0x5000: return (byte)(irqCounter & 0x00FF);
                case 0x5800: return (byte)((irqCounter & 0x7F00) >> 8);
            }
            return base.PeekPrg(address);
        }
        protected override byte PeekChr(int address)
        {
            int taddress = this.DecodeChrAddress(address);
            if (taddress < chr.Length)
                return chr[taddress];
            else
            { return CRAM[(taddress - chr.Length)]; }
        }
        protected override void PokeChr(int address, byte data)
        {
            int taddress = this.DecodeChrAddress(address);
            if (taddress >= chr.Length)
                CRAM[(taddress - chr.Length)] = data;
        }
        private void PokeNmt(int addr, byte data)
        {
            if (Nes.PpuMemory.nmtBank[(addr >> 10) & 0x03] >= 0xE0)
                Nes.PpuMemory.nmt[(Nes.PpuMemory.nmtBank[(addr >> 10) & 0x03] - 0xE0) & 1][addr & 0x03FF] = data;
        }
        private byte PeekNmt(int addr)
        {
            if (Nes.PpuMemory.nmtBank[(addr >> 10) & 0x03] < 0xE0)
                return chr[(Nes.PpuMemory.nmtBank[(addr >> 10) & 0x03] << 10) | (addr & 0x03FF)];
            else
                return Nes.PpuMemory.nmt[(Nes.PpuMemory.nmtBank[(addr >> 10) & 0x03] - 0xE0) & 1][addr & 0x03FF];
        }
        private void TickIRQTimer()
        {
            if (irqEnabled)
            {
                if ((irqCounter - 0x8000 < 0x7FFF) && (++irqCounter == 0xFFFF))
                {
                    irqEnabled = false;
                    irqCounter = 0;
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
                }
                //else
                //    irqCounter++;
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(irqCounter);
            stream.Write(irqEnabled, chrH, chrL);
            stream.Write(CRAM);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            irqCounter = stream.ReadUshort();
            bool[] flags = stream.ReadBooleans();
            irqEnabled = flags[0];
            chrH = flags[0];
            chrL = flags[0];
            stream.Read(CRAM);
        }
    }
}
