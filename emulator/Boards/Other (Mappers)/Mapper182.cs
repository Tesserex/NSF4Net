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
namespace MyNes.Core.Boards.Discreet
{
    [BoardName("Unknown", 182)]
    class Mapper182 : MMC3
    {
        public Mapper182() : base() { }
        public Mapper182(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xE001)
            {
                case 0x8000: break;
                case 0x8001: PokeA000(address, data); break;
                case 0xA000: Poke8000(address, data); break;
                case 0xA001: break;
                case 0xC000: Poke8001(address, data); break;
                case 0xC001: PokeC000(address, data); PokeC001(address, data); break;
                case 0xE000: PokeE000(address, data); break;
                case 0xE001: PokeE001(address, data); break;
            }
        }
        protected override void Poke8001(int address, byte data)
        {
            switch (addrSelect)
            {
                case 0: chrRegs[0] = data; SetupCHR(); break;
                case 1: chrRegs[3] = data; SetupCHR(); break;
                case 2: chrRegs[1] = data; SetupCHR(); break;
                case 3: chrRegs[5] = data; SetupCHR(); break;
                case 4: prgRegs[0] = (byte)(data & 0x3F); SetupPRG(); break;
                case 5: prgRegs[1] = (byte)(data & 0x3F); SetupPRG(); break;
                case 6: chrRegs[2] = data; SetupCHR(); break;
                case 7: chrRegs[4] = data; SetupCHR(); break;
            }
        }
    }
}
