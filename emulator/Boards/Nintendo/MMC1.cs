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
/*Written by Ala Ibrahim Hadid and Adam Becker*/
using MyNes.Core.Types;
namespace MyNes.Core.Boards.Nintendo
{
    [BoardName("MMC1", 1)]
    class MMC1 : Board
    {
        public MMC1()
            : base()
        { }
        public MMC1(byte[] chr, byte[] prg, byte[] trainer, bool isVram)
            : base(chr, prg, trainer, isVram)
        { }
        public override void Initialize()
        {
            base.Initialize();
            Nes.Cpu.ClockCycle = ClockCpu;
            timer = 0;
            boardType = MMC1BoardType.Normal;
            // let's checkout the rom info to see if this rom has info in data base
            if (Nes.RomInfo.DatabaseGameInfo.Game_Name != "" && Nes.RomInfo.DatabaseGameInfo.Board_Type != null)
            {
                if (Nes.RomInfo.DatabaseGameInfo.Board_Type.ToLower().Contains("surom"))
                {
                    boardType = MMC1BoardType.SUROM;
                    Console.WriteLine("Board Type: SUROM", DebugCode.Warning);
                }
                if (Nes.RomInfo.DatabaseGameInfo.Board_Type.ToLower().Contains("sorom"))
                {
                    boardType = MMC1BoardType.SOROM;
                    sram = new byte[1024 * 16];
                    Console.WriteLine("Board Type: SOROM", DebugCode.Warning);
                }
                if (Nes.RomInfo.DatabaseGameInfo.Board_Type.ToLower().Contains("sxrom"))
                {
                    boardType = MMC1BoardType.SXROM;
                    sram = new byte[1024 * 32];
                    Console.WriteLine("Board Type: SXROM", DebugCode.Warning);
                }
            }
            else if (Nes.RomInfo.SHA1.ToUpper() == "340F507CFC3F3827EE0B7269814E08D634B807F4")// Best Play Pro Yakyuu Special?
            {
                boardType = MMC1BoardType.SXROM;
                sram = new byte[1024 * 32];
                Console.WriteLine("Board Type: SXROM", DebugCode.Warning);
            }
            else// then to normal switch depending on size
            {
                if (Nes.RomInfo.PRGcount >= 32)
                { boardType = MMC1BoardType.SUROM; Console.WriteLine("Board Type: SUROM", DebugCode.Warning); }
            }
        }
        public override void HardReset()
        {
            base.HardReset();

            reg = new byte[4];
            reg[0] = 0x0C;
            reg[1] = reg[2] = reg[3] = 0;

            //setup prg
            switch (boardType)
            {
                case MMC1BoardType.SOROM:
                case MMC1BoardType.Normal:
                    base.Switch16KPRG(0, 0x8000);
                    base.Switch16KPRG((prg.Length - 0x4000) >> 14, 0xC000);
                    break;
                case MMC1BoardType.SUROM:
                case MMC1BoardType.SXROM:
                    base.Switch16KPRG(0, 0x8000);
                    base.Switch16KPRG(0xF, 0xC000);
                    break;
            }

            wramON = true;
            shift = 0;
            buffer = 0;
        }
        private MMC1BoardType boardType = MMC1BoardType.Normal;
        private byte[] reg = new byte[4];
        private byte shift = 0;
        private byte buffer = 0;
        private bool wramON = true;//enable sram, not sure about this but some docs mansion it
        private int timer = 0;
        private int sramPage = 0;

