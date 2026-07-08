//------------------------------------------------------------------------------
// <copyright file="CryptexManager.cs" company="Juan Esteban Quinchia Duque">
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
using System.Collections;
using UnityEngine.SceneManagement;

public class CryptexManager : MonoBehaviour
{
  [Header("UI de Instrucciones")]
  public TextMeshProUGUI textoAcertijo;
  public TextMeshProUGUI textoDatoCurioso;
  public float velocidadFade = 3f;

  [Header("Referencias")]
  public HandPinchDetector detector;
  public GameObject[] anillos;
  public Transform[] sensoresLetras;
  public Transform[] puntosReferencia;
  public Transform centroReferencia;

  [Header("Configuración")]
  public float sensibilidadGiro = 150f;
  [Tooltip("0.01 a 0.05 es lo ideal si las escalas están bien (1,1,1).")]
  public float distanciaUmbral = 0.05f;
  public Vector3 ejeGiro = Vector3.forward;
  public float tiempoEsperaLogo = 15f;

  [Header("Estado")]
  public int anilloActivoIndex = 0;
  private float _posicionYAnterior;
  private bool _descifrado = false;
  private bool _mostrandoDatoCurioso = false;

  void Start()
  {
    if (textoDatoCurioso != null)
    {
      Color c = textoDatoCurioso.color;
      c.a = 0f;
      textoDatoCurioso.color = c;
      textoDatoCurioso.gameObject.SetActive(false);
    }
  }

  void Update()
  {
    if (_descifrado || anilloActivoIndex >= anillos.Length)
    {
      GestionarTransicionTextos();
      return;
    }
    ManejarInteraccion();
  }

  void ManejarInteraccion()
  {
    if (detector.isHandVisible && detector.IsPinching(detector.primaryHand))
    {
      float posicionYActual = detector.indexPosition.y;
      float deltaY = (posicionYActual - _posicionYAnterior) * sensibilidadGiro;

      if (Mathf.Abs(deltaY) > 0.01f)
      {
        anillos[anilloActivoIndex].transform.RotateAround(
            centroReferencia.position,
            ejeGiro,
            deltaY
        );

        if (VerificarSnapPorSensor())
        {
          _posicionYAnterior = posicionYActual;
          return;
        }
      }
      _posicionYAnterior = posicionYActual;
    }
    else
    {
      _posicionYAnterior = detector.indexPosition.y;
      VerificarSnapPorSensor();
    }
  }

  bool VerificarSnapPorSensor()
  {
    if (anilloActivoIndex >= sensoresLetras.Length || anilloActivoIndex >= puntosReferencia.Length)
      return false;

    Transform sensorActual = sensoresLetras[anilloActivoIndex];
    Transform puntoRefActual = puntosReferencia[anilloActivoIndex];

    float distancia = Vector3.Distance(sensorActual.position, puntoRefActual.position);

    if (distancia < distanciaUmbral)
    {
      Vector3 dirRef = (puntoRefActual.position - centroReferencia.position).normalized;
      Vector3 dirSensor = (sensorActual.position - centroReferencia.position).normalized;

      float anguloDiferencia = Vector3.SignedAngle(dirSensor, dirRef, ejeGiro);

      anillos[anilloActivoIndex].transform.RotateAround(
          centroReferencia.position,
          ejeGiro,
          anguloDiferencia
      );

      Debug.Log($"<color=lime>[IUMAFIS]</color> Anillo {anilloActivoIndex} bloqueado por proximidad.");

      anilloActivoIndex++;
      if (anilloActivoIndex >= anillos.Length) _descifrado = true;

      return true;
    }
    return false;
  }

  private void OnDrawGizmos()
  {
    if (anilloActivoIndex < sensoresLetras.Length && anilloActivoIndex < puntosReferencia.Length)
    {
      if (sensoresLetras[anilloActivoIndex] != null && puntosReferencia[anilloActivoIndex] != null)
      {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(sensoresLetras[anilloActivoIndex].position, puntosReferencia[anilloActivoIndex].position);
        Gizmos.DrawWireSphere(puntosReferencia[anilloActivoIndex].position, distanciaUmbral);
      }
    }
  }

  void OcultarTextoAcertijo()
  {
    if (textoAcertijo != null)
    {
      Color c = textoAcertijo.color;
      if (c.a > 0)
      {
        c.a = Mathf.MoveTowards(c.a, 0f, Time.deltaTime * velocidadFade);
        textoAcertijo.color = c;
      }
      else
      {
        textoAcertijo.gameObject.SetActive(false);
      }
    }
  }

  void GestionarTransicionTextos()
  {
    if (textoAcertijo != null && textoAcertijo.gameObject.activeSelf)
    {
      Color c = textoAcertijo.color;
      c.a = Mathf.MoveTowards(c.a, 0f, Time.deltaTime * velocidadFade);
      textoAcertijo.color = c;

      if (c.a <= 0.01f)
      {
        textoAcertijo.gameObject.SetActive(false);
        _mostrandoDatoCurioso = true;
      }
    }

    if (_mostrandoDatoCurioso && textoDatoCurioso != null)
    {
      if (!textoDatoCurioso.gameObject.activeSelf) textoDatoCurioso.gameObject.SetActive(true);

      Color c = textoDatoCurioso.color;
      c.a = Mathf.MoveTowards(c.a, 1f, Time.deltaTime * velocidadFade);
      textoDatoCurioso.color = c;

      StartCoroutine(ReiniciarEscena());
    }
  }

  IEnumerator ReiniciarEscena()
  {
    yield return new WaitForSeconds(tiempoEsperaLogo);
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }
}
