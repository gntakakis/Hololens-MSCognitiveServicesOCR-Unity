using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayVoiceMessage : MonoBehaviour {

    public static PlayVoiceMessage Instance { get; private set; }
    
    public GameObject photoCaptureManagerGmObj;
    
    void Awake()
    {
        Instance = this;
    }

    public void PlayTextToSpeechMessage(MCSComputerVisionOCRDto computerVisionOCR)
    {
        string message = string.Empty;

        if (string.IsNullOrEmpty(computerVisionOCR.text))
            message = "I couldn't detect text";
        else
            message = string.Format("The text says, {0}", computerVisionOCR.text);

        // Try and get a TTS Manager
        TextToSpeechManager tts = null;

        if (photoCaptureManagerGmObj != null)
        {
            tts = photoCaptureManagerGmObj.GetComponent<TextToSpeechManager>();
        }

        if (tts != null)
        {
            //Play voice message
            tts.SpeakText(message);
        }
    }
}
