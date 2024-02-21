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
    [BoardName("VRC1", 75)]
    class VRC1 : Board
    {
        public VRC1() : base() { }
        public VRC1(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int[] chrRegs = new int[2];

        public override void HardReset()
        {
            base.HardReset();

            Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xF000)
            {
                case 0x8000: Switch08KPRG(data, 0x8000); break;
                case 0xA000: Switch08KPRG(data, 0xA000); break;
                case 0xC000: Switch08KPRG(data, 0xC000); break;

                case 0x9000: 
                    chrRegs[0] = (chrRegs[0] & 0x0F) | ((data & 0x2) << 3);
                    chrRegs[1] = (chrRegs[1] & 0x0F) | ((data & 0x4) << 2);
                    Nes.PpuMemory.SwitchMirroring((data & 1) == 1 ? Mirroring.ModeHorz : Mirroring.ModeVert);
                    SetupCHR();
                    break;
                case 0xE000: chrRegs[0] = (chrRegs[0] & 0x10) | (data & 0xF); SetupCHR(); break;
                case 0xF000: chrRegs[1] = (chrRegs[1] & 0x10) | (data & 0xF); SetupCHR(); break;
            }
        }
        private void SetupCHR()
        {
            Switch04kCHR(chrRegs[0], 0x0000); 
            Switch04kCHR(chrRegs[1], 0x1000);
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(chrRegs);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(chrRegs);
        }
    }
}
