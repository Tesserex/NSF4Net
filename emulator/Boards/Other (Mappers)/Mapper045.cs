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
    [BoardName("Mapper45 X in 1", 45)]
    class Mapper45Xin1 : MMC3
    {
        public Mapper45Xin1() : base() { }
        public Mapper45Xin1(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        public override void HardReset()
        {
            LOCK = false;
            writeRegister = 0;
            chrOR = 0;
            chrAND = 0xFF;
            prgOR = 0;
            prgAND = 0x3F;
            base.HardReset();
        }

        private bool LOCK = false;
        private int writeRegister = 0;
        private int chrOR = 0;
        private int chrAND = 0;
        private int prgOR = 0;
        private int prgAND = 0;

        protected override void PokeSram(int address, byte data)
        {
            if (LOCK)
                base.PokeSram(address, data);
            else
            {
                switch (writeRegister)
                {
                    case 0: chrOR = (chrOR & 0x0F00) | data; break;
                    case 1: prgOR = data; break;
                    case 2:
                        chrOR = (chrOR & 0x00FF) | ((data & 0xF0) << 4);
                        //chrAND = 0xFF >> (~(data & 0x0F));
                        if ((data & 0x8) == 0x8)
                            chrAND = ((1 << ((data & 0x7) + 1)) - 1);
                        else
                            chrAND = ((data > 0) ? 0 : ~0);
                        break;
                    case 3: prgAND = (data & 0x3F) ^ 0x3F; LOCK = (data & 0x40) == 0x40; break;
                }
                writeRegister = (writeRegister + 1) & 0x3;
                SetupCHR();
                SetupPRG();
            }
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
            stream.Write(LOCK);
            stream.Write(writeRegister);
            stream.Write(chrOR);
            stream.Write(chrAND);
            stream.Write(prgOR);
            stream.Write(prgAND);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            LOCK = stream.ReadBoolean();
            writeRegister = stream.ReadInt32();
            chrOR = stream.ReadInt32();
            chrAND = stream.ReadInt32();
            prgOR = stream.ReadInt32();
            prgAND = stream.ReadInt32();
        }
    }
}
