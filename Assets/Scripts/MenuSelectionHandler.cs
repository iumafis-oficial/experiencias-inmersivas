//------------------------------------------------------------------------------
// <copyright file="MenuSelectionHandler.cs" company="Juan Esteban Quinchia Duque">
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
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuSelectionHandler : MonoBehaviour
{
  public MenuInteractable menu;
  public HandPinchDetector detector;

  [Header("Estabilización del Gesto")]
  [Tooltip("Tiempo (seg) que debe mantenerse la pinza para confirmar la selección.")]
  public float tiempoConfirmacionGesto = 0.15f;
  public float delayTransicion = 0.6f;

  private float _timerPinch = 0f;
  private bool _yaSeleccionado = false;

  void Update()
  {
    if (detector == null || _yaSeleccionado) return;

    if (!detector.isHandVisible || detector.primaryHand == null)
    {
      _timerPinch = 0f;
      return;
    }

    bool pinchDetectadoEnFrame = detector.IsPinching(detector.primaryHand);

    if (pinchDetectadoEnFrame)
    {
      _timerPinch += Time.deltaTime;

      if (_timerPinch >= tiempoConfirmacionGesto)
      {
        _yaSeleccionado = true;
        Debug.Log("<color=green>[IUMAFIS] ¡GESTO CONFIRMADO TRAS FILTRO!</color>");
        StartCoroutine(ProcesarSeleccionEscena());
      }
    }
    else
    {
      _timerPinch = 0f;
    }
  }

  IEnumerator ProcesarSeleccionEscena()
  {
    yield return new WaitForSeconds(delayTransicion);

    Transform objetoSeleccionado = null;
    float menorDistancia = float.MaxValue;

    foreach (var t in menu.tarjetas)
    {
      float dist = Mathf.Abs(t.position.x);
      if (dist < menorDistancia)
      {
        menorDistancia = dist;
        objetoSeleccionado = t;
      }
    }

    if (objetoSeleccionado != null)
    {
      string nombreEscena = objetoSeleccionado.name.Replace("Pivot_", "").Replace("_Copy", "");

      Debug.Log($"<color=yellow>[IUMAFIS] Cargando escena: {nombreEscena}</color>");

      if (Application.CanStreamedLevelBeLoaded(nombreEscena))
      {
        SceneManager.LoadScene(nombreEscena);
      }
      else
      {
        Debug.LogError($"<color=red>[IUMAFIS] ERROR: La escena '{nombreEscena}' no está en Build Settings.</color>");
        _yaSeleccionado = false;
      }
    }
  }
}
