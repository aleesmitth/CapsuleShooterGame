using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairConfig : MonoBehaviour {
    
    private Color _colorUnlocked = Color.red;
    private Color _colorLocked = Color.green;
    private bool _isCursorLocked = false;
    private Vector3 _currentScale;
    
    public Image cursorSprite;

    // Start is called before the first frame update

    // Update is called once per frame

    private void Start() {
        Cursor.visible = false;
         _currentScale = cursorSprite.rectTransform.lossyScale;
        LockCursorOnScreenHandler();
    }

    private void Update() {
        LockCursorOnScreenHandler();
        KeyboardEventHandler();
    }

    private void KeyboardEventHandler() {
        if (Input.GetKey(KeyCode.Alpha1)) {
            _colorLocked = Color.white;
            cursorSprite.color = _colorLocked;
        }
        else if (Input.GetKey(KeyCode.Alpha2)) {
            _colorLocked = Color.black;
            cursorSprite.color = _colorLocked;
        }
        else if (Input.GetKey(KeyCode.Alpha3)) {
            _colorLocked = Color.red;
            cursorSprite.color = _colorLocked;
        }
        else if (Input.GetKey(KeyCode.Alpha4)) {
            _colorLocked = Color.green;
            cursorSprite.color = _colorLocked;
        }
        if (Input.GetKey("[+]")) {
            cursorSprite.rectTransform.localScale += _currentScale*.1f;
        }
        
        if (Input.GetKey("[-]")) {
            cursorSprite.rectTransform.localScale -= _currentScale*.1f;
        }
    }

    private void ChangeSpriteAccordingly() {
        cursorSprite.color = _isCursorLocked ? _colorLocked : _colorUnlocked; //hermosa ":?" expression
    }


    private void LockCursorOnScreenHandler() {
        if (!_isCursorLocked && !Input.GetKey(KeyCode.Escape) && Input.anyKey) {
            Cursor.lockState = CursorLockMode.Locked;
            _isCursorLocked = true;
            Cursor.visible = false;
            ChangeSpriteAccordingly();
        }
        else if (_isCursorLocked && Input.GetKey(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
            _isCursorLocked = false;
            Cursor.visible = true;
            ChangeSpriteAccordingly();
        }
    }
}
