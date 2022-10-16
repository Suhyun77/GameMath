using System;
using UnityEditor;

namespace EdtiorGUI
{
    internal class ChangeCheckScope
    {
        internal bool changed;

        public static implicit operator ChangeCheckScope(EditorGUI.ChangeCheckScope v)
        {
            throw new NotImplementedException();
        }
    }
}