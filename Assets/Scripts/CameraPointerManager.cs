//------------------------------------------------------------------------------
// <copyright file="CameraPointerManager.cs" company="Juan Esteban Quinchia Duque">
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

public class CameraPointerManager : MonoBehaviour
{
  [SerializeField] GameObject pointer;
  [SerializeField] float maxDistancePointer = 4.5f;
  [Range(0, 1)]
  [SerializeField] float disPointerObject = 0.95f;
  readonly string interactableTag = "Interactable";
  float scaleSize = 0.025f;

  private const float _maxDistance = 10;
  private GameObject _gazedAtObject = null;

  private void Start()
  {
    GazeManager.Instance.OnGazeSelection += GazeSelection;
  }

  void GazeSelection()
  {
    _gazedAtObject?.SendMessage("OnPointerClick", null, SendMessageOptions.DontRequireReceiver);
  }

  public void Update()
  {
    RaycastHit hit;

    if (Physics.Raycast(transform.position, transform.forward, out hit, _maxDistance))
    {
      if (_gazedAtObject != hit.transform.gameObject)
      {
        _gazedAtObject?.SendMessage("OnPointerExit", null, SendMessageOptions.DontRequireReceiver);
        _gazedAtObject = hit.transform.gameObject;

        _gazedAtObject.SendMessage("OnPointerEnter", null, SendMessageOptions.DontRequireReceiver);
        GazeManager.Instance.StartGazeSelection();
      }

      if (hit.transform.CompareTag(interactableTag))
      {
        pointerOnGaze(hit.point);
      }
      else
      {
        PointerOutGaze();
      }
    }
    else
    {
      _gazedAtObject?.SendMessage("OnPointerExit", null, SendMessageOptions.DontRequireReceiver);
      _gazedAtObject = null;
    }

    if (Google.XR.Cardboard.Api.IsTriggerPressed)
    {
      _gazedAtObject?.SendMessage("OnPointerClick", null, SendMessageOptions.DontRequireReceiver);
    }
  }

  void pointerOnGaze(Vector3 hitpoint)
  {
    float scalefactor = scaleSize * Vector3.Distance(transform.position, hitpoint);
    pointer.transform.localScale = Vector3.one * scalefactor;
    pointer.transform.parent.position = CalculatePointerPosition(transform.position, hitpoint, disPointerObject);
  }

  private void PointerOutGaze()
  {
    pointer.transform.localScale = Vector3.one * 0.1f;
    pointer.transform.parent.localPosition = new Vector3(0, 0, maxDistancePointer);
    pointer.transform.parent.parent.transform.rotation = transform.rotation;
    GazeManager.Instance.CancelGazeSelection();
  }

  Vector3 CalculatePointerPosition(Vector3 p0, Vector3 p1, float t)
  {
    float x = p0.x + t * (p1.x - p0.x);
    float y = p0.y + t * (p1.y - p0.y);
    float z = p0.z + t * (p1.z - p0.z);

    return new Vector3(x, y, z);
  }
}
