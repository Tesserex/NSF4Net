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
    [BoardName("Subor", 167)]
    class Subor : Board
    {
        public Subor() : base() { }
        public Subor(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int[] regs = new int[4];
        protected byte mode = 0;

        public override void HardReset()
        {
            base.HardReset();

            regs = new int[4];
            mode = 0;
            PokePrg(0x8000, 0);
        }
        protected override void PokePrg(int address, byte data)
        {
            regs[address >> 13 & 0x3] = data;

            int[] banks =
				{
					((regs[0]) ^ regs[1]) << 1 & 0x20,
					((regs[2]) ^ regs[3]) << 0 & 0x1F
				};

            if ((regs[1] & 0x8) == 0x8)
            {
                banks[0] += banks[1] & 0xFE;
                banks[1] = banks[0];
                banks[0] += mode ^ 1;
                banks[1] += mode ^ 0;
            }
            else if ((regs[1] & 0x4) == 0x4)
            {
                banks[1] = banks[0] + banks[1];
                banks[0] = 0x1F;
            }
            else
            {
                banks[0] = banks[0] + banks[1];
                banks[1] = mode == 1 ? 0x07 : 0x20;
            }
            Switch16KPRG(banks[0], 0x8000);
            Switch16KPRG(banks[1], 0xC000);
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(regs);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(regs);
        }
    }
}
