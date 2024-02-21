

To add new board, create new class and make it subclass of Board.cs
Example:

class NewBoard: Board
{
......
}

Add the attribute "BoardName" to specify the board name and mapper #

The board you add MUST contain two constructors, one with parameters and one with not.

The parameters MUST be (byte[] chr, byte[] prg, byte[] trainer, bool isVram) otherwise the board will not initialize.

Example:
[BoardName("New Board", x)]// x is INES mapper number, this will help to detect the board later.
class NewBoard: Board
{
        public NewBoard()
            : base()
        { }
        public NewBoard(byte[] chr, byte[] prg, byte[] trainer, bool isVram)//this MUST be like this.
            : base(chr, prg, trainer, isVram)
        { }
}

If your board is master board that other boards based on (like FFE.cs, this is a base board) then you MUST
mark this class as abstract to avoid to be detected.
See FFE.cs, it's abstract class so it's ignored by the manager while FFE_FE3xx.cs can be detected.

Never add duplicated board (e.g add two classes for the same mapper like adding UOROM and UNROM classes for the mapper 2). So
if the board behavior changed or there are more than board for the same mapper then you can implement this like it 
done in MMC1.cs

After adding the board class (anywhere you like in the project) the BoardsManager class will detect it automaticly and will
be active for use !!.