REM python WordsToMp3.py --input-file numbers.txt --output-dir ./audio/fr/numbers --voice "fr-FR-DeniseNeural"
REM python WordsToMp3.py --input-file numbers.txt --output-dir ./audio/es/numbers --voice "es-ES-ElviraNeural"
REM python WordsToMp3.py --input-file numbers.txt --output-dir ./audio/de/numbers --voice "de-DE-AmalaNeural"
REM python WordsToMp3.py --input-file numbers.txt --output-dir ./audio/ch/numbers --voice "de-CH-LeniNeural"
python WordsToMp3.py --input-file numbers.txt --output-dir ./audio/ch/numbers --voice "de-CH-LeniNeural" --rate "+50" --overwrite
pause
