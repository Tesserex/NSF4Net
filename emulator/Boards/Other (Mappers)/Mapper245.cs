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
using MyNes.Core.Boards.Nintendo;

namespace MyNes.Core.Boards.Other__Mappers_
{
    [BoardName("Unknown", 245)]
    class Mapper245 : MMC3
    {
        public Mapper245() : base() { }
        public Mapper245(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool prgMode2 = false;

        protected override void Poke8001(int address, byte data)
        {
            switch (addrSelect)
            {
                case 0: prgMode2 = ((chrRegs[0] & 0x2) == 0x2); chrRegs[addrSelect] = data; SetupCHR(); SetupPRG(); break;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5: chrRegs[addrSelect] = data; SetupCHR(); break;
                case 6: prgRegs[0] = (byte)(data & 0x3F); SetupPRG(); break;
                case 7: prgRegs[1] = (byte)(data & 0x3F); SetupPRG(); break;
            }
        }
        protected override void SetupCHR()
        {
            if (!isVram)
                base.SetupCHR();
            else
                if (!chrmode)
                {
                    Switch04kCHR(0, 0x0000);
                    Switch04kCHR(1, 0x1000);
                }
                else
                {
                    Switch04kCHR(1, 0x0000);
                    Switch04kCHR(0, 0x1000);
                }
        }
        protected override void SetupPRG()
        {
            int or = prgMode2 ? 0x40 : 0x0;
            if (!prgmode)
            {
                base.Switch08KPRG((prgRegs[0] & 0x3F) | or, 0x8000);
                base.Switch08KPRG((prgRegs[1] & 0x3F) | or, 0xA000);
                base.Switch08KPRG((prgRegs[2] & 0x3F) | or, 0xC000);
                base.Switch08KPRG((prgRegs[3] & 0x3F) | or, 0xE000);
            }
            else
            {
                base.Switch08KPRG((prgRegs[2] & 0x3F) | or, 0x8000);
                base.Switch08KPRG((prgRegs[1] & 0x3F) | or, 0xA000);
                base.Switch08KPRG((prgRegs[0] & 0x3F) | or, 0xC000);
                base.Switch08KPRG((prgRegs[3] & 0x3F) | or, 0xE000);
            }
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(prgMode2);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            prgMode2 = stream.ReadBoolean();
        }
    }
}
