//------------------------------------------------------------------------------
// <copyright file="HandLandmarkDetectionConfig.cs" company="Juan Esteban Quinchia Duque">
//      Developer: Juan Esteban Quinchia Duque
//      Project: InmersiveExperienceIUMAFIS
//      Optimizado para Eliminación de Stalls - Mayo 2026
// </copyright>
//------------------------------------------------------------------------------

using Mediapipe.Tasks.Vision.HandLandmarker;

namespace Mediapipe.Unity.Sample.HandLandmarkDetection
{
  public class HandLandmarkDetectionConfig
  {
    // DELEGATE: Se mantiene en CPU por compatibilidad de hardware industrial, 
    // pero si pruebas en PC/Android potente, cambiar a GPU liberará masivamente la CPU.
    public Tasks.Core.BaseOptions.Delegate Delegate { get; set; } = Tasks.Core.BaseOptions.Delegate.CPU;

    // CORRECCIÓN 1: Cambiamos a CPUAsync. Usa AsyncGPUReadback para transferir la textura 
    // en un hilo secundario, eliminando los bloqueos síncronos del hilo principal de Unity.
    public ImageReadMode ImageReadMode { get; set; } = ImageReadMode.CPUAsync;

    // RUNNING MODE: LIVE_STREAM es el correcto ya que procesa de forma asíncrona nativa.
    public Tasks.Vision.Core.RunningMode RunningMode { get; set; } = Tasks.Vision.Core.RunningMode.LIVE_STREAM;

    public int NumHands { get; set; } = 2;

    // CORRECCIÓN 2: Subimos la confianza a 0.50f. 
    // Al ser más estrictos, el algoritmo descarta el ruido visual del fondo inmediatamente 
    // y la CPU trabaja MENOS, enfocándose solo en manos reales.
    public float MinHandDetectionConfidence { get; set; } = 0.50f;
    public float MinHandPresenceConfidence { get; set; } = 0.50f;
    public float MinTrackingConfidence { get; set; } = 0.50f;

    public string ModelPath => "hand_landmarker.bytes";

    public HandLandmarkerOptions GetHandLandmarkerOptions(HandLandmarkerOptions.ResultCallback resultCallback = null)
    {
      return new HandLandmarkerOptions(
          new Tasks.Core.BaseOptions(Delegate, modelAssetPath: ModelPath),
          runningMode: RunningMode,
          numHands: NumHands,
          minHandDetectionConfidence: MinHandDetectionConfidence,
          minHandPresenceConfidence: MinHandPresenceConfidence,
          minTrackingConfidence: MinTrackingConfidence,
          resultCallback: resultCallback
      );
    }
  }
}
