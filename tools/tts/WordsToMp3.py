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


def normalize_rate(value: str) -> str:
    """Accept +10%, +10, 10, -10 and normalize to edge-tts format (+10%/-10%)."""
    match = re.fullmatch(r"([+-]?)(\d+)%?", value.strip())
    if not match:
        raise ValueError("--rate must be like +10%, -15%, 10, +10, or -15")
    sign, amount = match.groups()
    if sign == "":
        sign = "+"
    return f"{sign}{amount}%"


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
        description="Convert words from a text file to MP3 audio using edge-tts",
        add_help=True,
        formatter_class=argparse.ArgumentDefaultsHelpFormatter,
    )
    parser.add_argument(
        "--input-file",
        type=str,
        default=INPUT_FILE,
        help="Path to input text file",
    )
    parser.add_argument(
        "--output-dir",
        type=str,
        default=str(OUTPUT_DIR),
        help="Output directory for MP3 files",
    )
    parser.add_argument(
        "--voice",
        type=str,
        default=VOICE,
        help="TTS voice to use",
    )
    parser.add_argument(
        "--rate",
        type=str,
        default=RATE,
        help="TTS speaking rate (+10%%, -15%%, 10, +10, -15)",
    )
    parser.add_argument(
        "--concurrency",
        type=int,
        default=CONCURRENCY,
        help="Maximum concurrent requests",
    )
    parser.add_argument(
        "--overwrite",
        action="store_true",
        help="Regenerate MP3 files even if they already exist",
    )
    return parser.parse_args()


async def main() -> None:
    args = parse_args()
    input_path = Path(args.input_file)
    output_dir = Path(args.output_dir)
    voice = args.voice
    rate = normalize_rate(args.rate)
    concurrency = args.concurrency
    overwrite = args.overwrite
    if not input_path.exists():
        raise FileNotFoundError(f"Could not find '{INPUT_FILE}'. Create it with one word per line.")
    if concurrency < 1:
        raise ValueError("--concurrency must be >= 1")

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
    semaphore = asyncio.Semaphore(concurrency)

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
        if not overwrite and (output_dir / f"{base_name}.mp3").exists():
            skipped += 1
            continue

        # Two different entries in this run normalizing to the same filename get a suffix
        name = base_name if base_name not in claimed else f"{base_name}_{i}"
        claimed.add(base_name)
        jobs.append((word, output_dir / f"{name}.mp3"))

    if not jobs:
        print(f"Nothing to do: all {skipped} words already exist in '{output_dir}/'.")
        print("Tip: use --overwrite to regenerate files after changing --rate or --voice.")
        return

    tasks = [
        synthesize_word(word=word, out_path=out_path, semaphore=semaphore, voice=voice, rate=rate)
        for word, out_path in jobs
    ]
    await asyncio.gather(*tasks)
    print(f"\nDone! Generated {len(jobs)} MP3 files in '{output_dir}/' (skipped {skipped} existing).")


if __name__ == "__main__":
    asyncio.run(main())