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
    [BoardName("Unknown", 226)]
    class Mapper226 : Board
    {
        public Mapper226() : base() { }
        public Mapper226(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int prgReg = 0;
        private bool prgMode = false;

        public override void HardReset()
        {
            base.HardReset();

            prgReg = 0;
            prgMode = false;
        }
        private void SetupPRG()
        {
            if (prgMode)
            {
                Switch16KPRG(prgReg, 0x8000);
                Switch16KPRG(prgReg, 0xC000);
            }
            else
            {
                Switch32KPRG(prgReg >> 1);
            }
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0x8001)
            {
                case 0x8000:
                    prgReg = (prgReg & 0xE0) | (data & 0x1F) | ((data & 0x80) >> 2);
                    prgMode = (data & 0x20) == 0x20;
                    Nes.PpuMemory.SwitchMirroring((data & 0x40) == 0x40 ? Mirroring.ModeVert : Mirroring.ModeHorz);
                    SetupPRG();
                    break;
                case 0x8001: prgReg = (prgReg & 0x7F) | ((data & 1) << 7); SetupPRG(); break;
            }
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(prgReg);
            stream.Write(prgMode);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            prgReg = stream.ReadInt32();
            prgMode = stream.ReadBoolean();
        }
    }
}
