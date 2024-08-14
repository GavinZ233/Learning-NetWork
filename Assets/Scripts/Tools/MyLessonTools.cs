using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;


// Adds a component to the selected GameObjects

class MyLessonTools : EditorWindow
{
    string LessonName="LessonX_YYY";
    public bool needOpenScene = true;
    [MenuItem("GavinTools/CreatLesson")]
    static void Init()
    {
        var window = GetWindow<MyLessonTools>();
        window.position = new Rect(500, 500, 300, 200);
        window.Show();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0,0,100,30),"课件名称：") ;
        LessonName= GUI.TextField(new Rect(120,0,150,30),LessonName);
        needOpenScene = GUI.Toggle(new Rect(0,30,150,50),needOpenScene,"是否需要打开场景");
        if (GUI.Button(new Rect(120, 30, 150, 30), "生成课件"))
            CreatLesson();
        if (GUI.Button(new Rect(120, 60, 150, 30), "挂脚本"))
            AddScript();



    }

    void CreatLesson()
    {
        string path = Application.dataPath + "/Lesson/" + LessonName;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        //创建脚本
        string templatePath = System.IO.Path.Combine(UnityEditor.EditorApplication.applicationContentsPath, "Resources", "ScriptTemplates", "81-C# Script-NewBehaviourScript.cs.txt");
        string scriptStr =File.ReadAllText(templatePath);
        scriptStr = scriptStr.Replace("#ROOTNAMESPACEBEGIN#","");
        scriptStr = scriptStr.Replace("#ROOTNAMESPACEEND#", "");
        scriptStr = scriptStr.Replace("#SCRIPTNAME#", LessonName);
        File.WriteAllText(path + "/" + LessonName + ".cs", scriptStr);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //创建场景
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects,NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene,path+"/"+LessonName+".unity");
        AssetDatabase.Refresh();

        if (!needOpenScene)
        {
            EditorSceneManager.CloseScene(scene, true);

        }

        AssetDatabase.Refresh();

    }

    void AddScript()
    {
        Camera ca = Camera.main;
        ca.gameObject.AddComponent(Type.GetType(LessonName));
        
        CloseWindow();
    }

    void CloseWindow()
    {
        var window = GetWindow<MyLessonTools>();
        window.Close();

    }
}