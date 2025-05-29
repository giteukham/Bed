#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

public static class AudioKeyGenerator
{
    private const string OutputPath = "Assets/Feature Audio/Scripts/AudioKeys.cs";    // 생성될 파일 경로
    private const string ClassName = "AudioKeys";   // 생성될 클래스 이름

    [MenuItem("Tools/Audio/Generate AudioKeys")]
    public static void Generate()
    {
        var library = Resources.Load<AudioLibrary>("Audio/AudioLibrary"); // AudioLibrary 불러오기
        if (library == null)
        {
            Debug.LogError("AudioLibrary을 Resources/Audio/ 에 생성해주세요.");
            return;
        }

        List<string> keys = library.GetAllKeys();   // AudioLibrary에서 모든 키 가져오기
        StringBuilder sb = new StringBuilder(); // StrngBuilder 사용

        // 클래스 작성 시작
        sb.AppendLine("// AudioKeyGenerator로 자동 생성됨.");
        sb.AppendLine("public static class " + ClassName);
        sb.AppendLine("{");

        foreach (var key in keys)
        {
            string sanitized = SpacingRemove(key);
            sb.AppendLine($"\tpublic const string {sanitized} = \"{key}\";");
        }

        sb.AppendLine("}");
        // 클래스 작성 끝

        Directory.CreateDirectory(Path.GetDirectoryName(OutputPath)); // 디렉토리 생성
        File.WriteAllText(OutputPath, sb.ToString()); // 파일에 작성
        AssetDatabase.Refresh(); // 에셋 데이터베이스 새로고침

        Debug.Log($"{ClassName} 생성 완료: {keys.Count} keys");
    }

    // 공백 제거
    private static string SpacingRemove(string _key)
    {
        string key = _key.Replace(" ", "");
        return key;
    }
}
#endif
