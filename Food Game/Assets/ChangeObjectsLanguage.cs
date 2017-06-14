using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeObjectsLanguage : MonoBehaviour {

	[Multiline]
	public string danish;
	[Multiline]
	public string english;

	public Text myTextField;

	public bool isUsingDelegate = false;

	void Awake(){
		if (isUsingDelegate) {
			GameGenerator.ChangeToLanguage += UpdateLanguage;
		}
	}

	public void UpdateLanguage(bool languageIsDanish){
		myTextField.text = (languageIsDanish ? danish : english);
	}

	void OnEnable(){
		UpdateLanguage (GameGenerator.isCurrentlyDanish);
	}
}