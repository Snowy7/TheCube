using UnityEditor;

namespace SnowyEditor.Gui
{
    public static class SnEditorUtils
    {
        // Get all subclasses of the specified type
        public static System.Type[] GetSubclasses<T>()
        {
            System.Type parentType = typeof(T);
            System.Type[] types = System.Reflection.Assembly.GetAssembly(parentType).GetTypes();
            System.Collections.Generic.List<System.Type> subclasses = new System.Collections.Generic.List<System.Type>();
            
            foreach (System.Type type in types)
            {
                if (type.IsSubclassOf(parentType) && !type.IsAbstract)
                {
                    subclasses.Add(type);
                }
            }
            
            return subclasses.ToArray();
        }
    }
}