//------------------------------------------------------------------------------
// <copyright file="GazeBookButton.cs" company="Juan Esteban Quinchia Duque">
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
using UnityEngine.Events;

public class GazeBookButton : MonoBehaviour
{
  [Header("Configuración del Evento")]
  [SerializeField] private UnityEvent enClickPorGaze;

  public void OnPointerClick()
  {
    if (enClickPorGaze != null)
    {
      enClickPorGaze.Invoke();
    }
  }
}
