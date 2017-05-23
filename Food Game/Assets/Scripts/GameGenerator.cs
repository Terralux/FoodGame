using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameGenerator : MonoBehaviour {

	public bool findWordToMatchImage = true;

	public GameObject textObject;
	public GameObject imageObject;
	public List<VegetableScriptableObject> myVeggies = new List<VegetableScriptableObject>();

	private int correctVeggieIndex;
	private GameObject correct;
	private GameObject wrong;

	public delegate void ImageClicked(Sprite sprite);
	public delegate void TextClicked(string name);

	void Awake () {
		correct = GameObject.FindGameObjectWithTag ("Correct");
		wrong = GameObject.FindGameObjectWithTag ("Wrong");
		correct.SetActive (false);
		wrong.SetActive (false);

		NewQuizRound ();
	}

	void NewQuizRound(){
		ShuffleList ();
		GenerateCorrectAnswer ();
		GenerateIconPlacements ();
	}

	void ShuffleList(){
		for (int i = 0; i < myVeggies.Count; i++) {
			VegetableScriptableObject vSO = myVeggies [i];
			int randomIndex = Random.Range (i, myVeggies.Count);
			myVeggies [i] = myVeggies [randomIndex];
			myVeggies [randomIndex] = vSO;
		}
	}

	void GenerateCorrectAnswer(){
		correctVeggieIndex = Random.Range (0, myVeggies.Count);

		GameObject go;

		if (findWordToMatchImage) {
			go = Instantiate (imageObject, transform);
			go.GetComponent<Image> ().sprite = myVeggies [correctVeggieIndex].vegetableImage;
		} else {
			go = Instantiate (textObject, transform);
			go.GetComponent<Text> ().text = myVeggies [correctVeggieIndex].name;
		}

		go.transform.localPosition = Vector3.zero;
	}

	void GenerateIconPlacements(){
		float angle = 360f / (float)myVeggies.Count;
		Vector3 targetVector = Vector3.up;

		for (int i = 0; i < myVeggies.Count; i++) {
			targetVector = Quaternion.Euler (0, 0, angle) * targetVector;

			GameObject go;

			if (findWordToMatchImage) {
				go = Instantiate (textObject, transform);
				go.GetComponent<Text> ().text = myVeggies [i].name;
				go.GetComponent<Button> ().onClick.AddListener (go.GetComponent<ClickMethodContainer>().OnClick);
				Debug.Log (go.GetComponent<Button> ().onClick.GetPersistentEventCount ());
			} else {
				go = Instantiate (imageObject, transform);
				go.GetComponent<Image> ().sprite = myVeggies [i].vegetableImage;
				go.GetComponent<Button> ().onClick.AddListener (go.GetComponent<ClickMethodContainer>().OnClick);
			}

			go.transform.localPosition = Camera.main.ViewportToScreenPoint (targetVector) * 0.3f;
		}
	}

	public void IGotClicked(Sprite mySprite){
		if (myVeggies [correctVeggieIndex].vegetableImage == mySprite) {
			IReceivedAnAnswer(true);
		} else {
			IReceivedAnAnswer(false);
		}
	}

	public void IGotClicked(string text){
		Debug.Log (correctVeggieIndex);
		if (myVeggies [correctVeggieIndex].name == text) {
			IReceivedAnAnswer(true);
		} else {
			IReceivedAnAnswer(false);
		}
	}

	void IReceivedAnAnswer(bool isCorrectAnswer){
		Debug.Log ("I got an answer");
		if (isCorrectAnswer) {
			correct.SetActive (true);
		} else {
			wrong.SetActive (true);
		}
		StartCoroutine (WaitForNextQuiz ());
	}

	IEnumerator WaitForNextQuiz(){
		yield return new WaitForSeconds (3f);
		correct.SetActive (false);
		wrong.SetActive (false);
		Clear ();
		NewQuizRound ();
	}

	void Clear(){
		foreach (Transform t in transform.GetComponentsInChildren<Transform>()) {
			if(t != transform && t != correct && t != wrong){
				Destroy (t.gameObject);
			}
		}
	}
}