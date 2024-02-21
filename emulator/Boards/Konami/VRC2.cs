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
namespace MyNes.Core.Boards.Konami
{
    [BoardName("VRC2a", 22)]
    class VRC2 : Board
    {
        public VRC2() : base() { }
        public VRC2(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private byte[] chrRegs = new byte[8];

        public override void HardReset()
        {
            // Switch 32KB prg bank at 0x8000
            // Switch 08KB chr bank at 0x0000
            base.HardReset();

            base.Switch16KPRG((prg.Length - 0x4000) >> 14, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address)
            {
                case 0x8000:
                case 0x8001:
                case 0x8002:
                case 0x8003: Switch08KPRG(data & 0x0F, 0x8000); break;

                case 0x9000:
                case 0x9001:
                case 0x9002:
                case 0x9003:
                    switch (data & 0x3)
                    {
                        case 0: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.ModeVert); break;
                        case 1: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.ModeHorz); break;
                        case 2: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.Mode1ScA); break;
                        case 3: Nes.PpuMemory.SwitchMirroring(Types.Mirroring.Mode1ScB); break;
                    }
                    break;

                case 0xA000:
                case 0xA001:
                case 0xA002:
                case 0xA003: Switch08KPRG(data & 0x0F, 0xA000); break;

                case 0xB000: chrRegs[0] = (byte)((chrRegs[0] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[0] >> 1, 0x0000); break;
                case 0xB002: chrRegs[0] = (byte)((chrRegs[0] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[0] >> 1, 0x0000); break;
                case 0xB001: chrRegs[1] = (byte)((chrRegs[1] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[1] >> 1, 0x0400); break;
                case 0xB003: chrRegs[1] = (byte)((chrRegs[1] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[1] >> 1, 0x0400); break;
                case 0xC000: chrRegs[2] = (byte)((chrRegs[2] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[2] >> 1, 0x0800); break;
                case 0xC002: chrRegs[2] = (byte)((chrRegs[2] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[2] >> 1, 0x0800); break;
                case 0xC001: chrRegs[3] = (byte)((chrRegs[3] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[3] >> 1, 0x0C00); break;
                case 0xC003: chrRegs[3] = (byte)((chrRegs[3] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[3] >> 1, 0x0C00); break;

                case 0xD000: chrRegs[4] = (byte)((chrRegs[4] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[4] >> 1, 0x1000); break;
                case 0xD002: chrRegs[4] = (byte)((chrRegs[4] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[4] >> 1, 0x1000); break;
                case 0xD001: chrRegs[5] = (byte)((chrRegs[5] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[5] >> 1, 0x1400); break;
                case 0xD003: chrRegs[5] = (byte)((chrRegs[5] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[5] >> 1, 0x1400); break;
                case 0xE000: chrRegs[6] = (byte)((chrRegs[6] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[6] >> 1, 0x1800); break;
                case 0xE002: chrRegs[6] = (byte)((chrRegs[6] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[6] >> 1, 0x1800); break;
                case 0xE001: chrRegs[7] = (byte)((chrRegs[7] & 0xF0) | (data & 0x0F) << 0); Switch01kCHR(chrRegs[7] >> 1, 0x1C00); break;
                case 0xE003: chrRegs[7] = (byte)((chrRegs[7] & 0x0F) | (data & 0x0F) << 4); Switch01kCHR(chrRegs[7] >> 1, 0x1C00); break;
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(chrRegs);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(chrRegs);
        }
    }
}
