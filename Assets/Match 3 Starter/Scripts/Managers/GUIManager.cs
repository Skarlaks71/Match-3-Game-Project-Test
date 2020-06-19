/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour {
	public static GUIManager instance;

	public GameObject gameOverPanel;
    public GameObject playButton;
    public GameObject nextButton;
    [SerializeField]
	private Text _yourScoreTxt;
	[SerializeField]
	private Text _highScoreTxt;

	[SerializeField]
	private Text _scoreTxt;
	[SerializeField]
	private Text _moveCounterTxt;
    [SerializeField]
    private Text _metaScoreTxt;

    [SerializeField]
    private int _timeValue;
    [SerializeField]
    private int _metaValue;

	private int _score;
	private int _moveCounter;
    private int _metaScore;

    #region Propert Score, moveCounter and metaScore
    public int Score
    {
        get
        {
            return _score;
        }

        set
        {
            _score = value;
            _scoreTxt.text = _score.ToString();
        }
    }

    public int MoveCounter
    {
        get
        {
            return _moveCounter;
        }

        set
        {
            _moveCounter = value;
            if (_moveCounter <= 0)
            {
                _moveCounter = 0;
                StartCoroutine(WaitForShifting());
            }
            _moveCounterTxt.text = _moveCounter.ToString();
        }
    }
    public int MetaScore
    {
        get
        {
            return _metaScore;
        }

        set
        {
            _metaScore = value;
        }
    }
    #endregion

    void Awake() {
        if(_metaValue > GameManager.instance.newMeta)
        {
            _metaScore = _metaValue;
            Debug.Log("oi");
        }
        else
        {
            _metaScore = GameManager.instance.newMeta;
        }
        _metaScoreTxt.text = _metaScore.ToString();
        _moveCounter = _timeValue;
        _moveCounterTxt.text = _moveCounter.ToString();
		instance = GetComponent<GUIManager>();
	}
    private void Start()
    {
        Debug.Log(MetaScore);
    }

    private IEnumerator WaitForShifting()
    {
        yield return new WaitUntil(() => !BoardManager.instance.IsShifting);
        yield return new WaitForSeconds(.25f);
        GameOver();
    }

    // Show the game over panel
    public void GameOver() {
		GameManager.instance.gameOver = true;

		gameOverPanel.SetActive(true);

        // verify if the meta score has be complete and add new meta
        if(_score > _metaScore)
        {
            playButton.SetActive(false);
            GameManager.instance.newMeta = MetaScore + 1000;
        }
        else
        {
            nextButton.SetActive(false);
            
        }

		if (_score > PlayerPrefs.GetInt("HighScore")) {
			PlayerPrefs.SetInt("HighScore", _score);
			_highScoreTxt.text = "New Best: " + PlayerPrefs.GetInt("HighScore").ToString();
		} else {
			_highScoreTxt.text = "Best: " + PlayerPrefs.GetInt("HighScore").ToString();
		}

		_yourScoreTxt.text = _score.ToString();
	}

}
