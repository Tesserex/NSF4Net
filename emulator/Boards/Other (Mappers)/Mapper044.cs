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
using MyNes.Core.Boards.Nintendo;
namespace MyNes.Core.Boards.Discreet
{
    [BoardName("Mapper44 7 in 1", 44)]
    class Mapper447in1 : MMC3
    {
        public Mapper447in1() : base() { }
        public Mapper447in1(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int prgAND = 0;
        private int prgOR = 0;
        private int chrAND = 0;
        private int chrOR = 0;
        private int exReg = 0;

        public override void HardReset()
        {
            base.HardReset();
            prgAND = 0x0F; prgOR = 0x00; chrAND = 0x7F; chrOR = 0x000; SetupPRG(); SetupCHR();
        }
        protected override void PokeA001(int address, byte data)
        {
            base.PokeA001(address, data);//same as mmc3
            if (exReg == data) return;//if same, return to make things faster
            exReg = data;
            switch (data & 0x7)
            {
                case 0: prgAND = 0x0F; prgOR = 0x00; chrAND = 0x7F; chrOR = 0x000; break;
                case 1: prgAND = 0x0F; prgOR = 0x10; chrAND = 0x7F; chrOR = 0x080; break;
                case 2: prgAND = 0x0F; prgOR = 0x20; chrAND = 0x7F; chrOR = 0x100; break;
                case 3: prgAND = 0x0F; prgOR = 0x30; chrAND = 0x7F; chrOR = 0x180; break;
                case 4: prgAND = 0x0F; prgOR = 0x40; chrAND = 0x7F; chrOR = 0x200; break;
                case 5: prgAND = 0x0F; prgOR = 0x50; chrAND = 0x7F; chrOR = 0x280; break;
                case 6:
                case 7: prgAND = 0x1F; prgOR = 0x60; chrAND = 0xFF; chrOR = 0x300; break;
            }
            SetupPRG(); SetupCHR();
        }
        protected override void SetupPRG()
        {
            if (!prgmode)
            {
                base.Switch08KPRG((prgRegs[0] & prgAND) | prgOR, 0x8000);
                base.Switch08KPRG((prgRegs[1] & prgAND) | prgOR, 0xA000);
                base.Switch08KPRG((prgAND - 1) | prgOR, 0xC000);
                base.Switch08KPRG(prgAND | prgOR, 0xE000);
            }
            else
            {
                base.Switch08KPRG((prgAND - 1) | prgOR, 0x8000);
                base.Switch08KPRG((prgRegs[1] & prgAND) | prgOR, 0xA000);
                base.Switch08KPRG((prgRegs[0] & prgAND) | prgOR, 0xC000);
                base.Switch08KPRG(prgAND | prgOR, 0xE000);
            }
        }
        protected override void SetupCHR()
        {
            if (!chrmode)
            {
                base.Switch01kCHR((chrRegs[0] & chrAND) | chrOR, 0x0000);
                base.Switch01kCHR(((chrRegs[0] + 1) & chrAND) | chrOR, 0x0400);
                base.Switch01kCHR((chrRegs[1] & chrAND) | chrOR, 0x0800);
                base.Switch01kCHR(((chrRegs[1] + 1) & chrAND) | chrOR, 0x0C00);
                base.Switch01kCHR((chrRegs[2] & chrAND) | chrOR, 0x1000);
                base.Switch01kCHR((chrRegs[3] & chrAND) | chrOR, 0x1400);
                base.Switch01kCHR((chrRegs[4] & chrAND) | chrOR, 0x1800);
                base.Switch01kCHR((chrRegs[5] & chrAND) | chrOR, 0x1C00);
            }
            else
            {
                base.Switch01kCHR((chrRegs[0] & chrAND) | chrOR, 0x1000);
                base.Switch01kCHR(((chrRegs[0] + 1) & chrAND) | chrOR, 0x1400);
                base.Switch01kCHR((chrRegs[1] & chrAND) | chrOR, 0x1800);
                base.Switch01kCHR(((chrRegs[1] + 1) & chrAND) | chrOR, 0x1C00);
                base.Switch01kCHR((chrRegs[2] & chrAND) | chrOR, 0x0000);
                base.Switch01kCHR((chrRegs[3] & chrAND) | chrOR, 0x0400);
                base.Switch01kCHR((chrRegs[4] & chrAND) | chrOR, 0x0800);
                base.Switch01kCHR((chrRegs[5] & chrAND) | chrOR, 0x0C00);
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(prgAND); 
            stream.Write(prgOR);
            stream.Write(chrAND);
            stream.Write(chrOR); 
            stream.Write(exReg);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            prgAND = stream.ReadInt32();
            prgOR = stream.ReadInt32();
            chrAND = stream.ReadInt32();
            chrOR = stream.ReadInt32();
            exReg = stream.ReadInt32();
        }
    }
}
