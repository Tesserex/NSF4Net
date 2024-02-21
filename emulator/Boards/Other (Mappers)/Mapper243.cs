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
namespace MyNes.Core.Boards.Other__Mappers_
{
    [BoardName("Unknown", 243)]
    class Mapper243 : Board
    {
        public Mapper243() : base() { }
        public Mapper243(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int index = 0;
        private int chrReg = 0;

        public override void Initialize()
        {
            base.Initialize();

            Nes.CpuMemory.Hook(0x4100, 0x4FFF, PokePrg);
        }
        public override void HardReset()
        {
            base.HardReset();

            index = 0;
            chrReg = 0;
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0x4101)
            {
                case 0x4100: index = data & 0x7; break;
                case 0x4101:
                    switch (index)
                    {
                        case 2: chrReg = (chrReg & 0x7) | ((data << 3) & 0x8); Switch08kCHR(chrReg); break;
                        case 4: chrReg = (chrReg & 0xE) | (data & 0x1); Switch08kCHR(chrReg); break;
                        case 5: Switch32KPRG(data & 0x7); break;
                        case 6: chrReg = (chrReg & 0x9) | ((data << 1) & 0x6); Switch08kCHR(chrReg); break;
                        case 7:
                            switch (data >> 1 & 3)
                            {
                                case 0: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeHorz); break;
                                case 1: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeVert); break;
                                case 2: Nes.PpuMemory.SwitchMirroring(0x7); break;
                                case 3: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScB); break;
                            }
                            break;
                    }
                    break;
            }
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(index);
            stream.Write(chrReg);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            index = stream.ReadInt32();
            chrReg = stream.ReadInt32();
        }
    }
}
