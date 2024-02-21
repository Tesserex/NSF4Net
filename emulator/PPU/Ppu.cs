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
using System;
using MyNes.Core.CPU;

namespace MyNes.Core.PPU
{
    // Emulates the RP2C02/RP2C07 graphics synthesizer
    public class Ppu : ProcessorBase
    {
        private static int[] reverseLookup =
        {
            0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0,
            0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8,
            0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4, 0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4,
            0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC,
            0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2, 0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2,
            0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA,
            0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6,
            0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE, 0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE,
            0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1,
            0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9,
            0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5,
            0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD,
            0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3, 0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3,
            0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB,
            0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7,
            0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF,
        };
        private int[] paletteIndexes =
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
            0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F,
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
            0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F,
            0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F,
            0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F,
            0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F,
        };
        private Fetch fetch = new Fetch();
        private Fetch sprfetch = new Fetch();
        private Scroll scroll = new Scroll();
        private Sprite[] buffer = new Sprite[8];
        private Unit bkg = new Unit(272);
        private Unit spr = new Unit(256);
        private Action OamFetch;
        private Action OamPhase;
        private bool UseOddFrame;
        private byte chr;
        private int clipping;
        private int emphasis;
        private int[] colors;
        public int[][] screen;
        private bool oddSwap;
        private bool spr0Hit;
        private bool sprOverflow;
        //nmi and vbl
        private bool nmi_output;
        private bool nmi_ocurred;
        private bool suppressVbl;
        //Timing
        public int vbl_vclock_Start = 241;
        public int vbl_vclock_End = 261;
        private int frameEnd = 262;
        private int hclock;
        public int vclock;
        public Action<int> AddressLineUpdating;
        /// <summary>
        /// Tick each ppu cycle
        /// </summary>
        public Action CycleTimer;
        /// <summary>
        /// Tick each scanline
        /// </summary>
        public Action ScanlineTimer;
        //oam
        private byte oam_address;
        private byte[] oam = new byte[256];
        private byte oamData = 0;
        private byte oamCount = 0;
        private byte oamSlot = 0;

        public Ppu(TimingInfo.System system)
            : base(system)
        {
            timing.period = system.Master;
            timing.single = system.Gpu;
        }

        /*Background fetches*/
        private void FetchName_0()
        {
            fetch.addr = 0x2000 | (scroll.addr & 0xFFF);
            AddressLineUpdating(fetch.addr);
        }
        private void FetchName_1()
        {
            fetch.name = Nes.PpuMemory[fetch.addr];
        }
        private void FetchAttr_0()
        {
            fetch.addr = 0x23C0 | (scroll.addr & 0xC00) | (scroll.addr >> 4 & 0x38) | (scroll.addr >> 2 & 0x7);
            AddressLineUpdating(fetch.addr);
        }
        private void FetchAttr_1()
        {
            fetch.attr = Nes.PpuMemory[fetch.addr] >> ((scroll.addr >> 4 & 0x04) | (scroll.addr & 0x02));
        }
        private void FetchBit0_0()
        {
            fetch.addr = bkg.address | (fetch.name << 4) | 0 | (scroll.addr >> 12 & 7);
            AddressLineUpdating(fetch.addr);
        }
        private void FetchBit0_1()
        {
            fetch.bit0 = Nes.PpuMemory[fetch.addr];
        }
        private void FetchBit1_0()
        {
            fetch.addr = bkg.address | (fetch.name << 4) | 8 | (scroll.addr >> 12 & 7);
            AddressLineUpdating(fetch.addr);
        }
        private void FetchBit1_1()
        {
            fetch.bit1 = Nes.PpuMemory[fetch.addr];
        }
        /*Sprites fetches*/
        private void SprFetchBit0_0()
        {
            int index = hclock >> 3 & 7;
            int comparator = (vclock - buffer[index].y) ^ ((buffer[index].attr & 0x80) != 0 ? 0x0F : 0x00);
            if (spr.rasters == 0x10)
            {
                sprfetch.addr = (buffer[index].name << 0x0C & 0x1000) | (buffer[index].name << 0x04 & 0x0FE0) |
                             (comparator << 0x01 & 0x0010) | (comparator & 0x0007);
            }
            else
            {
                sprfetch.addr = spr.address | (buffer[index].name << 0x04) | (comparator & 0x0007);
            }
            AddressLineUpdating(sprfetch.addr);
        }
        private void SprFetchBit0_1()
        {
            sprfetch.bit0 = Nes.PpuMemory[sprfetch.addr];
            if ((buffer[hclock >> 3 & 7].attr & 0x40) != 0)
                sprfetch.bit0 = reverseLookup[sprfetch.bit0];
        }
        private void SprFetchBit1_0()
        {
            sprfetch.addr = sprfetch.addr | 0x08;
            AddressLineUpdating(sprfetch.addr);
        }
        private void SprFetchBit1_1()
        {
            sprfetch.bit1 = Nes.PpuMemory[sprfetch.addr];

            if ((buffer[hclock >> 3 & 7].attr & 0x40) != 0)
                sprfetch.bit1 = reverseLookup[sprfetch.bit1];

            sprfetch.attr = buffer[hclock >> 3 & 7].attr;
        }

