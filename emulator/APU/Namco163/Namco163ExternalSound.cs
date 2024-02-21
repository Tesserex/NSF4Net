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
namespace MyNes.Core.APU.Namco163
{
    class Namco163ExternalSound : IApuExternalChannelsMixer
    {
        public Namco163ExternalSound()
        {
            channels = new Namco163SoundChannel[8];
            EXRAM = new byte[128];
            sndReg = 0;
            enabledChannels = 0;
            channelIndex = 0;
            for (int i = 0; i < 8; i++)
                channels[i] = new Namco163SoundChannel(Nes.emuSystem, this);
        }
        private Namco163SoundChannel[] channels;
        public byte[] EXRAM = new byte[128];
        private byte sndReg = 0;
        public int enabledChannels = 0;
        private int channelIndex = 0;

        public void Poke4800(int address, byte data)
        {
            // write registers
            if (sndReg >= 0x40)
            {
                //Console.WriteLine("Reg write: address:" + string.Format("{0:X}", sndReg) + ", data:" +
                //     string.Format("{0:X}", data) + ", channel:" + string.Format("{0:X}", ((sndReg >> 4) & 0x7)));
                switch (sndReg & 0x7F)
                {
                    case 0x40: channels[0].PokeA(data); break;
                    case 0x42: channels[0].PokeB(data); break;
                    case 0x44: channels[0].PokeC(data); break;
                    case 0x46: channels[0].PokeD(data); break;
                    case 0x47: channels[0].PokeE(data); break;
                    case 0x48: channels[1].PokeA(data); break;
                    case 0x4A: channels[1].PokeB(data); break;
                    case 0x4C: channels[1].PokeC(data); break;
                    case 0x4E: channels[1].PokeD(data); break;
                    case 0x4F: channels[1].PokeE(data); break;
                    case 0x50: channels[2].PokeA(data); break;
                    case 0x52: channels[2].PokeB(data); break;
                    case 0x54: channels[2].PokeC(data); break;
                    case 0x56: channels[2].PokeD(data); break;
                    case 0x57: channels[2].PokeE(data); break;
                    case 0x58: channels[3].PokeA(data); break;
                    case 0x5A: channels[3].PokeB(data); break;
                    case 0x5C: channels[3].PokeC(data); break;
                    case 0x5E: channels[3].PokeD(data); break;
                    case 0x5F: channels[3].PokeE(data); break;
                    case 0x60: channels[4].PokeA(data); break;
                    case 0x62: channels[4].PokeB(data); break;
                    case 0x64: channels[4].PokeC(data); break;
                    case 0x66: channels[4].PokeD(data); break;
                    case 0x67: channels[4].PokeE(data); break;
                    case 0x68: channels[5].PokeA(data); break;
                    case 0x6A: channels[5].PokeB(data); break;
                    case 0x6C: channels[5].PokeC(data); break;
                    case 0x6E: channels[5].PokeD(data); break;
                    case 0x6F: channels[5].PokeE(data); break;
                    case 0x70: channels[6].PokeA(data); break;
                    case 0x72: channels[6].PokeB(data); break;
                    case 0x74: channels[6].PokeC(data); break;
                    case 0x76: channels[6].PokeD(data); break;
                    case 0x77: channels[6].PokeE(data); break;
                    case 0x78: channels[7].PokeA(data); break;
                    case 0x7A: channels[7].PokeB(data); break;
                    case 0x7C: channels[7].PokeC(data); break;
                    case 0x7E: channels[7].PokeD(data); break;
                    case 0x7F: channels[7].PokeE(data); Poke7F(data); break;
                }
            }
            EXRAM[sndReg & 0x7F] = data;
            if ((sndReg & 0x80) == 0x80)
                sndReg = (byte)(((sndReg + 1) & 0x7F) | 0x80);
        }
        public void PokeF800(int address, byte data)
        {
            sndReg = data;
        }
        public byte Peek4800(int address)
        {
            byte val = EXRAM[sndReg & 0x7F];
            if ((sndReg & 0x80) == 0x80)
                sndReg = (byte)(((sndReg + 1) & 0x7F) | 0x80);
            return val;
        }
        private void Poke7F(byte data)
        {
            enabledChannels = ((data & 0x70) >> 4);
            channelIndex = 0;
            int enabledChannels1 = enabledChannels + 1;
            for (int i = 7; i >= 0; i--)
            {
                if (enabledChannels1 > 0)
                {
                    channels[i].Enabled = true;
                    enabledChannels1--;
                }
                else
                    break;
            }
        }
        public short Mix()
        {
            short output = 0;

            int enabledChannels1 = enabledChannels + 1;
            for (int i = 7; i >= 0; i--)
            {
                if (enabledChannels1 > 0)
                {
                    enabledChannels1--;
                    output += channels[i].GetSample();
                }
                else
                    break;
            }

            return output;
        }

        public void HardReset()
        {
            for (int i = 0; i < 8; i++)
                channels[i].HardReset();
            EXRAM = new byte[128];
            sndReg = 0;
            enabledChannels = 0;
            channelIndex = 0;
        }

        public void SoftReset()
        {
            for (int i = 0; i < 8; i++)
                channels[i].SoftReset();
            sndReg = 0;
            enabledChannels = 0;
            channelIndex = 0;
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
            channels[7 - (channelIndex = ((channelIndex + 1) & enabledChannels))].Update(cycles);
        }

        public void SaveState(Types.StateStream stream)
        {
            for (int i = 0; i < 8; i++)
                channels[i].SaveState(stream);
            stream.Write(EXRAM);
            stream.Write(sndReg);
            stream.Write(enabledChannels);
            stream.Write(channelIndex);
        }

        public void LoadState(Types.StateStream stream)
        {
            for (int i = 0; i < 8; i++)
                channels[i].LoadState(stream);
            stream.Read(EXRAM);
            sndReg = stream.ReadByte();
            enabledChannels = stream.ReadInt32();
            channelIndex = stream.ReadInt32();
        }
    }
}
