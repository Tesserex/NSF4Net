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
    [BoardName("Pirate MMC3 variant", 191)]
    class Mapper191 : MMC3
    {
        public Mapper191() : base() { }
        public Mapper191(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private byte[] chrRam = new byte[1024 * 2];

        public override void HardReset()
        {
            base.HardReset();

            chrRam = new byte[1024 * 2];
        }
        protected override byte PeekChr(int address)
        {
            int newAddress = DecodeChrAddress(address);
            if (newAddress < chr.Length)
                return base.PeekChr(address);
            else
                return chrRam[newAddress - chr.Length];
        }
        protected override void SetupCHR()
        {
            if (!chrmode)
            {
                base.Switch01kCHR((chrRegs[0] & 0x80) == 0x80 ? (chrRegs[0] + 0) : (chr.Length >> 10), 0x0000);
                base.Switch01kCHR((chrRegs[0] & 0x80) == 0x80 ? (chrRegs[0] + 1) : (chr.Length >> 10), 0x0400);
                base.Switch01kCHR((chrRegs[1] & 0x80) == 0x80 ? (chrRegs[1] + 0) : (chr.Length >> 10), 0x0800);
                base.Switch01kCHR((chrRegs[1] & 0x80) == 0x80 ? (chrRegs[1] + 1) : (chr.Length >> 10), 0x0C00);
                base.Switch01kCHR((chrRegs[2] & 0x80) == 0x80 ? chrRegs[2] : (chr.Length >> 10), 0x1000);
                base.Switch01kCHR((chrRegs[3] & 0x80) == 0x80 ? chrRegs[3] : (chr.Length >> 10), 0x1400);
                base.Switch01kCHR((chrRegs[4] & 0x80) == 0x80 ? chrRegs[4] : (chr.Length >> 10), 0x1800);
                base.Switch01kCHR((chrRegs[5] & 0x80) == 0x80 ? chrRegs[5] : (chr.Length >> 10), 0x1C00);
            }
            else
            {
                base.Switch01kCHR((chrRegs[0] & 0x80) == 0x80 ? (chrRegs[0] + 0) : (chr.Length >> 10), 0x1000);
                base.Switch01kCHR((chrRegs[0] & 0x80) == 0x80 ? (chrRegs[0] + 1) : (chr.Length >> 10), 0x1400);
                base.Switch01kCHR((chrRegs[1] & 0x80) == 0x80 ? (chrRegs[1] + 0) : (chr.Length >> 10), 0x1800);
                base.Switch01kCHR((chrRegs[1] & 0x80) == 0x80 ? (chrRegs[1] + 1) : (chr.Length >> 10), 0x1C00);
                base.Switch01kCHR((chrRegs[2] & 0x80) == 0x80 ? chrRegs[2] : (chr.Length >> 10), 0x0000);
                base.Switch01kCHR((chrRegs[3] & 0x80) == 0x80 ? chrRegs[3] : (chr.Length >> 10), 0x0400);
                base.Switch01kCHR((chrRegs[4] & 0x80) == 0x80 ? chrRegs[4] : (chr.Length >> 10), 0x0800);
                base.Switch01kCHR((chrRegs[5] & 0x80) == 0x80 ? chrRegs[5] : (chr.Length >> 10), 0x0C00);
            }
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(chrRam);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream); 
            stream.Read(chrRam);
        }
    }
}
