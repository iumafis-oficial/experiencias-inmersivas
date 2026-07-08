//------------------------------------------------------------------------------
// <copyright file="PuzzleManager.cs" company="Juan Esteban Quinchia Duque">
//      Developer: Juan Esteban Quinchia Duque
//      Role: Systems Engineer / Full-stack Developer
//      Contact: juanes.10qd@gmail.com
//      Project: InmersiveExperienceIUMAFIS
//      Client: IUMAFIS
//      Date: May 2026
//      All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
  [Header("Configuración del Desafío")]
  public int totalPiezas = 9;
  private int _piezasEncajadas = 0;
  public float tiempoEsperaReinicio = 15f;

  [Header("Referencias de UI")]
  public GameObject panelVictoria;
  public TextMeshProUGUI textoDatoCurioso;
  public TextMeshProUGUI textoInstrucciones;

  [Header("Configuración Visual")]
  public float velocidadFade = 3.0f;
  [Range(0f, 1f)] public float opacidadDuranteInteraccion = 0.2f;

  public static GameObject piezaAgarradaActual = null;

  private bool _victoriaAlcanzada = false;

  void Update()
  {
    if (!_victoriaAlcanzada)
    {
      ManejarOpacidadInstrucciones();
    }
  }

  void ManejarOpacidadInstrucciones()
  {
    if (textoInstrucciones == null) return;

    float targetAlpha = (piezaAgarradaActual != null) ? opacidadDuranteInteraccion : 1f;

    Color c = textoInstrucciones.color;
    c.a = Mathf.MoveTowards(c.a, targetAlpha, Time.deltaTime * velocidadFade);
    textoInstrucciones.color = c;
  }

  public void ComprobarVictoria()
  {
    _piezasEncajadas++;
    if (_piezasEncajadas >= totalPiezas)
    {
      _victoriaAlcanzada = true;
      FinalizarExperiencia();
    }
  }

  void FinalizarExperiencia()
  {
    if (textoInstrucciones != null) textoInstrucciones.gameObject.SetActive(false);

    if (panelVictoria != null)
    {
      panelVictoria.SetActive(true);

      textoDatoCurioso.text = "<b></b>\n\n¿Sabías que la IUMAFIS ha integrado laboratorios de última generación para potenciar el talento tecnológico de Antioquia en sus 40 años?";

      StartCoroutine(ReiniciarEscena());
    }
  }

  IEnumerator ReiniciarEscena()
  {
    yield return new WaitForSeconds(tiempoEsperaReinicio);
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }
}
