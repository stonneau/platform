using UnityEngine;
using UnityEditor;
using SpeedFSM;
using SpeedFSM.GUI;

public class CreateFSM
{
	[MenuItem("Assets/Create/FSM")]
	public static void CreateAsset()
	{
		CustomAssetUtility.CreateAsset<StateMachine>();
		/*StateMachineEditorWindow window = (StateMachineEditorWindow) EditorWindow.GetWindow(typeof(StateMachineEditorWindow));
		StateMachine machine = StateMachine.CreateInstance<StateMachine>();
		window.Init(machine);
		//CustomAssetUtility.CreateAsset<StateMachine>();*/
	}
}