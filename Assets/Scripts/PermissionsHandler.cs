//------------------------------------------------------------------------------
// <copyright file="PermissionsHandler.cs" company="Juan Esteban Quinchia Duque">
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
using UnityEngine.Android;
using System.Collections;

public class PermissionsHandler : MonoBehaviour
{
  [Header("Referencia a MediaPipe")]
  public GameObject solutionObject;

  void Awake()
  {
    if (solutionObject != null)
    {
      solutionObject.SetActive(false);
    }

    if (Permission.HasUserAuthorizedPermission(Permission.Camera))
    {
      StartCoroutine(ActivateSolution());
    }
    else
    {
      Permission.RequestUserPermission(Permission.Camera);
      StartCoroutine(WaitAndActivate());
    }
  }

  IEnumerator WaitAndActivate()
  {
    while (!Permission.HasUserAuthorizedPermission(Permission.Camera))
    {
      yield return new WaitForSeconds(0.5f);
    }

    yield return StartCoroutine(ActivateSolution());
  }

  IEnumerator ActivateSolution()
  {
    yield return new WaitForSeconds(0.2f);

    if (solutionObject != null)
    {
      solutionObject.SetActive(true);
      Debug.Log("Cámara autorizada. Solution activada.");
    }
  }
}
