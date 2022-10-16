using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// Editor 폴더 내의 스크립트 컴포넌트는 프로젝트 빌드 시 가장 마지막으로 컴파일되므로 그때까지 컴파일된 모든 컴포넌트를 모두 참조할 수 있다.
// Editor 폴더 밖의 스크립트가  Editor 안의 컴포넌트를 참조할 수는 없다.

// 유니티 간이 계산기

[CustomEditor(typeof(Chapter04))]  // typeof(커스터마이즈 대상 클래스) 
public class Chapter04Editor : Editor
{
        Matrix4x4 matrix = new Matrix4x4();

        float determinant3x3;
        float determinant4x4;

        Vector4 rhs;
        Vector4 result;

        public override void OnInspectorGUI()
        {
                //base.OnInspectorGUI();

                serializedObject.Update();
                DrawPropertiesExcluding(serializedObject, new string[] { "m_Script" });
                serializedObject.ApplyModifiedProperties();

                // Chapter04 obj = target as Chapter04;

                // EditorGUI.EndChangeCheck가 호출될 때까지 그리는 요소가 변화하면 true값 반환(행렬 요소가 변화할 때 행렬식 값 갱신)
                EditorGUI.BeginChangeCheck(); 

                // GUI요소 세로 방향으로 그룹화
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField(new GUIContent("Matrix4x4"));
                // Matrix4x4의 내용을 GetRow 메서드로 한 줄씩 가져와 RowVector4Field 메서드에 설정
                matrix.SetRow(0, RowVector4Field(matrix.GetRow(0)));
                matrix.SetRow(1, RowVector4Field(matrix.GetRow(0)));
                matrix.SetRow(2, RowVector4Field(matrix.GetRow(0)));
                matrix.SetRow(3, RowVector4Field(matrix.GetRow(0)));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                determinant3x3 = EditorGUILayout.FloatField("Determinant (3x3)", determinant3x3);
                determinant4x4 = EditorGUILayout.FloatField("Determinant (4x4)", determinant4x4);
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                rhs = EditorGUILayout.Vector4Field("RHS", rhs);
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();

                // 각 버튼이 눌렸을 때의 처리
                if (GUILayout.Button("operator *"))
                {
                        result = matrix * rhs;
                }
                if (GUILayout.Button("MultiplyPoint"))
                {
                        result = matrix.MultiplyPoint(rhs);
                }
                if (GUILayout.Button("MultiplyPoint3x4"))
                {
                        result = matrix.MultiplyPoint3x4(rhs);
                }
                if (GUILayout.Button("MultiplyVector"))
                {
                        result = matrix.MultiplyVector(rhs);
                }

                // 결과
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.Vector4Field("Result", result);
                EditorGUILayout.EndVertical();

                if (EditorGUI.EndChangeCheck())
                {
                        determinant3x3 = getDeterminant3x3(matrix);
                        determinant4x4 = getDeterminant4x4(matrix);

                        Undo.RecordObject(target, "Chapter04EditorUndo");
                        EditorUtility.SetDirty(target);
                }
        }

        // 계산 코드
        public float getDeterminant3x3(Matrix4x4 m)
        {
                return m.m00 * m.m11 * m.m22 - m.m00 * m.m12 * m.m21 - m.m01 * m.m10 * m.m22
                        + m.m01 * m.m12 * m.m20 + m.m02 * m.m10 * m.m21 - m.m02 * m.m11 * m.m20
                ;
        }
        public float getDeterminant4x4(Matrix4x4 m)
        {
                return m.m03 * m.m12 * m.m21 * m.m30 - m.m02 * m.m13 * m.m21 * m.m30 - m.m03 * m.m11 * m.m22 * m.m30
                        + m.m01 * m.m13 * m.m22 * m.m30 + m.m02 * m.m11 * m.m23 * m.m30 - m.m01 * m.m12 * m.m23 * m.m30
                        - m.m03 * m.m12 * m.m20 * m.m31 + m.m02 * m.m13 * m.m20 * m.m31 + m.m03 * m.m10 * m.m22 * m.m31
                        - m.m00 * m.m13 * m.m22 * m.m31 - m.m02 * m.m10 * m.m23 * m.m31 + m.m00 * m.m12 * m.m23 * m.m31
                        + m.m03 * m.m11 * m.m20 * m.m32 - m.m01 * m.m13 * m.m20 * m.m32 - m.m03 * m.m10 * m.m21 * m.m32
                        + m.m00 * m.m13 * m.m21 * m.m32 + m.m01 * m.m10 * m.m23 * m.m32 - m.m00 * m.m11 * m.m23 * m.m32
                        - m.m02 * m.m11 * m.m20 * m.m33 + m.m01 * m.m12 * m.m20 * m.m33 + m.m02 * m.m10 * m.m21 * m.m33
                        - m.m00 * m.m12 * m.m21 * m.m33 - m.m01 * m.m10 * m.m22 * m.m33 + m.m00 * m.m11 * m.m22 * m.m33
                ;
        }
        public static Vector4 RowVector4Field(Vector4 value, params GUILayoutOption[] options)
        {
                Rect position = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.numberField, options);
                float[] values = new float[] { value.x, value.y, value.z, value.w };

                EditorGUI.BeginChangeCheck();

                EditorGUI.MultiFloatField(
                        position,
                        new GUIContent[] { new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent() },
                        values
                );

                if (EditorGUI.EndChangeCheck())
                {
                        value.Set(values[0], values[1], values[2], values[3]);
                }

                return value;
        }
}
