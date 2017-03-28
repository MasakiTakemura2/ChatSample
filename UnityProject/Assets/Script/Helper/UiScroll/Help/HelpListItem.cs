using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using EventManager;
using Http;
using ViewController;


public class HelpListItem : UIBehaviour 
{
    [SerializeField]
    private Text _question;
	[SerializeField]
	private Text _answer;


    /// <summary>
    /// Updates the item.
    /// </summary>
    /// <returns>The item.</returns>
    /// <param name="count">Count.</param>
    /// <param name="board">Board.</param>
	public void UpdateItem (int count, string question, string answer)
    {
		Debug.Log ("question " + question);
		Debug.Log ("answer " + answer);

		_question.text = question;
		_answer.text = answer;
	}
}
