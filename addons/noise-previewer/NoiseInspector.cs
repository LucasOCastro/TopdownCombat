using Godot;

namespace NoisePreviewer
{
    [Tool]
    public class NoiseInspector : EditorInspectorPlugin
    {
        private static string[] GetAllPropertyNames(Object @object)
        {
            var propertyList = @object.GetPropertyList();
            string[] allProperties = new string[propertyList.Count];
            for (int i = 0; i < propertyList.Count; i++){
                Godot.Collections.Dictionary dictionary = propertyList[i] as Godot.Collections.Dictionary;
                allProperties[i] = (string)dictionary["name"];
            }
            return allProperties;
        }

        public override bool CanHandle(Object @object)
        {
            return @object is OpenSimplexNoise;
        }

        public override void ParseBegin(Object @object)
        {
            NoiseEditor _noiseEditor = new NoiseEditor() {noise = @object as OpenSimplexNoise};
            AddPropertyEditorForMultipleProperties("", GetAllPropertyNames(@object), _noiseEditor);
        }
    }
}