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
/*Written by Adam Becker*/
namespace MyNes.Core.Boards.FFE
{
    [BoardName("UxROM", 2)]
    public class UxROM : Board
    {
        public UxROM()
            : base()
        { }
        public UxROM(byte[] chr, byte[] prg, byte[] trainer, bool isVram)
            : base(chr, prg, trainer, isVram)
        { }
        int mask = 0x7;
        public override void Initialize()
        {
            switch (Nes.RomInfo.PRGcount * 0x4000)
            {
                case 0x20000: mask = 0x7; break; // UNROM: 128 kB PRG, 8kB CHR-RAM
                case 0x40000: mask = 0xF; break; // UOROM: 256 kB PRG, 8kB CHR-RAM
            }
            base.Initialize();
        }
        public override void HardReset()
        {
            base.HardReset();
            base.Switch16KPRG(0, 0x8000);
            base.Switch16KPRG(mask, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            base.Switch16KPRG((data & mask), 0x8000);
        }
    }
}