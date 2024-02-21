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
using MyNes.Core.Types;
namespace MyNes.Core
{
    public class Memory
    {
        private PeekRegister[] peek;
        private PokeRegister[] poke;
        private int size;
        private int mask;

        public int Length { get { return size; } }

        public byte this[int address]
        {
            get { return peek[address &= mask](address); }
            set { poke[address &= mask](address, value); }
        }

        public Memory(int size)
        {
            this.peek = new PeekRegister[size];
            this.poke = new PokeRegister[size];
            this.size = size;
            this.mask = size - 1;
            this.Hook(0, size - 1,
                delegate { return 0; },
                delegate { });
        }

        public void Hook(int address, PeekRegister peek) { this.peek[address] = peek; }
        public void Hook(int address, PokeRegister poke) { this.poke[address] = poke; }
        public void Hook(int address, PeekRegister peek, PokeRegister poke)
        {
            Hook(address, peek);
            Hook(address, poke);
        }
        public void Hook(int address, int last, PeekRegister peek)
        {
            for (; address <= last; address++)
                Hook(address, peek);
        }
        public void Hook(int address, int last, PokeRegister poke)
        {
            for (; address <= last; address++)
                Hook(address, poke);
        }
        public void Hook(int address, int last, PeekRegister peek, PokeRegister poke)
        {
            for (; address <= last; address++)
                Hook(address, peek, poke);
        }

        public virtual byte DebugPeek(int address)
        {
            return this[address];
        }
        public virtual void DebugPoke(int address, byte data)
        {
            this[address] = data;
        }

        public virtual void Initialize() { }
        public virtual void Shutdown() { }
        public virtual void HardReset() { }
        public virtual void SoftReset() { }
        public virtual void SaveState(StateStream stream) { }
        public virtual void LoadState(StateStream stream) { }
    }
}