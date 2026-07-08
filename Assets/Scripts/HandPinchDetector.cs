//------------------------------------------------------------------------------
// <copyright file="HandPinchDetector.cs" company="Juan Esteban Quinchia Duque">
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
using Mediapipe.Unity;

public class HandPinchDetector : MonoBehaviour
{
  [Header("Configuración MediaPipe")]
  public MultiHandLandmarkListAnnotation multiHandAnnotation;

  [Header("Estado de Salida (Consumido por Escenas)")]
  [HideInInspector] public bool isHandVisible;
  [HideInInspector] public Vector3 indexPosition;
  [HideInInspector] public HandLandmarkListAnnotation primaryHand;

  [Header("Configuración del Gesto")]
  [Tooltip("Ajusta este valor según la distancia física en Unity. Prueba con 0.03 a 0.06")]
  public float pinchThreshold = 0.05f;

  private List<HandLandmarkListAnnotation> _usableHandsCache = new List<HandLandmarkListAnnotation>();
  private int _lastProcessedFrame = -1;

  public bool IsPinching(HandLandmarkListAnnotation hand)
  {
    if (hand == null || !hand.gameObject.activeInHierarchy) return false;

    Vector3 thumbPos = hand[4].transform.position;
    Vector3 indexPos = hand[8].transform.position;

    return Vector3.Distance(thumbPos, indexPos) < pinchThreshold;
  }

  private void Update()
  {
    if (multiHandAnnotation == null || Time.frameCount == _lastProcessedFrame) return;

    _usableHandsCache.Clear();

    multiHandAnnotation.GetComponentsInChildren<HandLandmarkListAnnotation>(_usableHandsCache);

    if (_usableHandsCache.Count > 0)
    {
      HandLandmarkListAnnotation manoElegida = SeleccionarManoDominante(_usableHandsCache);

      if (manoElegida != null && manoElegida.gameObject.activeInHierarchy)
      {
        primaryHand = manoElegida;
        isHandVisible = true;
        indexPosition = primaryHand[8].transform.position;
      }
      else
      {
        isHandVisible = false;
        primaryHand = null;
      }
    }
    else
    {
      isHandVisible = false;
      primaryHand = null;
    }

    _lastProcessedFrame = Time.frameCount;
  }

  private HandLandmarkListAnnotation SeleccionarManoDominante(List<HandLandmarkListAnnotation> listaManos)
  {
    HandLandmarkListAnnotation mejorMano = null;
    float alturaMaximaY = float.MinValue;

    foreach (var mano in listaManos)
    {
      if (mano == null || !mano.gameObject.activeInHierarchy) continue;

      float alturaActualY = mano[0].transform.position.y;

      if (alturaActualY > alturaMaximaY)
      {
        mejorMano = mano;
        alturaMaximaY = alturaActualY;
      }
    }

    return mejorMano;
  }
}
