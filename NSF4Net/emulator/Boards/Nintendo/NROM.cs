﻿/* This file is part of My Nes
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
/*Written by Adam Becker*/
using NSF4Net;

namespace MyNes.Core.Boards.Nintendo
{
    [BoardName("NROM", 0)]
    [BoardSoundChip((byte)EXT_SOUND.NONE)]
    public class NROM : Board
    {
        public NROM()
            : base()
        { }
        public NROM(byte[] chr, byte[] prg, byte[] trainer, bool isVram)
            : base(chr, prg, trainer, isVram)
        { }
    }
}