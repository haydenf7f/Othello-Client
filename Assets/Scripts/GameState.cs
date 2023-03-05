using System.Collections.Generic;

public class GameState {
    public const int Rows = 8;
    public const int Columns = 8;

    // A two-dimensional array of Player values
    public Player[,] Board { get; }

    // A dictionary of scores for the Player
    public Dictionary<Player, int> Score { get; }
    public Player CurrentPlayer { get; private set; }
    public bool GameOver { get; private set; }
    public Player Winner { get; private set; }

    // A dictionary of legal moves for the player
    public Dictionary<Position, List<Position>> LegalMoves { get; private set; }

    public GameState() {
        /*
            * Default constructor
            * Initializes the game state
        */

        // Set up the initial board
        Board = new Player[Rows, Columns];
        Board[3, 3] = Player.White;
        Board[3, 4] = Player.Black;
        Board[4, 3] = Player.Black;
        Board[4, 4] = Player.White;

        // Set up the initial scores
        Score = new Dictionary<Player, int>();
        Score.Add(Player.Black, 2);
        Score.Add(Player.White, 2);
        Score.Add(Player.None, 0);

        // Black goes first
        CurrentPlayer = Player.Black;

        // The game is not over and there is no winner yet
        GameOver = false;
        Winner = Player.None;

        // Find the legal moves for the current player
        LegalMoves = FindLegalMoves(CurrentPlayer);
    }

    public bool MakeMove(Position pos, out MoveInfo moveInfo) {
        /*
            * Makes a move for the current player at pos.
            * If the move is legal, the function returns true and moveInfo contains information about the move.
            * If the move is illegal, the function returns false and moveInfo is null.
        */

        // If the move is illegal, return false and set moveInfo to null
        if (!LegalMoves.ContainsKey(pos)) {
            moveInfo = null;
            return false;
        }

        // If the move is legal, store the current player.
        Player movePlayer = CurrentPlayer;
        // Get the outflanked positions from the dictionary of legal moves
        List<Position> outflankedPositions = LegalMoves[pos];

        // Update the state of the board
        Board[pos.Row, pos.Column] = movePlayer;

        // Flip the discs at the outlfanked positions
        FlipDiscs(outflankedPositions);

        // Update the score count for each player
        UpdateScore(movePlayer, outflankedPositions.Count);

        // Pass turn to the other player
        PassTurn();

        moveInfo = new MoveInfo { Player = movePlayer, Position = pos, Outflanked = outflankedPositions };
        return true;
    }

    public IEnumerable<Position> OccupiedPositions() {
        /*
            * Returns an enumerable list of positions that are occupied
        */
        for (int row = 0; row < Rows; row++) {
            for (int column = 0; column < Columns; column++) {
                if (Board[row, column] != Player.None) {
                    yield return new Position(row, column);
                }
            }
        }
    }

    
    private bool IsInsideBoard(int row, int column) {
        /*
            * Checks to see if the position (row, column) is inside the board
        */
        return row >= 0 && row < Rows && column >= 0 && column < Columns;
    }

    private List<Position> OutflankedInDirection(Player player, Position pos, int rDirection, int cDirection) {
        /*
            * Returns a list of positions that would be outflanked if the player played at pos.
            * rDirection and cDirection are the row and column directions to check.
            * For example, if rDirection is -1 and cDirection is 0, then the function will check
            * the row above the position.
            * Here is a table of the possible values for rDirection and cDirection:
            * |-------------------------------------|
            * | Direction | rDirection | cDirection |
            * |-------------------------------------|
            * | Up        | -1         | 0          |
            * | Down      | 1          | 0          |  
            * | Left      | 0          | -1         |
            * | Right     | 0          | 1          |
            * | Up Left   | -1         | -1         |
            * | Up Right  | -1         | 1          |
            * | Down Left | 1          | -1         |
            * | Down Right| 1          | 1          |   
            * |-------------------------------------|
        */
        List<Position> outflankedPositions = new List<Position>();

        // From the position, move in the direction specified by rDirection and cDirection
        int row = pos.Row + rDirection;
        int column = pos.Column + cDirection;

        // While the position is inside the board and the player at that position is not the current player
        while (IsInsideBoard(row, column) && Board[row, column] != Player.None) {
            if (Board[row, column] == player.Other()) {
                // If the player at that position is the opponent, add it to the list of outflanked positions
                outflankedPositions.Add(new Position(row, column));

                // Move to the next position in the direction specified by rDirection and cDirection
                row += rDirection;
                column += cDirection;
            } else {
                // If the player at that position is the current player, return the list of outflanked positions
                return outflankedPositions;
            }
        }

        // If the position moves outside the board and no outflanked positions are found, return an empty list
        return new List<Position>();    
    }

