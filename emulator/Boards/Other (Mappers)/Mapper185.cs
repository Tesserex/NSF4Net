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
    [BoardName("Unknown", 185)]
    class Mapper185 : Board
    {
        public Mapper185() : base() { }
        public Mapper185(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool lockchr = false;

        public override void HardReset()
        {
            base.HardReset();
            lockchr = false;
        }
        protected override byte PeekChr(int address)
        {
            if (!lockchr)
                return base.PeekChr(address);
            else
                return 0xFF;
        }
        protected override void PokePrg(int address, byte data)
        {
            lockchr = ((data & 0xF) == 0) || (data == 0x13);
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(lockchr);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            lockchr = stream.ReadBoolean();
        }
    }
}
