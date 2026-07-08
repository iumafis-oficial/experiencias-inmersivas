//------------------------------------------------------------------------------
// <copyright file="CameraSS.cs" company="Juan Esteban Quinchia Duque">
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
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class CameraSS : MonoBehaviour
{
  IEnumerator Start()
  {
    yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

    if (Application.HasUserAuthorization(UserAuthorization.WebCam))
    {
      Debug.Log("Permiso de cámara concedido.");
    }
    else
    {
      Debug.LogWarning("Permiso de cámara denegado.");
    }

#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
#endif
  }
}
