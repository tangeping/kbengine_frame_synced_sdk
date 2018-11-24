using UnityEngine;
using UnityEditor;
using KBEngine.Physics3D;

namespace KBEngine {

    /**
    *  @brief Custom editor to {@link FPRigidBody}.
    **/
    [CustomEditor(typeof(FPRigidBody))]
    [CanEditMultipleObjects]
    public class FPRigidBodyEditor : Editor {

        private bool constraintsFoldout;

        public override void OnInspectorGUI() {
            FPRigidBody tsRB = (target as FPRigidBody);

            DrawDefaultInspector();

            serializedObject.Update();

            constraintsFoldout = EditorGUILayout.Foldout(constraintsFoldout, "Constraints");

            if (constraintsFoldout) {
                EditorGUI.indentLevel++;

                FPRigidBodyConstraints freezeConstraintPos = tsRB.constraints, freezeConstraintRot = tsRB.constraints;

                DrawFreezePanel(ref freezeConstraintPos, true);
                DrawFreezePanel(ref freezeConstraintRot, false);

                tsRB.constraints = (freezeConstraintPos | freezeConstraintRot);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed) {
                EditorUtility.SetDirty(target);
            }
        }

        private static void DrawFreezePanel(ref FPRigidBodyConstraints freezeConstraint, bool position) {
            FPRigidBodyConstraints axisX = position ? FPRigidBodyConstraints.FreezePositionX : FPRigidBodyConstraints.FreezeRotationX;
            FPRigidBodyConstraints axisY = position ? FPRigidBodyConstraints.FreezePositionY : FPRigidBodyConstraints.FreezeRotationY;
            FPRigidBodyConstraints axisZ = position ? FPRigidBodyConstraints.FreezePositionZ : FPRigidBodyConstraints.FreezeRotationZ;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(position ? "Freeze Position" : "Freeze Rotation");

            Rect controlRect = GUILayoutUtility.GetLastRect();
            controlRect.width = 30;
            controlRect.x += EditorGUIUtility.labelWidth;

            bool fX = GUI.Toggle(controlRect, CheckAxis(freezeConstraint, axisX), "X");

            controlRect.x += 30;
            bool fY = GUI.Toggle(controlRect, CheckAxis(freezeConstraint, axisY), "Y");

            controlRect.x += 30;
            bool fZ = GUI.Toggle(controlRect, CheckAxis(freezeConstraint, axisZ), "Z");

            freezeConstraint = FPRigidBodyConstraints.None;

            if (fX) {
                freezeConstraint |= axisX;
            }

            if (fY) {
                freezeConstraint |= axisY;
            }

            if (fZ) {
                freezeConstraint |= axisZ;
            }

            EditorGUILayout.EndHorizontal();
        }

        private static bool CheckAxis(FPRigidBodyConstraints toCheck, FPRigidBodyConstraints axis) {
            return (toCheck & axis) == axis;
        }

    }

}