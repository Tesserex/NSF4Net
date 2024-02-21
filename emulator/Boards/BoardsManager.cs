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
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using MyNes.Core.ROM;

namespace MyNes.Core.Boards
{
    public class BoardsManager
    {
        private static Board[] boards;

        //Methods
        public static void LoadAvailableBoards()
        {
            List<Board> availableBoards = new List<Board>();
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type tp in types)
            {
                if (tp.IsSubclassOf(typeof(Board)))
                {
                    if (!tp.IsAbstract)
                    {
                        Board board = Activator.CreateInstance(tp) as Board;
                        availableBoards.Add(board);
                    }
                }
            }
            availableBoards.Sort(new BoardSorter(true, false));
            boards = availableBoards.ToArray();
        }
        public static Board GetBoard(INESHeader header, byte[] chr, byte[] prg, byte[] trainer)
        {
            foreach (Board board in boards)
            {
                if (board.INESMapperNumber == header.Mapper)
                {
                    Type boardType = board.GetType();
                    return ((Board)Activator.CreateInstance(boardType, new object[] { chr, prg, trainer, header.IsVram }));
                }
            }
            return null;
        }

        //Properties
        public static Board[] AvailableBoards { get { return boards; } }
    }
    class BoardSorter : IComparer<Board>
    {
        bool AtoZ = true;
        bool isMappers = false;
        public BoardSorter(bool AtoZ, bool isMappers)
        {
            this.AtoZ = AtoZ;
            this.isMappers = isMappers;
        }
        public int Compare(Board x, Board y)
        {
            if (!isMappers)
            {
                if (AtoZ)
                    return (StringComparer.Create(System.Threading.Thread.CurrentThread.CurrentCulture, false)).Compare(x.Name, y.Name);
                else
                    return (-1 * (StringComparer.Create(System.Threading.Thread.CurrentThread.CurrentCulture, false)).Compare(x.Name, y.Name));
            }
            else
            {
                if (AtoZ)
                    return (StringComparer.Create(System.Threading.Thread.CurrentThread.CurrentCulture, false)).Compare(x.INESMapperNumber, y.INESMapperNumber);
                else
                    return (-1 * (StringComparer.Create(System.Threading.Thread.CurrentThread.CurrentCulture, false)).Compare(x.INESMapperNumber, y.INESMapperNumber));
            }
        }
    }
}
