using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ScriptableObjectComparer : EditorWindow
{
    bool rowsEstablished;

    [MenuItem("Window/Custom Windows/ScriptableObjectComparer")]
    public static void ShowObjectComparer()
    {
        ScriptableObjectComparer wnd = CreateWindow<ScriptableObjectComparer>("ScriptableObjectComparer");
        wnd.titleContent = new GUIContent("ScriptableObjectComparer");
        wnd.Show();
    }

    //[MenuItem("Window/Custom Windows/ScriptableObjectComparer2")]
    //public static void ShowSecondObjectComparer()
    //{
    //    ScriptableObjectComparer wnd = CreateWindow<ScriptableObjectComparer>("ScriptableObjectComparer2");
    //    wnd.titleContent = new GUIContent("ScriptableObjectComparer2");
    //}

    public void CreateGUI()
    {
        rowsEstablished = false;

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        ObjectField objectField = new ObjectField();
        objectField.objectType = typeof(ScriptableObject);
        objectField.label = "Scriptable 1";
        objectField.name = "Scriptable";
        root.Add(objectField);

        Button b = new Button();
        b.text = "Add";
        b.clicked += AddScriptable;
        root.Add(b);

        Box gridContainer = new Box();
        gridContainer.style.flexDirection = FlexDirection.Row;
        gridContainer.name = "GridContainer";
        root.Add(gridContainer);
    }

    private void AddScriptable()
    {
        VisualElement root = rootVisualElement;

        ObjectField f = (ObjectField) root.Q("Scriptable");
        ScriptableObject scriptablesToCompare = f.value as ScriptableObject;

        var fieldValues = scriptablesToCompare.GetType().GetFields();

        // Set up rows
        Box parentContainer = new Box();
        parentContainer.style.flexDirection = FlexDirection.Column;

        Box titleContainer = new Box();
        titleContainer.style.flexDirection = FlexDirection.Row;
        if (!rowsEstablished)
        {
            Label spacer = new Label();
            spacer.style.width = 160;
            titleContainer.Add(spacer);
        }
        Label scriptableName = new Label(scriptablesToCompare.name);
        scriptableName.style.width = 100;
        titleContainer.Add(scriptableName);
        parentContainer.Add(titleContainer);

        foreach (FieldInfo field in fieldValues)
        {
            Box theContainer = new Box();
            theContainer.style.flexDirection = FlexDirection.Row;

            if (!rowsEstablished)
            {
                VisualElement label = new Label(field.Name);
                label.style.width = 160;
                label.style.borderRightColor = Color.black;
                label.style.borderRightWidth = 2;
                label.style.borderBottomColor = Color.black;
                label.style.borderBottomWidth = 2;

                theContainer.Add(label);
            }

            VisualElement inputField;
            var value = field.GetValue(scriptablesToCompare);

            if (value is string strVal)
            {
                inputField = new TextField();
                (inputField as TextField).value = strVal;
                inputField.AddToClassList(".unity-base-field__aligned");
                inputField.RegisterCallback<ChangeEvent<string>>((evt) =>
                {
                    
                    field.SetValue(scriptablesToCompare, evt.newValue);
                });
            }
            else if (value is float floatVal)
            {
                inputField = new TextField();
                (inputField as TextField).value = floatVal.ToString();
                inputField.AddToClassList(".unity-base-field__aligned");
                inputField.RegisterCallback<ChangeEvent<string>>((evt) =>
                {
                    field.SetValue(scriptablesToCompare, float.Parse(evt.newValue));
                });
            }
            else if (value is int intVal)
            {
                inputField = new TextField();
                (inputField as TextField).value = intVal.ToString();
                inputField.AddToClassList(".unity-base-field__aligned");
                inputField.RegisterCallback<ChangeEvent<string>>((evt) =>
                {
                    field.SetValue(scriptablesToCompare, int.Parse(evt.newValue));
                });
            }
            else if (value is bool boolVal)
            {
                inputField = new Toggle();
                (inputField as Toggle).value = boolVal;

                inputField.RegisterCallback<ChangeEvent<bool>>((evt) =>
                {
                    field.SetValue(scriptablesToCompare, evt.newValue);
                });

                //newTextField.AddToClassList(".unity-base-field__aligned");
            }
            else if (value is ScriptableObject so)
            {
                inputField = new ObjectField();
                (inputField as ObjectField).value = so;
                inputField.RegisterCallback<ChangeEvent<ScriptableObject>>((evt) =>
                {
                    field.SetValue(scriptablesToCompare, evt.newValue);
                });
            }
            else if (value is Sprite sprite)
            {
                inputField = new ObjectField();
                (inputField as ObjectField).value = sprite;
                inputField.RegisterCallback<ChangeEvent<Sprite>>((evt) =>
                {
                    field.SetValue(scriptablesToCompare, evt.newValue);
                });
            }
            else if (value is AudioClip audioClip)
            {
                inputField = new ObjectField();
                (inputField as ObjectField).value = audioClip;
                inputField.RegisterCallback<ChangeEvent<AudioClip>>((evt) =>
                {
                    field.SetValue(scriptablesToCompare, evt.newValue);
                });
            }
            else
            {
                inputField = new TextField("NotValid");
            }

            inputField.style.width = 100;
            inputField.style.borderRightColor = Color.black;
            inputField.style.borderRightWidth = 2;
            //newTextField.style.borderBottomColor = Color.black;
            //newTextField.style.borderBottomWidth = 2;

            

            theContainer.Add(inputField);

            parentContainer.Add(theContainer);

            //VisualElement horizLine = new Label();
            //horizLine.style.height = 2;
            //horizLine.style.backgroundColor = Color.black;

            //root.Add(horizLine);

            //root.Add(horizLine);
        }

        rowsEstablished = true;

        Box gridParent = (Box)root.Q("GridContainer");
        gridParent.Add(parentContainer);
        //root.Add(theContainer);

        //root.Add(labelFromUXML);
    }
}
