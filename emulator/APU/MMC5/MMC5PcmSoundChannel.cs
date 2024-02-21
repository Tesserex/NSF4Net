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
    class MMC5PcmSoundChannel : APU.Channel
    {
        public MMC5PcmSoundChannel(TimingInfo.System system)
            : base(system) { }
        private byte output = 0;
        private bool enableOutput = false;

        //By overriding these methods the counters disabled
        public override void ClockDuration()
        {
        }
        public override void ClockEnvelope()
        {
        }
        public override void ClockSingle(bool isClockingLength)
        {
        }
        protected override void PokeReg1(int address, byte data)
        {
            enableOutput = (data & 1) == 1;
        }
        protected override void PokeReg2(int address, byte data)
        {
            output = data;
        }
        protected override void PokeReg3(int address, byte data)
        {
        }
        protected override void PokeReg4(int address, byte data)
        {
        }

        public override void Initialize()
        {
            HardReset();
            base.Initialize();
        }
        public override void HardReset()
        {
            output = 0;
            enableOutput = false;
            base.HardReset();
        }

        public override byte GetSample()
        {
            if (enableOutput)
                return output;
            return 0;
        }
        public override void SaveState(Types.StateStream stream)
        {
            stream.Write(output);
            stream.Write(enableOutput);
        }
        public override void LoadState(Types.StateStream stream)
        {
            output = stream.ReadByte();
            enableOutput = stream.ReadBoolean();
        }
    }
}
