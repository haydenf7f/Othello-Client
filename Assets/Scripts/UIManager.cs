using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI topText;

    [SerializeField]
    private TextMeshProUGUI blackScoreText;

    [SerializeField]
    private TextMeshProUGUI whiteScoreText;

    [SerializeField]
    private TextMeshProUGUI winnerText;

    [SerializeField]
    private Image blackOverlay;

    [SerializeField]
    private RectTransform playAgainButton;

    public void SetPlayerText(Player currentPlayer) {
        if (currentPlayer == Player.Black) {
            topText.text = "Black's Turn <sprite name=DiscBlackUp>";
        } else  if (currentPlayer == Player.White) {
            topText.text = "White's Turn <sprite name=DiscWhiteUp>";
        }
        else {
            topText.text = "";
        }
    }

    public void SetSkippedText(Player skippedPlayer) {
        if (skippedPlayer == Player.Black) {
            topText.text = "Black Cannot Move <sprite name=DiscBlackUp>";
        } else if (skippedPlayer == Player.White) {
            topText.text = "White Cannot Move <sprite name=DiscWhiteUp>";
        }
    }

    public void SetTopText(string message) {
        topText.text = message;
    }

    public IEnumerator AnimateTopText() {
        topText.transform.LeanScale(Vector3.one * 1.2f, 0.25f).setLoopPingPong(2);
        yield return new WaitForSeconds(1);
    }

    private IEnumerator ScaleDown(RectTransform rect) {
        rect.LeanScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);
        rect.gameObject.SetActive(false);
    }

    private IEnumerator ScaleUp(RectTransform rect) {
        rect.gameObject.SetActive(true);
        rect.localScale = Vector3.zero;
        rect.LeanScale(Vector3.one, 0.2f);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator ShowScoreText() {
        yield return ScaleDown(topText.rectTransform);
        yield return ScaleUp(blackScoreText.rectTransform);
        yield return ScaleUp(whiteScoreText.rectTransform);
    }
    
    public void SetBlackScoreText(int score) {
        blackScoreText.text = $"<sprite name=DiscBlackUp> {score}";
    }

    public void SetWhiteScoreText(int score) {
        whiteScoreText.text = $"<sprite name=DiscWhiteUp> {score}";
    }

    private IEnumerator ShowOverlay() {
        blackOverlay.gameObject.SetActive(true);
        blackOverlay.color = Color.clear;
        blackOverlay.rectTransform.LeanAlpha(0.8f, 1);
        yield return new WaitForSeconds(1);
    }

    private IEnumerator HideOverlay() {
        blackOverlay.rectTransform.LeanAlpha(0, 1);
        yield return new WaitForSeconds(1);
        blackOverlay.gameObject.SetActive(false);
    }

    public void SetWinnerText(Player winner) {
        if (winner == Player.Black) {
            winnerText.text = "Black Wins!";
        } else if (winner == Player.White) {
            winnerText.text = "White Wins!";
        } else {
            winnerText.text = "It's a Draw!";
        }
    }

    public IEnumerator ShowEndScreen() {
        yield return ShowOverlay();
        yield return ScaleUp(winnerText.rectTransform);
        yield return ScaleUp(playAgainButton);
    }

    public IEnumerator HideEndScreen() {
        StartCoroutine(ScaleDown(winnerText.rectTransform));
        StartCoroutine(ScaleDown(blackScoreText.rectTransform));
        StartCoroutine(ScaleDown(whiteScoreText.rectTransform));
        StartCoroutine(ScaleDown(playAgainButton));

        yield return new WaitForSeconds(0.5f);
        yield return HideOverlay();
    }
}
