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
/*Written by Ala*/
using MyNes.Core.Types;
namespace MyNes.Core.Boards.Nintendo
{
    [BoardName("Pirate MMC3 variant", 74)]
    class Mapper074 : MMC3
    {
        public Mapper074() : base() { }
        public Mapper074(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }
       
        private byte[] exram = new byte[0x1000];

        public override void Initialize()
        {
            base.Initialize();
            Nes.CpuMemory.Hook(0x5000, 0x5FFF, PeekExram, PokeExram);
            // add 2k bytes as chr ram
            System.Array.Resize(ref chr, chr.Length + 0x2000);
        }
        public override void HardReset()
        {
            base.HardReset(); 
            exram = new byte[0x1000];
        }
        private void PokeExram(int address, byte data)
        {
            exram[address - 0x5000] = data;
        }
        private byte PeekExram(int address)
        {
            return exram[address - 0x5000];
        }
        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(exram);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            stream.Read(exram);
        }
    }
}
