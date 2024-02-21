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
namespace MyNes.Core.Boards.Other__Mappers_
{
    [BoardName("Unknown", 233)]
    class Mapper233 : Board
    {
        public Mapper233() : base() { }
        public Mapper233(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int title = 0;

        public override void HardReset()
        {
            base.HardReset();

            title = 0;
            Nes.CpuMemory[0x8000] = 0;
        }
        public override void SoftReset()
        {
            base.SoftReset();
            title ^= 0x20;
            Nes.CpuMemory[0x8000] = 0;
        }
        protected override void PokePrg(int address, byte data)
        {
            int bank = data & 0x1F;

            if ((data & 0x20) == 0x20)
            {
                Switch16KPRG(title | bank, 0x8000);
                Switch16KPRG(title | bank, 0xC000);
            }
            else
                Switch32KPRG(title >> 1 | bank >> 1);

            switch ((data >> 6) & 0x3)
            {
                case 0: Nes.PpuMemory.SwitchMirroring(0x80); break;
                case 1: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeVert); break;
                case 2: Nes.PpuMemory.SwitchMirroring(Mirroring.ModeHorz); break;
                case 3: Nes.PpuMemory.SwitchMirroring(Mirroring.Mode1ScB); break;
            }
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(title);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            title = stream.ReadInt32();
        }
    }
}
