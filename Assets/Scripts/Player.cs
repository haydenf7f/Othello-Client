public enum Player {
    None, Black, White
}

public static class PlayerExtensions {
    // Returns the other player
    public static Player Other(this Player player) {
        switch (player) {
            case Player.Black:
                return Player.White;
            case Player.White:
                return Player.Black;
            default:
                return Player.None;
        }
    }
}
