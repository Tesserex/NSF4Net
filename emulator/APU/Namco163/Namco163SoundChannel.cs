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
    class Namco163SoundChannel : Channel
    {
        public Namco163SoundChannel(TimingInfo.System system, Namco163ExternalSound externalSound)
            : base(system) { this.externalSound = externalSound; }
        private Namco163ExternalSound externalSound;
        public bool Enabled = false;
        private int LengthOfWaveform = 64 * 4;// assuming default L value is 0 (4 * (64-L))
        private int AddressOfWaveform = 0;
        private int LinearVolume = 0;
        private int Step = 0;
        private byte[] wavBuffer = new byte[64 * 4];
        private byte output = 0;
        private bool Freez = true;

        public override void HardReset()
        {
            base.HardReset();
            Enabled = false;
            LengthOfWaveform = 64 * 4;// assuming default L value is 0 (4 * (64-L))
            AddressOfWaveform = 0;
            LinearVolume = 0;
            Step = 0;
            wavBuffer = new byte[64 * 4];
            output = 0;
            Freez = true;
        }
        private void UpdateFrequency()
        {
            Freez = (frequency == 0);
            if (frequency > 0)
                timing.single = system.Cpu*(0xF0000  / frequency);
        }
        /* We can use directly the exram for registers, 
         * but i think it's better this way so freq updated only when reg get write onto*/
        public void PokeA(byte data)
        {
            frequency = (frequency & 0x03FF00) | data;
            UpdateFrequency();
        }
        public void PokeB(byte data)
        {
            frequency = (frequency & 0x0300FF) | (data << 8);
            UpdateFrequency();
        }
        public void PokeC(byte data)
        {
            frequency = (frequency & 0x00FFFF) | ((data & 0x3) << 16);
            UpdateFrequency();
            LengthOfWaveform = (64 - ((data >> 2) & 0x3F)) * 4;
            SetupWavBuffer();
        }
        public void PokeD(byte data)
        {
            AddressOfWaveform = data; SetupWavBuffer();
        }
        public void PokeE(byte data)
        {
            LinearVolume = data & 0xF;
        }
        private void SetupWavBuffer()
        {
            wavBuffer = new byte[LengthOfWaveform];
            // get the address (real address)
            int raddress = AddressOfWaveform / 2;
            int sampleAddress = 0;
            bool highLow = (AddressOfWaveform & 1) == 1;
            while (sampleAddress < LengthOfWaveform)
            {
                // get the instrument sample
                int sample = highLow ?
                    ((externalSound.EXRAM[raddress] & 0xF0) >> 4) : (externalSound.EXRAM[raddress] & 0xF);
                wavBuffer[sampleAddress] = (byte)sample;
                if (highLow)
                    raddress++;
                sampleAddress++;
                highLow = !highLow;
            }
            Step = 0;
        }
        public override void Update()
        {
            if (Freez) return;
            if (wavBuffer.Length <= LengthOfWaveform)
            {
                Step = (Step + 1) & (LengthOfWaveform - 1);
                output = (byte)(wavBuffer[Step] * LinearVolume);
            }
        }
        public override byte GetSample()
        {
            if (Enabled)
            {
                return output;
            }
            return 0;
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(Enabled); 
            stream.Write(LengthOfWaveform);
            stream.Write(AddressOfWaveform);
            stream.Write(LinearVolume);
            stream.Write(Step);
            stream.Write(wavBuffer);
            stream.Write(output);
            stream.Write(Freez);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            Enabled = stream.ReadBoolean();
            LengthOfWaveform = stream.ReadInt32();
            AddressOfWaveform = stream.ReadInt32();
            LinearVolume = stream.ReadInt32();
            Step = stream.ReadInt32();
            stream.Read(wavBuffer);
            output = stream.ReadByte();
            Freez = stream.ReadBoolean();
        }
    }
}
