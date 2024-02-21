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
    [BoardName("FFE F8xxx", 17)]
    class FFE_F8xxx : FFE
    {
        public FFE_F8xxx() : base() { }
        public FFE_F8xxx(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        public override void Initialize()
        {
            // Maps prg writes to 0x8000 - 0xFFFF. Maps sram reads and writes to 0x6000 - 0x8000.
            // Then do a hard reset.
            base.Initialize();

            //TODO: add your board initialize code like memory mapping. NEVER remover the previous line.
        }
        public override void HardReset()
        {
            // Switch 32KB prg bank at 0x8000
            // Switch 08KB chr bank at 0x0000
            base.HardReset();
            base.Switch16KPRG((prg.Length - 0x4000) >> 14, 0xC000);
            //TODO: add the hard reset code here. Also this called at board initialize so it should be power up code.
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address)
            {
                case 0x4504: base.Switch08KPRG(data, 0x8000); break;
                case 0x4505: base.Switch08KPRG(data, 0xA000); break;
                case 0x4506: base.Switch08KPRG(data, 0xC000); break;
                case 0x4507: base.Switch08KPRG(data, 0xE000); break;
                case 0x4510: base.Switch01kCHR(data, 0x0000); break;
                case 0x4511: base.Switch01kCHR(data, 0x0400); break;
                case 0x4512: base.Switch01kCHR(data, 0x0800); break;
                case 0x4513: base.Switch01kCHR(data, 0x0C00); break;
                case 0x4514: base.Switch01kCHR(data, 0x1000); break;
                case 0x4515: base.Switch01kCHR(data, 0x1400); break;
                case 0x4516: base.Switch01kCHR(data, 0x1800); break;
                case 0x4517: base.Switch01kCHR(data, 0x1C00); break;
            }
        }
    }
}
