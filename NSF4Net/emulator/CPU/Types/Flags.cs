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
namespace MyNes.Core.CPU.Types
{
    public struct Flags
    {
        public bool n;
        public bool v;
        public bool d;
        public bool i;
        public bool z;
        public bool c;

        public static implicit operator Flags(byte value)
        {
            return new Flags
            {
                n = (value & 0x80) != 0,
                v = (value & 0x40) != 0,
                d = (value & 0x08) != 0,
                i = (value & 0x04) != 0,
                z = (value & 0x02) != 0,
                c = (value & 0x01) != 0
            };
        }
        public static implicit operator byte(Flags value)
        {
            return (byte)(
                (value.n ? 0x80 : 0) |
                (value.v ? 0x40 : 0) |
                (value.d ? 0x08 : 0) |
                (value.i ? 0x04 : 0) |
                (value.z ? 0x02 : 0) |
                (value.c ? 0x01 : 0) | 0x20);
        }
    }
}