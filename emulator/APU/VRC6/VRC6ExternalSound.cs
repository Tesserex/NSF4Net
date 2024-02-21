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
namespace MyNes.Core.APU.VRC6
{
    class VRC6ExternalSound: IApuExternalChannelsMixer
    {
        public VRC6ExternalSound()
        {
            sndPulse1 = new VRC6pulseSoundChannel(Nes.emuSystem);
            sndPulse2 = new VRC6pulseSoundChannel(Nes.emuSystem);
            sndSawtooth = new VRC6sawtoothSoundChannel(Nes.emuSystem);
        }
        public VRC6pulseSoundChannel sndPulse1;
        public VRC6pulseSoundChannel sndPulse2;
        public VRC6sawtoothSoundChannel sndSawtooth;
        public short Mix()
        {
            short output = sndPulse1.GetSample();
            output += sndPulse2.GetSample();
            output += sndSawtooth.GetSample();
            return output;
        }

        public void HardReset()
        {
            sndPulse1.HardReset();
            sndPulse2.HardReset();
            sndSawtooth.HardReset();
        }

        public void SoftReset()
        {
            sndPulse1.SoftReset();
            sndPulse2.SoftReset();
            sndSawtooth.SoftReset();
        }

        public void ClockDuration()
        {
        }

        public void ClockEnvelope()
        {
        }

        public void ClockSingle(bool isClockingDuration)
        {
        }

        public void Update(int cycles)
        {
            sndPulse1.Update(cycles);
            sndPulse2.Update(cycles);
            sndSawtooth.Update(cycles);
        }

        public void SaveState(Types.StateStream stream)
        {
            sndPulse1.SaveState(stream);
            sndPulse2.SaveState(stream);
            sndSawtooth.SaveState(stream);
        }

        public void LoadState(Types.StateStream stream)
        {
            sndPulse1.LoadState(stream);
            sndPulse2.LoadState(stream);
            sndSawtooth.LoadState(stream);
        }
    }
}
