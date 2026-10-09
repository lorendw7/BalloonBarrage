"""Original synthesized music for BalloonBarrage; no sampled recordings.

Run with Python and NumPy. WAV masters live under Assets/Audio/Music.
Looping notes and their delay tails wrap around an exact bar-length buffer.
"""
from pathlib import Path
import json
import math
import wave

import numpy as np

ROOT = Path(__file__).resolve().parents[2]
OUTPUT = ROOT / "Assets" / "Audio" / "Music"
RATE = 44100
RNG = np.random.default_rng(20261009)


def envelope(t, attack, decay):
    return np.minimum(t / attack, 1) * np.exp(-t / decay)


def frequency(midi):
    return 440 * 2 ** ((midi - 69) / 12)


def kick(strength=1):
    t = np.arange(int(.42 * RATE)) / RATE
    phase = 2 * np.pi * (48 * t + 100 * .022 * (1 - np.exp(-t / .022)))
    body = np.sin(phase) * envelope(t, .0015, .105)
    click = RNG.normal(0, .13, t.size) * envelope(t, .0005, .005)
    return np.tanh((body + click * .15) * 1.1) * strength * .58


def snare(strength=1, rim=False):
    t = np.arange(int(.24 * RATE)) / RATE
    noise = RNG.normal(0, 1, t.size)
    noise -= np.convolve(noise, np.ones(18) / 18, mode="same")
    body = np.sin(2 * np.pi * 185 * t) + .35 * np.sin(2 * np.pi * 330 * t)
    if rim:
        return (body * .22 + noise * .12) * envelope(t, .001, .035) * strength
    return (body * .2 * np.exp(-t / .055) + noise * .29 * np.exp(-t / .065)) * np.minimum(t / .001, 1) * strength


def hat(strength=1, open_hat=False):
    duration = .22 if open_hat else .065
    t = np.arange(int(duration * RATE)) / RATE
    noise = RNG.normal(0, 1, t.size)
    noise = np.diff(noise, prepend=noise[0]) / 1.5
    metal = sum(np.sin(2 * np.pi * f * t) for f in (6230, 7919, 10313)) * .08
    brushed = np.convolve(noise, np.ones(10) / 10, mode="same")
    return brushed * envelope(t, .006, .038 if open_hat else .022) * .075 * strength


def bass(midi, beats, bpm):
    duration = beats * 60 / bpm
    t = np.arange(int((duration + .16) * RATE)) / RATE
    f = frequency(midi)
    body = np.sin(2 * np.pi * f * t) + .3 * np.sin(4 * np.pi * f * t) + .1 * np.sin(6 * np.pi * f * t)
    release = np.minimum(np.maximum((duration + .16 - t) / .16, 0), 1)
    return np.tanh(body * .9) * envelope(t, .008, .5) * release * .29


def keys(midi, strength=1, soft=False):
    return felt_piano(midi, 1.8 if soft else 1.4, strength * (.8 if soft else 1))


def felt_piano(midi, duration, strength=1):
    # Three slightly detuned strings, inharmonic partials and a quiet hammer transient.
    # This remains original synthesis, not a recording or sampled piano.
    t = np.arange(round((duration + .35) * RATE)) / RATE
    f = frequency(midi)
    body = np.zeros_like(t)
    for harmonic, weight in enumerate((1, .30, .13, .07, .035, .016), start=1):
        partial = f * harmonic * np.sqrt(1 + .00012 * harmonic * harmonic)
        decay = (1.35 + (72-midi)*.018) / (1 + .40*(harmonic-1))
        strings = sum(np.sin(2*np.pi*partial*ratio*t + .08*harmonic)
                      for ratio in (.9992, 1, 1.0009)) / 3
        body += weight * strings * np.exp(-t / max(decay, .24))
    hammer = RNG.normal(0,1,len(t))
    hammer = np.convolve(hammer,np.ones(24)/24,mode="same") * np.exp(-t/.009) * .022
    attack = 1-np.exp(-t/.008)
    release = np.minimum(np.maximum((duration+.35-t)/.35,0),1)
    return (body+hammer)*attack*release*strength*.19


def warm_pad(notes, seconds):
    t=np.arange(round(seconds*RATE))/RATE
    signal=np.zeros_like(t)
    for note in notes:
        f=frequency(note)
        signal += (np.sin(2*np.pi*f*.9994*t)+np.sin(2*np.pi*f*1.0006*t))*.5
        signal += np.sin(2*np.pi*f*2*t)*.045
    env=np.minimum(t/.22,1)*np.minimum((seconds-t)/.5,1)
    return signal*env*.014


def marimba(midi, strength=1):
    t = np.arange(int(.95 * RATE)) / RATE
    f = frequency(midi)
    body = np.sin(2 * np.pi * f * t) * np.exp(-t / .19)
    body += .35 * np.sin(2 * np.pi * f * 4 * t) * np.exp(-t / .055)
    return body * np.minimum(t / .002, 1) * .22 * strength