        protected override void PokePrg(int address, byte data)
        {
            //Too close writes ignored
            if (timer < 2)
            {
                return;
            }
            timer = 0;
            //Temporary reg port ($8000-FFFF):
            //[r... ...d]
            //r = reset flag
            //d = data bit

            //r is set
            if ((data & 0x80) == 0x80)
            {
                reg[0] |= 0x0C;//bits 2,3 of reg $8000 are set (16k PRG mode, $8000 swappable)
                shift = buffer = 0;//hidden temporary reg is reset
                return;
            }
            //d is set
            if ((data & 0x01) == 0x01)
                buffer |= (byte)(1 << shift);//'d' proceeds as the next bit written in the 5-bit sequence
            if (++shift < 5)
                return;
            // If this completes the 5-bit sequence:
            // - temporary reg is copied to actual internal reg (which reg depends on the last address written to)
            address = (ushort)((address & 0x7FFF) >> 13);
            reg[address] = buffer;

            // - temporary reg is reset (so that next write is the "first" write)
            shift = buffer = 0;
            switch (boardType)
            {
                case MMC1BoardType.Normal: PokeNormal(address, data); break;
                case MMC1BoardType.SOROM: PokeSOROM(address, data); break;
                case MMC1BoardType.SUROM: PokeSUROM(address, data); break;
                case MMC1BoardType.SXROM: PokeSXROM(address, data); break;
            }
        }
        private void PokeNormal(int address, byte data)
        {
            switch (address)
            {
                case 0:
                    switch (reg[0] & 3)
                    {
                        case 0: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScA); break;
                        case 1: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScB); break;
                        case 2: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeVert); break;
                        case 3: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeHorz); break;
                    }
                    break;

                case 1:
                    if ((reg[0] & 0x10) != 0)
                    {
                        base.Switch04kCHR(reg[1], 0);
                        base.Switch04kCHR(reg[2], 0x1000);
                    }
                    else
                    {
                        base.Switch08kCHR(reg[1] >> 1);
                    }
                    break;
                case 2:
                    if ((reg[0] & 0x10) != 0)
                    {
                        base.Switch04kCHR(reg[1], 0);
                        base.Switch04kCHR(reg[2], 0x1000);
                    }
                    else
                    {
                        base.Switch08kCHR(reg[1] >> 1);
                    }
                    break;

                case 3:
                    wramON = (reg[3] & 0x10) == 0;

                    if ((reg[0] & 0x08) == 0)
                    {
                        base.Switch32KPRG(reg[3] >> 1);
                    }
                    else
                    {
                        if ((reg[0] & 0x04) != 0)
                        {
                            base.Switch16KPRG(reg[3], 0x8000);
                            base.Switch16KPRG((prg.Length - 0x4000) >> 14, 0xC000);//last bank
                        }
                        else
                        {
                            base.Switch16KPRG(0, 0x8000);
                            base.Switch16KPRG(reg[3], 0xC000);
                        }
                    }
                    break;
            }
        }
        private void PokeSOROM(int address, byte data)
        {
            switch (address)
            {
                case 0:
                    switch (reg[0] & 3)
                    {
                        case 0: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScA); break;
                        case 1: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScB); break;
                        case 2: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeVert); break;
                        case 3: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeHorz); break;
                    }
                    break;

                case 1:
                    if ((reg[0] & 0x10) != 0)
                    {
                        base.Switch04kCHR(reg[1], 0);
                        base.Switch04kCHR(reg[2], 0x1000);
                    }
                    else
                    {
                        base.Switch08kCHR(reg[1] >> 1);
                    }
                    sramPage = (reg[1] & 0x10) << 9;
                    break;
                case 2:
                    if ((reg[0] & 0x10) != 0)
                    {
                        base.Switch04kCHR(reg[1], 0);
                        base.Switch04kCHR(reg[2], 0x1000);
                        sramPage = (reg[2] & 0x10) << 9;
                    }
                    else
                    {
                        base.Switch08kCHR(reg[1] >> 1);
                    }  
                    break;

                case 3:
                    wramON = (reg[3] & 0x10) == 0;

                    if ((reg[0] & 0x08) == 0)
                    {
                        base.Switch32KPRG(reg[3] >> 1);
                    }
                    else
                    {
                        if ((reg[0] & 0x04) != 0)
                        {
                            base.Switch16KPRG(reg[3], 0x8000);
                            base.Switch16KPRG((prg.Length - 0x4000) >> 14, 0xC000);//last bank
                        }
                        else
                        {
                            base.Switch16KPRG(0, 0x8000);
                            base.Switch16KPRG(reg[3], 0xC000);
                        }
                    }
                    break;
            }
        }
        private void PokeSUROM(int address, byte data)
        {
            if (address == 0)
            {
                switch (reg[0] & 3)
                {
                    case 0: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScA); break;
                    case 1: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScB); break;
                    case 2: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeVert); break;
                    case 3: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeHorz); break;
                }
            }

            if ((reg[0] & 0x10) == 0x10)
            {
                base.Switch04kCHR(reg[1], 0);
                base.Switch04kCHR(reg[2], 0x1000);
            }
            else
            {
                base.Switch08kCHR(reg[1] >> 1);
            }
            int BASE = reg[1] & 0x10;
            wramON = (reg[3] & 0x10) == 0;
            if ((reg[0] & 0x08) == 0)
            {
                base.Switch32KPRG((reg[3] & (0xF + BASE)) >> 1);
            }
            else
            {
                if ((reg[0] & 0x04) == 0x04)
                {
                    base.Switch16KPRG(BASE + (reg[3] & 0x0F), 0x8000);
                    base.Switch16KPRG(BASE + 0xF, 0xC000);
                }
                else
                {

                    base.Switch16KPRG(BASE, 0x8000);
                    base.Switch16KPRG(BASE + (reg[3] & 0x0F), 0xC000);
                }
            }
        }
        private void PokeSXROM(int address, byte data)
        {
            if (address == 0)
            {
                switch (reg[0] & 3)
                {
                    case 0: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScA); break;
                    case 1: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScB); break;
                    case 2: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeVert); break;
                    case 3: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeHorz); break;
                }
            }

            if ((reg[0] & 0x10) == 0x10)
            {
                base.Switch04kCHR(reg[1], 0);
                base.Switch04kCHR(reg[2], 0x1000);
                sramPage = (reg[1] & 0xC) << 11;
            }
            else
            {
                base.Switch08kCHR(reg[1] >> 1);
            }
            int BASE = reg[1] & 0x10;
            wramON = (reg[3] & 0x10) == 0;
            if ((reg[0] & 0x08) == 0)
            {
                base.Switch32KPRG((reg[3] & (0xF + BASE)) >> 1);
            }
            else
            {
                if ((reg[0] & 0x04) == 0x04)
                {
                    base.Switch16KPRG(BASE + (reg[3] & 0x0F), 0x8000);
                    base.Switch16KPRG(BASE + 0xF, 0xC000);
                }
                else
                {

                    base.Switch16KPRG(BASE, 0x8000);
                    base.Switch16KPRG(BASE + (reg[3] & 0x0F), 0xC000);
                }
            }
           
        }
        protected override void PokeSram(int address, byte data)
        {
            if (wramON)
                sram[sramPage | (address & 0x1FFF)]=data;
        }
        protected override byte PeekSram(int address)
        {
            if (wramON)
                return sram[sramPage | (address & 0x1FFF)];
            return 0;
        }
        public override byte[] GetSaveRam()
        {
            switch (boardType)
            { 
                case MMC1BoardType.SOROM:// only the second page can be battery-backed
                    byte[] newSram = new byte[0x2000];
                    for (int i = 0; i < 0x2000; i++)
                    {
                        newSram[i] = sram[i + 0x2000];
                    }
                    return newSram;
            }
            return base.GetSaveRam();
        }
        public override void SetSram(byte[] buffer)
        {
            if (boardType == MMC1BoardType.SOROM)
            {
                buffer.CopyTo(sram, 0x2000);
            }
            else
                base.SetSram(buffer);
        }

        private void ClockCpu()
        {
            timer++;
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(reg);
            stream.Write(sram);
            stream.Write(shift);
            stream.Write(buffer);
            stream.Write(wramON);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(reg);
            stream.Read(sram);
            shift = stream.ReadByte();
            buffer = stream.ReadByte();
            wramON = stream.ReadBoolean();
        }

        private enum MMC1BoardType
        {
            Normal, SUROM, SOROM, SXROM
        }
    }
}
