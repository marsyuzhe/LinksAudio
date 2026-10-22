# Public, non-speech fixtures

All audio in this directory is generated mathematically by tests/generate-fixture.c (MIT, Links Tam). No microphone, recorded speech, chat export, or third-party recording is used.

synthetic-tone.silk contains 6.560 seconds of 440 Hz and 660 Hz tones with short amplitude fades, encoded at 24 kHz with the included SILK SDK. reference.pcm is the SDK decoding of those same generated packets, signed little-endian 16-bit mono PCM at 24 kHz. It is a same-SDK regression oracle, not an independently captured recording or a proof of every real-world file's compatibility.

The test runner generates standard and Tencent-prefix container variants, deliberately corrupt/empty fixtures, and output files locally. Those derived files are excluded from the public repository. Recreate the two base fixtures with build/generate-fixtures.ps1 and Zig 0.13.0.
