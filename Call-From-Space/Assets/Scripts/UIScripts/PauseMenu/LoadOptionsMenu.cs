using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOptionsMenu : MonoBehaviour {
    public GameObject optionsMenu;
    public Interactor interactor;
    public void loadMenu() {
        
        optionsMenu.SetActive(true);
        interactor.inUI = true;
    }
}