        #region Sprites Evaluation
        private void EvaluateFetch()
        {
            oamData = oam[oam_address];
        }
        private void EvaluateReset()
        {
            oamData = 0xFF;
        }

        private void EvaluatePhase0()
        {
            if (hclock <= 64)
            {
                switch (hclock >> 1 & 0x03)
                {
                    case 0: buffer[hclock >> 3].y = 0xFF; break;
                    case 1: buffer[hclock >> 3].name = 0xFF; break;
                    case 2: buffer[hclock >> 3].attr = 0xFF; break;
                    case 3: buffer[hclock >> 3].x = 0xFF; buffer[hclock >> 3].zero = false; break;
                }
            }
        }
        private void EvaluatePhase1()
        {
            if (vclock == vbl_vclock_End)
                return;
            oamCount++;
            int comparator = (vclock - oamData) & int.MaxValue;

            if (comparator >= spr.rasters)
            {
                if (oamCount != 64)
                {
                    oam_address = (byte)(oamCount != 2 ? oam_address + 4 : 8);
                }
                else
                {
                    oam_address = 0;
                    OamPhase = EvaluatePhase9;
                }
            }
            else
            {
                oam_address++;
                OamPhase = EvaluatePhase2;
                buffer[oamSlot].y = oamData;
                buffer[oamSlot].zero = oamCount == 1;
            }
        }
        private void EvaluatePhase2()
        {
            oam_address++;
            OamPhase = EvaluatePhase3;
            buffer[oamSlot].name = oamData;
        }
        private void EvaluatePhase3()
        {
            oam_address++;
            OamPhase = EvaluatePhase4;
            buffer[oamSlot].attr = oamData;
        }
        private void EvaluatePhase4()
        {
            buffer[oamSlot].x = oamData;
            oamSlot++;

            if (oamCount != 64)
            {
                OamPhase = (oamSlot != 8 ? new Action(EvaluatePhase1) : new Action(EvaluatePhase5));
                if (oamCount != 2)
                {
                    oam_address++;
                }
                else
                {
                    oam_address = 8;
                }
            }
            else
            {
                oam_address = 0;
                OamPhase = EvaluatePhase9;
            }
        }
        private void EvaluatePhase5()
        {
            if (vclock == vbl_vclock_End)
                return;
            int comparator = (vclock - oamData) & int.MaxValue;

            if (comparator >= spr.rasters)
            {
                oam_address = (byte)(((oam_address + 4) & 0xFC) +
                    ((oam_address + 1) & 0x03));

                if (oam_address <= 5)
                {
                    OamPhase = EvaluatePhase9;
                    oam_address &= 0xFC;
                }
            }
            else
            {
                OamPhase = EvaluatePhase6;
                oam_address += (0x01) & 0xFF;
                sprOverflow = true;
            }
        }
        private void EvaluatePhase6()
        {
            OamPhase = EvaluatePhase7;
            oam_address += (0x01);
        }
        private void EvaluatePhase7()
        {
            OamPhase = EvaluatePhase8;
            oam_address += (0x01);
        }
        private void EvaluatePhase8()
        {
            OamPhase = EvaluatePhase9;
            oam_address += (0x01);

            if ((oam_address & 0x03) == 0x03)
                oam_address += (0x01);

            oam_address &= 0xFC;
        }
        private void EvaluatePhase9()
        {
            oam_address += 0x4;
        }

