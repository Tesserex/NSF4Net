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
    [BoardName("Irem", 77)]
    class Irem : Board
    {
        public Irem() : base() { }
        public Irem(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private byte[] chrRam = new byte[0x2000];

        protected override void PokePrg(int address, byte data)
        {
            Switch32KPRG(data & 0xF);
            Switch02kCHR((data & 0xF0) >> 4, 0x0000);
        }
        protected override void PokeChr(int address, byte data)
        {
            if (address < 0x800)
                base.PokeChr(address, data);
            else
                chrRam[address - 0x800] = data;
        }
        protected override byte PeekChr(int address)
        {
            if (address < 0x800)
                return base.PeekChr(address);
            else
                return chrRam[address - 0x800];
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(chrRam);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream); 
            stream.Read(chrRam);
        }
    }
}
