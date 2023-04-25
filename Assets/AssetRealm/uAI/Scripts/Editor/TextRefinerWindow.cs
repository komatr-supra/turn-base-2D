using UnityEngine;
using UnityEditor; 

namespace UAI{ 
    public class TextRefinerWindow : EditorWindow
    { 
        // This is the prompt that will be sent to GPT for initialization, so the AI knows what is wanted
        private string SystemInitPrompt = "You are a true master of words. You can check the spelling of any text and correct it. You can also improve the text by adding more details or making it sound more interesting.";
        private string input = "Hi, what are you doing here?";
        private string instructions = "Make it sound like a pirate saying it.";


        private string apiResponse = "";  
        private Vector2 scrollPos;

        private GUIStyle style;
        private bool copied = false;
    
        [MenuItem("Tools/AI Assistant/Text Refiner", false, 100)]
        static void Init()
        {
            TextRefinerWindow window = (TextRefinerWindow)EditorWindow.GetWindow(typeof(TextRefinerWindow), false, "Text Refiner"); 
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 400);
            window.Show();  
        } 

        void OnGUI()
        {    
            HelperFunctions.drawSettings();

            if(style == null){
                style = new GUIStyle(EditorStyles.textArea);
                style.wordWrap = true;
            }
            GUILayout.Space(10);
            GUILayout.Label("Paste the content that you want to improve.", EditorStyles.boldLabel);
            input = EditorGUILayout.TextArea(input, style, GUILayout.Height(100)); 

            GUILayout.Space(10);  
    
            GUILayout.Label("Instructions", EditorStyles.boldLabel);
            instructions = EditorGUILayout.TextArea( instructions, style, GUILayout.Height(100)); 

            if (GUILayout.Button("Improve the text")){  
                sendRequestToGPT(instructions + ". The text: \"" + input + "\"."); 
            } 
            
            if(GPTClient.Instance.status == GPTStatus.WaitingForResponse)  
                GUILayout.Label("Waiting for response..."); 

            if(apiResponse != "")
            {
                GUILayout.Label("Response", EditorStyles.boldLabel);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
                EditorGUILayout.TextArea(apiResponse, style, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
                
                HelperFunctions.drawCostLabel();
                
                if(copied){
                    GUILayout.Label("Copied to clipboard");
                }
                if(GUILayout.Button("Copy to clipboard")){
                    EditorGUIUtility.systemCopyBuffer = apiResponse;
                    copied = true;
                }
            }
        }

        private void sendRequestToGPT(string prompt)
        { 
            GPTClient.Instance.SystemInitPrompt = SystemInitPrompt;
            apiResponse = "";
                
            GPTClient.Instance.OnResponseReceived = null;
                
            GPTClient.Instance.OnResponseReceived += (response, index) =>
            {
                apiResponse = response;
                copied = false;
                Repaint();
            };
            GPTClient.Instance.SendRequest(prompt); 
        }
    }
}