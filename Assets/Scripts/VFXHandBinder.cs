//------------------------------------------------------------------------------
// <copyright file="VFXHandBinder.cs" company="Juan Esteban Quinchia Duque">
//      Developer: Juan Esteban Quinchia Duque
//      Role: Systems Engineer / Full-stack Developer
//      Contact: juanes.10qd@gmail.com
//      Project: InmersiveExperienceIUMAFIS - Exp 1
//      Client: IUMAFIS
//      Date: May 2026
//      All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class VFXHandBinder : MonoBehaviour
{
  [Header("Referencias")]
  public HandPinchDetector detector;
  public VisualEffect vfxEffect;
  public GameObject mensajeAniversario;

  [Header("Configuración de Propiedades VFX")]
  public string nombrePosicionMano = "HandPosition";
  public string nombreTransicionLogo = "TransitionProgress";

  [Header("Parámetros de Movimiento")]
  public float suavizado = 15f;
  public float profundidadZ = 82f;

  [Header("Evento Aniversario (40)")]
  public float umbralMovimiento = 500f;
  public float velocidadTransicion = 0.5f;
  public float tiempoEsperaLogo = 15f;

  private float _movimientoAcumulado = 0f;
  private Vector3 _ultimaPosicionMano;
  private float _valorTransicionActual = 0f;
  private bool _eventoIniciado = false;

  [Header("Colisión Física")]
  public bool usarColision = true;
  public Transform colisionadorDedo;

  private Vector3 _posicionSuavizada;
  private bool _vfxActivado = false;
  private readonly Vector3 _posicionFueraDeEscena = new Vector3(999, 999, 999);

  void Start()
  {
    _posicionSuavizada = _posicionFueraDeEscena;
    _ultimaPosicionMano = _posicionFueraDeEscena;

    if (vfxEffect != null)
    {
      vfxEffect.SetVector3(nombrePosicionMano, _posicionFueraDeEscena);
      vfxEffect.SetFloat(nombreTransicionLogo, 0f);
    }

    if (mensajeAniversario != null) mensajeAniversario.SetActive(false);
  }

  void Update()
  {
    if (detector == null || vfxEffect == null) return;

    if (detector.isHandVisible && !_eventoIniciado)
    {
      Vector3 worldTargetPos = detector.indexPosition;
      worldTargetPos.z = profundidadZ;

      if (!_vfxActivado)
      {
        vfxEffect.Play();
        _vfxActivado = true;
        _posicionSuavizada = worldTargetPos;
        _ultimaPosicionMano = worldTargetPos;
      }

      _posicionSuavizada = Vector3.Lerp(_posicionSuavizada, worldTargetPos, Time.deltaTime * suavizado);
      Vector3 localPos = vfxEffect.transform.InverseTransformPoint(_posicionSuavizada);
      vfxEffect.SetVector3(nombrePosicionMano, localPos);

      if (usarColision && colisionadorDedo != null)
      {
        if (!colisionadorDedo.gameObject.activeInHierarchy)
          colisionadorDedo.gameObject.SetActive(true);
        colisionadorDedo.position = _posicionSuavizada;
      }
    }
    else
    {
      if (_vfxActivado || _eventoIniciado)
      {
        vfxEffect.SetVector3(nombrePosicionMano, _posicionFueraDeEscena);
        if (colisionadorDedo != null) colisionadorDedo.gameObject.SetActive(false);

        if (!detector.isHandVisible && !_eventoIniciado) _vfxActivado = false;
      }
    }

    if (!_eventoIniciado && _valorTransicionActual < 1.0f)
    {
      float distanciaFrame = Vector3.Distance(_posicionSuavizada, _ultimaPosicionMano);
      if (distanciaFrame > 0.005f) _movimientoAcumulado += distanciaFrame;
      _ultimaPosicionMano = _posicionSuavizada;

      if (_movimientoAcumulado >= umbralMovimiento)
      {
        _valorTransicionActual = Mathf.MoveTowards(_valorTransicionActual, 1f, Time.deltaTime * velocidadTransicion);
        vfxEffect.SetFloat(nombreTransicionLogo, _valorTransicionActual);

        if (_valorTransicionActual > 0f)
        {
          EjecutarFinalizacion();
        }
      }
    }
  }

  void EjecutarFinalizacion()
  {
    _eventoIniciado = true;

    vfxEffect.SetVector3(nombrePosicionMano, _posicionFueraDeEscena);

    if (mensajeAniversario != null)
    {
      mensajeAniversario.SetActive(true);
    }

    StartCoroutine(ReiniciarEscena());
  }

  IEnumerator ReiniciarEscena()
  {
    yield return new WaitForSeconds(tiempoEsperaLogo);
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }
}
