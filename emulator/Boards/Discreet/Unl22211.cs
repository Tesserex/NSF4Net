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
    [BoardName("Unl22211", 172)]
    class Unl22211 : Board
    {
        public Unl22211() : base() { }
        public Unl22211(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private int[] regs = new int[4];
        protected int type = 0;

        public override void Initialize()
        {
            base.Initialize();

            Nes.CpuMemory.Hook(0x4100, Peek4100);
            Nes.CpuMemory.Hook(0x4100, 0x4103, Poke4100);
            Nes.CpuMemory.Hook(0x8000, 0xFFFF, Poke8000);
        }
        public override void HardReset()
        {
            base.HardReset();

            regs = new int[4];
            type = 0;
        }
        private byte Peek4100(int address)
        {
            return (byte)(((regs[1]) ^ regs[2]) | (type == 1 ? 0x41 : 0x40));
        }
        private void Poke4100(int address, byte data)
        {
            regs[address & 0x3] = data;
        }
        private void Poke8000(int address, byte data)
        {
            Switch32KPRG(regs[2] >> 2);
            Switch08kCHR((type == 0) ? ((data ^ regs[2]) >> 3 & 0x2) | ((data ^ regs[2]) >> 5 & 0x1) : regs[2]);
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
