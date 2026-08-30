using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;

    [System.Serializable]
    public struct AudioParameter
    {
        public string parameterName; 
        public Slider slider;        
    }

    [SerializeField] private AudioParameter[] audioParameters;

    private void Start()
    {
        foreach (var param in audioParameters)
        {
            if (param.slider != null)
            {
                param.slider.onValueChanged.AddListener(val => SetVolume(param.parameterName, val));

                float savedVol = PlayerPrefs.GetFloat(param.parameterName, 0.75f);
                param.slider.value = savedVol;

                SetVolume(param.parameterName, savedVol);
            }
        }
    }

    private void SetVolume(string paramName, float sliderValue)
    {
        float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        float volumeInDb = Mathf.Log10(clampedValue) * 20f;

        mainMixer.SetFloat(paramName, volumeInDb);

        PlayerPrefs.SetFloat(paramName, sliderValue);
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        foreach (var param in audioParameters)
        {
            if (param.slider != null)
            {
                param.slider.onValueChanged.RemoveAllListeners();
            }
        }
    }
}