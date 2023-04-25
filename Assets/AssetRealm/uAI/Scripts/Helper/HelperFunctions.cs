using System.Collections; 
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace UAI{ 
    public class HelperFunctions 
    {   
        public static string RemoveScriptTagFromOpenAIResponse(string text)
        {  
            if(text.Contains("```csharp"))
            {
                text = text.Replace("```csharp", "```");  
            }
            if(text.Contains("```C#"))
            {
                text = text.Replace("```C#", "```");  
            } 
            if(text.Contains("```cs"))
            {
                text = text.Replace("```cs", "```");  
            }
            if(text.Contains("```c#"))
            {
                text = text.Replace("```c#", "```");  
            } 

            return text;
        }

        
        private static int maxTokens_GPT3_5 = 4096;
        private static int maxTokens_GPT4_8k = 8192;
        private static int maxTokens_GPT4_32k = 32768;

#if UNITY_EDITOR
        public static void drawSettings(){
            GPTClient.showSettings = EditorGUILayout.Foldout(GPTClient.showSettings, "Settings"); 
            if(GPTClient.showSettings){
                GPTClient.modelIndex = EditorGUILayout.Popup("Model:", GPTClient.modelIndex, GPTClient.models);
                GPTClient.temperature = EditorGUILayout.Slider("Temperature:", GPTClient.temperature, 0.0f, 1.0f);
                GPTClient.n = EditorGUILayout.IntSlider("N:", GPTClient.n, 1, 10);

                GPTClient.maxTokens = EditorGUILayout.IntSlider("Max Tokens:", GPTClient.maxTokens, 1, getMaxTokenFromModel (GPTClient.models[GPTClient.modelIndex]));

                // GPTClient.defaultSavePath = EditorGUILayout.TextField("Default Save Path:", GPTClient.defaultSavePath);
                // GPTClient.askForSavePath = EditorGUILayout.Toggle("Ask for Save Path on every creation:", GPTClient.askForSavePath);

                GUILayout.Space(20);
            }
        }

        public static void drawCostLabel(){ 
            GUILayout.Label("Cost: " + GPTClient.cost + "$", EditorStyles.boldLabel);
        }

#endif
        public static int getMaxTokenFromModel(string model){
            if(model == "gpt-3.5-turbo" ){
                return maxTokens_GPT3_5;
            }else if(model == "gpt-4"){
                return maxTokens_GPT4_8k;
            }else if(model == "gpt-4-32k"){
                return maxTokens_GPT4_32k;
            }else{
                return maxTokens_GPT3_5;
            }
        }
    }
}
 