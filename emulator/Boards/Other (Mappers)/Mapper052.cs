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
    [BoardName("7-in-1", 52)]
    class Mapper527in1 : MMC3
    {
        public Mapper527in1() : base() { }
        public Mapper527in1(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private byte exreg = 0;
        private bool enableWrite = false;

        public override void HardReset()
        {
            exreg = 0;
            enableWrite = false;
            base.HardReset();
        }
        protected override void PokeSram(int address, byte data)
        {
            if (!enableWrite)
            {
                if (wramON && !wramReadOnly)
                {
                    exreg = data;
                    enableWrite = true;
                    SetupPRG();
                    SetupCHR();
                }
            }
            else
            {
                if (wramON && !wramReadOnly)
                    base.sram[address - 0x6000] = data;
            }
        }
        protected override void SetupPRG()
        {
            int and = (exreg << 1 & 0x10) ^ 0x1F;
            int or = ((exreg & 0x6) | (exreg >> 3 & exreg & 0x1)) << 4;

            if (!prgmode)
            {
                base.Switch08KPRG((prgRegs[0] & and) | or, 0x8000);
                base.Switch08KPRG((prgRegs[1] & and) | or, 0xA000);
                base.Switch08KPRG((prgRegs[2] & and) | or, 0xC000);
                base.Switch08KPRG((prgRegs[3] & and) | or, 0xE000);
            }
            else
            {
                base.Switch08KPRG((prgRegs[2] & and) | or, 0x8000);
                base.Switch08KPRG((prgRegs[1] & and) | or, 0xA000);
                base.Switch08KPRG((prgRegs[0] & and) | or, 0xC000);
                base.Switch08KPRG((prgRegs[3] & and) | or, 0xE000);
            }
        }
        protected override void SetupCHR()
        {
            int and = ((exreg & 0x40) << 1) ^ 0xFF;
            int or = ((exreg >> 3 & 0x4) | (exreg >> 1 & 0x2) | ((exreg >> 6) & (exreg >> 4) & 0x1)) << 7;
            if (!chrmode)
            {
                base.Switch01kCHR((chrRegs[0] & and) | or, 0x0000);
                base.Switch01kCHR(((chrRegs[0] + 1) & and) | or, 0x0400);
                base.Switch01kCHR((chrRegs[1] & and) | or, 0x0800);
                base.Switch01kCHR(((chrRegs[1] + 1) & and) | or, 0x0C00);

                base.Switch01kCHR((chrRegs[2] & and) | or, 0x1000);
                base.Switch01kCHR((chrRegs[3] & and) | or, 0x1400);
                base.Switch01kCHR((chrRegs[4] & and) | or, 0x1800);
                base.Switch01kCHR((chrRegs[5] & and) | or, 0x1C00);
            }
            else
            {
                base.Switch01kCHR((chrRegs[0] & and) | or, 0x1000);
                base.Switch01kCHR(((chrRegs[0] + 1) & and) | or, 0x1400);
                base.Switch01kCHR((chrRegs[1] & and) | or, 0x1800);
                base.Switch01kCHR(((chrRegs[1] + 1) & and) | or, 0x1C00);

                base.Switch01kCHR((chrRegs[2] & and) | or, 0x0000);
                base.Switch01kCHR((chrRegs[3] & and) | or, 0x0400);
                base.Switch01kCHR((chrRegs[4] & and) | or, 0x0800);
                base.Switch01kCHR((chrRegs[5] & and) | or, 0x0C00);
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(exreg); 
            stream.Write(enableWrite);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            exreg = stream.ReadByte();
            enableWrite = stream.ReadBoolean();
        }
    }
}
