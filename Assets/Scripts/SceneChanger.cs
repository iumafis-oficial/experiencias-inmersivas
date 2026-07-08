//------------------------------------------------------------------------------
// <copyright file="SceneChanger.cs" company="Juan Esteban Quinchia Duque">
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
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
  public void VolverAlMenu()
  {
    SceneManager.LoadScene("Menu");
  }

  public void CambiarEscena(string nombreEscena)
  {
    SceneManager.LoadScene(nombreEscena);
  }
}
