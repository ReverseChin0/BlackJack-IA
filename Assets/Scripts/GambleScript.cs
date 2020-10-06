using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GambleScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dineroApostadoTM = default, dineroJugadorTM = default, dineroCasaTM = default;
    [SerializeField] GameObject menuApuesta = default, menuGame = default;
    [SerializeField] GameObject menuFinal = default;
    [SerializeField] bool AIMoneyBased = false;
    [SerializeField] Image IAFace = default;
    [SerializeField] Sprite[] sprfaces = default;
    Queue<int> victorias;
    //victorias del jugador 1, de la IA 2, empates 0
    int dineroJugador = 1000;
    int dineroCasa = 10000, dineroInicial = 10000;
    int dineroApostado = 0;

    private void Awake() 
    {
        dineroJugadorTM.text = "$"+ dineroJugador.ToString();
        dineroApostadoTM.text = "$"+ dineroApostado.ToString();
        dineroCasaTM.text = "$" + dineroCasa.ToString();
        victorias = new Queue<int>();
        for (int i = 0; i < 5; i++)
        {
            victorias.Enqueue(0);
        }
    }
    
    public void btn_AddApuesta(int cant)
    {
        if(dineroJugador-cant >= 0){
            dineroApostado+=cant;
            dineroJugador-=cant;
            dineroJugadorTM.text = "$"+ dineroJugador.ToString();
            dineroApostadoTM.text = "$"+ dineroApostado.ToString();
            dineroCasaTM.text = "$" + dineroCasa.ToString();
        }
    }

    public void btn_subtractApuesta(int cant){
        if(dineroApostado-cant >= 0){
            dineroApostado-=cant;
            dineroJugador+=cant;
            dineroJugadorTM.text ="$"+ dineroJugador.ToString();
            dineroApostadoTM.text ="$"+ dineroApostado.ToString();
            dineroCasaTM.text = "$" + dineroCasa.ToString();
        }
    }

    public void btn_DoblarApuesta(){
        if(dineroJugador-dineroApostado>=0){
            dineroJugador-=dineroApostado;
            dineroApostado=dineroApostado*2;
            dineroJugadorTM.text ="$"+ dineroJugador.ToString();
            dineroApostadoTM.text ="$"+ dineroApostado.ToString();
            dineroCasaTM.text = "$" + dineroCasa.ToString();
            FindObjectOfType<GameManager>().Btn_pararTurno();
        }
    }

    public void GanarApuesta()
    {
        dineroJugador += (dineroApostado * 2);
        dineroCasa -= dineroApostado;
        dineroApostado = 0;
        dineroJugadorTM.text ="$"+ dineroJugador.ToString();
        dineroApostadoTM.text ="$"+ dineroApostado.ToString();
        dineroCasaTM.text = "$" + dineroCasa.ToString();
        victorias.Dequeue();
        victorias.Enqueue(1);
        if (dineroCasa <= 0)
            menuFinal.SetActive(true);
    }

    public void PerderApuesta()
    {
        dineroCasa += dineroApostado;
        dineroApostado = 0;
        dineroJugadorTM.text ="$"+ dineroJugador.ToString();
        dineroApostadoTM.text ="$"+ dineroApostado.ToString();
        dineroCasaTM.text = "$" + dineroCasa.ToString();
        victorias.Dequeue();
        victorias.Enqueue(2);
        if (dineroJugador <= 0)
            menuFinal.SetActive(true);

    }

    public void Empate()
    {
        dineroJugador += dineroApostado;
        dineroApostado = 0;
        dineroJugadorTM.text ="$"+ dineroJugador.ToString();
        dineroApostadoTM.text ="$"+ dineroApostado.ToString();
        dineroCasaTM.text = "$" + dineroCasa.ToString();
        victorias.Dequeue();
        victorias.Enqueue(0);
    }

    public void toggleApuestaJuego(bool apuesta)
    {
        menuApuesta.SetActive(apuesta);
        menuGame.SetActive(!apuesta);
    }

    internal int getStatusIA()
    {
        if(AIMoneyBased) //checar dinero
        {
                //true debes hacer trampa, false no
            if(dineroCasa < dineroInicial * 0.95f){
                IAFace.sprite = sprfaces[2];
                //haz trampa 
                return 1;
            }
            if(dineroCasa > dineroInicial * 1.05f){
                IAFace.sprite = sprfaces[1];
                //confiate
                return 2;
            }
           
        }
        else //checar Queue
        {
            int victoriasjugador = 0;
            int victoriasIA = 0;
            //print(victorias.Count);
            
            foreach (var n in victorias)
            {
                if(n==1)
                    victoriasjugador++;
                else if(n==2)
                    victoriasIA++;
            }
            if(victoriasjugador >2 && victoriasjugador > victoriasIA)
            {
                IAFace.sprite = sprfaces[2];
                //haz trampa
                return 1;
            }
            else if(victoriasIA >2 && victoriasIA > victoriasjugador)
            {
                IAFace.sprite = sprfaces[1];
                //confiate
                return 2;
            }
        }
        IAFace.sprite = sprfaces[0];
        //juega normal
        return 0;
    }
}
