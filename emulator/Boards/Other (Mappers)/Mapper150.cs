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
    [BoardName("Unknown", 150)]
    class Mapper150 : Board
    {
        public Mapper150() : base() { }
        public Mapper150(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int command = 0;
        private int dip = 0;
        public override void Initialize()
        {
            base.Initialize();

            for (int i = 0x4100; i < 0x6000; ++i)
            {
                switch (i & 0x4101)
                {
                    case 0x4100: Nes.CpuMemory.Hook(i, Peek4100, Poke4100); break;
                    case 0x4101: Nes.CpuMemory.Hook(i, Peek4100, Poke4101); break;
                }
            }
        }
        public override void HardReset()
        {
            base.HardReset();
            command = 0;
            dip = 1;
        }
        public override void SoftReset()
        {
            base.SoftReset();
            dip ^= 1;
        }
        private byte Peek4100(int address)
        {
            return (byte)((~(command & 0x7) & 0x3F) ^ dip);
        }
        private void Poke4100(int address, byte data)
        {
            command = data;
        }
        private void Poke4101(int address, byte data)
        {
            int[] banks = { ~0, ~0 };

            switch (command & 0x7)
            {
                case 0x2:

                    banks[0] = data & 0x1;
                    banks[1] = ((chrPage[0] >> 13) & ~0x8) | (data << 3 & 0x8);
                    break;

                case 0x4:

                    banks[1] = ((chrPage[0] >> 13) & ~0x4) | (data << 2 & 0x4);
                    break;

                case 0x5:

                    banks[0] = data & 0x7;
                    break;

                case 0x6:

                    banks[1] = ((chrPage[0] >> 13) & ~0x3) | (data << 0 & 0x3);
                    break;

                case 0x7:
                    {
                        byte[] mirr = { 5, 3, 7, 0 };
                        Nes.PpuMemory.SwitchMirroring(mirr[data >> 1 & 0x3]);
                        return;
                    }
            }

            if (banks[0] != ~0)
                Switch32KPRG(banks[0]);

            if (banks[1] != ~0)
            {
                Switch08kCHR(banks[1]);
            }
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(command);
            stream.Write(dip);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            command = stream.ReadInt32();
            dip = stream.ReadInt32();
        }
    }
}
