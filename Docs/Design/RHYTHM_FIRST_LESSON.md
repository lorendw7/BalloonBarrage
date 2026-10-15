# Rhythm lesson 1: music switching and a song-owned round clock

Updated: October 14, 2026. The learner has saved this implementation: assigned interface/game music sources, switching, scheduled playback, clip-length duration and DSP-clock countdown. The fixed-duration field is removed. Keep these steps as reference; do not repeat edits or add duplicate sources. See [Validation](../Project/VALIDATION.md) for executed checks and manual limits.

The prepared game song is about 123.429 seconds, non-looping, preloaded, 2D and pitch 1. This lesson uses scheduled playback and one audio-clock origin for the countdown and settlement. It replaces the earlier provisional frame-countdown version. Chart-driven targets and judgement follow. Scope/provenance live in [Game design](GAME_DESIGN.md) and [Music and charts](MUSIC_AND_CHARTS.md).

## 1. Replace the fixed-duration fields

In RoundController replace the two-line serialized roundDuration declaration with:

```csharp
private float songDuration;
private double songStartDspTime;
```

Keep remainingTime and both assigned music fields. songDuration caches the selected clip length; songStartDspTime records its scheduled start on Unity's audio timeline.

## 2. Initialize the menu and song duration

In Start replace remainingTime = roundDuration with:

```csharp
songDuration = gameMusic != null && gameMusic.clip != null
    ? gameMusic.clip.length
    : 0f;
remainingTime = songDuration;
```

At the end of Start, before its closing brace, add:

```csharp
if (gameMusic != null)
{
    gameMusic.Stop();
}

if (menuMusic != null)
{
    menuMusic.Stop();
    menuMusic.Play();
}
```

Retain all existing state/panel resets.

## 3. Replace BeginRound

Replace only the existing BeginRound method with:

```csharp
public void BeginRound()
{
    if (hasStarted)
    {
        return;
    }

    if (gameMusic == null || gameMusic.clip == null ||
        gameMusic.clip.loadState != AudioDataLoadState.Loaded)
    {
        Debug.LogWarning("Game music is missing or not loaded yet.");
        return;
    }

    Score.ResetScore();
    songDuration = gameMusic.clip.length;
    remainingTime = songDuration;
    hasStarted = true;
    IsPlaying = true;

    if (startScreen != null)
    {
        startScreen.SetActive(false);
    }

    if (menuMusic != null)
    {
        menuMusic.Stop();
    }

    gameMusic.Stop();
    songStartDspTime = AudioSettings.dspTime + 0.2d;
    gameMusic.PlayScheduled(songStartDspTime);
}
```

The prepared clip preloads synchronously. The guard keeps Start available when its reference/data is unavailable. The 0.2-second preparation lead is included in the recorded origin, so it does not consume song duration. During this lead the current play flag is already true; future chart cues and judgement must explicitly reject pre-origin time.

## 4. Replace only the countdown calculation in Update

Keep the existing play/pause guards, Esc handling, remainingTime > 0 return and EndRound call. Replace the old remainingTime minus Time.deltaTime calculation with:

```csharp
float elapsed = Mathf.Max(
    0f, (float)(AudioSettings.dspTime - songStartDspTime));
remainingTime = Mathf.Max(0f, songDuration - elapsed);
```

The same scheduled origin controls music and timer. Zero-clamping holds the full duration during the lead. Unity freezes DSP time under AudioListener.pause, so the existing TogglePause freezes both. EndRound runs on the first gameplay update at or after the complete song duration; normal non-looping audio ends naturally at the clip boundary. This is frame-bound settlement, not a promise of a sample-exact UI transition.

## 5. Return to interface music on settlement

In EndRound immediately after finalScore = Score.Value add:

```csharp
if (gameMusic != null)
{
    gameMusic.Stop();
}

if (menuMusic != null)
{
    menuMusic.Play();
}
```

Keep results text/panel updates and target/projectile cleanup. Retain AudioListener.pause in TogglePause. Resume must not call Play or PlayScheduled: that would change the song origin. Retry already reloads the scene, restoring the menu and full song duration.

## Inspector and acceptance

Exit Play Mode before saving. On scene StudioBackgroundMusic turn Play On Awake off and leave Loop on. GameMusic_GoinUnder keeps Play On Awake off, Loop off, Pitch 1, Spatial Blend 0 and ignoreListenerPause false. Leave the two saved controller references assigned. Save code and scene.

- Title plays only studio interface music.
- Start stops that loop; after a short lead Goin' Under plays from its beginning. The existing ceil-based HUD initially reads about 02:04.
- Pause at about ten seconds: music/countdown hold. Resume continues the same position.
- Listen to the complete song, including its intro/ending; results follow its end and interface music returns. No 90-second early cutoff.
- Play Again returns to the title; a new Start resets song and timer.
- A missing/unloaded game clip prevents starting and leaves a readable Console message.

Record these as manual checks after saving. This guide is not an executed test. [PlayScheduled](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AudioSource.PlayScheduled.html) schedules against DSP time with preparation lead; [AudioListener.pause](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AudioListener-pause.html) also freezes that clock.

## Next: chart events

Continue with [reading the prepared chart](RHYTHM_CHART_LESSON.md). Then reuse songStartDspTime as the event origin, preview/produce targets at its event times, and carry event/player identity through impact. The first event is about 9.143 seconds; do not invent scoring notes during the intro. Chart scheduling/judgement remains unimplemented.
