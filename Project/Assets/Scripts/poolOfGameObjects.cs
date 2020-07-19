using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class poolOfGameObjects : MonoBehaviour {
    //todo/ el serializefield me deja modificarlo, o darle refenrecia desde
    //todo/ el inspector, es como si fuera public pero no lo es.
    [SerializeField]
    private GameObject prefab;
// esto es basicamente un constructor privado, es el patron singleton pero con
// auto-propertys, es medio quilombo pero bue.
    public static poolOfGameObjects Instance { get; private set; }
    
    private readonly Stack<GameObject> gameObjectsStack = new Stack<GameObject>();

    private void Awake() {
        Instance = this;
    }

    public void AddToPool(GameObject gameObject, int secondsUntilDeath) {
        StartCoroutine(WaitThisSecondsAndAddToPool(gameObject, secondsUntilDeath));
    }

    public GameObject GetPartycle() {
        if (gameObjectsStack.Any()) {
            return gameObjectsStack.Pop();
        }

        GameObject gameObject = Instantiate(prefab);
        gameObject.SetActive(false);
        return gameObject;
    }
    
    private IEnumerator WaitThisSecondsAndAddToPool(GameObject gameObject, int seconds) {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
        gameObjectsStack.Push(gameObject);
    }
}
