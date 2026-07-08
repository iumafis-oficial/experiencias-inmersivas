//------------------------------------------------------------------------------
// <copyright file="ItemLevitation.cs" company="Juan Esteban Quinchia Duque">
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

public class ItemLevitation : MonoBehaviour
{
  [Header("Configuración de Levitación")]
  [Tooltip("Qué tan alto y bajo subirá el objeto.")]
  public float amplitud = 0.5f;
  [Tooltip("Velocidad del movimiento vertical.")]
  public float velocidadLevitacion = 2f;

  [Header("Configuración de Rotación (Opcional)")]
  [Tooltip("Velocidad de rotación constante sobre el eje Y.")]
  public float velocidadRotacion = 20f;

  private Vector3 _posicionInicial;

  void Start()
  {
    _posicionInicial = transform.localPosition;
  }

  void Update()
  {
    float offsetYAhora = Mathf.Sin(Time.time * velocidadLevitacion) * amplitud;

    transform.localPosition = new Vector3(
        transform.localPosition.x,
        _posicionInicial.y + offsetYAhora,
        transform.localPosition.z
    );
  }
}
