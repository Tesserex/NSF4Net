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
    [BoardName("6-in-1", 57)]
    class Mapper576in1 : Board
    {
        public Mapper576in1() : base() { }
        public Mapper576in1(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int chrReg = 0;
        private int chrA = 0;
        private int chrB = 0;

        public override void Initialize()
        {
            // Maps prg writes to 0x8000 - 0xFFFF. Maps sram reads and writes to 0x6000 - 0x8000.
            // Then do a hard reset.
            base.Initialize();

            //TODO: add your board initialize code like memory mapping. NEVER remover the previous line.
        }
        public override void HardReset()
        {
            base.HardReset();

            chrReg = 0;
            chrA = 0;
            chrB = 0;
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0x8800)
            {
                case 0x8000:
                    chrA = data & 0x7;
                    chrReg = ((data & 0x40) >> 3) | (chrA | chrB);
                    Switch08kCHR(chrReg);
                    break;

                case 0x8800:
                    chrB = data & 0x7;
                    chrReg = (chrReg & 0x8) | (chrA | chrB);
                    Switch08kCHR(chrReg);

                    if ((data & 0x10) == 0x10)
                        Switch32KPRG((data & 0xE0) >> 5);
                    else
                    {
                        Switch16KPRG((data & 0xE0) >> 5, 0x8000);
                        Switch16KPRG((data & 0xE0) >> 5, 0xC000);
                    }
                    Nes.PpuMemory.SwitchMirroring(((data & 0x8) == 0x8) ? Types.Mirroring.ModeHorz : Types.Mirroring.ModeVert);
                    break;
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(chrReg);
            stream.Write(chrA);
            stream.Write(chrB);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            chrReg = stream.ReadInt32();
            chrA = stream.ReadInt32();
            chrB = stream.ReadInt32();
        }
    }
}
