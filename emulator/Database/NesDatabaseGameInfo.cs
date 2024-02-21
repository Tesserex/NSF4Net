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
using System.Collections.Generic;

namespace MyNes.Core.Database
{
    public struct NesDatabaseGameInfo
    {
        //Game info
        public string Game_Name;
        public string Game_AltName;
        public string Game_Class;
        public string Game_Catalog;
        public string Game_Publisher;
        public string Game_Developer;
        public string Game_Region;
        public string Game_Players;
        public string Game_ReleaseDate;

        //cartridges, game may has one or more cartridge dump
        public List<NesDatabaseCartridgeInfo> Cartridges;

        //board info
        public string Board_Type;
        public string Board_Pcb;
        public string Board_Mapper;

        //prg
        public string PRG_name;
        public string PRG_size;
        public string PRG_crc;
        public string PRG_sha1;

        //chr
        public string CHR_name;
        public string CHR_size;
        public string CHR_crc;
        public string CHR_sha1;

        //vram
        public string VRAM_size;

        //chip, may be more than one chip
        public List<string> chip_type;

        //cic
        public string CIC_type;

        //pad
        public string PAD_h;
        public string PAD_v;
    }
    public struct NesDatabaseCartridgeInfo
    {
        //cartridge info
        public string System;
        public string CRC;
        public string SHA1;
        public string Dump;
        public string Dumper;
        public string DateDumped;
    }
}
