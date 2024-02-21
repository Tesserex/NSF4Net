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
namespace MyNes.Core.Boards.Konami
{
    [BoardName("VRC6b", 26)]
    class VRC6b : VRC6
    {
        public VRC6b() : base() { }
        public VRC6b(byte[] chr, byte[] prg, byte[] trainer, bool isVram)
            : base(chr, prg, trainer, isVram)
        {
            AD_8_0 = 0x8000;
            AD_8_1 = 0x8002;
            AD_8_2 = 0x8001;
            AD_8_3 = 0x8003;

            AD_9_0 = 0x9000;
            AD_9_1 = 0x9002;
            AD_9_2 = 0x9001;
            AD_9_3 = 0x9003;

            AD_A_0 = 0xA000;
            AD_A_1 = 0xA002;
            AD_A_2 = 0xA001;
            AD_A_3 = 0xA003;

            AD_B_0 = 0xB000;
            AD_B_1 = 0xB002;
            AD_B_2 = 0xB001;
            AD_B_3 = 0xB003;

            AD_C_0 = 0xC000;
            AD_C_1 = 0xC002;
            AD_C_2 = 0xC001;
            AD_C_3 = 0xC003;

            AD_D_0 = 0xD000;
            AD_D_1 = 0xD002;
            AD_D_2 = 0xD001;
            AD_D_3 = 0xD003;

            AD_E_0 = 0xE000;
            AD_E_1 = 0xE002;
            AD_E_2 = 0xE001;
            AD_E_3 = 0xE003;

            AD_F_0 = 0xF000;
            AD_F_1 = 0xF002;
            AD_F_2 = 0xF001;
        }
    }
}
