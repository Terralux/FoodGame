using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickMethodContainer : MonoBehaviour {

	[HideInInspector]
	public string danish;
	[HideInInspector]
	public string english;

	public void OnClick(){
		if (GetComponent<Image> () != null) {
			transform.parent.GetComponent<GameGenerator> ().IGotClicked (GetComponent<Image> ().sprite);
		} else {
			transform.parent.GetComponent<GameGenerator> ().IGotClicked (GetComponent<Text> ().text);
		}
	}

	public void UpdateLanguage(bool languageIsDanish){
		GetComponent<Text> ().text = (languageIsDanish ? danish : english);
	}
}