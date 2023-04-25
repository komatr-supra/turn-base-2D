using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic; 

namespace UAI{ 
    
    public class ScriptDocWindow: EditorWindow
    {  
        private string SystemInitPrompt = "You are a professional programmer.";
        private string apiResponse = ""; 
        private MonoScript scriptToCheck;
        private Vector2 ScrollPos;
        private int maxTokens = 3000;
        private GUIStyle style;

        private bool useFile = true;
        private string codeToCheck = "";

        private string errorMessageToFix = "";

        
        [MenuItem("Tools/AI Assistant/Script Doctor", false, 1)]
        static void Init()
        {
            ScriptDocWindow window = (ScriptDocWindow)EditorWindow.GetWindow(typeof(ScriptDocWindow), false, "Script Doctor");
            window.minSize = new Vector2(600, 400);
            window.Show();  
        }


        
        void OnGUI()
        {   
            HelperFunctions.drawSettings();

            if(style == null){
                style = new GUIStyle(EditorStyles.textArea);
                style.wordWrap = true;
            }

            GUILayout.BeginHorizontal();
                GUI.enabled = !useFile;
                if (GUILayout.Button("Check whole script")){
                    useFile = true;
                } 
                GUI.enabled = useFile;
                if (GUILayout.Button("Check code")){ 
                    useFile = false;
                }
                GUI.enabled = true;
            GUILayout.EndHorizontal();

            if(useFile){
                GUILayout.Label("Select a script:");
                scriptToCheck = (MonoScript)EditorGUILayout.ObjectField(scriptToCheck, typeof(MonoScript), false);
        
                if (scriptToCheck == null){  
                    GUILayout.Label("Please select a script first or drag and drop it into the field above.", EditorStyles.boldLabel);
                    apiResponse = "";
                }  
            }else{
                GUILayout.Label("Enter the code:");
                codeToCheck = EditorGUILayout.TextArea(codeToCheck, style);
            }


            GUILayout.Space(10);
                
            GUILayout.BeginHorizontal(); 
                GUILayout.BeginVertical();
                    drawCheckButtons();  
                GUILayout.EndVertical();
                GUILayout.BeginVertical(); 
                    drawImprovementButtons();
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(); 
                GUILayout.BeginVertical(); 
                    drawCommentButtons();
                GUILayout.EndVertical();
                GUILayout.BeginVertical(); 
                    drawFixButtons();
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
                

            if(GPTClient.Instance.status == GPTStatus.WaitingForResponse)
            {
                GUILayout.Label("Waiting for response...");
            }

            if (!string.IsNullOrEmpty(apiResponse))
            {
                GUILayout.Label("Response:");
                if(GUILayout.Button("Reset")){
                    apiResponse = "";
                }

                apiResponse = apiResponse.Replace("\\n", "\n");

                bool val = EditorStyles.textField.wordWrap;
                EditorStyles.textField.wordWrap = true;

                ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos, GUILayout.ExpandHeight(true));
                    GUILayout.Space(20); 
                        
                    string[] content = apiResponse.Split(new string[] { "```" }, System.StringSplitOptions.None); 
                    for(int i = 0; i < content.Length; i++){
                        if(content[i].Trim() == "") continue;

                        EditorGUILayout.BeginHorizontal(); 

                            EditorGUILayout.TextArea(content[i].Trim(),style, GUILayout.ExpandWidth(true));
                            
                            EditorGUILayout.BeginVertical(); 
                                if (GUILayout.Button("Copy", GUILayout.Width(60))) {
                                    TextEditor te = new TextEditor();
                                    te.text = content[i].Trim();
                                    te.SelectAll();
                                    te.Copy();
                                }
                                if (GUILayout.Button("Save file", GUILayout.Width(60)))
                                { 
                                    if(useFile){
                                        if (EditorUtility.DisplayDialog("Save to file", "Do you really want to overwrite the selected script(name: " + scriptToCheck.name + ")?", "Yes", "No"))
                                        { 
                                            string scriptFilePath = AssetDatabase.GetAssetPath(scriptToCheck);
                                            if (Path.GetExtension(scriptFilePath) != ".cs")
                                            {
                                                Debug.LogError("Selected asset is not a C# script.");
                                                return;
                                            }
                                            File.WriteAllText(scriptFilePath, content[i].Trim());
                                            AssetDatabase.Refresh();

                                            EditorUtility.DisplayDialog("Success", "The script was successfully saved.", "Ok");
                                        } 
                                    }else{
                                        // string path = EditorUtility.SaveFilePanel("Save file", GPTClient.defaultSavePath, "script", "cs");
                                        // if (path.Length != 0)
                                        // {
                                        //     File.WriteAllText(path, content[i].Trim());
                                        //     AssetDatabase.Refresh();
                                        // }

                                        string scriptName = content[i].Substring(content[i].IndexOf("public class ") + 13);
                                        scriptName = scriptName.Substring(0, scriptName.IndexOf(" ")); 
                                        if(scriptName[scriptName.Length - 1] == ':' || scriptName[scriptName.Length - 1] == '{'){
                                            scriptName = scriptName.Substring(0, scriptName.Length - 1);
                                        } 
                                        if(scriptName.Contains(":")){
                                            scriptName = scriptName.Substring(0, scriptName.IndexOf(":"));
                                        } 
                                        if(scriptName.Contains("{")){
                                            scriptName = scriptName.Substring(0, scriptName.IndexOf("{"));
                                        } 

                                        string scriptPath = GPTClient.defaultSavePath + "/" + scriptName + ".cs";
                                        if(GPTClient.askForSavePath){
                                            scriptPath = EditorUtility.SaveFilePanel("Save script", GPTClient.defaultSavePath, scriptName, "cs");
                                        }

                                        if (scriptPath.Length != 0)
                                        {
                                            File.WriteAllText(scriptPath, content[i].Trim());
                                            AssetDatabase.Refresh();

                                            EditorUtility.DisplayDialog("Script created", "The script " + scriptName + " was created.", "Ok");
                                            Selection.activeObject = AssetDatabase.LoadAssetAtPath(scriptPath, typeof(MonoScript));
                                        }
                                    }
                                    
                                }
                            EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }

                GUILayout.Space(20);
                EditorGUILayout.EndScrollView();
                
                HelperFunctions.drawCostLabel();

                EditorStyles.textField.wordWrap = val; 
            } 
        }


