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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace MyNes.Core.Database
{
    public class NesDatabase
    {
        static List<NesDatabaseGameInfo> _databaseRoms = new List<NesDatabaseGameInfo>();
        //Database file info
        public static string DBVersion = "";
        public static string DBConformance = "";
        public static string DBAgent = "";
        public static string DBAuthor = "";
        public static string DBTimeStamp = "";

        /// <summary>
        /// Load data base from file
        /// </summary>
        /// <param name="fileName">The full path of database file</param>
        public static void LoadDatabase(string fileName)
        {
            //1 clear the database
            _databaseRoms.Clear();
            //2 read the xml file
            Stream databaseStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            XmlReaderSettings sett = new XmlReaderSettings();
            sett.DtdProcessing = DtdProcessing.Ignore;
            sett.IgnoreWhitespace = true;
            XmlReader XMLread = XmlReader.Create(databaseStream, sett);

            NesDatabaseGameInfo drom = new NesDatabaseGameInfo();

            while (XMLread.Read())
            {
                //database Game info
                if (XMLread.Name == "xml" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("version"))
                        DBVersion = XMLread.Value;
                    if (XMLread.MoveToAttribute("conformance"))
                        DBConformance = XMLread.Value;
                    if (XMLread.MoveToAttribute("agent"))
                        DBAgent = XMLread.Value;
                    if (XMLread.MoveToAttribute("author"))
                        DBAuthor = XMLread.Value;
                    if (XMLread.MoveToAttribute("timestamp"))
                        DBTimeStamp = XMLread.Value;
                }
                //Is it a game ?
                if (XMLread.Name == "game" & XMLread.IsStartElement())
                {
                    drom = new NesDatabaseGameInfo();

                    if (XMLread.MoveToAttribute("name"))
                        drom.Game_Name = XMLread.Value;
                    if (XMLread.MoveToAttribute("altname"))
                        drom.Game_AltName = XMLread.Value;
                    if (XMLread.MoveToAttribute("class"))
                        drom.Game_Class = XMLread.Value;
                    if (XMLread.MoveToAttribute("catalog"))
                        drom.Game_Catalog = XMLread.Value;
                    if (XMLread.MoveToAttribute("publisher"))
                        drom.Game_Publisher = XMLread.Value;
                    if (XMLread.MoveToAttribute("developer"))
                        drom.Game_Developer = XMLread.Value;
                    if (XMLread.MoveToAttribute("region"))
                        drom.Game_Region = XMLread.Value;
                    if (XMLread.MoveToAttribute("players"))
                        drom.Game_Players = XMLread.Value;
                    if (XMLread.MoveToAttribute("date"))
                        drom.Game_ReleaseDate = XMLread.Value;
                }
                //End of game info ?
                if (XMLread.Name == "game" & !XMLread.IsStartElement())
                {
                    _databaseRoms.Add(drom);
                }
                //cartridge info
                if (XMLread.Name == "cartridge" & XMLread.IsStartElement())
                {
                    if (drom.Cartridges == null)
                        drom.Cartridges = new List<NesDatabaseCartridgeInfo>();
                    NesDatabaseCartridgeInfo crt = new NesDatabaseCartridgeInfo();
                    if (XMLread.MoveToAttribute("system"))
                        crt.System = XMLread.Value;
                    if (XMLread.MoveToAttribute("crc"))
                        crt.CRC = XMLread.Value;
                    if (XMLread.MoveToAttribute("sha1"))
                        crt.SHA1 = XMLread.Value;
                    if (XMLread.MoveToAttribute("dump"))
                        crt.Dump = XMLread.Value;
                    if (XMLread.MoveToAttribute("dumper"))
                        crt.Dumper = XMLread.Value;
                    if (XMLread.MoveToAttribute("datedumped"))
                        crt.DateDumped = XMLread.Value;
                    drom.Cartridges.Add(crt);
                }
                //board info
                if (XMLread.Name == "board" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("type"))
                        drom.Board_Type = XMLread.Value;
                    if (XMLread.MoveToAttribute("pcb"))
                        drom.Board_Pcb = XMLread.Value;
                    if (XMLread.MoveToAttribute("mapper"))
                        drom.Board_Mapper = XMLread.Value;
                }
                //prg info
                if (XMLread.Name == "prg" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("name"))
                        drom.PRG_name = XMLread.Value;
                    if (XMLread.MoveToAttribute("size"))
                        drom.PRG_size = XMLread.Value;
                    if (XMLread.MoveToAttribute("crc"))
                        drom.PRG_crc = XMLread.Value;
                    if (XMLread.MoveToAttribute("sha1"))
                        drom.PRG_sha1 = XMLread.Value;
                }
                //chr info
                if (XMLread.Name == "chr" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("name"))
                        drom.CHR_name = XMLread.Value;
                    if (XMLread.MoveToAttribute("size"))
                        drom.CHR_size = XMLread.Value;
                    if (XMLread.MoveToAttribute("crc"))
                        drom.CHR_crc = XMLread.Value;
                    if (XMLread.MoveToAttribute("sha1"))
                        drom.CHR_sha1 = XMLread.Value;
                }
                //vram info
                if (XMLread.Name == "vram" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("size"))
                        drom.VRAM_size = XMLread.Value;
                }
                //chip info
                if (XMLread.Name == "chip" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("type"))
                    {
                        if (drom.chip_type == null)
                            drom.chip_type = new List<string>();
                        drom.chip_type.Add(XMLread.Value);
                    }
                }
                //cic info
                if (XMLread.Name == "cic" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("type"))
                        drom.CIC_type = XMLread.Value;
                }
                //pad info
                if (XMLread.Name == "pad" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("h"))
                        drom.PAD_h = XMLread.Value;
                    if (XMLread.MoveToAttribute("v"))
                        drom.PAD_v = XMLread.Value;
                }
            }
            XMLread.Close();
            databaseStream.Close();
        }

        /// <summary>
        /// Get the data base roms collection (found in xml file)
        /// </summary>
        public static List<NesDatabaseGameInfo> DatabaseRoms
        {
            get { return _databaseRoms; }
        }
        /// <summary>
        /// Find a NesDatabaseRomInfo element
        /// </summary>
        /// <param name="Cart_sha1">The sha1 to match, file sha1 without header of INES (start index 16)</param>
        /// <returns>The matched element if found, otherwise null</returns>
        public static NesDatabaseGameInfo Find(string Cart_sha1)
        {
            foreach (NesDatabaseGameInfo item in _databaseRoms)
            {
                foreach (NesDatabaseCartridgeInfo crt in item.Cartridges)
                    if (crt.SHA1.ToLower() == Cart_sha1.ToLower())
                        return item;
            }
            return new NesDatabaseGameInfo();//null
        }
    }
    public class NesDatabaseObject
    {
        private List<NesDatabaseGameInfo> _databaseRoms = new List<NesDatabaseGameInfo>();
        //Database file info
        public string DBVersion = "";
        public string DBConformance = "";
        public string DBAgent = "";
        public string DBAuthor = "";
        public string DBTimeStamp = "";

        /// <summary>
        /// Load data base from file
        /// </summary>
        /// <param name="fileName">The full path of database file</param>
        public void LoadDatabase(string fileName)
        {
            //1 clear the database
            _databaseRoms.Clear();
            //2 read the xml file
            Stream databaseStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            XmlReaderSettings sett = new XmlReaderSettings();
            sett.DtdProcessing = DtdProcessing.Ignore;
            sett.IgnoreWhitespace = true;
            XmlReader XMLread = XmlReader.Create(databaseStream, sett);

            NesDatabaseGameInfo drom = new NesDatabaseGameInfo();

            while (XMLread.Read())
            {
                //database Game info
                if (XMLread.Name == "xml" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("version"))
                        DBVersion = XMLread.Value;
                    if (XMLread.MoveToAttribute("conformance"))
                        DBConformance = XMLread.Value;
                    if (XMLread.MoveToAttribute("agent"))
                        DBAgent = XMLread.Value;
                    if (XMLread.MoveToAttribute("author"))
                        DBAuthor = XMLread.Value;
                    if (XMLread.MoveToAttribute("timestamp"))
                        DBTimeStamp = XMLread.Value;
                }
                //Is it a game ?
                if (XMLread.Name == "game" & XMLread.IsStartElement())
                {
                    drom = new NesDatabaseGameInfo();

                    if (XMLread.MoveToAttribute("name"))
                        drom.Game_Name = XMLread.Value;
                    if (XMLread.MoveToAttribute("altname"))
                        drom.Game_AltName = XMLread.Value;
                    if (XMLread.MoveToAttribute("class"))
                        drom.Game_Class = XMLread.Value;
                    if (XMLread.MoveToAttribute("catalog"))
                        drom.Game_Catalog = XMLread.Value;
                    if (XMLread.MoveToAttribute("publisher"))
                        drom.Game_Publisher = XMLread.Value;
                    if (XMLread.MoveToAttribute("developer"))
                        drom.Game_Developer = XMLread.Value;
                    if (XMLread.MoveToAttribute("region"))
                        drom.Game_Region = XMLread.Value;
                    if (XMLread.MoveToAttribute("players"))
                        drom.Game_Players = XMLread.Value;
                    if (XMLread.MoveToAttribute("date"))
                        drom.Game_ReleaseDate = XMLread.Value;
                }
                //End of game info ?
                if (XMLread.Name == "game" & !XMLread.IsStartElement())
                {
                    _databaseRoms.Add(drom);
                }
                //cartridge info
                if (XMLread.Name == "cartridge" & XMLread.IsStartElement())
                {
                    if (drom.Cartridges == null)
                        drom.Cartridges = new List<NesDatabaseCartridgeInfo>();
                    NesDatabaseCartridgeInfo crt = new NesDatabaseCartridgeInfo();
                    if (XMLread.MoveToAttribute("system"))
                        crt.System = XMLread.Value;
                    if (XMLread.MoveToAttribute("crc"))
                        crt.CRC = XMLread.Value;
                    if (XMLread.MoveToAttribute("sha1"))
                        crt.SHA1 = XMLread.Value;
                    if (XMLread.MoveToAttribute("dump"))
                        crt.Dump = XMLread.Value;
                    if (XMLread.MoveToAttribute("dumper"))
                        crt.Dumper = XMLread.Value;
                    if (XMLread.MoveToAttribute("datedumped"))
                        crt.DateDumped = XMLread.Value;
                    drom.Cartridges.Add(crt);
                }
                //board info
                if (XMLread.Name == "board" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("type"))
                        drom.Board_Type = XMLread.Value;
                    if (XMLread.MoveToAttribute("pcb"))
                        drom.Board_Pcb = XMLread.Value;
                    if (XMLread.MoveToAttribute("mapper"))
                        drom.Board_Mapper = XMLread.Value;
                }
                //prg info
                if (XMLread.Name == "prg" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("name"))
                        drom.PRG_name = XMLread.Value;
                    if (XMLread.MoveToAttribute("size"))
                        drom.PRG_size = XMLread.Value;
                    if (XMLread.MoveToAttribute("crc"))
                        drom.PRG_crc = XMLread.Value;
                    if (XMLread.MoveToAttribute("sha1"))
                        drom.PRG_sha1 = XMLread.Value;
                }
                //chr info
                if (XMLread.Name == "chr" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("name"))
                        drom.CHR_name = XMLread.Value;
                    if (XMLread.MoveToAttribute("size"))
                        drom.CHR_size = XMLread.Value;
                    if (XMLread.MoveToAttribute("crc"))
                        drom.CHR_crc = XMLread.Value;
                    if (XMLread.MoveToAttribute("sha1"))
                        drom.CHR_sha1 = XMLread.Value;
                }
                //vram info
                if (XMLread.Name == "vram" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("size"))
                        drom.VRAM_size = XMLread.Value;
                }
                //chip info
                if (XMLread.Name == "chip" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("type"))
                    {
                        if (drom.chip_type == null)
                            drom.chip_type = new List<string>();
                        drom.chip_type.Add(XMLread.Value);
                    }
                }
                //cic info
                if (XMLread.Name == "cic" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("type"))
                        drom.CIC_type = XMLread.Value;
                }
                //pad info
                if (XMLread.Name == "pad" & XMLread.IsStartElement())
                {
                    if (XMLread.MoveToAttribute("h"))
                        drom.PAD_h = XMLread.Value;
                    if (XMLread.MoveToAttribute("v"))
                        drom.PAD_v = XMLread.Value;
                }
            }
            XMLread.Close();
            databaseStream.Close();
        }

        /// <summary>
        /// Get the data base roms collection (found in xml file)
        /// </summary>
        public List<NesDatabaseGameInfo> DatabaseRoms
        {
            get { return _databaseRoms; }
        }
        /// <summary>
        /// Find a NesDatabaseRomInfo element
        /// </summary>
        /// <param name="Cart_sha1">The sha1 to match, file sha1 without header of INES (start index 16)</param>
        /// <returns>The matched element if found, otherwise null</returns>
        public NesDatabaseGameInfo Find(string Cart_sha1)
        {
            foreach (NesDatabaseGameInfo item in _databaseRoms)
            {
                foreach (NesDatabaseCartridgeInfo crt in item.Cartridges)
                    if (crt.SHA1.ToLower() == Cart_sha1.ToLower())
                        return item;
            }
            return new NesDatabaseGameInfo();//null
        }
    }
}
