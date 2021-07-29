using System;
using UnityEngine;
using UnityEditor;
using System.Text;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine.Networking.PlayerConnection;

public class EditorConnectionExample : EditorWindow
{
    public static readonly Guid kMsgSendEditorToPlayer = new Guid("34d9b47f923142ff847c0d1f8b0554d9");
    public static readonly Guid kMsgSendPlayerToEditor = new Guid("12871ffeaf0c489189579946d8e0840f");

    [MenuItem("Test/EditorConnectionExample")]
    static void Init()
    {
        EditorConnectionExample window = (EditorConnectionExample)EditorWindow.GetWindow(typeof(EditorConnectionExample));
        window.Show();
        window.titleContent = new GUIContent("EditorConnectionExample");
    }

    void OnEnable()
    {
        EditorConnection.instance.Initialize();
        EditorConnection.instance.Register(kMsgSendPlayerToEditor, OnMessageEvent);
    }

    void OnDisable()
    {
        EditorConnection.instance.Unregister(kMsgSendPlayerToEditor, OnMessageEvent);
        EditorConnection.instance.DisconnectAll();
    }

    private void OnMessageEvent(MessageEventArgs args)
    {
        var text = Encoding.ASCII.GetString(args.data);
        Debug.Log("Message from player: " + text);
    }

    void OnGUI()
    {
        var playerCount = EditorConnection.instance.ConnectedPlayers.Count;
        StringBuilder builder = new StringBuilder();
        builder.AppendLine(string.Format("{0} players connected.", playerCount));
        int i = 0;
        foreach (var p in EditorConnection.instance.ConnectedPlayers)
        {
            builder.AppendLine(string.Format("[{0}] - {1}", i++, p.PlayerId));
        }
        EditorGUILayout.HelpBox(builder.ToString(), MessageType.Info);

        if (GUILayout.Button("Send message to player"))
        {
            EditorConnection.instance.Send(kMsgSendEditorToPlayer, Encoding.ASCII.GetBytes("Hello from Editor"));
        }
    }
}