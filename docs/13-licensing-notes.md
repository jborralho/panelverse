# 13 — Licensing notes

## Project license
- The application code will be released under a permissive open-source license (TBD, e.g., MIT/Apache-2.0).

## RAR/CBR support
- 7-Zip components are licensed under LGPL with an additional unRAR restriction. If bundling `7z.dll`/`7zz`, include license texts and attributions.
- The official UnRAR library is licensed under the UnRAR License, which imposes restrictions.
- RAR support will be optional and behind a feature flag. Users who do not need RAR can build without it and use `.cbz` exclusively.

Consult legal guidance if distributing binaries with RAR support in certain jurisdictions.
