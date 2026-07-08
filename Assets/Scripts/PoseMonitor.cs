//------------------------------------------------------------------------------
// <copyright file="PoseMonitor.cs" company="Juan Esteban Quinchia Duque">
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

public class PoseMonitor : MonoBehaviour
{
  private bool _cuerpoDetectado = false;

  void Update()
  {
    if (Time.frameCount % 60 == 0)
    {
      CheckPoseStatus();
    }
  }

  void CheckPoseStatus()
  {
    if (_cuerpoDetectado)
    {
      Debug.Log("<color=green>● POSE ACTIVO</color> - El sensor está recibiendo el cuerpo.");
    }
    else
    {
      Debug.Log("<color=red>○ POSE PERDIDO</color> - No se detecta cuerpo frente a la cámara.");
    }
  }

  public void OnPoseOutput(bool detected)
  {
    _cuerpoDetectado = detected;
  }
}
