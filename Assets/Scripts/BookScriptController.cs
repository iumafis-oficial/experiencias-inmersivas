//------------------------------------------------------------------------------
// <copyright file="BookScriptController.cs" company="Juan Esteban Quinchia Duque">
//      Developer: Juan Esteban Quinchia Duque
//      Role: Systems Engineer / Full-stack Developer
//      Contact: juanes.10qd@gmail.com
//      Project: InmersiveExperienceIUMAFIS
//      Client: IUMAFIS
//      Date: May 2026
//      All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class BookScriptController : MonoBehaviour
{
  [Header("Jerarquía de Páginas (Huesos)")]
  [SerializeField] private Transform[] paginas;

  [Header("Configuración del Ángulo de Giro")]
  [SerializeField] private Vector3 rotacionCerrada = new Vector3(0, -320, 0);
  [SerializeField] private Vector3 rotacionAbierta = new Vector3(0, -220, 0);

  [Header("Tiempos de Animación")]
  [SerializeField] private float duracionGiro = 1.2f;

  [Header("Componentes de Video")]
  [SerializeField] private VideoPlayer videoPlayer;
  [SerializeField] private VideoClip videoIntro;
  [SerializeField] private VideoClip[] videosHitos;

  [Header("Zonas de Interacción (Hotspots / Empty Objects)")]
  [SerializeField] private GameObject hotspotAvanzar;
  [SerializeField] private GameObject hotspotRetroceder;

  [Header("Textos del Canvas (TextMeshPro)")]
  [SerializeField] private TextMeshProUGUI textoAvanzar;
  [SerializeField] private TextMeshProUGUI textoRetroceder;

  private string[] titulosDecadas = { "Años 80", "Años 90", "Años 10", "Años 20" };
  private int paginaActualIndex = 0;
  private bool estaGirando = false;
  private Coroutine secuenciaVideoCoroutine;

  private void Start()
  {
    for (int i = 0; i < paginas.Length; i++)
    {
      if (paginas[i] != null)
      {
        paginas[i].localRotation = Quaternion.Euler(rotacionCerrada);
        if (i > 0) paginas[i].gameObject.SetActive(false);
      }
    }

    ReproducirVideoDePagina(0);
    ActualizarBotonesEInterfaz();
  }

  public void AvanzarPagina()
  {
    if (estaGirando || paginaActualIndex >= paginas.Length - 1) return;

    paginas[paginaActualIndex].gameObject.SetActive(true);
    StartCoroutine(GirarHueso(paginas[paginaActualIndex], rotacionCerrada, rotacionAbierta, true));
  }

  public void RetrocederPagina()
  {
    if (estaGirando || paginaActualIndex <= 0) return;

    paginaActualIndex--;
    paginas[paginaActualIndex].gameObject.SetActive(true);
    StartCoroutine(GirarHueso(paginas[paginaActualIndex], rotacionAbierta, rotacionCerrada, false));
  }

  private IEnumerator GirarHueso(Transform hueso, Vector3 anguloInicio, Vector3 anguloFin, bool avanzando)
  {
    estaGirando = true;

    hotspotAvanzar.SetActive(false);
    hotspotRetroceder.SetActive(false);

    float tiempoTranscurrido = 0f;
    Quaternion rotInicial = Quaternion.Euler(anguloInicio);
    Quaternion rotFinal = Quaternion.Euler(anguloFin);

    while (tiempoTranscurrido < duracionGiro)
    {
      tiempoTranscurrido += Time.deltaTime;
      float porcentaje = tiempoTranscurrido / duracionGiro;
      float curvaSuave = Mathf.SmoothStep(0f, 1f, porcentaje);

      hueso.localRotation = Quaternion.Slerp(rotInicial, rotFinal, curvaSuave);
      yield return null;
    }

    hueso.localRotation = rotFinal;

    if (avanzando)
    {
      paginaActualIndex++;
    }

    ReproducirVideoDePagina(paginaActualIndex);
    ActualizarBotonesEInterfaz();

    estaGirando = false;
  }

  private void ActualizarBotonesEInterfaz()
  {
    if (paginaActualIndex == 0)
    {
      hotspotRetroceder.SetActive(false);
      if (textoRetroceder != null) textoRetroceder.text = "";
    }
    else
    {
      hotspotRetroceder.SetActive(true);
      if (textoRetroceder != null)
        textoRetroceder.text = titulosDecadas[paginaActualIndex - 1];
    }

    if (paginaActualIndex >= paginas.Length - 1)
    {
      hotspotAvanzar.SetActive(false);
      if (textoAvanzar != null) textoAvanzar.text = "";
    }
    else
    {
      hotspotAvanzar.SetActive(true);
      if (textoAvanzar != null)
        textoAvanzar.text = titulosDecadas[paginaActualIndex + 1];
    }
  }

  private void ReproducirVideoDePagina(int index)
  {
    if (index < 0 || index >= videosHitos.Length) return;

    if (secuenciaVideoCoroutine != null)
    {
      StopCoroutine(secuenciaVideoCoroutine);
    }

    secuenciaVideoCoroutine = StartCoroutine(SecuenciaIntroYVideo(index));
  }

  private IEnumerator SecuenciaIntroYVideo(int videoIndex)
  {
    videoPlayer.Stop();
    videoPlayer.clip = videoIntro;
    videoPlayer.Play();

    float tiempoEspera = videoIntro != null ? (float)videoIntro.length : 11f;
    yield return new WaitForSeconds(tiempoEspera);

    if (videosHitos[videoIndex] != null)
    {
      videoPlayer.clip = videosHitos[videoIndex];
      videoPlayer.Play();
    }
  }
}
