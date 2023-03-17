using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private Disc discBlackUp;

    [SerializeField]
    private Disc discWhiteUp;

    [SerializeField]
    private GameObject highlightPrefab;

    [SerializeField]
    private UIManager uiManager;

    private Dictionary<Player, Disc> discPrefabs = new Dictionary<Player, Disc>();
    private GameState gameState = new GameState();
    private Disc[,] discs = new Disc[8, 8];
    private List<GameObject> highlights = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {   
        // Add the disc prefabs to the dictionary
        discPrefabs[Player.Black] = discBlackUp;
        discPrefabs[Player.White] = discWhiteUp;
        
        AddStarterDiscs();
        ShowLegalMoves();
        uiManager.SetPlayerText(gameState.CurrentPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if (Input.GetMouseButtonDown(0)) {

            // Use a raycast from the camera to the mouse position to get the position on the board
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // Check if the raycast hits a highlight GameObject (highlights are the only objects with colliders)
            if (Physics.Raycast(ray, out RaycastHit hitInfo)) {
                Vector3 impactPos = hitInfo.point;
                Position boardPos = GetBoardPos(impactPos);
                OnBoardClicked(boardPos);
            }
        }  
    }

    private void OnBoardClicked(Position pos) {
        if (gameState.MakeMove(pos, out MoveInfo moveInfo)) {
            StartCoroutine(OnMoveMade(moveInfo));
        }
    }

    private IEnumerator OnMoveMade(MoveInfo moveInfo) {
        HideLegalMoves();
        yield return ShowMove(moveInfo);
        yield return ShowTurnOutcome(moveInfo);
        ShowLegalMoves();
    }

    private void ShowLegalMoves() {
        /* 
            * Show the legal moves on the board
        */

        // Loop through all of the legal moves and instantiate a highlight at the position of each legal move
        foreach (Position pos in gameState.LegalMoves.Keys) {
            Vector3 scenePos = CenteredCoordinatePos(pos) + Vector3.up * 0.01f;
            GameObject highlight = Instantiate(highlightPrefab, scenePos, Quaternion.identity);
            highlights.Add(highlight);
        }
    }

    private void HideLegalMoves() {
        /* 
            * Destroy all of the highlights
        */

        highlights.ForEach(Destroy);
        highlights.Clear();
    }

    private Position GetBoardPos(Vector3 pos) {
        /*
            * Get the column and row of the position on the board
        */

        // Subtract 0.25f from the x and z coordinates to account for the offset of the board
        int column = Mathf.FloorToInt(pos.x - 0.25f);
        // Subtract from 7 because we count the rows from the top down but the z coordinates are from the bottom up
        int row = 7 - Mathf.FloorToInt(pos.z - 0.25f);
        return new Position(row, column);
    }

    private Vector3 CenteredCoordinatePos(Position pos) {
        /*
            * Get the position of the center of the square at the given position
        */

        // We offset 0.25 for the border width and 0.5 for the center of the square
        float x = pos.Column + 0.75f;
        float z = 7 - pos.Row + 0.75f;
        return new Vector3(x, 0, z);
    }

    private void PlaceDisc(Disc prefab, Position boardPos) {
        /*
            * Place a disc at the given position
        */

        // Get the position of the center of the square at the given position and offset the y coordinate by a factor of 0.1f so that the whole disc is above the board
        Vector3 centeredPos = CenteredCoordinatePos(boardPos) + Vector3.up * 0.1f;

        // Instantiate the disc at the given position and store it in the discs array
        discs[boardPos.Row, boardPos.Column] = Instantiate(prefab, centeredPos, Quaternion.identity);

        // Debug.Log("Placing disc at [" + boardPos.Row + ", " + boardPos.Column + "]" + " (" + centeredPos.x + ", " + centeredPos.z + ")");

    }

    private void AddStarterDiscs() {
        /*
            * Add the starting discs to the board
        */

        foreach (Position boardPos in gameState.OccupiedPositions()) {
            // Get the player at the given position
            Player player = gameState.Board[boardPos.Row, boardPos.Column];
            // Place a disc of the given player at the given position
            PlaceDisc(discPrefabs[player], boardPos);
        }
    }

    private void FlipDiscs(List<Position> positions) {
        /*
            * Flip the discs at the given positions
        */

        foreach (Position pos in positions) {
            // Flip the disc at the given position
            discs[pos.Row, pos.Column].Flip();
        }
    }

    private IEnumerator ShowMove(MoveInfo moveInfo) {
        /*
            * Show the move on the board
        */

        // Place the disc at the given position
        PlaceDisc(discPrefabs[moveInfo.Player], moveInfo.Position);
        // Wait for 0.33 seconds because of the animation
        yield return new WaitForSeconds(0.33f);
        // Flip the discs that were outflanked
        FlipDiscs(moveInfo.Outflanked);
        // Wait for 0.83 seconds because of the animation
        yield return new WaitForSeconds(0.83f);
    }

    private IEnumerator ShowTurnSkipped(Player skippedPlayer) {
        uiManager.SetSkippedText(skippedPlayer);
        yield return uiManager.AnimateTopText();
    }

    private IEnumerator ShowTurnOutcome(MoveInfo moveInfo) {
        if (gameState.GameOver) {
            yield return ShowGameOver(gameState.Winner);
            yield break;
        }
        
        Player currentPlayer = gameState.CurrentPlayer;

        // If the current player if the same as the player who made the move, the other player's move was skipped
        if (currentPlayer == moveInfo.Player) {
            yield return ShowTurnSkipped(currentPlayer.Other());
        }

        uiManager.SetPlayerText(currentPlayer);
    }

    private IEnumerator ShowGameOver(Player winner) {
        uiManager.SetTopText("Game Over");
        yield return uiManager.AnimateTopText();
        yield return uiManager.ShowScoreText();
        yield return new WaitForSeconds(0.5f);
        yield return ShowCounting();
        uiManager.SetWinnerText(winner);
        yield return uiManager.ShowEndScreen();
    }

    private IEnumerator ShowCounting() {
        /*
            * Show the counting animation at the end of the game
        */

        int black = 0;
        int white = 0;

        foreach(Position pos in gameState.OccupiedPositions()) {

            Player player = gameState.Board[pos.Row, pos.Column];

            if (player == Player.Black) {
                black++;
                uiManager.SetBlackScoreText(black);
            } else {
                white++;
                uiManager.SetWhiteScoreText(white);
            }

            discs[pos.Row, pos.Column].Jump();
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator RestartGame() {
        yield return uiManager.HideEndScreen();
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    public void OnPlayAgainClicked() {
        StartCoroutine(RestartGame());
    }
}
