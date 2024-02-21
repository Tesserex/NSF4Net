using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NSFAudio
{
    public enum EXT_SOUND : byte
    {
        NONE = 0x00,
        VRC6 = 0x01,
        VRC7 = 0x02,
        FDS = 0x04,
        MMC5 = 0x08,
        N163 = 0x10,
        S5B = 0x20
    }

    public class NSF
    {
        static byte NSF_HEADER_SIZE = 0x80;

        byte[] id = new byte[5];
        byte version;
        public byte num_songs;
        public byte start_song;
        public ushort load_addr;
        public ushort init_addr;
        public ushort play_addr;
        byte[] song_name = new byte[32];
        byte[] author_name = new byte[32];
        byte[] copyright = new byte[32];
        ushort ntsc_speed;
        public byte[] bankswitch_info = new byte[8];
        ushort pal_speed;
        byte pal_ntsc_bits;
        public EXT_SOUND ext_sound_type;
        byte[] reserved = new byte[4];

        public byte[] data;
        public uint length;
        public int playback_rate;
        public byte current_song;
        public bool bankswitched;

        public bool IsPal { get { return (this.pal_ntsc_bits & 1) > 0; } }

        public int CyclesPerFrame
        {
            get
            {
                if (this.IsPal)
                    return 26667;
                else
                    return 29830;
            }
        }

        public void setup()
        {
            int div_speed;

            this.current_song = this.start_song;

            if (this.IsPal)
                div_speed = this.pal_speed;
            else
                div_speed = this.ntsc_speed;

            if (div_speed != 0)
                this.playback_rate = 1000000 / div_speed;
            else
                this.playback_rate = 60; /* Assume 60 Hz */

            this.bankswitched = false;
            foreach (byte b in this.bankswitch_info)
            {
                if (b != 0)
                {
                    this.bankswitched = true;
                    break;
                }
            }
        }

        /* Load a ROM image into memory */
        public NSF(string filename)
        {
            FileStream fp;
            fp = File.OpenRead(filename);

            /* Didn't find the file?  Maybe the .NSF extension was omitted */
            if (!File.Exists(filename))
            {
                if (!filename.Contains('.'))
                    filename += ".nsf";

                fp = File.OpenRead(filename);
            }

            /* Read in the header */
            fp.Read(this.id, 0, 5);
            this.version = (byte)fp.ReadByte();
            this.num_songs = (byte)fp.ReadByte();
            this.start_song = (byte)fp.ReadByte();
            this.load_addr = ReadShort(fp);
            this.init_addr = ReadShort(fp);
            this.play_addr = ReadShort(fp);
            fp.Read(this.song_name, 0, 32);
            fp.Read(this.author_name, 0, 32);
            fp.Read(this.copyright, 0, 32);
            this.ntsc_speed = ReadShort(fp);
            fp.Read(this.bankswitch_info, 0, 8);
            this.pal_speed = ReadShort(fp);
            this.pal_ntsc_bits = (byte)fp.ReadByte();
            this.ext_sound_type = (EXT_SOUND)fp.ReadByte();
            fp.Read(this.reserved, 0, 4);

            /* we're now at position 80h */
            this.length = (uint)(fp.Length - NSF_HEADER_SIZE);

            this.data = new byte[this.length];
            fp.Read(this.data, 0, (int)this.length);

            fp.Close();

            /* Set up some variables */
            setup();
        }

        private ushort ReadShort(FileStream fs)
        {
            ushort lsb = (ushort)fs.ReadByte();
            ushort msb = (ushort)fs.ReadByte();
            return (ushort)((msb << 8) | lsb);
        }
    }
}