        private void EvaluationBegin()
        {
            this.OamFetch = this.EvaluateFetch;
            this.OamPhase = this.EvaluatePhase1;
            this.oamSlot = 0;
            this.oamCount = 0;
        }
        private void EvaluationReset()
        {
            this.OamFetch = this.EvaluateReset;
            this.OamPhase = this.EvaluatePhase0;
            this.oamSlot = 0;
            this.oam_address = 0;
            this.oamCount = 0;

            spr.pixels = new int[256];
        }
        #endregion

        private byte Peek____(int address) { return 0; }
        private byte Peek2002(int address)
        {
            //Read 1 cycle before vblank should suppress setting flag
            if (vclock == vbl_vclock_Start)
            {
                if (hclock == 0)
                {
                    suppressVbl = true;
                    Nes.Cpu.Interrupt(Cpu.IsrType.Ppu, false);
                }
                else if (hclock < 3)
                {
                    Nes.Cpu.Interrupt(Cpu.IsrType.Ppu, false);
                }
            }

            byte data = 0;

            if (nmi_ocurred)
                data |= 0x80;

            if (spr0Hit)
                data |= 0x40;

            if (sprOverflow)
                data |= 0x20;

            nmi_ocurred = false;
            scroll.swap = false;

            return data;
        }
        private byte Peek2004(int address) { return oam[oam_address]; }
        private byte Peek2007(int address)
        {
            byte tmp;

            if ((scroll.addr & 0x3F00) == 0x3F00)
            {
                tmp = Nes.PpuMemory[scroll.addr];
                chr = Nes.PpuMemory[scroll.addr & 0x2FFF];
            }
            else
            {
                tmp = chr;
                chr = Nes.PpuMemory[scroll.addr];
            }

            scroll.addr = (scroll.addr + scroll.step) & 0x7FFF;
            AddressLineUpdating(scroll.addr);
            return tmp;
        }
        private void Poke____(int address, byte data) { }
        private void Poke2000(int address, byte data)
        {
            scroll.temp = (scroll.temp & ~0x0C00) | (data << 10 & 0x0C00);
            scroll.step = (data & 0x04) != 0 ? 0x0020 : 0x0001;
            spr.address = (data & 0x08) != 0 ? 0x1000 : 0x0000;
            bkg.address = (data & 0x10) != 0 ? 0x1000 : 0x0000;
            spr.rasters = (data & 0x20) != 0 ? 0x0010 : 0x0008;

            bool oldNmi = nmi_output;
            nmi_output = (data & 0x80) != 0;

            if (vclock == vbl_vclock_Start & hclock < 3)
                Nes.Cpu.Interrupt(Cpu.IsrType.Ppu, nmi_output & nmi_ocurred);

            if (!oldNmi & nmi_output & nmi_ocurred)
                Nes.Cpu.Interrupt(Cpu.IsrType.Ppu, true);
        }
        private void Poke2001(int address, byte data)
        {
            clipping = (data & 0x01) != 0 ? 0x30 : 0x3F;
            emphasis = (data & 0xE0) << 1;

            bkg.clipped = (data & 0x02) == 0;
            spr.clipped = (data & 0x04) == 0;
            bkg.enabled = (data & 0x08) != 0;
            spr.enabled = (data & 0x10) != 0;
        }
        private void Poke2003(int address, byte data)
        {
            oam_address = data;
        }
        private void Poke2004(int address, byte data)
        {
            if ((oam_address & 0x03) == 0x02)
            {
                oam[oam_address++] = (byte)(data & 0xE3);
            }
            else
            {
                oam[oam_address++] = data;
            }
        }
        private void Poke2005(int address, byte data)
        {
            if (scroll.swap = !scroll.swap)
            {
                scroll.temp = (scroll.temp & ~0x001F) | (data >> 3 & 0x001F);
                scroll.fine = (data & 0x07);
            }
            else
            {
                scroll.temp = (scroll.temp & ~0x73E0) | (data << 2 & 0x03E0) | (data << 12 & 0x7000);
            }
        }
        private void Poke2006(int address, byte data)
        {
            if (scroll.swap = !scroll.swap)
            {
                scroll.temp = (scroll.temp & ~0xFF00) | (data << 8 & 0x3F00);
            }
            else
            {
                scroll.temp = (scroll.temp & ~0x00FF) | (data << 0 & 0x00FF);
                scroll.addr = (scroll.temp);
                AddressLineUpdating(scroll.addr);
            }
        }
        private void Poke2007(int address, byte data)
        {
            Nes.PpuMemory[scroll.addr] = data;

            scroll.addr = (scroll.addr + scroll.step) & 0x7FFF;
            AddressLineUpdating(scroll.addr);
        }
        private void Poke4014(int address, byte data)
        {
            Nes.Cpu.dma.OamTransfer(address, data);
        }

