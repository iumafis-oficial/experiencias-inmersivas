//------------------------------------------------------------------------------
// <copyright file="TextAniversarioAnim.cs" company="Juan Esteban Quinchia Duque">
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
using System.Collections;

public class TextAniversarioAnim : MonoBehaviour
{
  [Header("Configuración de Entrada")]
  public float tiempoEscalado = 0.8f;

  [Header("Calibración de Escala Final")]
  [Tooltip("La escala que quieres que tenga al final (ej: 0.7 o 1)")]
  public Vector3 escalaFinalDeseada = new Vector3(1f, 1f, 1f);

  [Header("Configuración de Flotación")]
  public float amplitud = 5f;
  public float frecuencia = 2f;

  private Vector3 _posicionInicialEditor;
  private bool _animandoEntrada = false;

  void Awake()
  {
    _posicionInicialEditor = transform.localPosition;
  }

  void OnEnable()
  {
    transform.localPosition = _posicionInicialEditor;
    transform.localScale = Vector3.zero;

    StartCoroutine(AnimarEntrada());
  }

  IEnumerator AnimarEntrada()
  {
    _animandoEntrada = true;
    float tiempo = 0;

    while (tiempo < tiempoEscalado)
    {
      tiempo += Time.deltaTime;
      float t = tiempo / tiempoEscalado;
      float smoothT = Mathf.SmoothStep(0, 1, t);

      transform.localScale = Vector3.Lerp(Vector3.zero, escalaFinalDeseada, smoothT);

      yield return null;
    }

    transform.localScale = escalaFinalDeseada;
    _animandoEntrada = false;
  }

  void Update()
  {
    if (!_animandoEntrada)
    {
      float nuevaY = _posicionInicialEditor.y + Mathf.Sin(Time.time * frecuencia) * amplitud;

      transform.localPosition = new Vector3(_posicionInicialEditor.x, nuevaY, _posicionInicialEditor.z);
    }
  }
}
