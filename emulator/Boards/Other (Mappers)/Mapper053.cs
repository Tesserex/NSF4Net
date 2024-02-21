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
    [BoardName("Unknown", 53)]
    class Mapper53 : Board
    {
        public Mapper53() : base() { }
        public Mapper53(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private byte[] regs = new byte[2];
        private int sramBank = 0;
        private bool epromFirst;

        public override void HardReset()
        {
            base.HardReset();
            regs = new byte[2];
            sramBank = 0; epromFirst = true;
        }
        protected override void PokePrg(int address, byte data)
        {
            regs[1] = data;
            UpdatePrg();
        }
        protected override void PokeSram(int address, byte data)
        {
            regs[0] = data;
            UpdatePrg();
            Nes.PpuMemory.SwitchMirroring((data & 0x20) == 0x20 ? Types.Mirroring.ModeHorz : Types.Mirroring.ModeVert);
        }
        protected override byte PeekSram(int address)
        {
            return prg[address & 0x1FFF | sramBank];
        }
        private void UpdatePrg()
        {
            int r = regs[0] << 3 & 0x78;

            sramBank = (r << 1 | 0xF) + (epromFirst ? 0x4 : 0x0) <<12;

            Switch16KPRG((regs[0] & 0x10) == 0x10 ? (r | (regs[1] & 0x7))
                + (epromFirst ? 0x2 : 0x0) : epromFirst ? 0x00 : 0x80, 0x8000);
            Switch16KPRG((regs[0] & 0x10) == 0x10 ? (r | (0xFF & 0x7)) + (epromFirst ? 0x2 : 0x0) : epromFirst ? 0x01 : 0x81,
                0xC000);
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(regs); 
            stream.Write(sramBank);
            stream.Write(epromFirst);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(regs); 
            sramBank = stream.ReadInt32();
            epromFirst = stream.ReadBoolean();
        }
    }
}
