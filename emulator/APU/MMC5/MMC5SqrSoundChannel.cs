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
using MyNes.Core.APU;

namespace MyNes.Core.APU.MMC5
{
    class MMC5SqrSoundChannel : APU.Channel
    {
        public MMC5SqrSoundChannel(TimingInfo.System system)
            : base(system) { }
        private static int[][] DutyForms =
        {
            new int[] { 1, 1, 1, 1, 1, 1, 1, 0 }, // 87.5%
            new int[] { 1, 1, 1, 1, 1, 1, 0, 0 }, // 75.0%
            new int[] { 1, 1, 1, 1, 0, 0, 0, 0 }, // 50.0%
            new int[] { 1, 1, 0, 0, 0, 0, 0, 0 }, // 25.0%
        };
        private int dutyForm;
        private int dutyStep;
        private byte output = 0;

        private void UpdateFrequency()
        {
            timing.single = GetCycles((frequency + 1));
        }
        private bool IsValidFrequency()
        {
            return (frequency >= 0x08 && frequency <= 0x7FF);
        }
        protected override void PokeReg1(int address, byte data)
        {
            base.PokeReg1(address, data);
            dutyForm = (data & 0xC0) >> 6;
        }
        protected override void PokeReg3(int address, byte data)
        {
            frequency = (frequency & 0x700) | data;

            UpdateFrequency();
        }
        protected override void PokeReg4(int address, byte data)
        {
            base.PokeReg4(address, data);
            frequency = (frequency & 0x00FF) | ((data & 0x07) << 8);
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
        public override byte GetSample()
        {
            if (DurationCounter > 0 && IsValidFrequency())
                return output;
            return 0;
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
            output = 0;
            base.HardReset();
        }
        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(dutyForm);
            stream.Write(dutyStep);
            stream.Write(output);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            dutyForm = stream.ReadInt32();
            dutyStep = stream.ReadInt32();
            output = stream.ReadByte();
        }
    }
}
