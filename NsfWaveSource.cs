using CSCore;
using MyNes.Core;
using MyNes.Core.APU;
using MyNes.Core.Boards;
using MyNes.Core.CPU;

namespace NSFAudio
{
    internal class NsfWaveSource : IWaveSource
    {
        private const int CHANNELS = 1;
        private const int FRAME_FIXED = 14;
        private const double nes_basecycles = 1789772;
        private WaveFormat _waveFormat;
        private double volume = 1.0;
        public bool IsRendering = false;

        private NSF nsf;
        private long fclocks_per_frame;

        public NsfWaveSource(string path)
        {
            _waveFormat = new WaveFormat(48000, 8, CHANNELS);
            //Task.Run(() => play_nsf_file(path, 1));
            play_nsf_file(path, 1);
        }

        public bool CanSeek => false;

        public WaveFormat WaveFormat => _waveFormat;

        public long Position { get => 0; set { } }

        public long Length => 0;

        public void Dispose()
        {
        }

        double cpuClockRemaining = 0;
        int oversampleMult = 10;
        int frameClocks = 0;
        bool breaked = false;

        public int Read(byte[] buffer, int offset, int count)
        {
            double clock_per_sample = nes_basecycles / _waveFormat.SampleRate;

            for (var bufPos = 0; bufPos < count; bufPos += CHANNELS)
            {
                cpuClockRemaining += clock_per_sample;
                int cpuClocks = (int)cpuClockRemaining;
                int mcclocks = 0;
                for (var i = 0; i < oversampleMult; i++)
                {
                    mcclocks += cpuClocks;
                    if (mcclocks >= oversampleMult)
                    {
                        int sub_clocks = mcclocks / oversampleMult;
                        for (var j = 0; j < sub_clocks; j++)
                        {
                            if (!breaked)
                                Nes.Cpu.Update();

                            Nes.Apu.Update(TimingInfo.NTSC.Cpu);

                            frameClocks += 1 << FRAME_FIXED;
                            if (Nes.Cpu.pc.Value == 0x4103)
                            {
                                breaked = true;
                                if (frameClocks >= fclocks_per_frame)
                                {
                                    breaked = false;
                                    nsf_setup(nsf.play_addr);
                                    frameClocks = 0;
                                }
                            }
                        }
                        mcclocks -= (sub_clocks * oversampleMult);
                    }

                    // I don't know how to math the oversampling, if I did I would do it here
                }

                // just sample here instead
                double sample = Nes.Apu.PullSample();
                var output = (byte)(2 * sample);
                buffer[bufPos] = output;

                cpuClockRemaining -= cpuClocks;
            }
            
            return count;
        }

        static void nsf_bankswitch(NSF nsf, uint address, byte value)
        {
            uint bank_src, bank_dest, bank_offset;
            uint bank_length;

            if (!nsf.bankswitched)
                return;

            /* destination in nes address space */
            bank_dest = 0x8000 + ((address & 7) << 12);

            /* offset of banks in nsf data */
            bank_offset = (uint)(nsf.load_addr & 0x0FFF);

            if (0 == value) /* special case for bank 0 */
            {
                bank_src = 0;
                bank_length = 0x1000 - bank_offset;
                bank_dest += bank_offset;
            }
            else
            {
                bank_src = (uint)((value << 12) - bank_offset);

                if (nsf.length - bank_src < 0x1000)
                    bank_length = nsf.length - bank_src;
                else
                    bank_length = 0x1000;
            }

            //memcpy(cpu->mem_page[0] + bank_dest, nsf_info->data + bank_src, bank_length);
            for (uint i = 0; i < bank_length; i++)
            {
                Nes.CpuMemory[(ushort)(bank_dest + i)] = nsf.data[bank_src + i];
            }
        }

        static void zero_memory(ushort start, ushort end)
        {
            for (ushort i = start; i <= end; i++)
            {
                Nes.CpuMemory[i] = 0;
            }
        }

