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

public class Tile : MonoBehaviour {
	private static Color _selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile _previousSelected = null;

	private SpriteRenderer _render;
	private bool _isSelected = false;

	private Vector2[] _adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private bool _matchFound = false;

    void Awake() {
		_render = GetComponent<SpriteRenderer>();
    }

	private void Select() {
		_isSelected = true;
		_render.color = _selectedColor;
		_previousSelected = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect() {
		_isSelected = false;
		_render.color = Color.white;
		_previousSelected = null;
	}

    void OnMouseDown()
    {
        
        if (_render.sprite == null || BoardManager.instance.IsShifting)
        {
            return;
        }

        if (_isSelected)
        { // Is it already selected?
            Deselect();
        }
        else
        {
            if (_previousSelected == null)
            { //  Is it the first tile selected?
                Select();
            }
            else
            {
                if (GetAllAdjacentTiles().Contains(_previousSelected.gameObject))
                { // verify if this tile is adjacent to previous tile.
                    SwapSprite(_previousSelected._render);
                    _previousSelected.ClearAllMatches();
                    _previousSelected.Deselect();
                    ClearAllMatches();

                }
                else
                { // if tile selected isn't adjacent deselect and select this new tile.
                    _previousSelected.GetComponent<Tile>().Deselect();
                    Select();
                }
            }

        }
    }

    public void SwapSprite(SpriteRenderer render2)
    {
        if (_render.sprite == render2.sprite)
        { 
            return;
        }

        Sprite tempSprite = render2.sprite;
        render2.sprite = _render.sprite; 
        _render.sprite = tempSprite; 
        SFXManager.instance.PlaySFX(Clip.Swap);
        GUIManager.instance.MoveCounter--;

    }

    private GameObject GetAdjacent(Vector2 castDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private List<GameObject> GetAllAdjacentTiles()
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < _adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(_adjacentDirections[i]));
        }
        return adjacentTiles;
    }

    private List<GameObject> FindMatch(Vector2 castDir)
    { 
        List<GameObject> matchingTiles = new List<GameObject>(); 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir); 
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == _render.sprite)
        { 
            matchingTiles.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
        }
        return matchingTiles; 
    }

    private void ClearMatch(Vector2[] paths) 
    {
        List<GameObject> matchingTiles = new List<GameObject>(); 
        for (int i = 0; i < paths.Length; i++) 
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }
        if (matchingTiles.Count >= 2) 
        {
            for (int i = 0; i < matchingTiles.Count; i++) 
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            _matchFound = true; 
        }
    }

    public void ClearAllMatches()
    {
        if (_render.sprite == null)
            return;

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
        if (_matchFound)
        {
            _render.sprite = null;
            _matchFound = false;
            StopCoroutine(BoardManager.instance.FindNullTiles());
            StartCoroutine(BoardManager.instance.FindNullTiles());
            SFXManager.instance.PlaySFX(Clip.Clear);
            
        }
    }


}