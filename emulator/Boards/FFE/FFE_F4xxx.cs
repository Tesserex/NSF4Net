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
namespace MyNes.Core.Boards.FFE
{
    [BoardName("FFE F4xxx", 6)]
    class FFE_F4xxx : FFE
    {
        public FFE_F4xxx()
            : base()
        { }
        public FFE_F4xxx(byte[] chr, byte[] prg, byte[] trainer, bool isVram)
            : base(chr, prg, trainer, isVram)
        { }
        public override void HardReset()
        {
            base.HardReset();
            base.Switch16KPRG(7, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            base.PokePrg(address, data);
            if (address >= 0x8000 && address <= 0xFFFF)
            {
                base.Switch16KPRG(((data & 0x3C) >> 2), 0x8000);

                base.Switch08kCHR((data & 0x3));
            }
        }
    }
}
