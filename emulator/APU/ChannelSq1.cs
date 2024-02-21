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
    public class ChannelSq1 : Channel
    {
        public ChannelSq1(TimingInfo.System system)
            : base(system) { }
        private static readonly byte[][] DutyForms =
        {
            new byte[] {  0, 1, 0, 0, 0, 0, 0, 0 }, // 12.5%
            new byte[] {  0, 1, 1, 0, 0, 0, 0, 0 }, // 25.0%
            new byte[] {  0, 1, 1, 1, 1, 0, 0, 0 }, // 50.0%
            new byte[] {  1, 0, 0, 1, 1, 1, 1, 1 }, // 75.0% (25.0% negated)
        };
        private int dutyForm;
        private int dutyStep;
        private int SweepDeviderPeriod = 0;
        private int SweepShiftCount = 0;
        private int SweepCounter = 0;
        private byte output = 0;
        private bool SweepEnable = false;
        private bool SweepReload = false;
        private bool SweepNegateFlag = false;

        private void ClockSweep()
        {
            SweepCounter--;
            if (SweepCounter == 0)
            {
                SweepCounter = SweepDeviderPeriod + 1;
                if (SweepEnable && (SweepShiftCount > 0) && IsValidFrequency())
                {
                    int sweep = frequency >> SweepShiftCount;
                    frequency += SweepNegateFlag ? ~sweep : sweep;
                }
            }
            if (SweepReload)
            {
                SweepReload = false;
                SweepCounter = SweepDeviderPeriod + 1;
            }
            UpdateFrequency();
        }
        public override void ClockDuration()
        {
            base.ClockDuration();
            ClockSweep();
        }
        private void UpdateFrequency()
        {
            timing.single = GetCycles((frequency + 1));
        }
        private bool IsValidFrequency()
        {
            return (frequency >= 0x8) && ((SweepNegateFlag) || (((frequency + (frequency >> SweepShiftCount)) & 0x800) == 0));
        }
        public override byte GetSample()
        {
            if (DurationCounter > 0 && IsValidFrequency())
                return output;
            return 0;
        }
        protected override void PokeReg1(int address, byte data)
        {
            base.PokeReg1(address, data);//envelope and duration controls
            dutyForm = (data & 0xC0) >> 6;
        }
        protected override void PokeReg2(int address, byte data)
        {
            SweepEnable = (data & 0x80) == 0x80;
            SweepDeviderPeriod = (data >> 4) & 7;
            SweepNegateFlag = (data & 0x8) == 0x8;
            SweepShiftCount = data & 7;
            SweepReload = true;
        }
        protected override void PokeReg3(int address, byte data)
        {
            frequency = (frequency & 0x700) | data;

            UpdateFrequency();
        }
        protected override void PokeReg4(int address, byte data)
        {
            base.PokeReg4(address, data);//duration
            frequency = (frequency & 0x00FF) | ((data & 7) << 8);

            UpdateFrequency();
            dutyStep = 0;
        }
        public override void Update()
        {
            dutyStep = (dutyStep + 1) & 0x7;
            if (DurationCounter > 0 && IsValidFrequency())
            {
                output = (byte)(DutyForms[dutyForm][dutyStep] * EnvelopeSound);
            }
            else
                output = 0;
        }

        public override void Initialize()
        {
            HardReset();
            base.Initialize();
        }
        public override void HardReset()
        {
            dutyForm = 0;
            dutyStep = 0;
            SweepDeviderPeriod = 0;
            SweepShiftCount = 0;
            SweepCounter = 0;
            output = 0;
            SweepEnable = false;
            SweepReload = false;
            SweepNegateFlag = false;
            base.HardReset();
        }
        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(dutyForm);
            stream.Write(dutyStep);
            stream.Write(SweepDeviderPeriod);
            stream.Write(SweepShiftCount);
            stream.Write(SweepCounter);
            stream.Write(output);
            stream.Write(SweepEnable, SweepReload, SweepNegateFlag);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            dutyForm = stream.ReadInt32();
            dutyStep = stream.ReadInt32();
            SweepDeviderPeriod = stream.ReadInt32();
            SweepShiftCount = stream.ReadInt32();
            SweepCounter = stream.ReadInt32();
            output = stream.ReadByte();
            bool[] flags = stream.ReadBooleans();
            SweepEnable = flags[0];
            SweepReload = flags[1];
            SweepNegateFlag = flags[2];
        }
    }
}