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
    [BoardName("Reset Based 4-in-1", 60)]
    class Mapper60 : Board
    {
        public Mapper60() : base() { }
        public Mapper60(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }
        private int latch = 0;
        private byte menu = 0;

        public override void HardReset()
        {
            base.HardReset();
            latch = 0;
            menu = 0;
        }
        public override void SoftReset()
        {
            latch = 0;
            menu = (byte)((menu + 1) & 0x3);
            Switch08kCHR(menu);
            Switch16KPRG(menu, 0x8000);
            Switch16KPRG(menu, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            latch = address & 0x100;
            Nes.PpuMemory.SwitchMirroring((address & 0x8)==0x8 ? Types.Mirroring.ModeHorz: Types.Mirroring.ModeVert);
            Switch16KPRG((address >> 4) & ~(~address >> 7 & 0x1), 0x8000);
            Switch16KPRG((address >> 4) | (~address >> 7 & 0x1), 0xC000);
            Switch08kCHR(address);
        }
        protected override byte PeekPrg(int address)
        {
            if (latch == 0)
                return base.PeekPrg(address);
            else
                return menu;
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(latch); 
            stream.Write(menu);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            latch = stream.ReadInt32();
            menu = stream.ReadByte();
        }
    }
}
