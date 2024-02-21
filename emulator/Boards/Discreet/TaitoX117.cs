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
namespace MyNes.Core.Boards.Discreet
{
    [BoardName("Taito X1-17", 82)]
    class TaitoX117 : Board
    {
        public TaitoX117() : base() { }
        public TaitoX117(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool chrMode = false;

        public override void HardReset()
        {
            base.HardReset();
            chrMode = false;
            Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
        }
        protected override void PokeSram(int address, byte data)
        {
            switch (address)
            {
                case 0x7EF0: Switch02kCHR(data >> 1, chrMode ? 0x1000 : 0x0000); break;
                case 0x7EF1: Switch02kCHR(data >> 1, chrMode ? 0x1800 : 0x0800); break;
                case 0x7EF2: Switch01kCHR(data, chrMode ? 0x0000 : 0x1000); break;
                case 0x7EF3: Switch01kCHR(data, chrMode ? 0x0400 : 0x1400); break;
                case 0x7EF4: Switch01kCHR(data, chrMode ? 0x0800 : 0x1800); break;
                case 0x7EF5: Switch01kCHR(data, chrMode ? 0x0C00 : 0x1C00); break;
                case 0x7EF6:
                    chrMode = (data & 0x2) == 0x2;
                    Nes.PpuMemory.SwitchMirroring((data & 0x1) == 1 ? Mirroring.ModeVert : Mirroring.ModeHorz);
                    break;
                case 0x7EFA: Switch08KPRG(data >> 2, 0x8000); break;
                case 0x7EFB: Switch08KPRG(data >> 2, 0xA000); break;
                case 0x7EFC: Switch08KPRG(data >> 2, 0xC000); break;
            }
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(chrMode);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            chrMode = stream.ReadBoolean();
        }
    }
}