def add(buffer, sound, beat, bpm, gain=1, pan=0, loop=True):
    start = round(beat * 60 / bpm * RATE)
    sound = np.asarray(sound, dtype=np.float32)
    stereo = sound[:, None] * np.array([math.sqrt((1 - pan) / 2), math.sqrt((1 + pan) / 2)]) * gain
    if loop:
        start %= len(buffer)
    if start >= len(buffer):
        return
    available = min(len(stereo), len(buffer) - start)
    buffer[start:start + available] += stereo[:available]
    if loop and available < len(stereo):
        remaining = stereo[available:]
        buffer[:len(remaining)] += remaining


def echo(buffer, bpm, gain=.14):
    # Circular delay preserves the tails from the final bar at the loop start.
    amount = round(.75 * 60 / bpm * RATE)
    stereo = np.roll(buffer, amount, axis=0)[:, ::-1] * gain
    return buffer + stereo + np.roll(stereo, amount, axis=0)[:, ::-1] * .38


def track(bpm, bars):
    count = round(bars * 4 * 60 / bpm * RATE)
    return np.zeros((count, 2), dtype=np.float32)


def write(name, signal, bpm, bars, loop):
    signal -= np.mean(signal, axis=0)
    if loop:
        # A 3 ms seam taper removes noise discontinuities without changing tempo.
        seam = round(.003 * RATE)
        fade = np.sin(np.linspace(0, np.pi / 2, seam)) ** 2
        signal[:seam] *= fade[:, None]
        signal[-seam:] *= fade[::-1, None]
    peak = float(np.max(np.abs(signal)))
    target_rms = .045 if "Drums" in name else .085
    signal *= min(.72 / max(peak, 1e-9), target_rms / max(float(np.sqrt(np.mean(signal ** 2))), 1e-9))
    pcm = np.round(signal * 32767).astype("<i2")
    if np.max(np.abs(signal)) > .861:
        raise ValueError("Unexpected clipping")
    output = OUTPUT / f"{name}.wav"
    with wave.open(str(output), "wb") as stream:
        stream.setnchannels(2)
        stream.setsampwidth(2)
        stream.setframerate(RATE)
        stream.writeframes(pcm.tobytes())
    with wave.open(str(output), "rb") as stream:
        assert stream.getnframes() == len(signal)
        assert stream.getnchannels() == 2 and stream.getframerate() == RATE
    return {
        "file": output.relative_to(ROOT).as_posix(), "bpm": bpm, "bars": bars,
        "duration_seconds": round(len(signal) / RATE, 6), "loop": loop,
        "peak_dbfs": round(20 * math.log10(max(float(np.max(np.abs(signal))), 1e-9)), 2),
        "rms_dbfs": round(20 * math.log10(max(float(np.sqrt(np.mean(signal ** 2))), 1e-9)), 2),
        "loop_boundary_step": round(float(np.max(np.abs(signal[0] - signal[-1]))), 6) if loop else None,
        "format": "44.1 kHz / stereo / 16-bit PCM WAV", "source": "original synthesized composition; no recordings or samples",
    }


def anime_lead(midi, beats, bpm, strength=.65, soft=False):
    return felt_piano(midi, max(.8, beats*60/bpm), strength)


