using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TimeControl : MonoBehaviour
{
    public Text Reloj;
    float segundos;
    int minutos;
    int horas;
    public int velocidaddeltiempo;
    public bool pausado;

    public void Update()
    {
        if (!pausado)
        {
            segundos += Time.deltaTime * velocidaddeltiempo;

            ModificarTextoDelTiempo();

            if (segundos >= 60)
            {
                minutos += 1;
                segundos = 0;
            }
            if (minutos == 60)
            {
                horas += 1;
                minutos = 0;
            }
        }
          
    }
    private void ModificarTextoDelTiempo()
    {
        Reloj.text = AgregarUnCeroAdelanteSiEsNecesario(minutos) + ":"
                  + AgregarUnCeroAdelanteSiEsNecesario(Mathf.FloorToInt(segundos));

    }
    
    
    public string AgregarUnCeroAdelanteSiEsNecesario(int n)
    {
        //si el numero es de una sola cifra, le agrega un cero adelante

        if (n < 10) return "0" + n.ToString();
        else return n.ToString();
    }

    public void ReiniciarReloj()
    {
        segundos = 0;
        minutos = 0;
        horas = 0;

        ModificarTextoDelTiempo();
    }

    public void PausarReloj()
    {
        pausado = true;
    }
}
