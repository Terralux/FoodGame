using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickMethodContainer : MonoBehaviour {
	public void OnClick(){
		if (GetComponent<Image> () != null) {
			transform.parent.GetComponent<GameGenerator> ().IGotClicked (GetComponent<Image> ().sprite);
		} else {
			transform.parent.GetComponent<GameGenerator> ().IGotClicked (GetComponent<Text> ().text);
		}
	}
}