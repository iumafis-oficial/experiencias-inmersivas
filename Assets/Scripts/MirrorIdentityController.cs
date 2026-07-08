//------------------------------------------------------------------------------
// <copyright file="MirrorIdentityController.cs" company="Juan Esteban Quinchia Duque">
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
using UnityEngine.UI;
using TMPro;
using Mediapipe.Unity;
using System.Collections;

public class MirrorIdentityController : MonoBehaviour
{
  [Header("Referencias de Experiencia")]
  public GameObject toga3D;
  public RawImage guiaSilueta;
  public TextMeshProUGUI textoInstrucciones;

  [Header("UI de Fotografía")]
  public TextMeshProUGUI textoContador;
  public RawImage panelFlashRaw;
  public RawImage pantallaExhibicionFoto;
  public GameObject marcoInstitucional;

  [Header("Configuración MediaPipe")]
  public MultiHandLandmarkListAnnotation multiHandAnnotation;

  [Header("Ajustes")]
  public float radioActivacion = 0.5f;
  public Transform puntoReferenciaToga;
  public float velocidadFade = 5.0f;
  public float tiempoExhibicionFoto = 15f;

  private bool _usuarioCerca;
  private bool _contando = false;
  private bool _fotoMostrada = false;
  private Coroutine _rutinaActual;

  void Start()
  {
    if (toga3D != null) toga3D.SetActive(false);
    if (pantallaExhibicionFoto != null) pantallaExhibicionFoto.gameObject.SetActive(false);
    if (marcoInstitucional != null) marcoInstitucional.SetActive(false);
    if (panelFlashRaw != null) { panelFlashRaw.gameObject.SetActive(false); }
    if (textoContador != null) { textoContador.text = ""; textoContador.alpha = 0f; }
  }

  void Update()
  {
    ValidarProximidadMano();
    ActualizarVisualesBase();

    if (_usuarioCerca && !_contando && !_fotoMostrada)
    {
      _rutinaActual = StartCoroutine(RutinaCapturaCongelada());
    }
  }

  private void ValidarProximidadMano()
  {
    if (multiHandAnnotation == null) return;
    var activeHands = multiHandAnnotation.GetComponentsInChildren<HandLandmarkListAnnotation>();
    if (activeHands != null && activeHands.Length > 0)
    {
      var manoActual = activeHands[0];
      if (manoActual.gameObject.activeInHierarchy)
      {
        float distancia = Vector3.Distance(manoActual[9].transform.position, puntoReferenciaToga.position);
        _usuarioCerca = (distancia < radioActivacion);
      }
      else { _usuarioCerca = false; }
    }
    else { _usuarioCerca = false; }
  }

  private void ActualizarVisualesBase()
  {
    if (_fotoMostrada) return;

    if (toga3D != null) toga3D.SetActive(_usuarioCerca);

    if (guiaSilueta != null)
    {
      float targetAlpha = _usuarioCerca ? 0f : 0.3f;
      Color c = guiaSilueta.color;
      c.a = Mathf.MoveTowards(c.a, targetAlpha, Time.deltaTime * velocidadFade);
      guiaSilueta.color = c;

      if (textoInstrucciones != null)
      {
        Color tc = textoInstrucciones.color;
        tc.a = (c.a > 0.05f) ? 1f : 0f;
        textoInstrucciones.color = tc;
      }
    }
  }

  IEnumerator RutinaCapturaCongelada()
  {
    _contando = true;

    textoContador.alpha = 1f;
    float tiempo = 10f;
    while (tiempo > 0)
    {
      if (!_usuarioCerca) { ResetearTodo(); yield break; }
      textoContador.text = Mathf.Ceil(tiempo).ToString();
      yield return new WaitForSeconds(1f);
      tiempo--;
    }

    textoContador.text = "";

    LineRenderer[] conexionesHuesos = null;
    MeshRenderer[] puntosArticulaciones = null;

    if (multiHandAnnotation != null)
    {
      conexionesHuesos = multiHandAnnotation.GetComponentsInChildren<LineRenderer>(true);
      foreach (var lr in conexionesHuesos) lr.enabled = false;

      puntosArticulaciones = multiHandAnnotation.GetComponentsInChildren<MeshRenderer>(true);
      foreach (var mr in puntosArticulaciones) mr.enabled = false;
    }

    yield return new WaitForEndOfFrame();

    if (pantallaExhibicionFoto != null)
    {
      int sw = UnityEngine.Screen.width;
      int sh = UnityEngine.Screen.height;

      if (pantallaExhibicionFoto.texture != null)
      {
        Texture2D oldTexture = pantallaExhibicionFoto.texture as Texture2D;
        pantallaExhibicionFoto.texture = null;
        Destroy(oldTexture);
      }

      Texture2D fotoCapturada = new Texture2D(sw, sh, TextureFormat.RGB24, false);
      fotoCapturada.ReadPixels(new Rect(0, 0, sw, sh), 0, 0);
      fotoCapturada.Apply();

      pantallaExhibicionFoto.texture = fotoCapturada;
      pantallaExhibicionFoto.color = Color.white;
    }

    if (conexionesHuesos != null)
    {
      foreach (var lr in conexionesHuesos) lr.enabled = true;
    }
    if (puntosArticulaciones != null)
    {
      foreach (var mr in puntosArticulaciones) mr.enabled = true;
    }

    if (panelFlashRaw != null)
    {
      panelFlashRaw.gameObject.SetActive(true);
      panelFlashRaw.color = Color.white;

      if (pantallaExhibicionFoto != null) pantallaExhibicionFoto.gameObject.SetActive(true);
      if (marcoInstitucional != null) marcoInstitucional.SetActive(true);

      _fotoMostrada = true;

      yield return new WaitForSeconds(0.5f);
      float t = 1f;
      while (t > 0)
      {
        t -= Time.deltaTime * 2f;
        panelFlashRaw.color = new Color(1, 1, 1, t);
        yield return null;
      }
      panelFlashRaw.gameObject.SetActive(false);
    }

    yield return new WaitForSeconds(tiempoExhibicionFoto);

    _fotoMostrada = false;
    if (pantallaExhibicionFoto != null) pantallaExhibicionFoto.gameObject.SetActive(false);
    ResetearTodo();
  }

  void ResetearTodo()
  {
    if (_rutinaActual != null) StopCoroutine(_rutinaActual);
    _contando = false;
    if (textoContador != null) { textoContador.alpha = 0f; textoContador.text = ""; }
    if (marcoInstitucional != null) marcoInstitucional.SetActive(false);
    if (panelFlashRaw != null) panelFlashRaw.gameObject.SetActive(false);
  }
}
