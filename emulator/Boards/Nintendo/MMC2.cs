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
/*Written by Adam Becker*/
namespace MyNes.Core.Boards.Nintendo
{
    [BoardName("MMC2", 9)]
    class MMC2 : Board
    {
        public MMC2()
            : base()
        { }
        public MMC2(byte[] chr, byte[] prg, byte[] trainer, bool isVram)
            : base(chr, prg, trainer, isVram)
        { }
        private byte latch_a = 0xFE;
        private byte latch_b = 0xFE;
        private byte[] reg = new byte[4];
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void HardReset()
        {
            base.HardReset();
            reg[0] = 0; reg[1] = 4;
            reg[2] = 0; reg[3] = 0;
            latch_a = 0xFE; latch_b = 0xFE;
            base.Switch08KPRG((prg.Length - 0x6000) >> 13, 0xA000);
            base.Switch08KPRG((prg.Length - 0x4000) >> 13, 0xC000);
            base.Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xF000)
            {
                case 0xA000: base.Switch08KPRG(data, 0x8000); break;
                case 0xB000: reg[0] = data; if (latch_a == 0xFD) base.Switch04kCHR(reg[0], 0x0000); break;
                case 0xC000: reg[1] = data; if (latch_a == 0xFE) base.Switch04kCHR(reg[1], 0x0000); break;
                case 0xD000: reg[2] = data; if (latch_b == 0xFD) base.Switch04kCHR(reg[2], 0x1000); break;
                case 0xE000: reg[3] = data; if (latch_b == 0xFE) base.Switch04kCHR(reg[3], 0x1000); break;
                case 0xF000: Nes.PpuMemory.SwitchMirroring((data & 1) == 1 ? Types.Mirroring.ModeHorz : Types.Mirroring.ModeVert); break;
            }
        }
        private void CHRlatch(int Address)
        {
            if ((Address & 0x1FF0) == 0x0FD0 && latch_a != 0xFD)
            {
                latch_a = 0xFD;
                base.Switch04kCHR(reg[0], 0x0000);
            }
            else if ((Address & 0x1FF0) == 0x0FE0 && latch_a != 0xFE)
            {
                latch_a = 0xFE;
                base.Switch04kCHR(reg[1], 0x0000);
            }
            else if ((Address & 0x1FF0) == 0x1FD0 && latch_b != 0xFD)
            {
                latch_b = 0xFD;
                base.Switch04kCHR(reg[2], 0x1000);
            }
            else if ((Address & 0x1FF0) == 0x1FE0 && latch_b != 0xFE)
            {
                latch_b = 0xFE;
                base.Switch04kCHR(reg[3], 0x1000);
            }
        }
        protected override byte PeekChr(int address)
        {
            byte value = base.PeekChr(address);
            CHRlatch(address);
            return value;
        }
        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(latch_a);
            stream.Write(latch_b);
            stream.Write(reg);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            latch_a = stream.ReadByte();
            latch_b = stream.ReadByte();
            stream.Read(reg);
        }
    }
}
