import asyncio
import argparse
import re
from pathlib import Path
import edge_tts

INPUT_FILE = "words.txt"
OUTPUT_DIR = Path("audio_output")

VOICE = "da-DK-ChristelNeural"  # Or switch to "da-DK-JeppeNeural"
# VOICE = "da-DK-ChristelNeural"      # Danish - Female
# VOICE = "da-DK-JeppeNeural"         # Danish - Male
# VOICE = "fr-FR-DeniseNeural"        # French - Female
# VOICE = "fr-FR-HenriNeural"         # French - Male
# VOICE = "es-ES-ElviraNeural"        # Spanish - Female
# VOICE = "es-ES-AlvaroNeural"        # Spanish - Male
# VOICE = "de-DE-AmalaNeural"         # German - Female
# VOICE = "de-DE-ConradNeural"        # German - Male
# VOICE = "nb-NO-PernilleNeural"      # Norwegian - Female
# VOICE = "nb-NO-FinnNeural"          # Norwegian - Male
# VOICE = "sv-SE-SofieNeural"         # Swedish - Female
# VOICE = "sv-SE-MattiasNeural"       # Swedish - Male
# VOICE = "nl-NL-ColetteNeural"       # Dutch - Female
# VOICE = "nl-NL-MaartenNeural"       # Dutch - Male
# VOICE = "en-GB-SoniaNeural"         # British English - Female
# VOICE = "en-GB-RyanNeural"          # British English - Male
# VOICE = "it-IT-IsabellaNeural"      # Italian - Female
# VOICE = "it-IT-DiegoNeural"         # Italian - Male

RATE = "+0%"                  # Adjust speed if desired, e.g. "-15%" for slower learner audio
CONCURRENCY = 5               # Keep modest to avoid hammering the endpoint


def sanitize_filename(word: str) -> str:
    """Turn Danish words (including æ, ø, å) into safe, readable filenames."""
    cleaned = word.strip().lower()
    cleaned = cleaned.replace("æ", "ae").replace("ø", "oe").replace("å", "aa")
    cleaned = re.sub(r"[^\w\-]+", "_", cleaned).strip("_")
    return cleaned or "word"


async def synthesize_word(
    word: str,
    out_path: Path,
    semaphore: asyncio.Semaphore,
    voice: str = VOICE,
    rate: str = RATE,
) -> None:
    async with semaphore:
        communicate = edge_tts.Communicate(text=word, voice=voice, rate=rate)
        await communicate.save(str(out_path))
        print(f"Saved: {out_path}  <-  '{word}'")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Convert words from a text file to MP3 audio using edge-tts"
    )
    parser.add_argument(
        "--input-file",
        type=str,
        default=INPUT_FILE,
        help=f"Path to input text file (default: {INPUT_FILE})",
    )
    parser.add_argument(
        "--output-dir",
        type=str,
        default=str(OUTPUT_DIR),
        help=f"Output directory for MP3 files (default: {OUTPUT_DIR})",
    )
    parser.add_argument(
        "--voice",
        type=str,
        default=VOICE,
        help=f"TTS voice to use (default: {VOICE})",
    )
    return parser.parse_args()


async def main() -> None:
    args = parse_args()
    input_path = Path(args.input_file)
    output_dir = Path(args.output_dir)
    voice = args.voice
    if not input_path.exists():
        raise FileNotFoundError(f"Could not find '{INPUT_FILE}'. Create it with one word per line.")

    # Read non-empty, non-comment lines with UTF-8 encoding so æ, ø, å load properly
    lines = [
        line.strip()
        for line in input_path.read_text(encoding="utf-8").splitlines()
        if line.strip() and not line.strip().startswith("#")
    ]

    if not lines:
        print("No words found in input file.")
        return

    output_dir.mkdir(parents=True, exist_ok=True)
    semaphore = asyncio.Semaphore(CONCURRENCY)

    # Only generate words that don't already have an MP3 in the output folder
    jobs: list[tuple[str, Path]] = []
    seen_words: set[str] = set()
    claimed: set[str] = set()
    skipped = 0
    for i, word in enumerate(lines, start=1):
        if word in seen_words:
            continue  # exact duplicate line in words.txt
        seen_words.add(word)

        base_name = sanitize_filename(word)
        if (output_dir / f"{base_name}.mp3").exists():
            skipped += 1
            continue

        # Two different entries in this run normalizing to the same filename get a suffix
        name = base_name if base_name not in claimed else f"{base_name}_{i}"
        claimed.add(base_name)
        jobs.append((word, output_dir / f"{name}.mp3"))

    if not jobs:
        print(f"Nothing to do: all {skipped} words already exist in '{output_dir}/'.")
        return

    tasks = [
        synthesize_word(word=word, out_path=out_path, semaphore=semaphore, voice=voice)
        for word, out_path in jobs
    ]
    await asyncio.gather(*tasks)
    print(f"\nDone! Generated {len(jobs)} MP3 files in '{output_dir}/' (skipped {skipped} existing).")


if __name__ == "__main__":
    asyncio.run(main())