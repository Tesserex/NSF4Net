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

        private NSF? nsf;
        private long fclocks_per_frame;
        double cpuClockRemaining = 0;
        int oversampleMult = 10;
        int frameClocks = 0;
        bool breaked = false;
        private readonly int sampleRate;

        public NsfPlayer(int sampleRate)
        {
            this.sampleRate = sampleRate;
            Nes.TurnOn();
        }

        public void LoadNsf(string filename)
        {
            nsf = new NSF(filename);
            fclocks_per_frame = (long)((1 << FRAME_FIXED) * nsf.CyclesPerFrame);
            clock_per_sample = nsf.CpuSpeed / sampleRate;
            nsf_setupsong();
        }

        public void SelectSong(int song)
        {
            if (nsf is null || song < 1 || song > nsf.num_songs)
                return;

            nsf.current_song = (byte)song;
            runRoutine(nsf.init_addr);
            while (Nes.Cpu.pc.Value != 0x4103) Nes.Cpu.Update();
            runRoutine(nsf.play_addr);
        }

        public short TickSample()
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
                                runRoutine(nsf.play_addr);
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

        private void nsf_bankswitch(uint address, byte index)
        {
            if (nsf is null) return;

            uint bank_src, bank_dest, bank_offset;
            uint bank_length;
            byte value = nsf.bankswitch_info[index];

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

            for (uint i = 0; i < bank_length; i++)
            {
                Nes.CpuMemory[(ushort)(bank_dest + i)] = nsf.data[bank_src + i];
            }
        }

        private void zero_memory(ushort start, ushort end)
        {
            for (ushort i = start; i <= end; i++)
            {
                Nes.CpuMemory[i] = 0;
            }
        }

        private void nsf_setupsong()
        {
            InitializeComponents();
            Nes.Apu.SetupPlayback(new ApuPlaybackDescription(44100));

            zero_memory(0, 0x07ff);
            zero_memory(0x6000, 0x7fff);
            zero_memory(0x4000, 0x4013);
            Nes.CpuMemory[0x4015] = 0x0F;
            Nes.CpuMemory[0x4017] = 0x40;
            Nes.Cpu.a = (byte)(nsf.current_song - 1);

            if (nsf.bankswitched)
            {
                nsf_bankswitch(0x5FF8, 0);
                nsf_bankswitch(0x5FF9, 1);
                nsf_bankswitch(0x5FFA, 2);
                nsf_bankswitch(0x5FFB, 3);
                nsf_bankswitch(0x5FFC, 4);
                nsf_bankswitch(0x5FFD, 5);
                nsf_bankswitch(0x5FFE, 6);
                nsf_bankswitch(0x5FFF, 7);
            }
            else
            {
                for (ushort i = 0; i < nsf.length; i++)
                {
                    Nes.CpuMemory[(ushort)(i + nsf.load_addr)] = nsf.data[i];
                }
            }

            Nes.Cpu.x = nsf.IsPal ? (byte)1 : (byte)0;
            runRoutine(nsf.init_addr);
            while (Nes.Cpu.pc.Value != 0x4103) Nes.Cpu.Update();
            runRoutine(nsf.play_addr);
        }

        private void InitializeComponents()
        {
            BoardsManager.LoadAvailableBoards();
            Nes.Board = BoardsManager.GetBoard((byte)nsf.ext_sound_type);
            Nes.CpuMemory = new CpuMemory();
            Nes.CpuMemory.Initialize();

            Nes.Cpu = new Cpu(Nes.emuSystem);
            Nes.Apu = new Apu(Nes.emuSystem);

            Nes.Apu.Initialize();
            Nes.Board.Initialize();
            Nes.Cpu.Initialize();
            Nes.Initialized = true;
        }

        private static void runRoutine(ushort addr)
        {
            Nes.CpuMemory[0x4100] = 0x20; /* jsr */
            Nes.CpuMemory[0x4101] = (byte)(addr & 0xFF);
            Nes.CpuMemory[0x4102] = (byte)(addr >> 8);   /* init addr */
            Nes.CpuMemory[0x4103] = 0xFF; /* kill switch for CPU emulation */

            Nes.Cpu.pc.Value = 0x4100;
        }
    }
}
