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
    [BoardName("11-in-1", 51)]
    class Mapper5111in1 : Board
    {
        public Mapper5111in1() : base() { }
        public Mapper5111in1(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int bank = 0;
        private int mode = 1;
        private int sramBank = 0;

        public override void HardReset()
        {
            base.HardReset();
            bank = 0;
            mode = 1;
            sramBank = 0;
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xE000)
            {
                case 0x8000:
                case 0xE000: bank = data & 0xF; UpdateBanks(); break;
                case 0xC000: bank = data & 0xF; mode = (data >> 3 & 0x2) | (mode & 0x1); UpdateBanks(); break;
            }
        }
        protected override void PokeSram(int address, byte data)
        {
            mode = (data >> 3 & 0x2) | (data >> 1 & 0x1);
            UpdateBanks();
        }
        protected override byte PeekSram(int address)
        {
            return prg[(address & 0x1FFF) | sramBank];
        }
        private void UpdateBanks()
        {
            int offset = 0;

            if ((mode & 0x1) == 0x1)
            {
                Switch32KPRG(bank);
                offset = 0x23;
            }
            else
            {
                Switch08KPRG((bank << 1) | (mode >> 1), 0x8000);
                Switch08KPRG(bank << 1 | 0x7, 0x8000);
                offset = 0x2F;
            }

            sramBank = (offset | (bank << 2)) << 13;
            Nes.PpuMemory.SwitchMirroring((mode == 0x3) ? Types.Mirroring.ModeHorz : Types.Mirroring.ModeVert);
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(bank);
            stream.Write(mode); 
            stream.Write(sramBank);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            bank = stream.ReadInt32();
            mode = stream.ReadInt32();
            sramBank = stream.ReadInt32();
        }
    }
}
