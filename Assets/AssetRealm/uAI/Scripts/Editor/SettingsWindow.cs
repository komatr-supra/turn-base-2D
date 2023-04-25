using UnityEditor;
using UnityEngine;

namespace UAI{ 
    public class SettingsWindow : EditorWindow
    {
        //models

        private static string _secretKey = ""; 

        
        public static float _temperature = 0.7f;
        public static int _maxTokens = 3000;
        public static int _n = 1;

        public static string _defaultSavePath = "Assets/";
        public static bool _askForSavePath = true;

        [MenuItem("Tools/AI Assistant/Settings", false, 1000)]
        public static void ShowWindow()
        {
            SettingsWindow window = GetWindow<SettingsWindow>("AI Assistant Settings"); 
            window.minSize = new Vector2(400, 250);
        }

        private void OnEnable() {
            LoadAPIKey();

            _temperature = EditorPrefs.GetFloat("GPTTemperature", 0.7f);
            _maxTokens = EditorPrefs.GetInt("GPTMaxTokens", 3000);
            _n = EditorPrefs.GetInt("GPTN", 1);
            _defaultSavePath = EditorPrefs.GetString("GPTDefaultSavePath", "Assets/");
            _askForSavePath = EditorPrefs.GetBool("GPTAskForSavePath", false);

            GPTClient.temperature = _temperature;
            GPTClient.maxTokens = _maxTokens;
            GPTClient.n = _n;
            GPTClient.defaultSavePath = _defaultSavePath;
            GPTClient.askForSavePath = _askForSavePath; 
        }

        public static string LoadAPIKey()  
        {
            _secretKey = EditorPrefs.GetString("UAISecretKey", "");
            GPTClient.Instance.apiKey = _secretKey;

            GPTClient.Instance.model = EditorPrefs.GetString("GPTModel", "gpt-3.5-turbo");

            
            for (int i = 0; i < GPTClient.models.Length; i++)
            {
                if (GPTClient.models[i] == GPTClient.Instance.model)
                {
                    GPTClient.modelIndex = i;
                    break;
                }
            }

            return _secretKey;
        } 

        public void SaveSettings()
        {
            GPTClient.Instance.model = GPTClient.models[GPTClient.modelIndex];

            EditorPrefs.SetString("UAISecretKey", _secretKey);
            EditorPrefs.SetString("GPTModel", GPTClient.Instance.model);
            EditorPrefs.SetFloat("GPTTemperature", _temperature);
            EditorPrefs.SetInt("GPTMaxTokens", _maxTokens);
            EditorPrefs.SetInt("GPTN", _n);
            EditorPrefs.SetString("GPTDefaultSavePath", _defaultSavePath);
            EditorPrefs.SetBool("GPTAskForSavePath", _askForSavePath);

            GPTClient.Instance.apiKey = _secretKey;
            GPTClient.temperature = _temperature;
            GPTClient.maxTokens = _maxTokens;
            GPTClient.n = _n;
            GPTClient.defaultSavePath = _defaultSavePath;
            GPTClient.askForSavePath = _askForSavePath;
        }

        private void OnGUI()
        {
            GUILayout.Label("AI Assistant Settings", EditorStyles.boldLabel);
            _secretKey = EditorGUILayout.PasswordField("OpenAI API Key:", _secretKey);

            //drop down menu for_model
            GPTClient.modelIndex = EditorGUILayout.Popup("Model:", GPTClient.modelIndex, GPTClient.models);

            //space
            GUILayout.Space(10);

            _temperature = EditorGUILayout.FloatField("Temperature:", _temperature);
            if(_temperature < 0)
                _temperature = 0;
            if(_temperature > 1)
                _temperature = 1;

            _maxTokens = EditorGUILayout.IntField("Max Tokens:", _maxTokens);
            _n = EditorGUILayout.IntField("N:", _n);

            _defaultSavePath = EditorGUILayout.TextField("Default Save Path:", _defaultSavePath);
            //label
            GUILayout.Label("Ask for Save Path on every creation?");
            _askForSavePath = EditorGUILayout.Toggle("Ask for Save Path?", _askForSavePath); 


    
            GUILayout.Space(10);

            GUI.enabled = GPTClient.Instance.apiKey != _secretKey || GPTClient.Instance.model != GPTClient.models[GPTClient.modelIndex] || GPTClient.temperature != _temperature || GPTClient.maxTokens != _maxTokens || GPTClient.n != _n || GPTClient.defaultSavePath != _defaultSavePath || GPTClient.askForSavePath != _askForSavePath;
            if (GUILayout.Button("Save Settings"))
            {
                SaveSettings();
            }
            GUI.enabled = true;

            GUILayout.Space(10);
            if(GUILayout.Button("Get API Key"))
            {
                Application.OpenURL("https://platform.openai.com/account/api-keys");
            }
    
        }
    }
}