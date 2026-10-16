"""Create an original, deterministic trigger-and-paint shot. No recordings or samples.

Run from any directory with Python 3 and NumPy. Never replace the paired metas.
"""
from pathlib import Path
import hashlib
import json
import math
import wave

import numpy as np

ROOT = Path(__file__).resolve().parents[2]
OUTPUT = ROOT / "Assets/Audio/SFX"
RATE = 44100
SEED = 20261016


def main():
    rng = np.random.default_rng(SEED)
    t = np.arange(round(.175 * RATE)) / RATE
    noise = rng.normal(size=t.size)
    # A short, cushioned mechanism click gives each shot a clear time marker.
    click = (np.sin(2 * math.pi * 1750 * t) + .48 * np.sin(2 * math.pi * 2870 * t))
    click *= (1 - np.exp(-t / .00035)) * np.exp(-t / .0038) * .18
    # Air has its own body and a 2.2 kHz pressure resonance rather than only hiss.
    smooth = np.convolve(noise, np.ones(5) / 5, mode="same")
    low = np.convolve(smooth, np.ones(33) / 33, mode="same")
    air = (smooth - low) * (1 - np.exp(-t / .0015)) * np.exp(-t / .038)
    # Three overlapping sinusoidal resonances give the expelled paint a wet thump.
    phase = 2 * math.pi * (170 * t + 260 * .012 * (1 - np.exp(-t / .012)))
    wet = (np.sin(phase) + .28 * np.sin(phase * 2.03))
    wet *= (1 - np.exp(-t / .0009)) * np.exp(-t / .024) * .25
    pressure = np.sin(2 * math.pi * (2200 * t - 4500 * t * t))
    pressure *= (1 - np.exp(-t / .0008)) * np.exp(-t / .009) * .065
    drops = np.zeros_like(t)
    for start, frequency, gain in ((.022, 1170, .05), (.044, 840, .035), (.068, 970, .022)):
        dt = np.maximum(t - start, 0)
        phase_drop = 2 * math.pi * frequency * .008 * (1 - np.exp(-dt / .008))
        drops += np.sin(phase_drop) * (dt > 0) * np.exp(-dt / .007) * gain
    signal = click + air * .62 + wet + pressure + drops
    signal -= signal.mean()
    signal *= np.minimum(t / .0006, 1) * np.minimum((t[-1] - t) / .022, 1)
    # Gentle saturation adds body while retaining headroom for the music bus.
    signal /= np.max(np.abs(signal))
    signal = np.tanh(signal * 3.40)
    signal *= .78 / np.max(np.abs(signal))
    pcm = np.round(signal * 32767).astype("<i2")
    OUTPUT.mkdir(parents=True, exist_ok=True)
    path = OUTPUT / "PaintShot_AirPuff.wav"
    with wave.open(str(path), "wb") as wav:
        wav.setnchannels(1)
        wav.setsampwidth(2)
        wav.setframerate(RATE)
        wav.writeframes(pcm.tobytes())
    metadata = {
        "asset": path.name,
        "origin": "Original mathematical synthesis for BalloonBarrage; no external audio, samples or AI audio generation.",
        "generator": "Tools/Audio/synthesize_paint_shot.py",
        "seed": SEED,
        "format": "mono PCM16 WAV, 44100 Hz",
        "duration_seconds": len(pcm) / RATE,
        "sha256": hashlib.sha256(path.read_bytes()).hexdigest(),
        "peak_dbfs": round(20 * math.log10(np.max(np.abs(pcm.astype(float))) / 32768), 2),
        "rms_dbfs": round(20 * math.log10(np.sqrt(np.mean((pcm.astype(float) / 32768) ** 2))), 2),
        "revision": 3,
        "processing": "Original mechanism click, descending wet body, band-limited pressure/air and three liquid transients. Tanh drive 3.40 after unit-peak normalization; final peak 0.78; 0.6 ms attack and 22 ms end fade.",
        "prior_revisions": [
            {"revision": 1, "sha256": "2c6bd8ebbfdd682b3ab0c3f57c4d6a000738e4a84d45e46e664de702cbf98c3f", "rms_dbfs": -20.97, "scene_volume": .38},
            {"revision": 2, "sha256": "1378d5eea215d3430f15ba0cf5224f06ae0d7a73e463c719a0752ad6d842772d", "rms_dbfs": -14.99, "scene_volume": .50}
        ],
        "revision_reason": "Creator requested a clearer, more distinct firing sound after the louder air-puff revision.",
        "scene_volume": .50,
        "notes": "175 ms, shorter than the current 200 ms shot interval. Fixed source pitch for rhythm clarity. Same Unity GUID/reference. No song/model license is changed."
    }
    (OUTPUT / "PaintShot_Provenance.json").write_text(json.dumps(metadata, indent=2) + "\n", encoding="utf-8")
    assert pcm[0] == pcm[-1] == 0 and np.max(np.abs(pcm.astype(int))) < 32767
    print(json.dumps(metadata, indent=2))


if __name__ == "__main__":
    main()
