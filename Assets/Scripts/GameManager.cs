using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject btnPedir=default, btnPararTurno=default, btnApostar = default, btnDoblar = default;
    [SerializeField] Transform playerSpace = default, IaSpace = default;
    [SerializeField] GambleScript gamblerScr = default;
    public GameObject cardPref = default;
    [SerializeField]Sprite[] cartasSprite = default;
    List<Carta> baraja;
    List<Carta> manoJugador, manoIA;
    public int sumaJugador = 0,sumaIA = 0;
    bool terminarIA = false;
    int minSuma = 11 , maxSuma = 21;

    [SerializeField]TextMeshProUGUI winner = default;

    void Start()
    {
        manoJugador = new List<Carta>();
        manoIA = new List<Carta>();
        baraja = new List<Carta>();
        CreateCards();
        PedirCarta(manoJugador,0);
        PedirCarta(manoJugador,0);
        PedirCarta(manoIA,1);
        PedirCarta(manoIA,3);
    }

    [ContextMenu("Reiniciar")]
    public void Restart()
    {
        StartCoroutine(RestartCoroutine());
    }

    public IEnumerator RestartCoroutine(){
        sumaJugador = sumaIA = 0;
        terminarIA = false;
        winner.text = "";
        for (int i = 0; i < playerSpace.childCount; i++)
        {
            Transform current = playerSpace.GetChild(i);
            if(current.childCount>0){
                Destroy(current.GetChild(0).gameObject);
            }
            current = IaSpace.GetChild(i);
            if(current.childCount>0){
                Destroy(current.GetChild(0).gameObject);
            }
        }
        yield return new WaitForSeconds(1.0f);
        btnApostar.SetActive(false);
        btnPedir.SetActive(true);
        btnPararTurno.SetActive(true);
        btnDoblar.SetActive(true);
        manoJugador = new List<Carta>();
        manoIA = new List<Carta>();
        baraja = new List<Carta>();
        CreateCards();
        PedirCarta(manoJugador,0);
        PedirCarta(manoJugador,0);
        PedirCarta(manoIA,1);
        PedirCarta(manoIA,3);
    }

    public void Btn_pedirCarta(){
        PedirCarta(manoJugador,2);
    }

    public void Btn_pararTurno(){
        btnPedir.SetActive(false);
        btnPararTurno.SetActive(false);
        btnDoblar.SetActive(false);
        StartCoroutine(IniciarIAs());
        StartCoroutine(BehaviourIA());
    }

    public IEnumerator BehaviourIA()
    {
        //Si la mano es menor a 21, que vuelva a acceder
        //Si ya pasó de 21, ya no puede pedir más cartas
        //(Opcional) Si la mano es menor a 21, pero es mayor a la del jugador, ya no puede pedir cartas
         yield return new WaitForSeconds(2.0f);
        //Crear una variable como probabilidad de tomar otra carta (0-100)
        //Si la mano de la IA es emnor o igual a 11, la probabilidad es de 100%
        //De lo contrario, entre más alta sea su mano, menor será la probabilidad de tomar otra
        while((sumaIA<=21 /*&& sumaIA < sumaJugador */&& !terminarIA)|| sumaJugador>21)
        {
            if(sumaIA>11){
                float probabilidaDeJugar = Remap(sumaIA,minSuma,maxSuma,0,100);
                int probabilidad = Random.Range(0,101);
                //Debug.LogWarning("La probabi es de: "+probabilidad + " y se necesita "+probabilidaDeJugar +" jugar");
                if(probabilidad<=probabilidaDeJugar){
                    terminarIA = true;
                }
                else
                    PedirCarta(manoIA,3);
            }
            else
            {
                PedirCarta(manoIA,3);
            }
           yield return new WaitForSeconds(1.0f);
        }
        ElegirGanador();
    }
    
    public void PedirCarta(List<Carta> LaLista,int IA)
    {
        if(IA == 3){
            if(checkHacerTrampa(LaLista)){
                print("hare trampa");
                return;
            }
        }    

        int cartaInd = Random.Range(0,baraja.Count);
        Carta cartaretirada = baraja[cartaInd];
        baraja.RemoveAt(cartaInd);
        int valorcarta = cartaretirada.CardIndex;

        switch(valorcarta){
            case 1: valorcarta = 11; break;
            case 11: valorcarta = 10; break;
            case 12: valorcarta = 10; break;
            case 13: valorcarta = 10; break;
            default: break;
        }

        LaLista.Add(cartaretirada);
        
        switch(IA){
            case 0: //primerTurnoJugador
                sumaJugador += valorcarta;
                spawnearCarta(cartaretirada,0);
            break;
            case 1:  //primerTurnoIA
                sumaIA += valorcarta;
                spawnearCarta(cartaretirada,1,true);
            break;
            case 2: //Jugador
                sumaJugador += valorcarta;
                spawnearCarta(cartaretirada,0);
            break;
            case 3: //IA
                sumaIA += valorcarta;
                spawnearCarta(cartaretirada,1);
            break;
            default: break;
        }
        
        if(sumaIA>21||sumaJugador>21){
            CambiarAs(LaLista, IA);
            if(sumaIA > 21 || sumaJugador > 21)
            ElegirGanador();
        }
            
    }

    private bool checkHacerTrampa(List<Carta> LaLista)
    {
        int statusIA = gamblerScr.getStatusIA();
        if(statusIA == 1)
        {
            int cardindets = 0;
            Carta cartaretirada = default;
            for(int i=0;i<baraja.Count;i++)
            {
                int nuevaSuma = sumaIA + baraja[i].CardIndex;
                if(nuevaSuma >16 && nuevaSuma<=18)
                {
                    sumaIA = nuevaSuma;
                    cartaretirada = baraja[i];
                   
                    int valorcarta = cartaretirada.CardIndex;
                    break;
                }
                else
                {
                    cardindets++;
                }
            }

            if (cardindets >= baraja.Count) 
            {
                return false;
            }
            else 
            {
                print("carta trampa es " + cardindets + "y hay " + baraja.Count);
                baraja.RemoveAt(cardindets);
                if (cartaretirada != null)
                    spawnearCarta(cartaretirada, 1, false);
                if (sumaIA > 21 || sumaJugador > 21)
                {
                    CambiarAs(LaLista, 3);
                    if (sumaIA > 21 || sumaJugador > 21)
                        ElegirGanador();
                }
                return true;
            }
            
        }
        else if (statusIA == 2)
        {//confiate
            minSuma = 11;
            maxSuma = 16;
        }
        else
        {
            //juega normal
            minSuma = 11;
            maxSuma = 21;
        }
        return false;
    }

    void spawnearCarta(Carta cartanueva , int jugador , bool volteada = false)
    {
        Transform espacioCarta = jugador == 0? playerSpace : IaSpace;
        Transform padreCarta = default;
        for (int i = espacioCarta.childCount-1; i >= 0; i--)
        {
            Transform current = espacioCarta.GetChild(i);
            if(current.childCount<=0){
                padreCarta = current;
            }
        }
        GameObject cartaSpr = Instantiate(cardPref,Vector3.zero,Quaternion.identity,padreCarta);
        cartaSpr.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        int spriteindex = cartanueva.CardIndex-1 + (cartanueva.CardSuit * 13);
        cartaSpr.GetComponentInChildren<Image>().sprite = cartasSprite[spriteindex];

        if(volteada){
            cartaSpr.transform.GetChild(0).gameObject.SetActive(false);
            cartaSpr.transform.GetChild(1).gameObject.SetActive(true);
        }
            
    }
    void  CambiarAs(List<Carta> laLista,int ia)
    {
        foreach(Carta c in laLista){
            if(c.CardIndex == 1){
                if(ia==1||ia==3){
                    if(sumaIA>21)
                    sumaIA-=11;
                    c.CardIndex=0;
                }else{
                    if(sumaJugador>21){
                        sumaJugador-=11;
                        c.CardIndex=0;
                    }
                }
            }
            
        }
    }
    public void ElegirGanador()
    {
        StartCoroutine(IniciarIAs());
        btnPedir.SetActive(false);
        btnPararTurno.SetActive(false);
        btnApostar.SetActive(true);
        //sumaJugador
        //sumaIA

        if(sumaJugador>21){
            Debug.Log("IA Gana");
            winner.text = "IA Gana";
            gamblerScr.PerderApuesta();
        }
        else if(sumaIA>21)
        {
            Debug.Log("Jugador Gana");
            winner.text = "Jugador Gana";
            gamblerScr.GanarApuesta();
        }
        else{
            if(sumaJugador>sumaIA){
                Debug.Log("Jugador Gana");
                winner.text = "Jugador Gana";
                gamblerScr.GanarApuesta();
            }
            else if(sumaIA>sumaJugador)
            {
                 Debug.Log("IA Gana");
                 winner.text = "IA Gana";
                 gamblerScr.PerderApuesta();
            }
            else{
                 Debug.Log("Empate");
                 winner.text = "Empate";
                 gamblerScr.Empate();
            }
                
        }
    }

    /*void printScr(string message, Color clrtxt)
    {
        print(message);
        string lastmssg = screenText[0].text;
        Color lastColor = screenText[0].color;
        screenText[0].text = message;
        screenText[0].color = clrtxt;
        for (int i = 1; i < screenText.Length; i++)
        {
            string tempmssg = lastmssg;
            Color tempColor = lastColor;
            lastmssg = screenText[i].text;
            lastColor = screenText[i].color;
            screenText[i].text=tempmssg;
            screenText[i].color=tempColor;
        }
        
    }*/
    public float Remap (float from, float fromMin, float fromMax, float toMin,  float toMax)
    {
        var fromAbs  =  from - fromMin;
        var fromMaxAbs = fromMax - fromMin;      
       
        var normal = fromAbs / fromMaxAbs;
 
        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;
 
        var to = toAbs + toMin;
       
        return to;
    }
    void CreateCards()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                Carta nueva = new Carta(j+1,i);
                baraja.Add(nueva);
            }
        }
    }

    public IEnumerator IniciarIAs()
    {
        yield return new WaitForSeconds(1.0f);
        IaSpace.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
        IaSpace.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
    }
}
