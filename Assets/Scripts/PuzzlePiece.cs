//------------------------------------------------------------------------------
// <copyright file="PuzzlePiece.cs" company="Juan Esteban Quinchia Duque">
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

public class PuzzlePiece : MonoBehaviour
{
  [Header("Configuración de Encaje")]
  public Transform slotCorrecto;
  [Tooltip("Distancia para el autosnap. 18f es la mitad de tu ancho de 36")]
  public float distanciaSnap = 18.0f;

  private bool _estaEncajada = false;
  private bool _siendoArrastrada = false;
  private HandPinchDetector _detector;
  private Renderer _renderer;
  private Vector3 _escalaOriginal;

  void Start()
  {
    _detector = FindAnyObjectByType<HandPinchDetector>();
    _renderer = GetComponent<Renderer>();

    _escalaOriginal = transform.localScale;

    float lado = (Random.value > 0.5f) ? 1f : -1f;
    float posX = Random.Range(50f, 65f) * lado;
    float posY = Random.Range(-18f, 18f);

    transform.position = new Vector3(posX, posY, 85f);
    transform.rotation = Quaternion.Euler(0, 0, Random.Range(-20f, 20f));
  }

  void Update()
  {
    if (_estaEncajada || _detector == null || !_detector.isHandVisible)
    {
      if (_siendoArrastrada) SoltarYVerificar();
      return;
    }

    Vector2 posPieza2D = new Vector2(transform.position.x, transform.position.y);
    Vector2 posMano2D = new Vector2(_detector.indexPosition.x, _detector.indexPosition.y);

    float distAMano = Vector2.Distance(posPieza2D, posMano2D);
    bool haciendoPinch = _detector.IsPinching(_detector.primaryHand);

    if (haciendoPinch && distAMano < 20f)
    {
      if (PuzzleManager.piezaAgarradaActual == null || PuzzleManager.piezaAgarradaActual == gameObject)
      {
        _siendoArrastrada = true;
        PuzzleManager.piezaAgarradaActual = gameObject;
      }
    }

    if (_siendoArrastrada)
    {
      if (haciendoPinch)
      {
        Vector3 targetPos = new Vector3(_detector.indexPosition.x, _detector.indexPosition.y, 85f);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 15f);

        float distAlSlot = Vector2.Distance(posPieza2D, new Vector2(slotCorrecto.position.x, slotCorrecto.position.y));
        float t = 1.0f - Mathf.Clamp01(distAlSlot / 25f);
        _renderer.material.color = Color.Lerp(Color.white, Color.cyan, t);
        transform.localScale = Vector3.Lerp(_escalaOriginal, _escalaOriginal * 1.05f, t);
      }
      else
      {
        SoltarYVerificar();
      }
    }
  }

  void SoltarYVerificar()
  {
    _siendoArrastrada = false;
    if (PuzzleManager.piezaAgarradaActual == gameObject) PuzzleManager.piezaAgarradaActual = null;

    Vector2 posPieza2D = new Vector2(transform.position.x, transform.position.y);
    Vector2 posSlot2D = new Vector2(slotCorrecto.position.x, slotCorrecto.position.y);

    if (Vector2.Distance(posPieza2D, posSlot2D) < distanciaSnap)
    {
      transform.position = new Vector3(slotCorrecto.position.x, slotCorrecto.position.y, 85f);
      transform.rotation = Quaternion.identity;
      transform.localScale = _escalaOriginal;
      _renderer.material.color = Color.white;

      _estaEncajada = true;
      FindAnyObjectByType<PuzzleManager>().ComprobarVictoria();
      Debug.Log("<color=green>[IUMAFIS] Pieza alineada perfectamente.</color>");
    }
    else
    {
      _renderer.material.color = Color.white;
      transform.localScale = _escalaOriginal;
    }
  }
}
