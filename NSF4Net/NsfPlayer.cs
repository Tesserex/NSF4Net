using MyNes.Core.APU;
using MyNes.Core.Boards;
using MyNes.Core.CPU;
using MyNes.Core;

namespace NSF4Net
{
    public class NsfPlayer
    {
        private const int FRAME_FIXED = 14;
        private double clock_per_sample;

        private long fclocks_per_frame;
        private double cpuClockRemaining = 0;
        private int oversampleMult = 10;
        private int frameClocks = 0;

        // cpu waiting for frame to end
        private bool cpuWaiting = false;

        // user controlled play state
        private bool playing = false;

        // forbid cpu from running while we're setting up the song
        private bool hold = false;

        public NSF? Nsf { get; private set; }
        public int SampleRate { get; private init; }
        public bool Playing { get => playing && !hold; set { playing = value; } }

        public NsfPlayer(int sampleRate)
        {
            this.SampleRate = sampleRate;
            Nes.TurnOn();
        }

        public void LoadNsf(string filename)
        {
            Nsf = new NSF(filename);
            fclocks_per_frame = (long)((1 << FRAME_FIXED) * Nsf.CyclesPerFrame);
            clock_per_sample = Nsf.CpuSpeed / SampleRate;
            nsfInit();
            nsfSetupSong();
        }

        public void SelectSong(int song)
        {
            if (Nsf is null || song < 1 || song > Nsf.num_songs)
                return;

            Nsf.current_song = (byte)song;
            nsfSetupSong();
        }

        public short TickSample()
        {
            if (Nsf is null || !Playing)
                return 0;

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
                        if (!cpuWaiting)
                            Nes.Cpu.Update();

                        Nes.Apu.Update(TimingInfo.NTSC.Cpu);

                        frameClocks += 1 << FRAME_FIXED;
                        if (Nes.Cpu.pc.Value == 0x4103)
                        {
                            cpuWaiting = true;
                            if (frameClocks >= fclocks_per_frame)
                            {
                                runRoutine(Nsf.play_addr);
                                frameClocks = 0;
                            }
                        }
                    }
                    mcclocks -= (sub_clocks * oversampleMult);
                }
                // I don't know how to math the oversampling, if I did I would do it here
            }

            cpuClockRemaining -= cpuClocks;

            return Nes.Apu.PullSample();
        }

        private void NsfBankswitch(uint address, byte index)
        {
            if (Nsf is null) return;

            uint bank_src, bank_dest, bank_offset;
            uint bank_length;
            byte value = Nsf.bankswitch_info[index];

            if (!Nsf.bankswitched)
                return;

            /* destination in nes address space */
            bank_dest = 0x8000 + ((address & 7) << 12);

            /* offset of banks in nsf data */
            bank_offset = (uint)(Nsf.load_addr & 0x0FFF);

            if (0 == value) /* special case for bank 0 */
            {
                bank_src = 0;
                bank_length = 0x1000 - bank_offset;
                bank_dest += bank_offset;
            }
            else
            {
                bank_src = (uint)((value << 12) - bank_offset);

                if (Nsf.length - bank_src < 0x1000)
                    bank_length = Nsf.length - bank_src;
                else
                    bank_length = 0x1000;
            }

            for (uint i = 0; i < bank_length; i++)
            {
                Nes.CpuMemory[(ushort)(bank_dest + i)] = Nsf.data[bank_src + i];
            }
        }

        private void zero_memory(ushort start, ushort end)
        {
            for (ushort i = start; i <= end; i++)
            {
                Nes.CpuMemory[i] = 0;
            }
        }

        private void nsfInit()
        {
            InitializeComponents();
            Nes.Apu.SetupPlayback(new ApuPlaybackDescription(SampleRate));
        }

        private void nsfSetupSong()
        {
            if (Nsf is null) return;

            hold = true;
            zero_memory(0, 0x07ff);
            zero_memory(0x6000, 0x7fff);
            zero_memory(0x4000, 0x4013);
            Nes.CpuMemory[0x4015] = 0x0F;
            Nes.CpuMemory[0x4017] = 0x40;
            Nes.Cpu.a = (byte)(Nsf.current_song - 1);

            if (Nsf.bankswitched)
            {
                NsfBankswitch(0x5FF8, 0);
                NsfBankswitch(0x5FF9, 1);
                NsfBankswitch(0x5FFA, 2);
                NsfBankswitch(0x5FFB, 3);
                NsfBankswitch(0x5FFC, 4);
                NsfBankswitch(0x5FFD, 5);
                NsfBankswitch(0x5FFE, 6);
                NsfBankswitch(0x5FFF, 7);
            }
            else
            {
                for (ushort i = 0; i < Nsf.length; i++)
                {
                    Nes.CpuMemory[(ushort)(i + Nsf.load_addr)] = Nsf.data[i];
                }
            }

            Nes.Cpu.x = Nsf.IsPal ? (byte)1 : (byte)0;
            runRoutine(Nsf.init_addr);
            while (Nes.Cpu.pc.Value != 0x4103) Nes.Cpu.Update();
            runRoutine(Nsf.play_addr);
            hold = false;
        }

        private void InitializeComponents()
        {
            if (Nsf is null) return;

            BoardsManager.LoadAvailableBoards();
            Nes.Board = BoardsManager.GetBoard((byte)Nsf.ext_sound_type);
            Nes.CpuMemory = new CpuMemory();
            Nes.CpuMemory.Initialize();

            Nes.Cpu = new Cpu(Nes.emuSystem);
            Nes.Apu = new Apu(Nes.emuSystem);

            Nes.Apu.Initialize();
            Nes.Board.Initialize();
            Nes.Cpu.Initialize();
            Nes.Initialized = true;
        }

        private void runRoutine(ushort addr)
        {
            Nes.CpuMemory[0x4100] = 0x20; /* jsr */
            Nes.CpuMemory[0x4101] = (byte)(addr & 0xFF);
            Nes.CpuMemory[0x4102] = (byte)(addr >> 8);   /* init addr */
            Nes.CpuMemory[0x4103] = 0xFF; /* kill switch for CPU emulation */

            Nes.Cpu.pc.Value = 0x4100;

            cpuWaiting = false;
        }
    }
}
