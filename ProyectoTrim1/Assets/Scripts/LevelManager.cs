using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.SceneManagement;



public class LevelManager : MonoBehaviour
{
    [Header("Nombre del nivel")]
    public string levelName;

    public float tiempo = 0;

   // public int monedasRecogidas = 0;

   // public int monedasTotales;

   // public TextMeshProUGUI monedasTexto;

    public int intentos;

    public TextMeshProUGUI tiempoTexto;

    public bool completado;

    public TextMeshProUGUI intentosTexto;

    public TextMeshProUGUI nombreNivelText;

    int cantidadMonedas = 0;

    public Transform spawnPointActual;

    public Transform player;



    private void Start()
    {
        contadorTiempo();
        asignarNombreNivel();
        //monedasTotalesCuenta();
    }

    public void Update()
    {
        contadorTiempo();
    }

    public void nuevoIntento()
    {

    }

    public void contadorTiempo()
    {
        // Sumar el tiempo transcurrido
        tiempo += Time.deltaTime;

        // Calcular minutos y segundos a partir del tiempo total
        int minutos = (int)(tiempo / 60);
        int segundos = (int)(tiempo % 60);

        // Mostrar con formato 00:00
        tiempoTexto.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    public void asignarNombreNivel()
    {
        nombreNivelText.text = levelName;
    }

   /* 
     public void monedasTotalesCuenta()
    {
        monedaController[] monedasTotales = FindObjectsByType<monedaController>(FindObjectsSortMode.None);
        cantidadMonedas = monedasTotales.Length;
        monedasTexto.text = "Monedas:" + monedasRecogidas + "/" + cantidadMonedas;

    }
   */

    public void reespawnPlayer()
    {
        player.position = spawnPointActual.position;
    }

    public void setSpawnPoint(Transform antiguo)
    {
        spawnPointActual = antiguo;
    }







}
