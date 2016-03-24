﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BlockBarricade : Entity {

    public float blockBarrierHealth = 5;
    float myHealth;
    protected override void Start() {
        base.Start();
        myFirstName = "Block_Barricade";
        entityLL = new LinkedList<MyTargets>();
        health = blockBarrierHealth;
        myHealth = health;
    }

    // Update is called once per frame
    void Update () {
        healthSlider.GetComponent<Image>().fillAmount = health / myHealth;
    }
}
