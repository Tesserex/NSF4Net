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
    public class ChannelTri : Channel
    {
        public ChannelTri(TimingInfo.System system)
            : base(system) { }
        private static readonly byte[] StepSequence =
        {
            0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00,
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
        };
        private byte linearCounter = 0;
        private byte LinearCounterReload;
        private byte output;
        private byte Step;
        private bool LinearCounterHalt;
        private bool HALT;

        public override void ClockEnvelope()
        {
            if (HALT)
            {
                linearCounter = LinearCounterReload;
            }
            else
            {
                if (linearCounter != 0)
                {
                    linearCounter--;
                }
            }

            HALT &= LinearCounterHalt;
        }
        private void UpdateFrequency()
        {
            int time = (frequency + 1) / 2;
            if (time == 0)
                time = 1;
            timing.single = GetCycles(time);
        }
        protected override void PokeReg1(int address, byte data)
        {
            LinearCounterHalt = DurationHaltRequset = (data & 0x80) != 0;
            LinearCounterReload = (byte)(data & 0x7F);
        }
        protected override void PokeReg3(int address, byte data)
        {
            frequency = (frequency & 0x700) | data;
            UpdateFrequency();
        }
        protected override void PokeReg4(int address, byte data)
        {
            frequency = (frequency & 0x00FF) | ((data & 7) << 8);
            UpdateFrequency();

            DurationReload = DurationTable[data >> 3];
            DurationReloadRequst = true;
            HALT = true;
        }

        public override byte GetSample()
        {
            return output;
        }
        public override void Update()
        {
            if (DurationCounter > 0 && linearCounter > 0)
            {
                if (frequency >= 4)
                {
                    Step++;
                    Step &= 0x1F;
                    output = StepSequence[Step];
                }
            }
        }

        public override void Initialize()
        {
            HardReset();
            base.Initialize();
        }
        public override void HardReset()
        {
            linearCounter = 0;
            LinearCounterReload = 0;
            output = 0;
            Step = 0;
            LinearCounterHalt = false;
            HALT = false;
            base.HardReset();
        }
        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(linearCounter);
            stream.Write(LinearCounterReload);
            stream.Write(output);
            stream.Write(Step);
            stream.Write(LinearCounterHalt, HALT);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            linearCounter = stream.ReadByte();
            LinearCounterReload = stream.ReadByte();
            output = stream.ReadByte();
            Step = stream.ReadByte();
            bool[] flags = stream.ReadBooleans();
            LinearCounterHalt = flags[0];
            HALT = flags[0];
        }
    }
}