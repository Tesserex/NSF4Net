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
namespace MyNes.Core.APU.Sunsoft5B
{
    class Sunsoft5BExternalSound : IApuExternalChannelsMixer
    {
        public Sunsoft5BExternalSound()
        {
            sndChannel0 = new Sunsoft5BSoundChannel(Nes.emuSystem);
            sndChannel1 = new Sunsoft5BSoundChannel(Nes.emuSystem);
            sndChannel2 = new Sunsoft5BSoundChannel(Nes.emuSystem);
        }
        public Sunsoft5BSoundChannel sndChannel0;
        public Sunsoft5BSoundChannel sndChannel1;
        public Sunsoft5BSoundChannel sndChannel2;
        public short Mix()
        {
            short output = sndChannel0.GetSample();
            output += sndChannel1.GetSample();
            output += sndChannel2.GetSample();
            return output;
        }

        public void HardReset()
        {
            sndChannel0.HardReset();
            sndChannel1.HardReset();
            sndChannel2.HardReset();
        }

        public void SoftReset()
        {
            sndChannel0.SoftReset();
            sndChannel1.SoftReset();
            sndChannel2.SoftReset();
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
            sndChannel0.Update(cycles);
            sndChannel1.Update(cycles);
            sndChannel2.Update(cycles);
        }

        public void SaveState(Types.StateStream stream)
        {
            sndChannel0.SaveState(stream);
            sndChannel1.SaveState(stream);
            sndChannel2.SaveState(stream);
        }

        public void LoadState(Types.StateStream stream)
        {
            sndChannel0.LoadState(stream);
            sndChannel1.LoadState(stream);
            sndChannel2.LoadState(stream);
        }
    }
}
