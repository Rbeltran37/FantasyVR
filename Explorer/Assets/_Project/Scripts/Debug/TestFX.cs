using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestFX : MonoBehaviour
{
    public Text currentText;
    public Text previousText;
    public Text nextText;


    private List<GameObject> gameObjects = new List<GameObject>();
    private int childcount = 0;
    private int currentIndex = 0;


    private void Awake() {

        populateList();
        updateText();
    }


    public void next() {

        gameObjects[currentIndex].SetActive(false);

        currentIndex = getNextIndex();
        activateGameObject();
        updateText();
    }

    public void previous() {

        gameObjects[currentIndex].SetActive(false);

        currentIndex = getPreviousIndex();
        activateGameObject();
        updateText();
    }

    public void replay() {

        gameObjects[currentIndex].SetActive(false);
        activateGameObject();
        updateText();
    }


    private void populateList() {
        
        childcount = gameObject.transform.childCount;
        for (int i = 0; i < childcount; i++) {
            gameObjects.Add(gameObject.transform.GetChild(i).gameObject);
        }
    }

    private int getNextIndex() {

        var nextIndex = currentIndex + 1;
        if (nextIndex >= childcount) {
            nextIndex = 0;
        }
        return nextIndex;
    }

    private int getPreviousIndex() {

        var previousIndex = currentIndex - 1;
        if (previousIndex < 0) {
            previousIndex = 0;
        }
        return previousIndex;
    }

    private void activateGameObject() {

        gameObjects[currentIndex].transform.localPosition = new Vector3();
        gameObjects[currentIndex].SetActive(true);
    }

    private void updateText() {

        currentText.text = "Current: " + gameObjects[currentIndex].name;
        previousText.text = "Previous: " + gameObjects[getPreviousIndex()].name;
        nextText.text = "Next: " + gameObjects[getNextIndex()].name;
    }
}
