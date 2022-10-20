using UnityEditor;

[CustomEditor(typeof(RacingGame), editorForChildClasses: true)]
public class RacingGameEditor : Editor
{
    private RacingGame currentRacingGameData;

    private void OnEnable()
    {
        currentRacingGameData = (RacingGame)target;
    }

    public override void OnInspectorGUI()
    {
        RaceDataControls.DrawRacingGameUI(ref currentRacingGameData, currentRacingGameData.GetType());
    }
}