        private void RenderPixel()
        {
            //Render is enabled here, draw background color at 0x3F00
            byte bkgc = (byte)(Nes.PpuMemory[0x3F00]);
            screen[vclock][hclock] = colors[paletteIndexes[bkgc & (clipping | emphasis)]];

            var bkgPixel = 0x3F00 | bkg.GetPixel(hclock, scroll.fine);
            var sprPixel = 0x3F10 | spr.GetPixel(hclock);
            var pixel = 0;

            //Priority *******************************
            if ((sprPixel & 0x8000) != 0)
                pixel = sprPixel;
            else
                pixel = bkgPixel;
            //****************************************

            // Transparency **************************
            if ((bkgPixel & 0x03) == 0)
            {
                pixel = sprPixel;
                goto render;
            }

            if ((sprPixel & 0x03) == 0)
            {
                pixel = bkgPixel;
                goto render;
            }
            //****************************************

            //Sprite 0 Hit
            if ((sprPixel & 0x4000) != 0 & hclock < 255)
                spr0Hit = true;

        render:
            if ((pixel & 0x03) != 0)
                screen[vclock][hclock] = colors[paletteIndexes[Nes.PpuMemory[pixel] & (clipping | emphasis)]];
        }
        private void SynthesizeBkgPixels()
        {
            var pos = (hclock + 9) % 336;

            for (int i = 0; i < 8 && pos < 272; i++, pos++, fetch.bit0 <<= 1, fetch.bit1 <<= 1)
                bkg.pixels[pos] = (fetch.attr << 2 & 12) | (fetch.bit0 >> 7 & 1) | (fetch.bit1 >> 6 & 2);
        }
        private void SynthesizeSprPixels()
        {
            int index = hclock >> 3 & 7;

            if (buffer[index].x == 255)
                return;

            int pos = buffer[index].x;
            int object0 = buffer[index].zero ? 0x4000 : 0x0000;
            int infront = ((buffer[index].attr & 0x20) == 0) ? 0x8000 : 0x0000;

            for (int i = 0; i < 8 && pos < 256; i++, pos++, sprfetch.bit0 <<= 1, sprfetch.bit1 <<= 1)
            {
                if (pos > 255)
                    return;

                int pixel = (sprfetch.attr << 2 & 12) | (sprfetch.bit0 >> 7 & 1) | (sprfetch.bit1 >> 6 & 2) | object0 | infront;
                if ((spr.pixels[pos] & 0x03) == 0 && (pixel & 0x03) != 0)
                    spr.pixels[pos] = pixel;
            }
        }

