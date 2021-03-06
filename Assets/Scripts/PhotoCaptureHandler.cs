﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.XR.WSA.WebCam;

public class PhotoCaptureHandler : MonoBehaviour
{
    PhotoCapture photoCapture = null;

    public void StartPhotoCapture()
    {
        //Create capture async
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCapture = captureObject;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.JPEG;

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    private void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCapture.Dispose();
        photoCapture = null;
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        /* Load to Memory */
        if (result.success)
        {
            try
            {
                photoCapture.TakePhotoAsync(OnCapturedPhotoToMemory);
            }
            catch (System.ArgumentException e)
            {
                Debug.LogError("System.ArgumentException:\n" + e.Message);
            }
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            Debug.Log("Saved Photo to disk!");
            photoCapture.StopPhotoModeAsync(OnStoppedPhotoMode);
        }
        else
        {
            Debug.Log("Failed to save Photo to disk");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            List<byte> imageBufferList = new List<byte>();

            Debug.Log("OnCapturedPhotoToMemory Copy Started");

            photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);

            Debug.Log("OnCapturedPhotoToMemory " + imageBufferList.Count);

            //Execute OCR Coroutine
            ExecuteMCSComputerVisionOCR(imageBufferList, "ocr");
        }
        else
        {
            Debug.Log("Failed to save Photo to memory");
        }

        photoCapture.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    public void ExecuteMCSComputerVisionOCR(List<byte> imageBufferList, string type)
    {
        Debug.Log("Started PostToComputerVision OCR processing");
        MCSCognitiveServices mCSPostApi = new MCSCognitiveServices();
        StartCoroutine(mCSPostApi.PostToComputerVisionOCR(imageBufferList.ToArray(), type));
        Debug.Log("Ended PostToComputerVision OCR processing coroutine");
    }

}
