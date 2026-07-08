using UnityEngine;
using UnityEngine.XR;

public class CardboardSimulator : MonoBehaviour
{
  public bool UseCardboardSimulator = true;

  [SerializeField] private float horizontalSpeed = 2.0f; // Aumenté un poco la velocidad base
  [SerializeField] private float verticalSpeed = 2.0f;
  [SerializeField] private float rotationX = 0.0f;
  [SerializeField] private float rotationY = 0.0f;
  private Camera cam;

  void Start()
  {
    // Esto se ejecutará en Editor y en la Build de PC, pero no afectará Android
#if UNITY_EDITOR || UNITY_STANDALONE
    cam = GetComponent<Camera>();
    if (cam == null) cam = Camera.main;

    // Si por alguna razón hay un casco VR real conectado en PC, desactivamos el simulador
    if (XRSettings.isDeviceActive && !Application.isEditor)
    {
      UseCardboardSimulator = false;
    }
#endif
  }

  void Update()
  {
    // La directiva STANDALONE asegura que esto compile para el .exe de Windows
    // pero que sea ignorado totalmente en el .apk de Android
#if UNITY_EDITOR || UNITY_STANDALONE
    if (!UseCardboardSimulator) return;

    // En la pantalla táctil de 85", el toque sostenido cuenta como MouseButton(0)
    if (Input.GetMouseButton(0))
    {
      float mouseX = Input.GetAxis("Mouse X") * horizontalSpeed;
      float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;

      rotationY += mouseX;
      rotationX -= mouseY;

      // Un poco más de margen para que puedan ver bien el techo y el suelo del cine
      rotationX = Mathf.Clamp(rotationX, -80, 80);

      cam.transform.localEulerAngles = new Vector3(rotationX, rotationY, 0.0f);
    }
#endif
  }

  public void UpdatePlayerPositonSimulator()
  {
    if (cam != null)
    {
      rotationX = 0;
      rotationY = cam.transform.localEulerAngles.y;
    }
  }
}
