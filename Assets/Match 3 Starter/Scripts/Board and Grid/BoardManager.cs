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
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {
	public static BoardManager instance;
    [SerializeField]
	private List<Sprite> _characters = new List<Sprite>();
    [SerializeField]
	private GameObject _tile;
    [SerializeField]
	private int _xSize, _ySize;

	private GameObject[,] _tiles;

	public bool IsShifting { get; set; }

	void Start () {
		instance = GetComponent<BoardManager>();

		Vector2 offset = _tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
    }

    public IEnumerator FindNullTiles()
    {
        for (int x = 0; x < _xSize; x++)
        {
            for (int y = 0; y < _ySize; y++)
            {
                if (_tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
            }
        }
        for (int x = 0; x < _xSize; x++)
        {
            for (int y = 0; y < _ySize; y++)
            {
                _tiles[x, y].GetComponent<Tile>().ClearAllMatches();
            }
        }

    }

    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f)
    {
        IsShifting = true;
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;

        for (int y = yStart; y < _ySize; y++)
        {  
            SpriteRenderer render = _tiles[x, y].GetComponent<SpriteRenderer>();
            if (render.sprite == null)
            { 
                nullCount++;
            }
            renders.Add(render);
        }

        for (int i = 0; i < nullCount; i++)
        { 
            yield return new WaitForSeconds(shiftDelay);
            for (int k = 0; k < renders.Count-1; k++)
            { 
                renders[k].sprite = renders[k + 1].sprite;
                renders[k + 1].sprite = GetNewSprite(x, _ySize - 1);
            }
        }
        IsShifting = false;
    }

    private void CreateBoard (float xOffset, float yOffset) {
		_tiles = new GameObject[_xSize, _ySize];

        float startX = transform.position.x;
		float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[_ySize];
        Sprite previousBelow = null;

        for (int x = 0; x < _xSize; x++) {
			for (int y = 0; y < _ySize; y++) {
                
                GameObject newTile = Instantiate(_tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), _tile.transform.rotation);
				_tiles[x, y] = newTile;
                newTile.transform.parent = transform; 

                List<Sprite> possibleCharacters = new List<Sprite>();
                possibleCharacters.AddRange(_characters);

                possibleCharacters.Remove(previousLeft[y]);
                possibleCharacters.Remove(previousBelow);

                Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];

                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
                previousLeft[y] = newSprite;
                previousBelow = newSprite;


            }
        }

        

    }

    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(_characters);

        if (x > 0)
        {
            possibleCharacters.Remove(_tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < _xSize - 1)
        {
            possibleCharacters.Remove(_tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0)
        {
            possibleCharacters.Remove(_tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }


}
