////------------------------------------------------------------------------------
//// Refactorización de Alto Rendimiento - IUMAFIS
//// Optimizaciones: PropertyBlock para materiales y control de ejecución por estado.
////------------------------------------------------------------------------------

//using UnityEngine;
//using Mediapipe.Unity;

//public class InmersiveButton : MonoBehaviour
//{
//  public HandPinchDetector detector;
//  public Color colorNormal = Color.gray;
//  public Color colorHover = Color.green;
//  public Color colorPressed = Color.red;

//  private Renderer _renderer;
//  private Vector3 _initialScale;
//  private MaterialPropertyBlock _propBlock; // Para cambiar color sin crear copias de material

//  private bool _isHandInside = false;
//  private bool _isPinching = false;
//  private HandLandmarkListAnnotation _handTouchingMe;

//  void Awake()
//  {
//    _renderer = GetComponent<Renderer>();
//    _initialScale = transform.localScale;
//    _propBlock = new MaterialPropertyBlock();

//    SetColor(colorNormal);
//  }

//  private void SetColor(Color color)
//  {
//    // Esta es la forma más eficiente de cambiar colores en Unity sin generar basura
//    _renderer.GetPropertyBlock(_propBlock);
//    _propBlock.SetColor("_Color", color);
//    _renderer.SetPropertyBlock(_propBlock);
//  }

//  void OnTriggerEnter(Collider other)
//  {
//    // Cacheamos el componente para no repetir la búsqueda
//    var hand = other.GetComponentInParent<HandLandmarkListAnnotation>();

//    if (hand != null && other.transform == hand[8].transform)
//    {
//      _isHandInside = true;
//      _handTouchingMe = hand;
//      SetColor(colorHover);
//    }
//  }

//  void OnTriggerExit(Collider other)
//  {
//    if (_handTouchingMe != null && other.transform == _handTouchingMe[8].transform)
//    {
//      _isHandInside = false;
//      _isPinching = false;
//      _handTouchingMe = null;
//      SetColor(colorNormal);
//      transform.localScale = _initialScale;
//    }
//  }

//  void Update()
//  {
//    // Si no hay mano, el script muere aquí (ahorro de CPU)
//    if (!_isHandInside || _handTouchingMe == null) return;

//    bool currentlyPinching = detector.CheckPinch(_handTouchingMe);

//    if (currentlyPinching && !_isPinching)
//    {
//      _isPinching = true;
//      ExecuteAction();
//    }
//    else if (!currentlyPinching && _isPinching)
//    {
//      _isPinching = false;
//      SetColor(colorHover);
//      transform.localScale = _initialScale;
//    }
//  }

//  void ExecuteAction()
//  {
//    SetColor(colorPressed);
//    transform.localScale = _initialScale * 0.9f;
//    Debug.Log("¡Acción ejecutada!");

//#if UNITY_ANDROID && !UNITY_EDITOR
//        Handheld.Vibrate();
//#endif
//  }
//}
