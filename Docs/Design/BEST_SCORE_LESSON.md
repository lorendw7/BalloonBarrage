# Lesson: local solo best score

Updated: October 17, 2026. [Roadmap](ROADMAP.md) owns delivery; [game design](GAME_DESIGN.md) defines record identity and acceptance. Reading, higher-only saving and results display are saved. The creator reports saving/retry reading and highest-score display passed. Source/reference review confirms the saved scene connects bestScoreText to BestScoreValue. Independent method-driven standalone checks now pass higher-score saving, scene-retry reading, process-relaunch reading, lower-score retention and six-digit display using isolated synthetic scores. See [Validation](../Project/VALIDATION.md#independent-standalone-diagnostic-october-17) for evidence and limits; older unreported acceptance statements below describe the earlier checkpoint. These additions are not in public v0.1.0-demo.1.

## First increment: read the record

In RoundController, beneath `private int finalScore;`, add:

```csharp
private const string BestScoreKey = "BestScore.GoinUnder.Beginner.Solo.v1";
private int bestScore;
```

The constant identifies a saved record for this song/chart/solo scoring version. It is the entry name, not the score. `bestScore` holds the integer read for the current scene instance.

In `Start()`, immediately after its existing `Score.ResetScore();`, add:

```csharp
bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
Debug.Log($"Best score loaded: {bestScore}");
```

PlayerPrefs.GetInt reads the integer stored under that name. The second argument is the fallback when no record exists. A first run should print `Best score loaded: 0`. This first increment explains reading only. The learner has subsequently added higher-only saving during settlement; that saved state is described below. UnityEngine is already imported.

## Saved comparison and persistence

EndRound captures `finalScore`, then compares it with `bestScore`. Only a higher result updates the variable, calls `PlayerPrefs.SetInt(BestScoreKey, bestScore)` and explicitly saves. The creator reports saving and retry reading passed. Source review confirms the comparison is before showing results and uses the same key as Start. Quit/relaunch persistence and retaining a higher record after a lower completed round remain separate acceptance checks unless explicitly reported.

## Implemented results text

The assistant prepared `ResultsScreen/Dialog/BestScoreRow`, between FinalScorePlaque and PlayAgainButton. It has a static **BEST SCORE** caption and dynamic **BestScoreValue** Text, centered row position (0, -64), size 350 × 26 and unit scale. The value uses the existing dynamic Lato Bold font at size 21, right alignment and Raycast Target off. Its saved `------` is a placeholder, not a loaded record. Original UI object ids, positions, button events and gameplay fields were preserved.

In RoundController, directly beneath the existing `private Text finalScoreText;`, the learner adds:

```csharp
[SerializeField]
private Text bestScoreText;
```

UnityEngine.UI is already imported. SerializeField exposes the private Text reference. In PrototypeScene select the **RoundController** object, then drag **ResultsScreen > Dialog > BestScoreRow > BestScoreValue** into its **Best Score Text** field. This is a reference hookup; runtime assignment is a separate next small increment.

The learner has added the null-guarded `bestScoreText.text = bestScore.ToString("D6");` in EndRound, after the higher-score comparison and before activating the results screen. The saved scene reference resolves to the prepared BestScoreValue Text. The creator reports display passed; this is not an independent Unity runtime test. No gameplay field or assignment was inserted by UI preparation.

## UI verification and remaining acceptance

The isolated Unity check passed text fit and viewport bounds at 1280 × 720, 1920 × 1080, 2560 × 1440, 1280 × 960 and 3440 × 1440. It checked the row does not overlap FinalScorePlaque or PlayAgainButton, both new Text components ignore raycasts, production-scene inheritance works and the saved Play Again callback still targets RoundController.RestartRound. Per-object comparison retained all 47 original prefab objects; ten UI objects were added and only Dialog's child list changed. Learner source and canonical scene hashes were unchanged by UI preparation. It did not invoke gameplay saving or write PlayerPrefs.

[UI preview](../../Assets/StudioArtPack/InterfaceV2/Previews/Results_BestScore_1080p.png) is a native Unity render with illustrative values, not a recorded best score. Logs are ignored under `.local-backups/best-score-ui-20261017`. Current source prefab saves `------`; the screenshot's 001860 is only a layout sample.

After binding, accept a new record, a lower subsequent score, normal retry, and quit/relaunch persistence; verify results show the retained record in six-digit formatting. Retry resets the current score while retaining the stored record.

The learner writes Assets/Scripts gameplay. The assistant prepares UI/assets and reviews each saved small edit.
