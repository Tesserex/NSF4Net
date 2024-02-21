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
namespace MyNes.Core.Boards.Discreet
{
    [BoardName("Unknown", 164)]
    class Mapper164 : Board
    {
        public Mapper164() : base() { }
        public Mapper164(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int[] regs = new int[2];
        public override void Initialize()
        {
            base.Initialize();
            for (int i = 0x5000; i < 0x6000; i += 0x400)
                Nes.CpuMemory.Hook(i + 0x00, i + 0x1FF, Poke5000);
        }
        public override void HardReset()
        {
            base.HardReset();

            regs = new int[2];
            regs[0] = 0xFFF;
            regs[1] = 0x00;
            Poke5000(0x5000, 0);
        }
        private void Poke5000(int address, byte data)
        {
            address = address >> 8 & 0x1;

            if (regs[address] != data)
            {
                regs[address] = data;
                data = (byte)((regs[1] << 5) & 0x20);

                switch (regs[0] & 0x70)
                {
                    case 0x00:
                    case 0x20:
                    case 0x40:
                    case 0x60:
                        Switch16KPRG(data | (regs[0] >> 1 & 0x10) | (regs[0] & 0xF), 0x8000);
                        Switch16KPRG(data | 0x1F, 0xC000);
                        break;

                    case 0x50:

                        Switch32KPRG((data >> 1) | (regs[0] & 0xF));
                        break;

                    case 0x70:
                        Switch16KPRG(data | (regs[0] << 1 & 0x10) | (regs[0] & 0xF), 0x8000);
                        Switch16KPRG(data | 0x1F, 0xC000);
                        break;
                }
            }
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(regs);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(regs);
        }
    }
}