        public override void Initialize()
        {
            Console.WriteLine("Initializing PPU...");

            for (int i = 0x0000; i < 0x2000; i += 8)
            {
                Nes.CpuMemory.Hook(0x2000 + i, Peek____, Poke2000);
                Nes.CpuMemory.Hook(0x2001 + i, Peek____, Poke2001);
                Nes.CpuMemory.Hook(0x2002 + i, Peek2002, Poke____);
                Nes.CpuMemory.Hook(0x2003 + i, Peek____, Poke2003);
                Nes.CpuMemory.Hook(0x2004 + i, Peek2004, Poke2004);
                Nes.CpuMemory.Hook(0x2005 + i, Peek____, Poke2005);
                Nes.CpuMemory.Hook(0x2006 + i, Peek____, Poke2006);
                Nes.CpuMemory.Hook(0x2007 + i, Peek2007, Poke2007);
            }
            Nes.CpuMemory.Hook(0x4014, Poke4014);
            HardReset();
            Console.UpdateLine("Initializing PPU... Success!", DebugCode.Good);
        }
        public override void HardReset()
        {
            UseOddFrame = system.Master == TimingInfo.NTSC.Master;
            if (system.Master == TimingInfo.NTSC.Master)
            {
                vbl_vclock_Start = 241;//20 scanlines for VBL
                vbl_vclock_End = 261;
                frameEnd = 262;
            }
            else if (system.Master == TimingInfo.PALB.Master)
            {
                vbl_vclock_Start = 241;//70 scanlines for VBL
                vbl_vclock_End = 311;
                frameEnd = 312;
            }
            else if (system.Master == TimingInfo.Dendy.Master)
            {
                vbl_vclock_Start = 291;//51 dummy scanlines, 20 VBL's
                vbl_vclock_End = 311;
                frameEnd = 312;
            }
            screen = new int[240][];
            for (int i = 0; i < 240; i++)
                screen[i] = new int[256];

            if (AddressLineUpdating == null)
                AddressLineUpdating = new Action<int>(AddressLineupdating);
            if (CycleTimer == null)
                CycleTimer = TickCycleTimer;
            if (ScanlineTimer == null)
                ScanlineTimer = TickScanlineTimer;
            EvaluationReset();

            fetch = new Fetch();
            sprfetch = new Fetch();
            scroll = new Scroll();
            buffer = new Sprite[8];
            bkg = new Unit(272);
            spr = new Unit(256);

            chr = 0;
            clipping = 0xF3;
            emphasis = 0;
            oddSwap = false;
            spr0Hit = false;
            sprOverflow = false;
            //nmi and vbl
            nmi_output = false;
            nmi_ocurred = false;
            suppressVbl = false;
            hclock = 0;
            vclock = 0;

            //oam
            oam_address = 0;
            oam = new byte[256];
            oamData = 0;
            oamCount = 0;
            oamSlot = 0;
        }
        public override void Shutdown() { }
        public override void Update()
        {
            CycleTimer();
            if (vclock < 240 || vclock == vbl_vclock_End)
            {
                #region Rendering is on
                if (IsRenderingOn())
                {
                    if (hclock < 256)
                    {
                        #region Bkg Fetches

                        switch (hclock & 7)
                        {
                            case 0: FetchName_0(); break;
                            case 1: FetchName_1(); break;
                            case 2: FetchAttr_0(); break;
                            case 3: FetchAttr_1();

                                if (hclock == 251)
                                    scroll.ClockY();
                                else
                                    scroll.ClockX();
                                break;
                            case 4: FetchBit0_0(); break;
                            case 5: FetchBit0_1(); break;
                            case 6: FetchBit1_0(); break;
                            case 7: FetchBit1_1(); SynthesizeBkgPixels(); break;
                        }

                        #endregion

                        switch (hclock & 1)
                        {
                            case 0: OamFetch(); break;
                            case 1: OamPhase(); break;
                        }

                        if (vclock < 240)
                            RenderPixel();

                        if (hclock == 63)
                            EvaluationBegin();

                        if (hclock == 255)
                            EvaluationReset();
                    }
                    else if (hclock < 320)
                    {
                        if (hclock == 256)
                            scroll.ResetX();
                        if (hclock == 304 && vclock == vbl_vclock_End)
                            scroll.ResetY();
                        #region Spr Fetches

                        switch (hclock & 7)
                        {
                            case 0: FetchName_0(); break;
                            case 1: FetchName_1(); break;
                            case 2: FetchAttr_0(); break;
                            case 3: FetchAttr_1(); break;
                            case 4: SprFetchBit0_0(); break;
                            case 5: SprFetchBit0_1(); break;
                            case 6: SprFetchBit1_0(); break;
                            case 7: SprFetchBit1_1(); SynthesizeSprPixels(); break;
                        }

                        #endregion
                    }
                    else if (hclock < 336)
                    {
                        #region Bkg Fetches

                        switch (hclock & 7)
                        {
                            case 0: FetchName_0(); break;
                            case 1: FetchName_1(); break;
                            case 2: FetchAttr_0(); break;
                            case 3: FetchAttr_1(); scroll.ClockX(); break;
                            case 4: FetchBit0_0(); break;
                            case 5: FetchBit0_1(); break;
                            case 6: FetchBit1_0(); break;
                            case 7: FetchBit1_1(); SynthesizeBkgPixels(); break;
                        }

                        #endregion
                    }
                    else if (hclock < 340)
                    {
                        #region Dummy Fetches

                        switch (hclock & 1)
                        {
                            case 0: FetchName_0(); break;
                            case 1: FetchName_1(); break;
                        }

                        #endregion
                    }
                    else
                    {
                        // Idle cycle
                    }
                }
                #endregion
                #region Rendering is off
                else
                {
                    // Rendering is off, draw color at vram address if it in range 0x3F00 - 0x3FFF
                    if (hclock < 255 & vclock < 240)
                    {
                        int pixel = 0;//this will clear the previous line
                        if ((scroll.addr & 0x3F00) == 0x3F00)
                            pixel = colors[paletteIndexes[Nes.PpuMemory[scroll.addr & 0x3FFF] & clipping | emphasis]];
                        else
                            pixel = colors[paletteIndexes[Nes.PpuMemory[0x3F00] & clipping | emphasis]];
                        screen[vclock][hclock] = pixel;
                    }
                }
                #endregion
            }
            hclock++;
            #region odd frame
            if (hclock == 328)
            {
                if (UseOddFrame)
                {
                    if (vclock == vbl_vclock_End)
                    {
                        oddSwap = !oddSwap;
                        if (!oddSwap & bkg.enabled)
                        {
                            hclock++;

                            //timing.cycles -= system.Gpu;
                        }
                    }
                }
            }
            #endregion
            #region VBLANK, NMI and frame end
            if (hclock == 1)
            {
                //set vbl
                if (vclock == vbl_vclock_Start)
                {
                    if (!suppressVbl)
                        nmi_ocurred = true;
                    suppressVbl = false;
                }
                //clear vbl
                if (vclock == vbl_vclock_End)
                {
                    spr0Hit = false;
                    sprOverflow = false;
                    nmi_ocurred = false;
                }
            }

            //frame end
            if (hclock == 341)
            {
                hclock = 0;
                vclock++;

                ScanlineTimer();
                //set vbl
                if (vclock == vbl_vclock_Start)
                {
                    if (nmi_output)
                        Nes.Cpu.Interrupt(Cpu.IsrType.Ppu, true);
                }

                if (vclock == frameEnd)
                {
                    vclock = 0;
                    Nes.FinishFrame(screen);
                }
            }
            #endregion
        }
        public int GetPixel(int x, int y)
        {
            return screen[y][x];
        }
        public void SetupPalette(int[] colors)
        {
            this.colors = colors;
            this.paletteIndexes = VSUnisystem.GetPalette(Nes.RomInfo.SHA1);
        }
        //null
        private void AddressLineupdating(int test)
        { }
        private void TickCycleTimer()
        { }
        private void TickScanlineTimer()
        {
        }

