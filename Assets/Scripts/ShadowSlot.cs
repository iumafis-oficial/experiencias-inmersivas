//------------------------------------------------------------------------------
// <copyright file="ShadowSlot.cs" company="Juan Esteban Quinchia Duque">
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

public class ShadowSlot : MonoBehaviour
{
  [Header("Configuración Visual")]
  [Range(0, 1)] public float transparenciaSombra = 0.4f;
  public Color colorSombra = Color.black;

  [Header("Configuración del Borde")]
  public bool mostrarBorde = true;
  public Color colorBorde = Color.cyan;
  public float grosorBorde = 1.05f;

  private Renderer _renderer;
  private MaterialPropertyBlock _propBlock;

  void Awake()
  {
    Inicializar();
    if (mostrarBorde) CrearBorde();
    AplicarEstilo();
  }

  void Inicializar()
  {
    if (_renderer == null) _renderer = GetComponent<Renderer>();
    if (_propBlock == null) _propBlock = new MaterialPropertyBlock();
  }

  void CrearBorde()
  {
    if (transform.Find("Borde_" + name)) return;

    GameObject bordeGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
    bordeGO.name = "Borde_" + name;

    bordeGO.transform.SetParent(this.transform);
    bordeGO.transform.localPosition = new Vector3(0, 0, 0.1f);
    bordeGO.transform.localRotation = Quaternion.identity;
    bordeGO.transform.localScale = new Vector3(grosorBorde, grosorBorde, 1f);

    Renderer rendBorde = bordeGO.GetComponent<Renderer>();
    rendBorde.material = new Material(Shader.Find("Unlit/Color"));
    rendBorde.material.color = colorBorde;

    if (bordeGO.GetComponent<MeshCollider>())
      DestroyImmediate(bordeGO.GetComponent<MeshCollider>());
  }

  void AplicarEstilo()
  {
    Inicializar();

    if (_renderer == null) return;

    Color finalColor = colorSombra;
    finalColor.a = transparenciaSombra;

    _renderer.GetPropertyBlock(_propBlock);
    _propBlock.SetColor("_Color", finalColor);
    _renderer.SetPropertyBlock(_propBlock);
  }

  void OnValidate()
  {
    Inicializar();
    AplicarEstilo();
  }
}
