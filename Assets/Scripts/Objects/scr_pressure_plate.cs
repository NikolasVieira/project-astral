using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_pressure_plate : MonoBehaviour
{
    public delegate void PressurePlateEventHandler(object sender, EventArgs e);
    public event PressurePlateEventHandler OnPressurePlateActivated;
    public event PressurePlateEventHandler OnPressurePlateDesactivated;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("PushableBox")) {
            // Dispara o evento quando a caixa repousar sobre a placa
            OnPressurePlateActivated?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("PushableBox")) {
            // Dispara o evento quando a caixa sair de cima da placa
            OnPressurePlateDesactivated?.Invoke(this, EventArgs.Empty);
        }
    }
}
