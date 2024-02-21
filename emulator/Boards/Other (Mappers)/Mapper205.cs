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
    [BoardName("Unknown", 205)]
    class Mapper205 : MMC3
    {
        public Mapper205() : base() { }
        public Mapper205(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int chrAND = 0;
        private int chrOR = 0;
        private int prgAND = 0;
        private int prgOR = 0;
        public override void HardReset()
        {
            prgAND = 0x1F; prgOR = 0x00; chrAND = 0xFF; chrOR = 0x000; // mode 0
            base.HardReset();
        }
        protected override void PokeSram(int address, byte data)
        {
            switch (data & 0x3)
            {
                case 0: prgAND = 0x1F; prgOR = 0x00; chrAND = 0xFF; chrOR = 0x000; break;
                case 1: prgAND = 0x1F; prgOR = 0x10; chrAND = 0xFF; chrOR = 0x080; break;
                case 2: prgAND = 0x0F; prgOR = 0x20; chrAND = 0x7F; chrOR = 0x100; break;
                case 3: prgAND = 0x0F; prgOR = 0x30; chrAND = 0x7F; chrOR = 0x180; break;
            }
            SetupCHR();
            SetupPRG();
        }
        protected override void SetupPRG()
        {
            if (!prgmode)
            {
                base.Switch08KPRG((prgRegs[0] & prgAND) | prgOR, 0x8000);
                base.Switch08KPRG((prgRegs[1] & prgAND) | prgOR, 0xA000);
                base.Switch08KPRG((prgRegs[2] & prgAND) | prgOR, 0xC000);
                base.Switch08KPRG((prgRegs[3] & prgAND) | prgOR, 0xE000);
            }
            else
            {
                base.Switch08KPRG((prgRegs[2] & prgAND) | prgOR, 0x8000);
                base.Switch08KPRG((prgRegs[1] & prgAND) | prgOR, 0xA000);
                base.Switch08KPRG((prgRegs[0] & prgAND) | prgOR, 0xC000);
                base.Switch08KPRG((prgRegs[3] & prgAND) | prgOR, 0xE000);
            }
        }
        protected override void SetupCHR()
        {
            if (!chrmode)
            {
                base.Switch01kCHR((chrRegs[0] & chrAND) | chrOR, 0x0000);
                base.Switch01kCHR(((chrRegs[0] & chrAND) + 1) | chrOR, 0x0400);
                base.Switch01kCHR((chrRegs[1] & chrAND) | chrOR, 0x0800);
                base.Switch01kCHR(((chrRegs[1] & chrAND) + 1) | chrOR, 0x0C00);
                base.Switch01kCHR((chrRegs[2] & chrAND) | chrOR, 0x1000);
                base.Switch01kCHR((chrRegs[3] & chrAND) | chrOR, 0x1400);
                base.Switch01kCHR((chrRegs[4] & chrAND) | chrOR, 0x1800);
                base.Switch01kCHR((chrRegs[5] & chrAND) | chrOR, 0x1C00);
            }
            else
            {
                base.Switch01kCHR((chrRegs[0] & chrAND) | chrOR, 0x1000);
                base.Switch01kCHR(((chrRegs[0] & chrAND) + 1) | chrOR, 0x1400);
                base.Switch01kCHR((chrRegs[1] & chrAND) | chrOR, 0x1800);
                base.Switch01kCHR(((chrRegs[1] & chrAND) + 1) | chrOR, 0x1C00);
                base.Switch01kCHR((chrRegs[2] & chrAND) | chrOR, 0x0000);
                base.Switch01kCHR((chrRegs[3] & chrAND) | chrOR, 0x0400);
                base.Switch01kCHR((chrRegs[4] & chrAND) | chrOR, 0x0800);
                base.Switch01kCHR((chrRegs[5] & chrAND) | chrOR, 0x0C00);
            }
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(chrAND);
            stream.Write(chrOR);
            stream.Write(prgAND);
            stream.Write(prgOR);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            chrAND = stream.ReadInt32();
            chrOR = stream.ReadInt32();
            prgAND = stream.ReadInt32();
            prgOR = stream.ReadInt32();
        }
    }
}
