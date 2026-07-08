using System.Collections;
using Mediapipe.Tasks.Vision.HandLandmarker;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mediapipe.Unity.Sample.HandLandmarkDetection
{
  public class HandLandmarkerRunner : VisionTaskApiRunner<HandLandmarker>
  {
    [SerializeField] private HandLandmarkerResultAnnotationController _handLandmarkerResultAnnotationController;

    private Experimental.TextureFramePool _textureFramePool;
    public readonly HandLandmarkDetectionConfig config = new HandLandmarkDetectionConfig();

    // --- OPTIMIZACIÓN IUMAFIS ---
    [Header("Optimización de Rendimiento")]
    [Range(1, 10)][SerializeField] private int _processEveryNFrames = 3; // Analiza 1 de cada 3 frames
    private int _frameCounter = 0; // Mantenido por compatibilidad estructural interna

    public override void Stop()
    {
      base.Stop();
      _textureFramePool?.Dispose();
      _textureFramePool = null;
    }

    protected override IEnumerator Run()
    {
      // Inicialización nativa del entorno y tasas de refresco
      QualitySettings.vSyncCount = 0;
      Application.targetFrameRate = 60;
      yield return AssetLoader.PrepareAssetAsync(config.ModelPath);

      var options = config.GetHandLandmarkerOptions(config.RunningMode == Tasks.Vision.Core.RunningMode.LIVE_STREAM ? OnHandLandmarkDetectionOutput : null);
      taskApi = HandLandmarker.CreateFromOptions(options, GpuManager.GpuResources);
      var imageSource = ImageSourceProvider.ImageSource;

      yield return imageSource.Play();

      if (!imageSource.isPrepared) { yield break; }

      _textureFramePool = new Experimental.TextureFramePool(imageSource.textureWidth, imageSource.textureHeight, TextureFormat.RGBA32, 10);
      screen.Initialize(imageSource);

      // Blindaje contra NullReference si decides quitar las anotaciones
      if (_handLandmarkerResultAnnotationController != null)
      {
        SetupAnnotationController(_handLandmarkerResultAnnotationController, imageSource);
      }

      var transformationOptions = imageSource.GetTransformationOptions();
      var flipHorizontally = transformationOptions.flipHorizontally;
      var flipVertically = transformationOptions.flipVertically;
      var imageProcessingOptions = new Tasks.Vision.Core.ImageProcessingOptions(rotationDegrees: (int)transformationOptions.rotationAngle);

      AsyncGPUReadbackRequest req = default;
      var waitUntilReqDone = new WaitUntil(() => req.done);

      // CORRECCIÓN DE RENDIMIENTO 1: Reemplazamos WaitForEndOfFrame por yield return null (espera al inicio del sig. frame).
      // Esto evita sobrecargar la CPU durante la fase crítica de renderizado de la GPU en PC y Android.
      var waitForNextFrame = new System.Object(); // Representa la espera estándar (null) de forma limpia

      var result = HandLandmarkerResult.Alloc(options.numHands);

      var canUseGpuImage = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 && GpuManager.GpuResources != null;
      using var glContext = canUseGpuImage ? GpuManager.GetGlContext() : null;

      while (true)
      {
        if (isPaused) { yield return new WaitWhile(() => isPaused); }

        // Mantenemos el incremento por si alguna dependencia lee esta variable
        _frameCounter++;

        // CORRECCIÓN DE RENDIMIENTO 2: Sincronización amarrada al reloj global de Unity (Time.frameCount).
        // Al usar la lectura asíncrona (CPUAsync), la corrutina se congela. Si usábamos un contador manual, 
        // el salto de frames se volvía errático provocando tirones. Time.frameCount garantiza un salto estrictamente uniforme.
        if (Time.frameCount % _processEveryNFrames != 0)
        {
          yield return null;
          continue;
        }

        if (!_textureFramePool.TryGetTextureFrame(out var textureFrame))
        {
          yield return null;
          continue;
        }

        Image image;
        switch (config.ImageReadMode)
        {
          case ImageReadMode.GPU:
            textureFrame.ReadTextureOnGPU(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
            image = textureFrame.BuildGPUImage(glContext);
            yield return null; // Optimizado (Antes WaitForEndOfFrame)
            break;
          case ImageReadMode.CPU:
            yield return null; // Optimizado (Antes WaitForEndOfFrame)
            textureFrame.ReadTextureOnCPU(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
            image = textureFrame.BuildCPUImage();
            textureFrame.Release();
            break;
          default: // CPUAsync (El modo recomendado y optimizado para Android)
            req = textureFrame.ReadTextureAsync(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
            yield return waitUntilReqDone; // Espera segura a que la GPU entregue la textura al hilo secundario
            if (req.hasError) { continue; }
            image = textureFrame.BuildCPUImage();
            textureFrame.Release();
            break;
        }

        // Ejecución de la tarea según el modo sin alterar el comportamiento de las mallas
        if (taskApi != null)
        {
          switch (taskApi.runningMode)
          {
            case Tasks.Vision.Core.RunningMode.IMAGE:
              if (taskApi.TryDetect(image, imageProcessingOptions, ref result))
              {
                if (_handLandmarkerResultAnnotationController != null) _handLandmarkerResultAnnotationController.DrawNow(result);
              }
              break;
            case Tasks.Vision.Core.RunningMode.VIDEO:
              if (taskApi.TryDetectForVideo(image, GetCurrentTimestampMillisec(), imageProcessingOptions, ref result))
              {
                if (_handLandmarkerResultAnnotationController != null) _handLandmarkerResultAnnotationController.DrawNow(result);
              }
              break;
            case Tasks.Vision.Core.RunningMode.LIVE_STREAM:
              // Modo óptimo: MediaPipe procesa en su propio hilo nativo (C++) sin congelar a Unity
              taskApi.DetectAsync(image, GetCurrentTimestampMillisec(), imageProcessingOptions);
              break;
          }
        }
      }
    }

    private void OnHandLandmarkDetectionOutput(HandLandmarkerResult result, Image image, long timestamp)
    {
      // Solo dibuja si el controlador de anotaciones existe (Mantiene tu lógica de seguridad intacta)
      if (_handLandmarkerResultAnnotationController != null)
      {
        _handLandmarkerResultAnnotationController.DrawLater(result);
      }
    }
  }
}
