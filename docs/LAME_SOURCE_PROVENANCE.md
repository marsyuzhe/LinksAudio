# LAME source and binary provenance

LAME 3.100 upstream source archive SHA-256:
`ddfe36cab873794038ae2c1210557ad34857a4b6bdc515785d1da9e175b1da1e`

Official release reference: https://sourceforge.net/projects/lame/files/lame/3.100/lame-3.100.tar.gz/download
The original local archive was obtained from the MacPorts source mirror and matched that hash.

For this publication, `testcase.wav` and `testcase.mp3` were omitted from the redistributed archive. Neither file participates in library compilation. All source modules, headers, build scripts and license files remain included, with no algorithm source edits. This avoids carrying any recorded-media example through the source distribution.

Redistributed corresponding-source archive (`docs/lame-3.100.tar.gz`) SHA-256:
`a76be4b76c158a7c2c52b0912856ba1bc35ee62f768b16bbe89fa23dc27e3487`

The DLL was rebuilt with Zig 0.13.0 for Windows x64, using the provided `build/lame/config.h` and `build/build-lame.ps1`: `-shared -O2 -s -fwrapv`, C sources under libmp3lame plus vector/xmm_quantize_sub.c, and export-all-symbols. HAVE_MPGLIB is not defined. No FFmpeg or MP3 decoder is compiled into this DLL.

A first build attempt failed because the manual Windows config lacked ieee754_float32_t. The configuration was corrected by declaring the floating-point types; the subsequent build and forced-LAME conversion checks succeeded. This was a build configuration correction, not a change to the encoding algorithm.

The resulting DLL is separate and replaceable. Its exact SHA-256 is recorded in build-info.json / docs/validation/native-manifest.json. Corresponding source, this configuration and build instructions travel with the portable release under licenses/sources/. The app imposes no restriction on reverse engineering for debugging modifications to this LGPL library.

To rebuild from the release-only copy, extract the archive, place lame-config.h as config.h in its lame-3.100 root, and use the source list/flags above with Zig. For the fully automated path, use build/build-lame.ps1 from the repository. Upstream Makefile.MSVC and INSTALL files are also retained for other toolchains.
