using System.IO;
using UnityEngine;


namespace ADG.DebugTools
{

    public sealed class Screenshot : MonoBehaviour
    {
        public KeyCode Button = KeyCode.F12;
        public string Directory = "Assets/";
        public string Name = "Screenshot";

        void Update()
        {
            if (Input.GetKeyDown(Button))
                Shoot(Directory, Name);
        }

        public static void Shoot(string directory, string name)
        {
            var count = 0;
            var info = new DirectoryInfo(directory);
            if (info.Exists)
            {
                var files = info.GetFiles(name + "*");
                for (var i = 0; i < files.Length; i++)
                {
                    var file = files[i];
                    var fileName = file.Name;
                    if (fileName.Length != name.Length + 8)
                        continue;

                    var fileNumber = fileName.Substring(name.Length + 1, 3);
                    if (int.TryParse(fileNumber, out var number))
                    {
                        number++;
                        if (number > count)
                            count = number;
                    }
                }
            }
            else info.Create();

            var path = $"{directory}/{name}_{count:000}.png";
            ScreenCapture.CaptureScreenshot(path);
            Debug.Log("<color=blue>Screenshot taken:</color> " + path);

            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
            # endif
        }
    }
}