using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;//4.2.8
using UnityEngine.XR.ARSubsystems;//4.2.8
using ZXing;

public class ARQRCodeScanner : MonoBehaviour
{
    private ARCameraManager arCameraManager;
    private BarcodeReader barcodeReader;

    void Start()
    {
        arCameraManager = FindObjectOfType<ARCameraManager>();
        barcodeReader = new BarcodeReader();
    }

    void Update()
    {
        if (arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, image.width, image.height),
                outputDimensions = new Vector2Int(image.width, image.height),
                outputFormat = TextureFormat.RGBA32,
                transformation = XRCpuImage.Transformation.MirrorY
            };

            var rawTextureData = new NativeArray<byte>(image.GetConvertedDataSize(conversionParams), Allocator.Temp);
            image.Convert(conversionParams, rawTextureData);
            image.Dispose();

            var colors = new Color32[rawTextureData.Length / 4];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color32(rawTextureData[i * 4], rawTextureData[i * 4 + 1], rawTextureData[i * 4 + 2], rawTextureData[i * 4 + 3]);
            }

            try
            {
                var result = barcodeReader.Decode(colors, image.width, image.height);
                if (result != null)
                {
                    Debug.Log("Decoded: " + result.Text);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
            
            rawTextureData.Dispose();
        }
    }
}
