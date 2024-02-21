using CSCore;
using NSF4Net;

namespace Example
{
    public class NsfWaveSource : IWaveSource
    {
        private const int CHANNELS = 2;
        private NsfPlayer player;
        private WaveFormat _waveFormat;
        private double volume = 1.0;

        public NsfWaveSource(string path)
        {
            _waveFormat = new WaveFormat(48000, 16, CHANNELS);
            player = new NsfPlayer(48000);
            player.LoadNsf(path);
            player.SelectSong(2);
        }

        public bool CanSeek => false;

        public WaveFormat WaveFormat => _waveFormat;

        public long Position { get => 0; set { } }

        public long Length => 0;

        public void Dispose()
        {
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            for (var bufPos = 0; bufPos < count;)
            {
                double sample = player.TickSample();
                var output = (ushort)(256 * volume * sample);
                for (int i = 0; i < CHANNELS; i++)
                {
                    buffer[bufPos++] = (byte)(output & 0xFF);
                    buffer[bufPos++] = (byte)(output >> 8);
                }
            }

            return count;
        }
    }
}
