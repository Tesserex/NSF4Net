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
    [BoardName("Asder", 112)]
    class Asder : Board
    {
        public Asder() : base() { }
        public Asder(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int index = 0;

        public override void HardReset()
        {
            base.HardReset();

            Switch16KPRG(prg.Length - 0x4000 >> 14, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xE001)
            {
                case 0x8000: index = data & 0x7; break;
                case 0xA000:
                    switch (index)
                    {
                        case 0: Switch08KPRG(data, 0x8000); break;
                        case 1: Switch08KPRG(data, 0xA000); break;
                        case 2: Switch02kCHR(data >> 1, 0x0000); break;
                        case 3: Switch02kCHR(data >> 1, 0x0800); break;
                        case 4: Switch01kCHR(data, 0x1000); break;
                        case 5: Switch01kCHR(data, 0x1400); break;
                        case 6: Switch01kCHR(data, 0x1800); break;
                        case 7: Switch01kCHR(data, 0x1C00); break;
                    }
                    break;
                case 0xE000: Nes.PpuMemory.SwitchMirroring((data & 0x1) == 0x1 ? Mirroring.ModeHorz : Mirroring.ModeVert); break;
            }
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(index);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            index = stream.ReadInt32();
        }
    }
}