        /* set up a track to play */
        void nsf_setupsong(NSF nsf)
        {
            InitializeComponents(nsf);
            Nes.SetupOutput(null, null, new ApuPlaybackDescription(44100));

            zero_memory(0, 0x07ff);
            zero_memory(0x6000, 0x7fff);
            zero_memory(0x4000, 0x4013);
            Nes.CpuMemory[0x4015] = 0x0F;
            Nes.CpuMemory[0x4017] = 0x40;
            Nes.Cpu.a = (byte)(nsf.current_song - 1);

            if (nsf.bankswitched)
            {
                nsf_bankswitch(nsf, 0x5FF8, nsf.bankswitch_info[0]);
                nsf_bankswitch(nsf, 0x5FF9, nsf.bankswitch_info[1]);
                nsf_bankswitch(nsf, 0x5FFA, nsf.bankswitch_info[2]);
                nsf_bankswitch(nsf, 0x5FFB, nsf.bankswitch_info[3]);
                nsf_bankswitch(nsf, 0x5FFC, nsf.bankswitch_info[4]);
                nsf_bankswitch(nsf, 0x5FFD, nsf.bankswitch_info[5]);
                nsf_bankswitch(nsf, 0x5FFE, nsf.bankswitch_info[6]);
                nsf_bankswitch(nsf, 0x5FFF, nsf.bankswitch_info[7]);
            }
            else
            {
                for (ushort i = 0; i < nsf.length; i++)
                {
                    Nes.CpuMemory[(ushort)(i + nsf.load_addr)] = nsf.data[i];
                }
            }

            Nes.Cpu.x = nsf.IsPal ? (byte)1 : (byte)0;
            nsf_setup(nsf.init_addr);
            while (Nes.Cpu.pc.Value != 0x4103) Nes.Cpu.Update();
            nsf_setup(nsf.play_addr);
        }

        static void InitializeComponents(NSF nsf)
        {
            BoardsManager.LoadAvailableBoards();
            Nes.Board = BoardsManager.GetBoard(new MyNes.Core.ROM.INESHeader() { Mapper = (byte)nsf.ext_sound_type }, new byte[0x8000], new byte [0x8000], new byte[0x8000]);
            Nes.CpuMemory = new CpuMemory();
            Nes.CpuMemory.Initialize();

            Nes.Cpu = new Cpu(Nes.emuSystem);
            Nes.Apu = new Apu(Nes.emuSystem);

            Nes.Apu.Initialize();
            Nes.Board.Initialize();
            Nes.Cpu.Initialize();
            Nes.Initialized = true;
        }

        void nsf_setup(ushort addr)
        {
            Nes.CpuMemory[0x4100] = 0x20; /* jsr */
            Nes.CpuMemory[0x4101] = (byte)(addr & 0xFF);
            Nes.CpuMemory[0x4102] = (byte)(addr >> 8);   /* init addr */
            Nes.CpuMemory[0x4103] = 0xFF; /* kill switch for CPU emulation */

            Nes.Cpu.pc.Value = 0x4100;
        }

        /* play that thar NSF file */
        public int play_nsf_file(string filename, byte track)
        {
            /* load up an NSF file */
            try
            {
                nsf = new NSF(filename);
            }
            catch
            {
                //Console.WriteLine("Error opening " + filename);
                return 1;
            }

            /* determine which track to play */
            if (track > nsf.num_songs || track < 1)
            {
                nsf.current_song = nsf.start_song;
                //Console.WriteLine("track %d out of range, playing track %d\n", track, nsf.current_song);
            }
            else
                nsf.current_song = track;

            fclocks_per_frame = (long)((1 << FRAME_FIXED) * nes_basecycles / nsf.playback_rate);

            /* reset state of the NES */
            nsf_setupsong(nsf);
            Nes.TurnOn();

            return 0;
        }
    }
}
