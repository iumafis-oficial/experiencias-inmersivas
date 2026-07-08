// Refactorizado para evitar sobrecarga en UI - IUMAFIS 23 de Abril
using UnityEngine;
using UnityEngine.UI;
using Mediapipe.Unity.Sample.UI;

namespace Mediapipe.Unity.Sample.HandLandmarkDetection.UI
{
  public class HandLandmarkDetectionConfigWindow : ModalContents
  {
    [SerializeField] private Dropdown _delegateInput;
    [SerializeField] private Dropdown _imageReadModeInput;
    [SerializeField] private Dropdown _runningModeInput;
    [SerializeField] private InputField _numHandsInput;
    [SerializeField] private InputField _minHandDetectionConfidenceInput;
    [SerializeField] private InputField _minHandPresenceConfidenceInput;
    [SerializeField] private InputField _minTrackingConfidenceInput;

    private HandLandmarkDetectionConfig _config;
    private bool _isChanged;

    private void Start()
    {
      // OPTIMIZACIÓN: Cacheamos la referencia de forma segura
      var runner = GameObject.Find("Solution")?.GetComponent<HandLandmarkerRunner>();
      if (runner != null)
      {
        _config = runner.config;
        InitializeContents();
      }
      else
      {
        Debug.LogError("No se encontró el Solution/HandLandmarkerRunner en la escena.");
      }
    }

    public override void Exit()
    {
      // Si hubo cambios, cerramos y aplicamos
      GetModal().CloseAndResume(_isChanged);
    }

    // --- MÉTODOS DE ACTUALIZACIÓN ---
    // He simplificado esto para que no sature la CPU con cada cambio de texto

    private void SwitchDelegate()
    {
      _config.Delegate = (Tasks.Core.BaseOptions.Delegate)_delegateInput.value;
      _isChanged = true;
    }

    private void SwitchImageReadMode()
    {
      _config.ImageReadMode = (ImageReadMode)_imageReadModeInput.value;
      _isChanged = true;
    }

    private void SwitchRunningMode()
    {
      _config.RunningMode = (Tasks.Vision.Core.RunningMode)_runningModeInput.value;
      _isChanged = true;
    }

    private void SetNumHands()
    {
      if (int.TryParse(_numHandsInput.text, out var value))
      {
        _config.NumHands = Mathf.Clamp(value, 1, 2); // Forzamos máximo 2 manos para no matar la CPU
        _isChanged = true;
      }
    }

    // Los métodos de confianza se mantienen igual pero aseguran el cambio de flag
    private void SetMinHandDetectionConfidence() { if (float.TryParse(_minHandDetectionConfidenceInput.text, out var value)) { _config.MinHandDetectionConfidence = value; _isChanged = true; } }
    private void SetMinHandPresenceConfidence() { if (float.TryParse(_minHandPresenceConfidenceInput.text, out var value)) { _config.MinHandPresenceConfidence = value; _isChanged = true; } }
    private void SetMinTrackingConfidence() { if (float.TryParse(_minTrackingConfidenceInput.text, out var value)) { _config.MinTrackingConfidence = value; _isChanged = true; } }

    private void InitializeContents()
    {
      InitializeDelegate();
      InitializeImageReadMode();
      InitializeRunningMode();
      InitializeNumHands();
      InitializeMinHandDetectionConfidence();
      InitializeMinHandPresenceConfidence();
      InitializeMinTrackingConfidence();
    }

    // Los métodos Initialize se mantienen para no romper la compatibilidad con tu prefab de Canva/UI
    private void InitializeDelegate() { InitializeDropdown<Tasks.Core.BaseOptions.Delegate>(_delegateInput, _config.Delegate.ToString()); _delegateInput.onValueChanged.AddListener(delegate { SwitchDelegate(); }); }
    private void InitializeImageReadMode() { InitializeDropdown<ImageReadMode>(_imageReadModeInput, _config.ImageReadMode.GetDescription()); _imageReadModeInput.onValueChanged.AddListener(delegate { SwitchImageReadMode(); }); }
    private void InitializeRunningMode() { InitializeDropdown<Tasks.Vision.Core.RunningMode>(_runningModeInput, _config.RunningMode.ToString()); _runningModeInput.onValueChanged.AddListener(delegate { SwitchRunningMode(); }); }
    private void InitializeNumHands() { _numHandsInput.text = _config.NumHands.ToString(); _numHandsInput.onValueChanged.AddListener(delegate { SetNumHands(); }); }
    private void InitializeMinHandDetectionConfidence() { _minHandDetectionConfidenceInput.text = _config.MinHandDetectionConfidence.ToString(); _minHandDetectionConfidenceInput.onValueChanged.AddListener(delegate { SetMinHandDetectionConfidence(); }); }
    private void InitializeMinHandPresenceConfidence() { _minHandPresenceConfidenceInput.text = _config.MinHandPresenceConfidence.ToString(); _minHandPresenceConfidenceInput.onValueChanged.AddListener(delegate { SetMinHandPresenceConfidence(); }); }
    private void InitializeMinTrackingConfidence() { _minTrackingConfidenceInput.text = _config.MinTrackingConfidence.ToString(); _minTrackingConfidenceInput.onValueChanged.AddListener(delegate { SetMinTrackingConfidence(); }); }
  }
}
