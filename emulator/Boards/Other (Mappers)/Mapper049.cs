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
    [BoardName("4-in-1", 49)]
    class Mapper494in1 : MMC3
    {
        public Mapper494in1() : base() { }
        public Mapper494in1(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private byte exreg = 0;

        public override void HardReset()
        {
            base.HardReset(); 
            exreg = 0;
        }
        protected override void PokeSram(int address, byte data)
        {
            if (wramON && !wramReadOnly)
            {
                exreg = data;
                SetupPRG();
                SetupCHR();
            }
        }
        protected override void SetupPRG()
        {
            if ((exreg & 0x1) == 0x1)
            {
                int r = exreg >> 2 & 0x30;
                if (!prgmode)
                {
                    base.Switch08KPRG((prgRegs[0] & 0x0F) | r, 0x8000);
                    base.Switch08KPRG((prgRegs[1] & 0x0F) | r, 0xA000);
                    base.Switch08KPRG((prgRegs[2] & 0x0F) | r, 0xC000);
                    base.Switch08KPRG((prgRegs[3] & 0x0F) | r, 0xE000);
                }
                else
                {
                    base.Switch08KPRG((prgRegs[2] & 0x0F) | r, 0x8000);
                    base.Switch08KPRG((prgRegs[1] & 0x0F) | r, 0xA000);
                    base.Switch08KPRG((prgRegs[0] & 0x0F) | r, 0xC000);
                    base.Switch08KPRG((prgRegs[3] & 0x0F) | r, 0xE000);
                }
            }
            else
            {
                Switch32KPRG(exreg >> 4 & 0x3);
            }
        }
        protected override void SetupCHR()
        {
            int r = (exreg & 0xC0)<<1;
            if (!chrmode)
            {
                base.Switch01kCHR((chrRegs[0] & 0x7F) | r, 0x0000);
                base.Switch01kCHR(((chrRegs[0] + 1) & 0x7F) | r, 0x0400);
                base.Switch01kCHR((chrRegs[1] & 0x7F) | r, 0x0800);
                base.Switch01kCHR(((chrRegs[1] + 1) & 0x7F) | r, 0x0C00);
                base.Switch01kCHR((chrRegs[2] & 0x7F) | r, 0x1000);
                base.Switch01kCHR((chrRegs[3] & 0x7F) | r, 0x1400);
                base.Switch01kCHR((chrRegs[4] & 0x7F) | r, 0x1800);
                base.Switch01kCHR((chrRegs[5] & 0x7F) | r, 0x1C00);
            }
            else
            {
                base.Switch01kCHR((chrRegs[0] & 0x7F) | r, 0x1000);
                base.Switch01kCHR(((chrRegs[0] + 1) & 0x7F) | r, 0x1400);
                base.Switch01kCHR((chrRegs[1] & 0x7F) | r, 0x1800);
                base.Switch01kCHR(((chrRegs[1] + 1) & 0x7F) | r, 0x1C00);
                base.Switch01kCHR((chrRegs[2] & 0x7F) | r, 0x0000);
                base.Switch01kCHR((chrRegs[3] & 0x7F) | r, 0x0400);
                base.Switch01kCHR((chrRegs[4] & 0x7F) | r, 0x0800);
                base.Switch01kCHR((chrRegs[5] & 0x7F) | r, 0x0C00);
            }
        }
        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(exreg);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            exreg = stream.ReadByte();
        }
    }
}
