//------------------------------------------------------------------------------
// <copyright file="FPSCounter.cs" company="Juan Esteban Quinchia Duque">
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

public class FPSCounter : MonoBehaviour
{
  [Header("Referencias de Interfaz (UI)")]
  public TextMeshProUGUI fpsText;
  public TextMeshProUGUI distanciaPinchText;

  [Header("Referencias de Sistemas")]
  public HandPinchDetector handPinchDetector;

  [Header("Configuración FPS")]
  [Tooltip("Intervalo en segundos para actualizar el contador de FPS.")]
  public float updateInterval = 0.5f;

  private float _accumulatedTime = 0f;
  private int _framesCount = 0;
  private float _timeLeft;

  void Start()
  {
    if (fpsText == null)
    {
      Debug.LogError("[FPSCounter] Referencia de texto FPS no asignada.");
      enabled = false;
      return;
    }

    if (handPinchDetector == null)
    {
      handPinchDetector = FindAnyObjectByType<HandPinchDetector>();
      if (handPinchDetector == null)
      {
        Debug.LogWarning("[FPSCounter] HandPinchDetector no encontrado en la escena. Telemetría de distancia deshabilitada.");
      }
    }

    _timeLeft = updateInterval;
  }

  void Update()
  {
    _timeLeft -= Time.unscaledDeltaTime;
    _accumulatedTime += Time.unscaledDeltaTime;
    _framesCount++;

    if (_timeLeft <= 0.0)
    {
      float fps = _framesCount / _accumulatedTime;
      fpsText.text = string.Format("{0:F1} FPS", fps);

      if (fps < 30) fpsText.color = Color.red;
      else if (fps < 55) fpsText.color = Color.yellow;
      else fpsText.color = Color.green;

      _timeLeft = updateInterval;
      _accumulatedTime = 0f;
      _framesCount = 0;
    }

    ActualizarTelemetriaDistancia();
  }

  private void ActualizarTelemetriaDistancia()
  {
    if (distanciaPinchText == null) return;

    if (handPinchDetector != null && handPinchDetector.isHandVisible && handPinchDetector.primaryHand != null)
    {
      try
      {
        Vector3 thumbPos = handPinchDetector.primaryHand[4].transform.position;
        Vector3 indexPos = handPinchDetector.primaryHand[8].transform.position;

        float distanciaActual = Vector3.Distance(thumbPos, indexPos);
        distanciaPinchText.text = string.Format("Dist. Dedos: {0:F4}", distanciaActual);

        if (distanciaActual < handPinchDetector.pinchThreshold)
        {
          distanciaPinchText.color = Color.cyan;
        }
        else
        {
          distanciaPinchText.color = Color.white;
        }
      }
      catch
      {
        distanciaPinchText.text = "Dist. Dedos: Leyendo...";
        distanciaPinchText.color = Color.gray;
      }
    }
    else
    {
      distanciaPinchText.text = "Dist. Dedos: Sin Mano";
      distanciaPinchText.color = Color.gray;
    }
  }
}