    private List<Position> Outflanked(Position pos, Player player) {
        /*
            * Returns a list of positions that would be outflanked if the player played at pos.
            * This function checks all eight directions.
        */

        List<Position> outflankedPositions = new List<Position>();

        // A nested loop that increments over all possible directions
        for( int rDirection = -1; rDirection <= 1; rDirection++) {
            for( int cDirection = -1; cDirection <= 1; cDirection++) {
                // Skip the case where rDirection and cDirection are both 0
                if (rDirection == 0 && cDirection == 0) {
                    continue;
                }
                outflankedPositions.AddRange(OutflankedInDirection(player, pos, rDirection, cDirection));
            }
        }

        return outflankedPositions;
    }

    private bool IsLegalMove(Player player, Position pos, out List<Position> outflankedPositions) {
        /*
            * Returns true if the player can play at pos.
            * If the player can play at pos, outflankedPositions will contain a list of positions that would be outflanked.
            * If the player cannot play at pos, outflankedPositions will be an empty list.
        */

        // If the position is not inside the board, the move is not legal
        if (!IsInsideBoard(pos.Row, pos.Column)) {
            outflankedPositions = null;
            return false;
        }

        // If the position is already occupied, the move is not legal
        if (Board[pos.Row, pos.Column] != Player.None) {
            outflankedPositions = null;
            return false;
        }

        // Get the list of positions that would be outflanked if the player played at pos
        outflankedPositions = Outflanked(pos, player);

        // If the list of outflanked positions is empty, the move is not legal
        if (outflankedPositions.Count == 0) {
            return false;
        }

        // The move is legal
        return true;
    }

    private Dictionary<Position, List<Position>> FindLegalMoves(Player player) {
        /*
            * Returns a dictionary of legal moves for the player.
            * The keys of the dictionary are the positions where the player can play.
            * The values of the dictionary are the lists of positions that would be outflanked if the player played at the key position.
        */

        Dictionary<Position, List<Position>> legalMoves = new Dictionary<Position, List<Position>>();

        // A nested loop that iterates over all positions on the board
        for (int row = 0; row < Rows; row++) {
            for (int column = 0; column < Columns; column++) {
                Position pos = new Position(row, column);

                // Check if the player can play at pos
                if(IsLegalMove(player, pos, out List<Position> outflankedPositions)) {
                    // If the player can play at pos, add it to the dictionary of legal moves
                    legalMoves.Add(pos, outflankedPositions);
                }
            }
        }

        return legalMoves;
    }


    private void FlipDiscs(List<Position> positions) {
        /*
            * Flips the discs at the positions in the list.
        */

        foreach (Position pos in positions) {
            Board[pos.Row, pos.Column] = Board[pos.Row, pos.Column].Other();
        }
    }

    private void UpdateScore(Player movePlayer, int outflankedCount) {
        /*
            * Updates the score count for each player.
        */

        // Update the score count for the player who made the move
        Score[movePlayer] += outflankedCount + 1;

        // Update the score count for the opponent
        Score[movePlayer.Other()] -= outflankedCount;
    }

    private void ChangePlayer() {
        /*
            * Passes the turn to the other player.
            * Updates the dictionary of legal moves for the other player.
        */

        CurrentPlayer = CurrentPlayer.Other();
        LegalMoves = FindLegalMoves(CurrentPlayer);
    }

    private void PassTurn() {
        // Passes the turn to the other player
        ChangePlayer();

        // If the other player has no legal moves, pass the turn again
        if (LegalMoves.Count == 0) {
            ChangePlayer();
            // If both players have no legal moves, the game is over
            if (LegalMoves.Count == 0) {
                CurrentPlayer = Player.None;
                GameOver = true;
                Winner = FindWinnner();
            }
        }
        return;
    }

    private Player FindWinnner() {
        /*
            * Returns the winner of the game.
            * If the score is equal returns Player.None.
        */
        if (Score[Player.Black] > Score[Player.White]) {
            return Player.Black;
        } else if (Score[Player.Black] < Score[Player.White]) {
            return Player.White;
        } else {
            return Player.None;
        }
    }


}
