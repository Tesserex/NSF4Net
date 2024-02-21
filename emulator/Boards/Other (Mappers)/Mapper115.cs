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
namespace MyNes.Core.Boards.Nintendo
{
    [BoardName("MMC3 variant", 115)]
    class Mapper115 : MMC3
    {
        public Mapper115() : base() { }
        public Mapper115(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool PRGMode = false;
        private int PrgPageOf6000 = 0;
        private int chrOR = 0;

        protected override void PokeSram(int address, byte data)
        {
            switch (address & 0x6001)
            {
                case 0x6000: PRGMode = (data & 0x80) == 0x80; PrgPageOf6000 = data & 0xF; SetupPRG(); break;
                case 0x6001: chrOR = (data & 1) << 8; SetupCHR(); break;
            }
        }
        protected override void SetupPRG()
        {
            if (!PRGMode)
            {
                if (!prgmode)
                {
                    base.Switch08KPRG(prgRegs[0], 0x8000);
                    base.Switch08KPRG(prgRegs[1], 0xA000);
                    base.Switch08KPRG(prgRegs[2], 0xC000);
                    base.Switch08KPRG(prgRegs[3], 0xE000);
                }
                else
                {
                    base.Switch08KPRG(prgRegs[2], 0x8000);
                    base.Switch08KPRG(prgRegs[1], 0xA000);
                    base.Switch08KPRG(prgRegs[0], 0xC000);
                    base.Switch08KPRG(prgRegs[3], 0xE000);
                }
            }
            else
            {
                Switch16KPRG(PrgPageOf6000, 0x8000);
                if (!prgmode)
                {
                    base.Switch08KPRG(prgRegs[2], 0xC000);
                    base.Switch08KPRG(prgRegs[3], 0xE000);
                }
                else
                {
                    base.Switch08KPRG(prgRegs[0], 0xC000);
                    base.Switch08KPRG(prgRegs[3], 0xE000);
                }
            }
        }
        protected override void SetupCHR()
        {
            if (!chrmode)
            {
                base.Switch01kCHR(chrRegs[0] | chrOR, 0x0000);
                base.Switch01kCHR((chrRegs[0] + 1) | chrOR, 0x0400);
                base.Switch01kCHR(chrRegs[1] | chrOR, 0x0800);
                base.Switch01kCHR((chrRegs[1] + 1) | chrOR, 0x0C00);
                base.Switch01kCHR(chrRegs[2] | chrOR, 0x1000);
                base.Switch01kCHR(chrRegs[3] | chrOR, 0x1400);
                base.Switch01kCHR(chrRegs[4] | chrOR, 0x1800);
                base.Switch01kCHR(chrRegs[5] | chrOR, 0x1C00);
            }
            else
            {
                base.Switch01kCHR(chrRegs[0] | chrOR, 0x1000);
                base.Switch01kCHR((chrRegs[0] + 1) | chrOR, 0x1400);
                base.Switch01kCHR(chrRegs[1] | chrOR, 0x1800);
                base.Switch01kCHR((chrRegs[1] + 1) | chrOR, 0x1C00);
                base.Switch01kCHR(chrRegs[2] | chrOR, 0x0000);
                base.Switch01kCHR(chrRegs[3] | chrOR, 0x0400);
                base.Switch01kCHR(chrRegs[4] | chrOR, 0x0800);
                base.Switch01kCHR(chrRegs[5] | chrOR, 0x0C00);
            }
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(PRGMode);
            stream.Write(PrgPageOf6000);
            stream.Write(chrOR);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            PRGMode = stream.ReadBoolean();
            PrgPageOf6000 = stream.ReadInt32();
            chrOR = stream.ReadInt32();
        }
    }
}
