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
    public struct TimingInfo
    {
        public static readonly System NTSC = new System("NTSC", 0x0, 236250000, 132, 44, 264); // 1.78977267~ MHz
        public static readonly System PALB = new System("PALB", 0x1, 212813696, 128, 40, 256); // 1.662607~ MHz
        public static readonly System Dendy = new System("Dendy", 0x2, 228774792, 129, 43, 258); // 1.773448~ MHz

        public int cycles;
        public int period;
        public int single;

        public void SaveState(StateStream stream)
        {
            stream.Write(cycles);
            stream.Write(period);
            stream.Write(single);
        }
        public void LoadState(StateStream stream)
        {
            cycles = stream.ReadInt32();
            period = stream.ReadInt32();
            single = stream.ReadInt32();
        }

        public struct System
        {
            public string Name;
            public int Master;
            public int Serial;
            public int Cpu;
            public int Gpu;
            public int Spu;

            public System(string Name, int serial, int master, int cpu, int gpu, int spu)
            {
                this.Name = Name;
                this.Master = master;
                this.Serial = serial;
                this.Cpu = cpu;
                this.Gpu = gpu;
                this.Spu = spu;
            }
        }
    }
}