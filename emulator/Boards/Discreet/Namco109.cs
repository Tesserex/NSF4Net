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
    [BoardName("Namco 109", 76)]
    class Namco109 : Board
    {
        public Namco109() : base() { }
        public Namco109(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int command = 0;
        private bool prgMode = false;

        public override void HardReset()
        {
            base.HardReset();

            Switch08KPRG(0xFE, 0xC000);
            Switch08KPRG(prg.Length - 0x2000 >> 13, 0xE000);
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xE001)
            {
                case 0x8000: command = data & 0x7; prgMode = (data & 0x40) == 0x40; break;
                case 0x8001:
                    switch (command)
                    {
                        case 0x2: Switch02kCHR(data, 0x0000); break;
                        case 0x3: Switch02kCHR(data, 0x0800); break;
                        case 0x4: Switch02kCHR(data, 0x1000); break;
                        case 0x5: Switch02kCHR(data, 0x1800); break;
                        case 0x6: Switch08KPRG(data, prgMode ? 0xC000 : 0x8000); break;
                        case 0x7: Switch08KPRG(data, 0xA000); break;
                    }
                    break;
                case 0xA000: Nes.PpuMemory.SwitchMirroring((data & 1) == 1 ? Mirroring.ModeHorz : Mirroring.ModeVert); break;
            }
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(command);
            stream.Write(prgMode);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            command = stream.ReadInt32();
            prgMode = stream.ReadBoolean();
        }
    }
}
