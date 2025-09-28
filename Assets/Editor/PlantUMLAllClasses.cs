using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlantUMLConnectScanner : EditorWindow
{
    private string outputFileName = "PlantUML_ConnectScripts.puml";

    [MenuItem("Tools/Generate PlantUML (Connect Scripts)")]
    public static void ShowWindow()
    {
        GetWindow<PlantUMLConnectScanner>();
    }

    void OnGUI()
    {
        GUILayout.Label("PlantUML Generator (Connect_Core & Connect_Common)", EditorStyles.boldLabel);
        outputFileName = EditorGUILayout.TextField("Output File", outputFileName);

        if (GUILayout.Button("Generate UML"))
        {
            GeneratePlantUML(outputFileName);
        }
    }

    private static string SanitizeName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "Unknown";
        return name.Replace('.', '_')
                   .Replace('+', '_')
                   .Replace('`', '_')
                   .Replace('<', '_')
                   .Replace('>', '_')
                   .Replace(',', '_')
                   .Replace(' ', '_')
                   .Replace('[', '_')
                   .Replace(']', '_')
                   .Replace('=', '_')
                   .Replace('{', '_')
                   .Replace('}', '_')
                   .Replace('-', '_');
    }

    private class SourceClass
    {
        public string FullName;
        public string BaseClass;
        public List<string> FieldTypes = new List<string>();
    }

    private static readonly HashSet<string> IgnoredTypes = new HashSet<string>
    {
        "int","float","double","bool","string","Color","Vector2","Vector2Int","Vector3","Vector3Int","Vector4",
        "TMP_Text","TMP_InputField","TMP_Dropdown","TextMeshProUGUI","Button","UnityAction","Object","Material","Tween"
    };

    private static void GeneratePlantUML(string fileName)
    {
        string assetsPath = Application.dataPath;
        var csFiles = Directory.GetFiles(assetsPath, "*.cs", SearchOption.AllDirectories);

        var nsRegex = new Regex(@"namespace\s+([A-Za-z0-9_.]+)");
        var classRegex = new Regex(@"\b(class|struct|interface)\s+([A-Za-z_][A-Za-z0-9_]*)\s*(?:\:\s*([A-Za-z0-9_<>\.,\s_]+))?");

        var allClasses = new List<SourceClass>();
        var knownSanitized = new HashSet<string>();

        // 1) Собираем все классы Connect
        foreach (var file in csFiles)
        {
            string text;
            try { text = File.ReadAllText(file); } catch { continue; }

            string fileNs = null;
            var mns = nsRegex.Match(text);
            if (mns.Success) fileNs = mns.Groups[1].Value.Trim();

            foreach (Match m in classRegex.Matches(text))
            {
                string className = m.Groups[2].Value.Trim();
                if (string.IsNullOrEmpty(className)) continue;

                string fullName = string.IsNullOrEmpty(fileNs) ? className : (fileNs + "." + className);

                if (!fullName.StartsWith("Connect.Core") && !fullName.StartsWith("Connect.Common"))
                    continue;

                if (knownSanitized.Contains(SanitizeName(fullName))) continue;

                string baseClass = null;
                if (m.Groups.Count >= 4 && !string.IsNullOrEmpty(m.Groups[3].Value))
                {
                    baseClass = m.Groups[3].Value.Split(',')[0].Trim();
                    baseClass = Regex.Replace(baseClass, @"\<.*\>", "");
                }

                var sc = new SourceClass { FullName = fullName, BaseClass = baseClass };

                // Публичные и защищённые поля
                var fieldMatches = Regex.Matches(text, @"\b(public|protected)\s+([A-Za-z0-9_\.]+)\s+[A-Za-z0-9_]+\s*;");
                foreach (Match f in fieldMatches)
                {
                    string typeName = f.Groups[2].Value.Trim();
                    if (!IgnoredTypes.Contains(typeName))
                        sc.FieldTypes.Add(typeName);
                }

                allClasses.Add(sc);
                knownSanitized.Add(SanitizeName(fullName));
            }
        }

        // 2) Генерация PlantUML
        var sb = new StringBuilder();
        sb.AppendLine("@startuml");
        sb.AppendLine("skinparam classPadding 8");
        sb.AppendLine("skinparam classMargin 8");
        sb.AppendLine("skinparam linetype ortho");
        sb.AppendLine("skinparam defaultFontSize 14");
        sb.AppendLine("skinparam classFontSize 14");

        // Добавляем базовые Unity-классы, чтобы стрелки работали
        sb.AppendLine("class MonoBehaviour");
        sb.AppendLine("class ScriptableObject");
        sb.AppendLine("class CertificateHandler");

        // Добавляем все классы
        foreach (var c in allClasses)
        {
            string san = SanitizeName(c.FullName);
            sb.AppendLine($"class {san} {{");
            foreach (var fType in c.FieldTypes)
            {
                sb.AppendLine($"  {SanitizeName(fType)} <field>");
            }
            sb.AppendLine("}");
        }

        // Наследование
        foreach (var c in allClasses)
        {
            string san = SanitizeName(c.FullName);
            if (!string.IsNullOrEmpty(c.BaseClass))
            {
                string baseSan = SanitizeName(c.BaseClass);
                sb.AppendLine($"{baseSan} <|-- {san}");
            }
        }

        // Поля-связи
        foreach (var c in allClasses)
        {
            string san = SanitizeName(c.FullName);
            foreach (var fType in c.FieldTypes)
            {
                string targetSan = SanitizeName(fType);
                if (knownSanitized.Contains(targetSan))
                    sb.AppendLine($"{san} --> {targetSan}");
            }
        }

        sb.AppendLine("@enduml");

        // Запись файла
        string outputPath = Path.Combine(Application.dataPath, fileName);
        try
        {
            File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
            Debug.Log($"[PlantUML] UML файл сохранён: {outputPath} (Classes: {allClasses.Count})");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[PlantUML] Ошибка записи файла: {ex.Message}");
        }
    }
}
