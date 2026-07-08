//------------------------------------------------------------------------------
// <copyright file="MenuInteractable.cs" company="Juan Esteban Quinchia Duque">
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
using System.Collections.Generic;

public class MenuInteractable : MonoBehaviour
{
  [Header("Configuración de Movimiento")]
  public float suavizado = 5f;
  public float espacioEntreTarjetas = 60f;

  [Header("Configuración de Impulso (Swipe)")]
  [Tooltip("Velocidad mínima del movimiento de la mano para registrar un cambio de tarjeta.")]
  public float umbralVelocidadSwipe = 0.8f;

  [Tooltip("Tiempo de espera mínimo (segundos) entre deslizamientos para evitar desplazamientos múltiples.")]
  public float cooldownSwipe = 1.0f;

  [Header("Ajuste Magnético (Snap)")]
  public float velocidadSnap = 10f;
  private bool _isSnapping = true;

  [Header("Escala Visual")]
  public float escalaBase = 20f;
  public float multiplicadorResaltado = 1.5f;

  [Header("Referencias de Jerarquía")]
  public List<Transform> tarjetas;

  private float _posicionObjetivoX;
  private float _ultimaManoX;
  private bool _manoDetectadaPreviamente = false;
  private HandPinchDetector _handPinchDetector;
  private Transform _myTransform;
  private bool _necesitaActualizarEscala = true;
  private float _timerCooldownSwipe = 0f;
  private bool _swipeLiberado = true;

  void Start()
  {
    _myTransform = transform;
    _handPinchDetector = FindAnyObjectByType<HandPinchDetector>();

    if (_handPinchDetector == null)
      Debug.LogError("[MenuInteractable] Error: HandPinchDetector no encontrado.");

    for (int i = 0; i < tarjetas.Count; i++)
    {
      tarjetas[i].localPosition = new Vector3((i - 2) * espacioEntreTarjetas, 0, 0);
    }

    _posicionObjetivoX = _myTransform.localPosition.x;
  }

  void Update()
  {
    bool handVisible = _handPinchDetector != null && _handPinchDetector.isHandVisible;

    bool isPinching = false;
    if (handVisible && _handPinchDetector.primaryHand != null)
    {
      isPinching = _handPinchDetector.IsPinching(_handPinchDetector.primaryHand);
    }

    if (_timerCooldownSwipe > 0)
    {
      _timerCooldownSwipe -= Time.deltaTime;
    }

    if (handVisible && !isPinching)
    {
      if (!_manoDetectadaPreviamente)
      {
        _ultimaManoX = _handPinchDetector.indexPosition.x;
        _manoDetectadaPreviamente = true;
      }

      ProcesarNavegacionPorImpulso(_handPinchDetector.indexPosition.x);
    }
    else
    {
      if (_manoDetectadaPreviamente)
      {
        _manoDetectadaPreviamente = false;
      }

      _swipeLiberado = true;
      AplicarSnapAlCentro();
    }

    EjecutarInterpolacion();
    GestionarEfectoInfinito();

    if (_necesitaActualizarEscala)
    {
      AplicarEfectoEscala();
      if (Mathf.Abs(_myTransform.localPosition.x - _posicionObjetivoX) < 0.01f)
      {
        _necesitaActualizarEscala = false;
      }
    }
  }

  void ProcesarNavegacionPorImpulso(float xActual)
  {
    float deltaX = xActual - _ultimaManoX;
    _ultimaManoX = xActual;

    float velocidadManoX = deltaX / Time.deltaTime;

    if (Mathf.Abs(velocidadManoX) < (umbralVelocidadSwipe * 0.4f))
    {
      _swipeLiberado = true;
    }

    if (_timerCooldownSwipe > 0) return;

    if (_swipeLiberado && Mathf.Abs(velocidadManoX) > umbralVelocidadSwipe)
    {
      if (velocidadManoX > 0)
      {
        _posicionObjetivoX += espacioEntreTarjetas;
      }
      else
      {
        _posicionObjetivoX -= espacioEntreTarjetas;
      }

      _timerCooldownSwipe = cooldownSwipe;
      _swipeLiberado = false;

      _isSnapping = true;
      _necesitaActualizarEscala = true;
    }
  }

  void AplicarSnapAlCentro()
  {
    float indiceCercano = Mathf.Round(_posicionObjetivoX / espacioEntreTarjetas);
    float nuevoObjetivo = indiceCercano * espacioEntreTarjetas;

    if (Mathf.Abs(_posicionObjetivoX - nuevoObjetivo) > 0.01f)
    {
      _posicionObjetivoX = nuevoObjetivo;
      _necesitaActualizarEscala = true;
    }
  }

  void EjecutarInterpolacion()
  {
    float suavizadoFinal = _isSnapping ? velocidadSnap : suavizado;
    Vector3 posActual = _myTransform.localPosition;

    float nuevaX = Mathf.Lerp(posActual.x, _posicionObjetivoX, Time.deltaTime * suavizadoFinal);

    if (Mathf.Abs(posActual.x - nuevaX) > 0.0001f)
    {
      posActual.x = nuevaX;
      _myTransform.localPosition = posActual;
      _necesitaActualizarEscala = true;
    }
  }

  void GestionarEfectoInfinito()
  {
    float anchoTotalCiclo = 5 * espacioEntreTarjetas;

    if (_myTransform.localPosition.x < -anchoTotalCiclo)
    {
      _posicionObjetivoX += anchoTotalCiclo;
      _myTransform.localPosition += new Vector3(anchoTotalCiclo, 0, 0);
    }
    else if (_myTransform.localPosition.x > 0)
    {
      _posicionObjetivoX -= anchoTotalCiclo;
      _myTransform.localPosition -= new Vector3(anchoTotalCiclo, 0, 0);
    }
  }

  void AplicarEfectoEscala()
  {
    for (int i = 0; i < tarjetas.Count; i++)
    {
      Transform t = tarjetas[i];
      float distanciaAlCentro = Mathf.Abs(t.position.x);

      float factorProximidad = Mathf.Clamp(multiplicadorResaltado - (distanciaAlCentro * 0.04f), 1f, multiplicadorResaltado);
      float escalaFinal = escalaBase * factorProximidad;

      Vector3 targetScale = new Vector3(escalaFinal, escalaFinal, escalaFinal);
      if (Vector3.SqrMagnitude(t.localScale - targetScale) > 0.001f)
      {
        t.localScale = Vector3.Lerp(t.localScale, targetScale, Time.deltaTime * 10);
      }
    }
  }
}
