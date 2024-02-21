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
    [BoardName("MMC3 Variant", 189)]
    class Mapper189 : MMC3
    {
        public Mapper189() : base() { }
        public Mapper189(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }
        public override void Initialize()
        {
            base.Initialize();
            Nes.CpuMemory.Hook(0x4120, 0x7FFF, PokeAB);
        }
        public override void HardReset()
        {
            base.HardReset();
            Switch32KPRG(0);
        }
        protected override void Poke8001(int address, byte data)
        {
            switch (addrSelect)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5: chrRegs[addrSelect] = data; SetupCHR(); break;
                // case 6: prgRegs[0] = (byte)(data & 0x3F); SetupPRG(); break;
                // case 7: prgRegs[1] = (byte)(data & 0x3F); SetupPRG(); break;
            }
        }
        private void PokeAB(int address, byte data)
        {
            Switch32KPRG((data >> 4) | data);
        }
        protected override void SetupPRG()
        {
            // disable this for 0x4120-0x7FFF control
        }
    }
}
