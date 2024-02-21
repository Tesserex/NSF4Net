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
using MyNes.Core.APU.MMC5;
namespace MyNes.Core.Boards.Nintendo
{
    [BoardName("MMC5", 5)]
    class MMC5 : Board
    {
        public MMC5()
            : base()
        { }
        public MMC5(byte[] chr, byte[] prg, byte[] trainer, bool isVram)
            : base(chr, prg, trainer, isVram)
        { }
        MMC5ExternalSound externalSound;
        private int sramPage = 0;
        private bool sramWritable = true;

        private byte sramProtectA = 0;
        private byte sramProtectB = 0;
        private byte ExRAMmode = 0;
        private byte chrSelectMode = 3;
        private int[] chrBGPage = new int[8];
        private int[] EXChrBank = new int[0x08];
        private int chrSwitchHigh = 0;
        private byte prgSelectMode = 3;
        private int IRQStatus = 0;
        private int irq_scanline = 0;
        private int irq_line = 0;
        private int irq_clear = 0;
        private int irq_enable = 0;
        private byte Multiplier_A = 0;
        private byte Multiplier_B = 0;

        private int split_scroll = 0;
        private int split_control = 0;
        private int split_page = 0;
        private int lastAccessVRAM = 0;

        public override void SetSram(byte[] buffer)
        {
            buffer.CopyTo(sram, 0);
        }
        public override byte[] GetSaveRam()
        {
            return sram;
        }
        public override void Initialize()
        {
            base.Initialize();
            Nes.CpuMemory.Hook(0x4018, 0x5FFF, PeekPrg, PokePrg);
            Nes.CpuMemory.Hook(0x6000, 0x7FFF, PeekSram, PokeSram);
            Nes.PpuMemory.Hook(0x2000, 0x3EFF, NmtPeek, NmtPoke);

            Nes.Ppu.ScanlineTimer = TickIRQTimer;

            //sound channels
            externalSound = new MMC5ExternalSound();
            Nes.Apu.AddExternalMixer(externalSound);
        }
        public override void HardReset()
        {
            base.HardReset();
            chrBGPage = new int[8];
            EXChrBank = new int[0x08];
            base.Switch08KPRG((prg.Length - 0x2000) >> 13, 0x8000);
            base.Switch08KPRG((prg.Length - 0x2000) >> 13, 0xA000);
            base.Switch08KPRG((prg.Length - 0x2000) >> 13, 0xC000);
            base.Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
            //setup chr
            chrSelectMode = 3;
            Switch08kBGCHR(0);

            sram = new byte[0x10000];//64 kB
            sramPage = 0;
            sramWritable = true;

            sramProtectA = 0;
            sramProtectB = 0;
            ExRAMmode = 0;
            chrSelectMode = 3;

            chrSwitchHigh = 0;
            prgSelectMode = 3;
            IRQStatus = 0;
            irq_scanline = 0;
            irq_line = 0;
            irq_clear = 0;
            irq_enable = 0;
            Multiplier_A = 0;
            Multiplier_B = 0;

            split_scroll = 0;
            split_control = 0;
            split_page = 0;
            lastAccessVRAM = 0;
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address)
            {
                //Sound Channels (already mapped to 0x5000 -> 0x5014)
                case 0x5015:
                    externalSound.soundChn1.Status = (data & 0x01) != 0;
                    externalSound.soundChn2.Status = (data & 0x02) != 0;
                    break;

                //Misc Modes and Setup
                case 0x5102: sramProtectA = (byte)(data & 0x3); sramWritable = (sramProtectA == 0x2 && sramProtectB == 0x01); break;
                case 0x5103: sramProtectB = (byte)(data & 0x3); sramWritable = (sramProtectA == 0x2 && sramProtectB == 0x01); break;
                case 0x5104: ExRAMmode = (byte)(data & 0x3); break;

                #region PRG/RAM Setup
                /*
              $6000   $8000   $A000   $C000   $E000  
              +-------+-------------------------------+
   P=%00:     | $5113 |           <<$5117>>           |
              +-------+-------------------------------+
   P=%01:     | $5113 |    <$5115>    |    <$5117>    |
              +-------+---------------+-------+-------+
   P=%10:     | $5113 |    <$5115>    | $5116 | $5117 |
              +-------+---------------+-------+-------+
   P=%11:     | $5113 | $5114 | $5115 | $5116 | $5117 |
              +-------+-------+-------+-------+-------+*/
                case 0x5100: prgSelectMode = (byte)(data & 0x3); break;
                case 0x5113: sramPage = (data & 0x7) << 13; break;
                case 0x5114:
                    if (prgSelectMode == 3)
                    {
                        if ((data & 0x80) == 0x80)
                        { base.Switch08KPRG(data & 0x7F, 0x8000); }
                        else
                        { }
                    }
                    break;
                case 0x5115:
                    if (prgSelectMode == 1 || prgSelectMode == 2)
                    {
                        if ((data & 0x80) == 0x80)
                        { base.Switch16KPRG((data & 0x7E) >> 1, 0x8000); }
                        else
                        { }
                    }
                    else if (prgSelectMode == 3)
                    {
                        if ((data & 0x80) == 0x80)
                        { base.Switch08KPRG(data & 0x7F, 0xA000); }
                        else
                        { }
                    }
                    break;
                case 0x5116:
                    if (prgSelectMode == 2 || prgSelectMode == 3)
                    {
                        if ((data & 0x80) == 0x80)
                        { base.Switch08KPRG(data & 0x7F, 0xC000); }
                        else
                        { }
                    }
                    break;
                case 0x5117:
                    switch (prgSelectMode)
                    {
                        case 0: base.Switch32KPRG((data & 0x7C) >> 2); break;
                        case 1: base.Switch16KPRG((data & 0x7E) >> 1, 0xC000); break;
                        case 2:
                        case 3: base.Switch08KPRG(data, 0xE000); break;
                    }
                    break;
                #endregion
                #region CHR Setup
                case 0x5101: chrSelectMode = (byte)(data & 0x3); break;
                case 0x5130: chrSwitchHigh = data & 0x3; break;
                /* [SET A]
          $0000   $0400   $0800   $0C00   $1000   $1400   $1800   $1C00 
          +---------------------------------------------------------------+
C=%00:    |                             $5127                             |
          +---------------------------------------------------------------+
C=%01:    |             $5123             |             $5127             |
          +-------------------------------+-------------------------------+
C=%10:    |     $5121     |     $5123     |     $5125     |     $5127     |
          +---------------+---------------+---------------+---------------+
C=%11:    | $5120 | $5121 | $5122 | $5123 | $5124 | $5125 | $5126 | $5127 |
          +-------+-------+-------+-------+-------+-------+-------+-------+*/
                case 0x5120:
                    if (chrSelectMode == 3)
                        base.Switch01kCHR(data, 0x0000);
                    break;
                case 0x5121:
                    if (chrSelectMode == 2)
                        base.Switch02kCHR(data, 0x0000);
                    else if (chrSelectMode == 3)
                        base.Switch01kCHR(data, 0x0400);
                    break;
                case 0x5122:
                    if (chrSelectMode == 3)
                        base.Switch01kCHR(data, 0x0800);
                    break;
                case 0x5123:
                    if (chrSelectMode == 1)
                        base.Switch04kCHR(data, 0x0000);
                    else if (chrSelectMode == 2)
                        base.Switch02kCHR(data, 0x0800);
                    else if (chrSelectMode == 3)
                        base.Switch01kCHR(data, 0x0C00);
                    break;
                case 0x5124:
                    if (chrSelectMode == 3)
                        base.Switch01kCHR(data, 0x1000);
                    break;
                case 0x5125:
                    if (chrSelectMode == 2)
                        base.Switch02kCHR(data, 0x1000);
                    else if (chrSelectMode == 3)
                        base.Switch01kCHR(data, 0x1400);
                    break;
                case 0x5126:
                    if (chrSelectMode == 3)
                        base.Switch01kCHR(data, 0x1800);
                    break;
                case 0x5127:
                    if (chrSelectMode == 0)
                        base.Switch08kCHR(data);
                    else if (chrSelectMode == 1)
                        base.Switch04kCHR(data, 0x1000);
                    else if (chrSelectMode == 2)
                        base.Switch02kCHR(data, 0x1800);
                    else if (chrSelectMode == 3)
                        base.Switch01kCHR(data, 0x1C00);
                    break;
                /*[SET B]
          $0000   $0400   $0800   $0C00   $1000   $1400   $1800   $1C00 
          +-------------------------------+
C=%00:    |             $512B*            |
          +-------------------------------+
C=%01:    |             $512B             |
          +-------------------------------+   $1xxx always mirrors $0xxx
C=%10:    |     $5129     |     $512B     |
          +---------------+---------------+
C=%11:    | $5128 | $5129 | $512A | $512B |
          +-------+-------+-------+-------+
          *$512B in 8k mode is an 8k page number, but only the first half of the 8k page is used.
*/
                case 0x5128:
                    if (chrSelectMode == 3)
                    {
                        Switch01kBGCHR(data, 0x0000);
                        Switch01kBGCHR(data, 0x1000);
                    }
                    break;
                case 0x5129:
                    if (chrSelectMode == 2)
                    {
                        Switch02kBGCHR(data, 0x0000);
                        Switch02kBGCHR(data, 0x1000);
                    }
                    if (chrSelectMode == 3)
                    {
                        Switch01kBGCHR(data, 0x0400);
                        Switch01kBGCHR(data, 0x1400);
                    }
                    break;
                case 0x512A:
                    if (chrSelectMode == 3)
                    {
                        Switch01kBGCHR(data, 0x0800);
                        Switch01kBGCHR(data, 0x1800);
                    }
                    break;
                case 0x512B:
                    if (chrSelectMode == 0)
                    {
                        Switch08kBGCHR(data);
                    }
                    else if (chrSelectMode == 1)
                    {
                        Switch04kBGCHR(data, 0x0000);
                        Switch04kBGCHR(data, 0x1000);
                    }
                    else if (chrSelectMode == 2)
                    {
                        Switch02kBGCHR(data, 0x0800);
                        Switch02kBGCHR(data, 0x1800);
                    }
                    else if (chrSelectMode == 3)
                    {
                        Switch01kBGCHR(data, 0x0C00);
                        Switch01kBGCHR(data, 0x1C00);
                    }
                    break;
                #endregion
                //Mirroring
                case 0x5105: Nes.PpuMemory.SwitchNmt(data); break;
                //Fill-mode tile
                case 0x5106:
                    for (int i = 0; i < 0x3C0; i++)
                        Nes.PpuMemory.nmt[3][i] = data;
                    break;
                //Fill-mode color
                case 0x5107:
                    for (int i = 0x3C0; i < (0x3C0 + 0x40); i++)
                    {
                        byte value = (byte)((2 << (data & 0x03)) | (data & 0x03));
                        value |= (byte)((value & 0x0F) << 4);
                        Nes.PpuMemory.nmt[3][i] = value;
                    }
                    break;

                //Split Screen
                case 0x5200: split_control = data; break;
                case 0x5201: split_scroll = data; break;
                case 0x5202: split_page = data; break;

                //IRQ Operation
                case 0x5203: irq_line = data; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;
                case 0x5204: irq_enable = data; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false); break;

