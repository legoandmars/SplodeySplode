using CrossyRoad.UI;
using CrossyRoad.World;
using UnityEngine;

namespace CrossyRoad.Score
{
    public class ScoreController : MonoBehaviour
    {
        [SerializeField]
        private WorldSegmentController _worldSegmentController = null!;

        [SerializeField]
        private ScoreTextPanel _scoreTextPanel = null!;

        // all time highscore here?
        private int _maxScore = 0;
        private int _highScore = 0;
        private string _highScoreKey = "HighScore";
        
        public void Start()
        {
            _scoreTextPanel.SetScore(0);

            if (PlayerPrefs.HasKey(_highScoreKey))
            {
                _highScore = PlayerPrefs.GetInt(_highScoreKey);
                Debug.Log("LETS GO?");
                Debug.Log(_highScore);
            }
            
            _scoreTextPanel.SetHighScore(_highScore);
        }
        
        public void UpdateScore(int score)
        {
            if (score > _maxScore)
            {
                _maxScore = score;
                _scoreTextPanel.SetScore(_maxScore);
                _worldSegmentController.ScoreUpdated(_maxScore);
            }

            if (score > _highScore)
            {
                _scoreTextPanel.SetHighScore(score);
            }
        }

        public void SaveHighScoreIfNeeded()
        {
            if (_maxScore > _highScore)
            {
                Debug.Log("Setting high score...");
                PlayerPrefs.SetInt(_highScoreKey, _maxScore);
                _highScore = _maxScore;
            }
        }
        // handle score display/saving on death here
    }
}