        public bool IsBGFetchTime()
        {
            return (hclock < 256 | hclock >= 320);
        }
        public bool IsOamSize()
        {
            return (spr.rasters == 0x10);
        }
        public bool IsRenderingOn()
        {
            return (bkg.enabled || spr.enabled);
        }
        public bool IsRenderingScanline()
        {
            return (vclock < 240);
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(fetch.addr);
            stream.Write(fetch.attr);
            stream.Write(fetch.bit0);
            stream.Write(fetch.bit1);
            stream.Write(fetch.name);

            stream.Write(scroll.addr);
            stream.Write(scroll.fine);
            stream.Write(scroll.step);
            stream.Write(scroll.swap);
            stream.Write(scroll.temp);

            for (int i = 0; i < 8; i++)
            {
                stream.Write(buffer[i].attr);
                stream.Write(buffer[i].name);
                stream.Write(buffer[i].x);
                stream.Write(buffer[i].y);
                stream.Write(buffer[i].zero);
            }

            stream.Write(bkg.address);
            stream.Write(bkg.clipped);
            stream.Write(bkg.enabled);
            stream.Write(bkg.rasters);

            stream.Write(spr.address);
            stream.Write(spr.clipped);
            stream.Write(spr.enabled);
            stream.Write(spr.rasters);

            stream.Write(chr);
            stream.Write(clipping);
            stream.Write(emphasis);
            stream.Write(oddSwap, spr0Hit, sprOverflow, nmi_output, nmi_ocurred, suppressVbl);
            stream.Write(hclock);
            stream.Write(vclock);
            stream.Write(oam_address);
            stream.Write(oam);
            stream.Write(oamData);
            stream.Write(oamCount);
            stream.Write(oamSlot);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            fetch.addr = stream.ReadInt32();
            fetch.attr = stream.ReadInt32();
            fetch.bit0 = stream.ReadInt32();
            fetch.bit1 = stream.ReadInt32();
            fetch.name = stream.ReadInt32();

            scroll.addr = stream.ReadInt32();
            scroll.fine = stream.ReadInt32();
            scroll.step = stream.ReadInt32();
            scroll.swap = stream.ReadBoolean();
            scroll.temp = stream.ReadInt32();

            for (int i = 0; i < 8; i++)
            {
                buffer[i].attr = stream.ReadByte();
                buffer[i].name = stream.ReadByte();
                buffer[i].x = stream.ReadByte();
                buffer[i].y = stream.ReadByte();
                buffer[i].zero = stream.ReadBoolean();
            }

            bkg.address = stream.ReadInt32();
            bkg.clipped = stream.ReadBoolean();
            bkg.enabled = stream.ReadBoolean();
            bkg.rasters = stream.ReadInt32();

            spr.address = stream.ReadInt32();
            spr.clipped = stream.ReadBoolean();
            spr.enabled = stream.ReadBoolean();
            spr.rasters = stream.ReadInt32();


            chr = stream.ReadByte();
            clipping = stream.ReadInt32();
            emphasis = stream.ReadInt32();
            bool[] flags = stream.ReadBooleans();
            oddSwap = flags[0];
            spr0Hit = flags[1];
            sprOverflow = flags[2];
            nmi_output = flags[3];
            nmi_ocurred = flags[4];
            suppressVbl = flags[5];

            hclock = stream.ReadInt32();
            vclock = stream.ReadInt32();
            oam_address = stream.ReadByte();
            stream.Read(oam);
            oamData = stream.ReadByte();
            oamCount = stream.ReadByte();
            oamSlot = stream.ReadByte();
        }
    }
}