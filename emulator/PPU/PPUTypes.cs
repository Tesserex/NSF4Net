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
namespace MyNes.Core.PPU
{
    public class Fetch
    {
        public int addr;
        public int attr;
        public int bit0;
        public int bit1;
        public int name;
    }
    public class Scroll
    {
        public bool swap;
        public int addr;
        public int fine;
        public int step = 1;
        public int temp;

        public void ClockX()
        {
            if ((addr & 0x001F) == 0x001F)
                addr ^= 0x041F;
            else
                addr++;
        }
        public void ClockY()
        {
            if ((addr & 0x7000) != 0x7000)
            {
                addr += 0x1000;
            }
            else
            {
                addr ^= 0x7000;

                switch (addr & 0x3E0)
                {
                    case 0x3A0: addr ^= 0xBA0; break;
                    case 0x3E0: addr ^= 0x3E0; break;
                    default: addr += 0x20; break;
                }
            }
        }
        public void ResetX()
        {
            addr = (addr & ~0x41F) | (temp & 0x41F);
        }
        public void ResetY()
        {
            addr = temp;
        }
    }

    public struct Sprite
    {
        public byte y;
        public byte name;
        public byte attr;
        public byte x;
        public bool zero;
    }
    public class Unit
    {
        public bool clipped;
        public bool enabled;
        public int address;
        public int rasters = 8;
        public int[] pixels;

        public Unit(int capacity)
        {
            this.pixels = new int[capacity];
        }

        public int GetPixel(int hclock, int offset = 0)
        {
            if (!enabled || (clipped && hclock < 8))
                return 0;

            return pixels[hclock + offset];
        }
    }
}