def anime_groove():
    # Warm original game-pop: felt-key phrasing with space and restrained percussion.
    bpm, bars = 108, 32
    drums, band, lead = (track(bpm, bars) for _ in range(3))
    chords = [(48,55,59,64), (53,57,60,64), (55,59,62,67), (52,55,59,62),
              (45,52,55,60), (50,53,57,60), (55,59,62,65), (48,55,60,64)]
    roots = [36,41,43,40,33,38,43,36]
    hooks = [
        [(0,76,.5),(.75,79,.25),(1,81,.5),(1.75,79,.25),(2,76,.5),(2.75,74,.25),(3,72,.65)],
        [(0,74,.5),(.75,76,.25),(1,77,.75),(2,76,.5),(2.75,72,.25),(3,69,.65)],
        [(0,74,.5),(.75,79,.25),(1,83,.5),(1.75,81,.25),(2,79,.75),(3,77,.65)],
        [(0,76,.75),(1,79,.5),(1.75,76,.25),(2,74,.5),(2.75,71,.25),(3,72,.65)],
        [(0,72,.5),(.75,76,.25),(1,81,.75),(2,79,.5),(2.75,76,.25),(3,74,.65)],
        [(0,77,.75),(1,76,.5),(1.75,74,.25),(2,72,.5),(2.75,69,.25),(3,74,.65)],
        [(0,71,.5),(.75,74,.25),(1,79,.5),(1.75,81,.25),(2,79,.5),(2.75,74,.25),(3,71,.65)],
        [(0,72,.75),(1,76,.5),(1.75,79,.25),(2,84,1.4)],
    ]
    for bar in range(bars):
        offset, chord = bar * 4, bar % 8
        b_part = 16 <= bar < 24
        for beat in (0, 2):
            add(drums, kick(.34), offset+beat, bpm)
        for beat in (1,3):
            add(drums, snare(.18, rim=True), offset+beat, bpm, pan=-.12)
        for step in range(8):
            add(drums,hat(.18 if step%2 else .24),offset+step*.5,bpm,pan=.20)
        if bar%8==7:
            for step in range(4):
                add(drums,snare(.05+step*.015,rim=True),offset+3+step*.25,bpm,pan=(step-1.5)*.12)
        for beat in (0, 1.5, 2.5):
            note = roots[chord] + (7 if beat==1.5 else 0)
            add(band,bass(note,.9,bpm),offset+beat,bpm,gain=.40)
        for beat in (0,2):
            for index,note in enumerate(chords[chord]):
                timing = float(RNG.uniform(0,.018))
                velocity = float(RNG.uniform(.38,.46))
                add(band,keys(note,velocity,soft=True),offset+beat+index*.025+timing,bpm,pan=(-.35 if index%2 else .35))
        add(band,warm_pad(chords[chord],4*60/bpm+.3),offset,bpm,pan=.05)
        for step in (1,5):
            note=chords[chord][step%4]+12
            add(band,keys(note,.15,soft=True),offset+step*.5,bpm,pan=.35 if step%2 else -.35)
        for index,(beat,note,length) in enumerate(hooks[chord]):
            if index%2: continue
            if b_part:
                note = note-12 if chord%2 else note-5
            add(lead,anime_lead(note,min(length*1.65,1.5),bpm,.55 if b_part else .65,soft=True),offset+beat,bpm,pan=-.08)
            if bar>=24 and beat in (0,2):
                add(lead,anime_lead(note-12,length,bpm,.2,soft=True),offset+beat,bpm,pan=.18)
    return bpm, drums+echo(band,bpm,.18)+echo(lead,bpm,.20), drums


def anime_menu():
    bpm, bars = 84, 8
    band, drums = track(bpm,bars), track(bpm,bars)
    chords=[(60,64,67,71),(57,60,64,67),(53,57,60,64),(55,59,62,65)]
    melody=[(0,76),(1,79),(2,76),(3,72),(4,74),(5,76),(6,72),(7,71)]
    for bar in range(bars):
        offset=bar*4; chord=(bar//2)%4
        for index,note in enumerate(chords[chord]):
            add(band,keys(note,.70,soft=True),offset+index*.12,bpm,pan=(index-1.5)*.15)
        for beat in (0,2):
            add(band,bass(chords[chord][0]-24,1.1,bpm),offset+beat,bpm,gain=.4)
        add(band,anime_lead(melody[bar][1],1.4,bpm,.36,soft=True),offset+.5,bpm)
        add(band,keys(chords[chord][2]+12,.25,soft=True),offset+2.5,bpm,pan=.2)
        for step in range(8):
            add(drums,hat(.12),offset+step*.5,bpm,pan=.2)
        for beat in (1,3):
            add(drums,snare(.15,rim=True),offset+beat,bpm)
        add(drums,kick(.20),offset,bpm)
    return bpm, drums+echo(band,bpm,.14)


def anime_cues():
    bpm=108; start,complete=track(bpm,1),track(bpm,1)
    for beat in range(4):
        add(start,snare(.22 if beat==3 else .13,rim=True),beat,bpm,loop=False)
        add(start,anime_lead(79 if beat==3 else 76,.3,bpm,.30,soft=True),beat,bpm,loop=False)
    for beat,note in ((0,72),(.25,76),(.5,79),(.75,84),(1.25,88)):
        add(complete,anime_lead(note-12,.7,bpm,.5,soft=True),beat,bpm,loop=False)
    for note in (60,64,67,72):
        add(complete,keys(note,.40,soft=True),1.25,bpm,loop=False)
    for cue in (start,complete):
        cue *= np.minimum(np.arange(len(cue))[::-1,None]/(RATE*.08),1)
    return start,complete


def main():
    OUTPUT.mkdir(parents=True, exist_ok=True)
    bpm, full, drums = anime_groove()
    menu_bpm, lobby = anime_menu()
    start, complete = anime_cues()
    records = [
        write("StudioDaylight_Loop", full, bpm, 32, True),
        write("StudioDaylight_Drums_Loop", drums, bpm, 32, True),
        write("StudioDaylight_Menu_Loop", lobby, menu_bpm, 8, True),
        write("RoundComplete_Soft", complete, bpm, 1, False),
        write("RoundStart_Soft", start, bpm, 1, False),
    ]
    (OUTPUT / "MusicManifest.json").write_text(json.dumps({"tracks": records}, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(records, indent=2))


if __name__ == "__main__":
    main()
