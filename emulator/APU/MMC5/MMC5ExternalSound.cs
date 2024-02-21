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
    class MMC5ExternalSound : IApuExternalChannelsMixer
    {
        public MMC5ExternalSound()
        {
            soundChn1 = new MMC5SqrSoundChannel(Nes.emuSystem);
            soundChn1.Hook(0x5000);

            soundChn2 = new MMC5SqrSoundChannel(Nes.emuSystem);
            soundChn2.Hook(0x5004);

            soundChn3 = new MMC5PcmSoundChannel(Nes.emuSystem);
            soundChn3.Hook(0x5010);
        }
        public MMC5SqrSoundChannel soundChn1;
        public MMC5SqrSoundChannel soundChn2;
        public MMC5PcmSoundChannel soundChn3;
        public short Mix()
        {
            short output = soundChn1.GetSample();
            output += soundChn2.GetSample();
            output += soundChn3.GetSample();
            return output;
        }

        public void HardReset()
        {
            soundChn1.HardReset();
            soundChn2.HardReset();
            soundChn3.HardReset();
        }

        public void SoftReset()
        {
            soundChn1.SoftReset();
            soundChn2.SoftReset();
            soundChn3.SoftReset();
        }

        public void ClockDuration()
        {
            soundChn1.ClockDuration();
            soundChn2.ClockDuration();
            soundChn3.ClockDuration();
        }

        public void ClockEnvelope()
        {
            soundChn1.ClockEnvelope();
            soundChn2.ClockEnvelope();
            soundChn3.ClockEnvelope();
        }

        public void ClockSingle(bool isClockingDuration)
        {
            soundChn1.ClockSingle(isClockingDuration);
            soundChn2.ClockSingle(isClockingDuration);
            soundChn3.ClockSingle(isClockingDuration);
        }

        public void Update(int cycles)
        {
            soundChn1.Update(cycles);
            soundChn2.Update(cycles);
            soundChn3.Update(cycles);
        }

        public void SaveState(Types.StateStream stream)
        {
            soundChn1.SaveState(stream);
            soundChn2.SaveState(stream);
            soundChn3.SaveState(stream);
        }

        public void LoadState(Types.StateStream stream)
        {
            soundChn1.LoadState(stream);
            soundChn2.LoadState(stream);
            soundChn3.LoadState(stream);
        }
    }
}
