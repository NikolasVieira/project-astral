using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_bridge : MonoBehaviour
{
    public scr_pressure_plate script_pressure_plate;
    public GameObject sprite;
    public GameObject invisibleWall;

    private void OnEnable() 
    {
        script_pressure_plate.OnPressurePlateActivated += ActivateBridge;
        script_pressure_plate.OnPressurePlateDesactivated += DesactivateBridge;
    }

    private void OnDisable() 
    {
        script_pressure_plate.OnPressurePlateActivated -= ActivateBridge;
        script_pressure_plate.OnPressurePlateDesactivated -= DesactivateBridge;
    }

    private void ActivateBridge(object sender, EventArgs e) {
        sprite.SetActive(true); 
        invisibleWall.SetActive(false);
    }

    private void DesactivateBridge(object sender, EventArgs e) {
        sprite.SetActive(false);
        invisibleWall.SetActive(true);
    }
}
