using TMPro;
using UnityEngine;

namespace CrossyRoad.UI
{
    public class ScoreTextPanel : MonoBehaviour
    {
        [SerializeField]
        public TMP_Text _scoreText = null!;
        
        [SerializeField]
        public TMP_Text _scoreTextShadow = null!;

        [SerializeField]
        public TMP_Text _highScoreText = null!;
        
        [SerializeField]
        public TMP_Text _highScoreTextShadow = null!;

        public void SetScore(int score)
        {
            _scoreText.SetText(score.ToString());
            _scoreTextShadow.SetText(score.ToString());
        }
        
        public void SetHighScore(int highScore)
        {
            _highScoreText.SetText("BEST:\n"+highScore.ToString());
            _highScoreTextShadow.SetText("BEST:\n"+highScore.ToString());
        }
    }
}