        private void drawCheckButtons(){
            GUILayout.Label("Check", EditorStyles.boldLabel);
            if (GUILayout.Button("Check the script for bugs")){
                sendRequestToGPT("Check the following script for bugs:´SCRIPT_TEXT´"); 
            } 
            if (GUILayout.Button("Check the script for vulnerabilities")){ 
                sendRequestToGPT("Check the following script for vulnerabilities:´SCRIPT_TEXT´"); 
            }
            if (GUILayout.Button("Check the script for performance issues")){ 
                sendRequestToGPT("Check the following script for performance issues:´SCRIPT_TEXT´"); 
            }
            if (GUILayout.Button("Check the script for readability issues")){ 
                sendRequestToGPT("Check the following script for readability issues:´SCRIPT_TEXT´"); 
            }
            if (GUILayout.Button("Check the script for structure issues")){ 
                sendRequestToGPT("Check the following script for structure issues:´SCRIPT_TEXT´"); 
            }
        }
        private void drawImprovementButtons()
        {
            GUILayout.Label("Improvements", EditorStyles.boldLabel);
            if (GUILayout.Button("Improve script performance")){ 
                sendRequestToGPT("Improve the following script for performance and add comments with the improvements:´SCRIPT_TEXT´"); 
            }
            if (GUILayout.Button("Improve script readability")){ 
                sendRequestToGPT("Improve the following script for readability:´SCRIPT_TEXT´"); 
            }
            if (GUILayout.Button("Improve script structure")){ 
                sendRequestToGPT("Improve the following script for structure:´SCRIPT_TEXT´"); 
            }

            if (GUILayout.Button("Give me a better solution for the script")){ 
                sendRequestToGPT("Give me a better solution for the following script:´SCRIPT_TEXT´"); 
            }
            if(GUILayout.Button("Give me a suggestion for the script")){ 
                sendRequestToGPT("Give me a suggestion for the following script:´SCRIPT_TEXT´"); 
            }  
        }
        private void drawFixButtons(){
            GUILayout.Label("Fix", EditorStyles.boldLabel);

            GUILayout.Label("Error Message", EditorStyles.boldLabel);
            errorMessageToFix = GUILayout.TextField(errorMessageToFix);
            if (GUILayout.Button("Fix this error in the code")){ 

                sendRequestToGPT("Fix this error:`"+errorMessageToFix+"` in the following code:´SCRIPT_TEXT´"); 
            } 
            if(GUILayout.Button("Explain the error message")){ 
                sendRequestToGPT("Explain the following error: ´"+errorMessageToFix+"´ in this code:´SCRIPT_TEXT´");
            }
            if(GUILayout.Button("Explain why this error happens in the code")){ 
                sendRequestToGPT("Explain why this error happens: ´"+errorMessageToFix+"´ in this code:´SCRIPT_TEXT´");
            }
        }
        private void drawCommentButtons()
        {
            GUILayout.Label("Comments", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();  
                if (GUILayout.Button("Add comments to the script")){  
                    sendRequestToGPT("Add a comment to each line (after it) of the following script:´SCRIPT_TEXT´"); 
                } 
                
                // if (GUILayout.Button(new GUIContent("Partial", "Partial for longer scripts."))){  
                //     sendPartialRequestToGPT("Add comments to this piece of script and don't add code!:´SCRIPT_TEXT´"); 
                // }
            GUILayout.EndHorizontal();
            //remove
            if (GUILayout.Button("Remove comments from the script")){ 
                sendRequestToGPT("Remove comments from the following script:´SCRIPT_TEXT´"); 
            }
            if (GUILayout.Button("Add a comment block before each function")){ 
                sendRequestToGPT("Add a comment block before each function that explains what they do. Give me the whole script:´SCRIPT_TEXT´"); 
            }
            if (GUILayout.Button("Add a comment block before each class")){ 
                sendRequestToGPT("Add a comment block before each class that explains what they do. Give me the whole script:´SCRIPT_TEXT´"); 
            }
            if (GUILayout.Button("Add a comment block before each variable")){ 
                sendRequestToGPT("Add a comment block before each variable that explains what they do. Give me the whole script:´SCRIPT_TEXT´"); 
            }
        }

        private void sendRequestToGPT(string prompt)
        {  
            if (useFile && scriptToCheck == null){  
                Debug.Log("Please select a script first or drag and drop it into the field above.");
                return;
            }
            apiResponse = "";

            GPTClient.Instance.SystemInitPrompt = SystemInitPrompt;

            string scriptText = "";

            if(useFile){
                string scriptFilePath = AssetDatabase.GetAssetPath(scriptToCheck);
                if (Path.GetExtension(scriptFilePath) != ".cs")
                {
                    Debug.LogError("Selected asset is not a C# script.");
                    return;
                } 
                scriptText = File.ReadAllText(scriptFilePath); 
            }else{
                scriptText = codeToCheck;
            }
    
    
            prompt = prompt.Replace("SCRIPT_TEXT", scriptText);

            GPTClient.maxTokens = maxTokens;
    
            GPTClient.Instance.OnResponseReceived = null;

            GPTClient.Instance.OnResponseReceived += (response, index) =>
            {
                apiResponse = response;
                apiResponse = HelperFunctions.RemoveScriptTagFromOpenAIResponse(apiResponse);
                Repaint();
            };
            GPTClient.Instance.SendRequest(prompt); 
        }
    }
}