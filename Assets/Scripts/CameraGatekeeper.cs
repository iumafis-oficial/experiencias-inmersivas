//------------------------------------------------------------------------------
// <copyright file="CameraGatekeeper.cs" company="Juan Esteban Quinchia Duque">
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

public class CameraGatekeeper : MonoBehaviour
{
  [Header("Configuración de MediaPipe")]
  [Tooltip("Referencia al GameObject raíz que contiene el pipeline de MediaPipe.")]
  public GameObject objetoMediaPipe;

  [Header("Ajustes de Tiempo")]
  [SerializeField] private float retardoPostPermiso = 0.5f;

  IEnumerator Start()
  {
    Debug.Log("[Gatekeeper] Iniciando solicitud de permisos de cámara...");

    yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

    if (Application.HasUserAuthorization(UserAuthorization.WebCam))
    {
      Debug.Log("[Gatekeeper] Permiso otorgado por el sistema.");

      WebCamDevice[] devices = WebCamTexture.devices;
      Debug.Log($"[Gatekeeper] Cámaras detectadas por Unity: {devices.Length}");

      foreach (var device in devices)
      {
        Debug.Log($"[Gatekeeper] Dispositivo encontrado: {device.name}");
      }

      yield return new WaitForSeconds(retardoPostPermiso);

      if (objetoMediaPipe != null)
      {
        Debug.Log("[Gatekeeper] Inicializando MediaPipe...");
        objetoMediaPipe.SetActive(true);
      }
      else
      {
        Debug.LogError("[Gatekeeper] Error: No se asignó el objeto de MediaPipe.");
      }
    }
    else
    {
      Debug.LogError("[Gatekeeper] Permiso denegado. MediaPipe no se iniciará.");
    }
  }
}
