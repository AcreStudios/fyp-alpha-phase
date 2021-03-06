﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class EventManager : MonoBehaviour {

    [System.Serializable]
    public struct Events {
        public string eventName;
        public string missionUI;
        public Triggers eventTriggers;       
        public Triggered results;
    }

    

    [System.Serializable]
    public struct Triggers {
        public Vector3 triggerPosition;
        public float triggerRadius;
        public Collider toCalculate;
        public GameObject checkIfDestroyed;
    }

    [System.Serializable]
    public struct Triggered {
        public GameObject[] spawns;
        public GameObject[] toDestroy;
    }


    public Events[] gameEventFlow;
    public Text missionUI;
    public bool ableToEdit;

    int currentGameEvent;
    int prevCount;

    void Start() {
        currentGameEvent = 0;
    }

    void Update() {

        if (ableToEdit) {
            for (var i = 0; i < gameEventFlow.Length; i++) {
                if (gameEventFlow[i].eventTriggers.toCalculate != null) {
                    gameEventFlow[i].eventTriggers.triggerPosition = gameEventFlow[i].eventTriggers.toCalculate.bounds.center;
                    gameEventFlow[i].eventTriggers.triggerRadius = gameEventFlow[i].eventTriggers.toCalculate.bounds.extents.x;
                }
                for (var j = 0; j < gameEventFlow[i].results.spawns.Length; j++) {
                    if (gameEventFlow[i].results.spawns[j] != null) {
                        gameEventFlow[i].results.spawns[j].SetActive(false);
                    }
                }
            }
        }

        if (missionUI)
            missionUI.text = gameEventFlow[currentGameEvent].missionUI;

        if (currentGameEvent < gameEventFlow.Length) {
            if (gameEventFlow[currentGameEvent].eventTriggers.triggerRadius > 0 || gameEventFlow[currentGameEvent].eventTriggers.toCalculate) {
                Collider[] temp;

                temp = Physics.OverlapSphere(gameEventFlow[currentGameEvent].eventTriggers.triggerPosition, gameEventFlow[currentGameEvent].eventTriggers.triggerRadius);

                if (temp.Length != prevCount) {
                    foreach (Collider obj in temp) {
                        if (obj.tag == "Player") {
                            ActivateEvent(gameEventFlow[currentGameEvent].results);                         
                        }
                    }
                }

                prevCount = temp.Length;
            } else {
                if (!gameEventFlow[currentGameEvent].eventTriggers.checkIfDestroyed) {
                    ActivateEvent(gameEventFlow[currentGameEvent].results);

                }
            }
        }
    }

    void ActivateEvent(Triggered endResult) {
        foreach (GameObject spawn in endResult.spawns) {
            spawn.SetActive(true);
        }

        foreach (GameObject destroy in endResult.toDestroy) {
            Destroy(destroy);
        }
        currentGameEvent++;
    }
}


