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
namespace MyNes.Core.APU
{
    public class ChannelNoi : Channel
    {
        private static readonly int[][] FrequencyTable = 
        { 
            new int [] //NTSC
            {  
                0x002,0x004,0x008,0x010,0x020,0x030,0x040,0x050,
                0x065,0x07F,0x0BE,0x0FE,0x17D,0x1FC,0x3F9,0x7F2
            },
            new int [] //PAL
            {  
                0x002,0x003,0x007,0x00F,0x01E,0x02C,0x03B,0x04A,
	            0x05E,0x076,0x0B1,0x0EC,0x162,0x1D8,0x3B1,0x761
            },
            new int [] //DENDY (same as ntsc for now)
            {  
                0x002,0x004,0x008,0x010,0x020,0x030,0x040,0x050,
                0x065,0x07F,0x0BE,0x0FE,0x17D,0x1FC,0x3F9,0x7F2
            }
        };

        private bool ModeFlag = false;
        private int systemIndex = 0;
        private int ShiftRegister = 1;

        public ChannelNoi(TimingInfo.System system)
            : base(system) { }

        protected override void PokeReg3(int address, byte data)
        {
            timing.single = GetCycles(FrequencyTable[systemIndex][data & 0x0F]);
            ModeFlag = (data & 0x80) == 0x80;
        }

        public override byte GetSample()
        {
            if (DurationCounter > 0 && (ShiftRegister & 1) == 0)
                return EnvelopeSound;
            return 0;
        }

        public override void Update()
        {
            if (ModeFlag)
                ShiftRegister = (ShiftRegister << 1) | (((ShiftRegister >> 14) ^ (ShiftRegister >> 8)) & 0x1);
            else
                ShiftRegister = (ShiftRegister << 1) | (((ShiftRegister >> 14) ^ (ShiftRegister >> 13)) & 0x1);
        }
        public override void Initialize()
        {
            HardReset();
            base.Initialize();
        }
        public override void HardReset()
        {
            if (system.Master == TimingInfo.NTSC.Master)
                systemIndex = 0;
            else if (system.Master == TimingInfo.PALB.Master)
                systemIndex = 1;
            else if (system.Master == TimingInfo.Dendy.Master)
                systemIndex = 2;
            ModeFlag = false;
            ShiftRegister = 1;
            base.HardReset();
        }
        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(ModeFlag);
            stream.Write(ShiftRegister);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            ModeFlag = stream.ReadBoolean();
            ShiftRegister = stream.ReadInt32();
        }
    }
}