                //8 * 8 -> 16 Multiplier
                case 0x5205: Multiplier_A = data; break;
                case 0x5206: Multiplier_B = data; break;

                default:
                    if (address >= 0x5C00 && address <= 0x5FFF)
                    {
                        if (ExRAMmode == 2)
                        {
                            Nes.PpuMemory.nmt[2][(ushort)(address & 0x3FF)] = data;
                        }
                        else if (ExRAMmode != 3)
                        {
                            if ((IRQStatus & 0x40) == 0x40)
                            {
                                Nes.PpuMemory.nmt[2][(ushort)(address & 0x3FF)] = data;
                            }
                            else
                            {
                                Nes.PpuMemory.nmt[2][(ushort)(address & 0x3FF)] = 0;
                            }
                        }
                    }
                    break;
            }
        }
        protected override byte PeekPrg(int address)
        {
            byte data = 0;
            if (address < 0x5C00)
            {
                switch (address)
                {
                    case 0x5015:
                        byte rt = 0;
                        if (externalSound.soundChn1.Status)
                            rt |= 0x01;
                        if (externalSound.soundChn2.Status)
                            rt |= 0x02;
                        return rt;
                    case 0x5204:
                        data = (byte)IRQStatus;
                        IRQStatus &= ~0x80;
                        Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
                        break;
                    case 0x5205:
                        return (byte)(Multiplier_A * Multiplier_B);
                    case 0x5206:
                        return (byte)((Multiplier_A * Multiplier_B) >> 8);
                }
            }
            else if (address >= 0x5C00 && address <= 0x5FFF)
            {
                if (ExRAMmode >= 2)
                {
                    return Nes.PpuMemory.nmt[2][address & 0x3FF];
                }
            }
            else if (address >= 0x8000)
                return base.PeekPrg(address);

            return data;
        }

        protected override byte PeekChr(int address)
        {
            if (ExRAMmode != 1)
            {
                if (Nes.Ppu.IsBGFetchTime() & Nes.Ppu.IsOamSize())
                    return chr[((address & 0x03FF) | chrBGPage[address >> 10 & 0x07])];
                else
                    return base.PeekChr(address);
            }
            else//Extended Attribute mode
            {
                if (Nes.Ppu.IsBGFetchTime())//bk fetch
                {
                    int EXtilenumber = Nes.PpuMemory.nmt[2][lastAccessVRAM] & 0x3F;
                    Switch04kCHRExtra(EXtilenumber, address & 0x1000);
                    return chr[((address & 0x03FF) | EXChrBank[address >> 10 & 0x07])];
                }
                else//Sprites are not effected
                    return base.PeekChr(address);
            }
        }
        protected override void PokeChr(int address, byte data)
        {
            if (Nes.Ppu.IsBGFetchTime() & Nes.Ppu.IsOamSize())
                chr[((address & 0x03FF) | chrBGPage[address >> 10 & 0x07])] = data;
            else
                base.PokeChr(address, data);
        }
        private byte NmtPeek(int addr)
        {
            if (ExRAMmode == 1)
            {
                if ((addr & 0x03FF) <= 0x3BF)
                {
                    lastAccessVRAM = addr & 0x03FF;
                }
                else
                {
                    int paletteNo = Nes.PpuMemory.nmt[2][lastAccessVRAM] & 0xC0;
                    //Fix Attribute bits
                    int shift = ((lastAccessVRAM >> 4 & 0x04) | (lastAccessVRAM & 0x02));
                    switch (shift)
                    {
                        case 0: return (byte)(paletteNo >> 6);
                        case 2: return (byte)(paletteNo >> 4);
                        case 4: return (byte)(paletteNo >> 2);
                        case 6: return (byte)(paletteNo >> 0);
                    }
                }
            }
            return Nes.PpuMemory.nmt[Nes.PpuMemory.nmtBank[addr >> 10 & 0x03]][addr & 0x03FF];
        }
        private void NmtPoke(int addr, byte data)
        {
            if (ExRAMmode == 1)
            {
                if ((addr & 0x03FF) <= 0x3BF)
                {
                    lastAccessVRAM = addr & 0x03FF;
                }
            }
            Nes.PpuMemory.nmt[Nes.PpuMemory.nmtBank[addr >> 10 & 0x03]][addr & 0x03FF] = data;
        }
        protected override void PokeSram(int address, byte data)
        {
            if (sramWritable)
                sram[(address - 0x6000) | sramPage] = data;
        }
        protected override byte PeekSram(int address)
        {
            return sram[(address - 0x6000) | sramPage];
        }

        private void TickIRQTimer()
        {
            if (Nes.Ppu.IsRenderingOn())
            {
                if (Nes.Ppu.IsRenderingScanline())
                {
                    irq_scanline++;
                    IRQStatus |= 0x40;
                    irq_clear = 0;
                }
            }

            if (irq_scanline == irq_line)
            {
                IRQStatus |= 0x80;
            }

            if (++irq_clear > 2)
            {
                irq_scanline = 0;
                IRQStatus &= ~0x80;
                IRQStatus &= ~0x40;

                Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
            }

            if ((irq_enable & 0x80) == 0x80 && (IRQStatus & 0x80) == 0x80 && (IRQStatus & 0x40) == 0x40)
                Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);
        }
        /*New switches for bg chr*/
        /// <summary>
        /// Switch 1k chr bank to area
        /// </summary>
        /// <param name="index">The index within cart</param>
        /// <param name="where">The area where to switch. 0x0000 to 0x1C00</param>
        private void Switch01kBGCHR(int index, int where)
        {
            chrBGPage[where >> 10 & 0x07] = index << 10;
        }
        /// <summary>
        /// Switch 2k chr bank to area
        /// </summary>
        /// <param name="index">The index within cart</param>
        /// <param name="where">The area where to switch. 0x0000, 0x800, 0x1000 or 1800</param>
        private void Switch02kBGCHR(int index, int where)
        {
            int area = where >> 10 & 0x07;
            int bank = index << 11;
            for (int i = 0; i < 2; i++)
            {
                chrBGPage[area] = bank;
                area++;
                bank += 0x400;
            }
        }
        /// <summary>
        /// Switch 4k chr bank to area
        /// </summary>
        /// <param name="index">The index within cart</param>
        /// <param name="where">The area where to switch. 0x0000, or 0x1000</param>
        private void Switch04kBGCHR(int index, int where)
        {
            int area = where >> 10 & 0x07;
            int bank = index << 12;
            for (int i = 0; i < 4; i++)
            {
                chrBGPage[area] = bank;
                area++;
                bank += 0x400;
            }
        }
        /// <summary>
        /// Switch 8k chr bank to 0x0000
        /// </summary>
        /// <param name="index">The index within cart</param>
        private void Switch08kBGCHR(int index)
        {
            int bank = index << 13;
            for (int i = 0; i < 8; i++)
            {
                chrBGPage[i] = bank;
                bank += 0x400;
            }
        }
        private void Switch04kCHRExtra(int index, int where)
        {
            int area = where >> 10 & 0x07;
            int bank = index << 12;
            for (int i = 0; i < 4; i++)
            {
                EXChrBank[area] = bank;
                area++;
                bank += 0x400;
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(sram);
            stream.Write(sramPage);
            stream.Write(sramWritable);
            stream.Write(sramProtectA);
            stream.Write(sramProtectB);
            stream.Write(ExRAMmode);
            stream.Write(chrSelectMode);
            stream.Write(chrBGPage);
            stream.Write(EXChrBank);
            stream.Write(chrSwitchHigh);
            stream.Write(prgSelectMode);
            stream.Write(IRQStatus);
            stream.Write(irq_scanline);
            stream.Write(irq_line);
            stream.Write(irq_clear);
            stream.Write(irq_enable);
            stream.Write(Multiplier_A);
            stream.Write(Multiplier_B);
            stream.Write(split_scroll);
            stream.Write(split_control);
            stream.Write(split_page);
            stream.Write(lastAccessVRAM);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(sram);
            sramPage = stream.ReadInt32();
            sramWritable = stream.ReadBoolean();
            sramProtectA = stream.ReadByte();
            sramProtectB = stream.ReadByte();
            ExRAMmode = stream.ReadByte();
            chrSelectMode = stream.ReadByte();
            stream.Read(chrBGPage);
            stream.Read(EXChrBank);
            chrSwitchHigh = stream.ReadInt32();
            prgSelectMode = stream.ReadByte();
            IRQStatus = stream.ReadInt32();
            irq_scanline = stream.ReadInt32();
            irq_line = stream.ReadInt32();
            irq_clear = stream.ReadInt32();
            irq_enable = stream.ReadInt32();
            Multiplier_A = stream.ReadByte();
            Multiplier_B = stream.ReadByte();
            split_scroll = stream.ReadInt32();
            split_control = stream.ReadInt32();
            split_page = stream.ReadInt32();
            lastAccessVRAM = stream.ReadInt32();
        }
    }
}
