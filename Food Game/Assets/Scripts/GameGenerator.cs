using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameGenerator : MonoBehaviour {

	public delegate void ChangeLanguage(bool languageIsDanish);
	public static ChangeLanguage ChangeToLanguage;

	public bool findWordToMatchImage = true;

	public GameObject textObject;
	public GameObject imageObject;
	public List<VegetableScriptableObject> myVeggies = new List<VegetableScriptableObject>();
	private List<VegetableScriptableObject> selectedVeggies = new List<VegetableScriptableObject> ();

	private int correctVeggieIndex;
	private GameObject correct;
	private GameObject wrong;

	public delegate void ImageClicked(Sprite sprite);
	public delegate void TextClicked(string name);

	[HideInInspector]
	public static bool isCurrentlyDanish = true;

	private GameObject questionObject;

	[Range(0,69)]
	public int veggieCount;

	void Awake () {
		isCurrentlyDanish = true;

		correct = GameObject.FindGameObjectWithTag ("Correct");
		wrong = GameObject.FindGameObjectWithTag ("Wrong");
		correct.SetActive (false);
		wrong.SetActive (false);
	}

	public void BeginGame(bool isComparingTextToImages){
		findWordToMatchImage = isComparingTextToImages;
		NewQuizRound ();
	}

	void NewQuizRound(){
		ShuffleList ();
		GetVeggieSelection ();
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

	void GetVeggieSelection(){
		selectedVeggies.Clear ();

		if (veggieCount == 0) {
			selectedVeggies = myVeggies;
		} else {
			for (int i = 0; i < veggieCount; i++) {
				int randomIndex = Random.Range (0, myVeggies.Count);

				while (selectedVeggies.Contains (myVeggies [randomIndex])) {
					randomIndex = Random.Range (0, myVeggies.Count);
				}

				selectedVeggies.Add (myVeggies [randomIndex]);
			}
		}
	}

	void GenerateCorrectAnswer(){
		correctVeggieIndex = Random.Range (0, selectedVeggies.Count);

		if (findWordToMatchImage) {
			questionObject = Instantiate (imageObject, transform);
			questionObject.GetComponent<Image> ().sprite = selectedVeggies [correctVeggieIndex].vegetableImage;
		} else {
			questionObject = Instantiate (textObject, transform);
			questionObject.GetComponent<Text> ().text = (isCurrentlyDanish ? selectedVeggies [correctVeggieIndex].danishName : selectedVeggies [correctVeggieIndex].englishName);
		}

		questionObject.transform.localPosition = Vector3.zero;
	}

	void GenerateIconPlacements(){
		float angle = 360f / (float)selectedVeggies.Count;
		Vector3 targetVector = Vector3.up;

		for (int i = 0; i < selectedVeggies.Count; i++) {
			targetVector = Quaternion.Euler (0, 0, angle) * targetVector;

			GameObject go;

			if (findWordToMatchImage) {
				go = Instantiate (textObject, transform);
				ClickMethodContainer cmc = go.GetComponent<ClickMethodContainer> ();
				go.GetComponent<Text> ().text = (isCurrentlyDanish ? selectedVeggies [i].danishName : selectedVeggies [i].englishName);
				go.GetComponent<Button> ().onClick.AddListener (cmc.OnClick);
				cmc.danish = selectedVeggies [i].danishName;
				cmc.english = selectedVeggies [i].englishName;

				ChangeToLanguage += cmc.UpdateLanguage;
			} else {
				go = Instantiate (imageObject, transform);
				ClickMethodContainer cmc = go.GetComponent<ClickMethodContainer> ();
				go.GetComponent<Image> ().sprite = selectedVeggies [i].vegetableImage;
				go.GetComponent<Button> ().onClick.AddListener (cmc.OnClick);
			}

			go.transform.localPosition = Camera.main.ViewportToScreenPoint (targetVector) * 0.3f;
		}
	}

	public void IGotClicked(Sprite mySprite){
		if (selectedVeggies [correctVeggieIndex].vegetableImage == mySprite) {
			IReceivedAnAnswer(true);
		} else {
			IReceivedAnAnswer(false);
		}
	}

	public void IGotClicked(string text){
		Debug.Log (correctVeggieIndex);
		if (selectedVeggies [correctVeggieIndex].danishName == text || selectedVeggies [correctVeggieIndex].englishName == text) {
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
				if (t.GetComponent<ClickMethodContainer> () != null) {
					ChangeToLanguage -= t.GetComponent<ClickMethodContainer> ().UpdateLanguage;
				}
				Destroy (t.gameObject);
			}
		}
	}

	public void ChangedLanguage(bool languageIsDanish){
		isCurrentlyDanish = languageIsDanish;

		if (correct.activeInHierarchy) {
			correct.GetComponent<ChangeObjectsLanguage> ().UpdateLanguage (languageIsDanish);
		}

		if (wrong.activeInHierarchy) {
			wrong.GetComponent<ChangeObjectsLanguage> ().UpdateLanguage (languageIsDanish);
		}

		if (!findWordToMatchImage) {
			if (questionObject != null) {
				questionObject.GetComponent<Text> ().text = (languageIsDanish ? selectedVeggies [correctVeggieIndex].danishName : selectedVeggies [correctVeggieIndex].englishName);
			}
		}

		ChangeToLanguage (languageIsDanish);
	}
}