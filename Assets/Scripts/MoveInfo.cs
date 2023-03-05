using System.Collections.Generic;

public class MoveInfo  {
    // The player that made the move
    public Player Player { get; set; }

    // The position that was played
    public Position Position { get; set; }

    // The positions that were flipped by this move
    public List<Position> Outflanked { get; set; }
}
