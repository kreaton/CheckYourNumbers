// The recorded voice, synthesized sound effects, the saved score, and the physical keyboard.

// One <audio> element plays every recording. iOS only lets audio start inside a user gesture, but
// once an element has played in one it may play again later, e.g. when the timer reveals an answer.
window.voice = (() => {
    const el = new Audio();
    el.preload = "auto";
    let done = null;
    const cache = new Map();

    function finish(ok) {
        if (done) {
            const resolve = done;
            done = null;
            resolve(ok);
        }
    }

    el.addEventListener("ended", () => finish(true));
    el.addEventListener("error", () => finish(false));

    return {
        // Resolves when the recording has finished (true) or could not be played (false).
        play(url) {
            finish(true);
            return new Promise(resolve => {
                done = resolve;
                el.pause();
                el.src = url;
                const started = el.play();
                if (started) started.catch(() => finish(false));
            });
        },

        stop() {
            el.pause();
            finish(true);
        },

        // Fetches a recording ahead, so the next round starts without a delay.
        preload(url) {
            if (cache.has(url)) return;
            if (cache.size > 6) cache.delete(cache.keys().next().value);
            const a = new Audio();
            a.preload = "auto";
            a.src = url;
            cache.set(url, a);
        },
    };
})();

window.gameSounds = (() => {
    let ctx = null;
    let master = null;

    function audio() {
        if (!ctx) {
            const Ctx = window.AudioContext || window.webkitAudioContext;
            if (!Ctx) return null;
            ctx = new Ctx();
            const comp = ctx.createDynamicsCompressor();
            comp.threshold.value = -14;
            comp.ratio.value = 5;
            master = ctx.createGain();
            master.gain.value = 0.8;
            master.connect(comp).connect(ctx.destination);
        }
        if (ctx.state === "suspended") ctx.resume();
        return ctx;
    }

    // Wake the audio context on the first tap, as iOS requires.
    function unlock() {
        const c = audio();
        if (!c) return;
        const source = c.createBufferSource();
        source.buffer = c.createBuffer(1, 1, 22050);
        source.connect(c.destination);
        source.start(0);
        window.removeEventListener("pointerdown", unlock);
        window.removeEventListener("keydown", unlock);
    }
    window.addEventListener("pointerdown", unlock);
    window.addEventListener("keydown", unlock);

    // A soft electric-piano-like tone: a sine with a quiet overtone and a quick decay.
    function tone(c, { freq, start = 0, length = 0.4, volume = 0.2, type = "sine", slideTo = null }) {
        const t = c.currentTime + start;
        const gain = c.createGain();
        gain.gain.setValueAtTime(0.0001, t);
        gain.gain.exponentialRampToValueAtTime(volume, t + 0.008);
        gain.gain.exponentialRampToValueAtTime(0.0001, t + length);
        gain.connect(master);

        for (const [mult, level] of [[1, 1], [2, 0.25]]) {
            const osc = c.createOscillator();
            const g = c.createGain();
            osc.type = type;
            osc.frequency.setValueAtTime(freq * mult, t);
            if (slideTo) osc.frequency.exponentialRampToValueAtTime(slideTo * mult, t + length);
            g.gain.value = level;
            osc.connect(g).connect(gain);
            osc.start(t);
            osc.stop(t + length + 0.05);
        }
    }

    const play = fn => () => {
        const c = audio();
        if (c) fn(c);
    };

    return {
        // A rising major arpeggio.
        success: play(c => [523.25, 659.25, 783.99, 1046.5].forEach((freq, i) =>
            tone(c, { freq, start: i * 0.075, length: 0.6, volume: 0.16 }))),

        // A short muted buzz.
        error: play(c => tone(c, { freq: 155, slideTo: 120, length: 0.18, volume: 0.22, type: "triangle" })),

        // "Not yet": two quiet falling notes, kinder than the buzz.
        soft: play(c => {
            tone(c, { freq: 440, length: 0.22, volume: 0.1 });
            tone(c, { freq: 349.2, start: 0.12, length: 0.35, volume: 0.1 });
        }),

        // Time's up: a falling "wah-wah".
        timeout: play(c => {
            tone(c, { freq: 392, slideTo: 349.2, length: 0.32, volume: 0.2, type: "triangle" });
            tone(c, { freq: 311.1, slideTo: 196, start: 0.3, length: 0.7, volume: 0.22, type: "triangle" });
        }),

        // A soft clock tick for each of the last seconds.
        tick: play(c => tone(c, { freq: 880, length: 0.06, volume: 0.08, type: "square" })),

        // A quiet click for each digit.
        key: play(c => tone(c, { freq: 1318.5, length: 0.08, volume: 0.05 })),
    };
})();

window.gameStore = {
    load(key) {
        try { return localStorage.getItem(key); } catch { return null; }
    },
    save(key, value) {
        try { localStorage.setItem(key, value); } catch { /* private mode: the score is not kept */ }
    },
};

// Forwards digits, Backspace, Enter and Space from the physical keyboard to the game.
window.gameKeys = {
    attach(dotnet) {
        this.detach();
        this.handler = e => {
            if (e.ctrlKey || e.metaKey || e.altKey) return;
            // Leave the timer select alone.
            if (e.target instanceof Element && e.target.closest("select, input, textarea")) return;
            const key = e.key;
            if (/^[0-9]$/.test(key) || key === "Backspace" || key === "Enter" || key === " ") {
                // Also stops Enter and Space from clicking the button that has focus.
                e.preventDefault();
                if (!e.repeat) dotnet.invokeMethodAsync("OnKey", key);
            }
        };
        // Space clicks a focused button on key up, so that is stopped as well.
        this.upHandler = e => {
            if (e.key === " " && !(e.target instanceof Element && e.target.closest("select, input, textarea"))) e.preventDefault();
        };
        document.addEventListener("keydown", this.handler);
        document.addEventListener("keyup", this.upHandler);
    },
    detach() {
        if (this.handler) document.removeEventListener("keydown", this.handler);
        if (this.upHandler) document.removeEventListener("keyup", this.upHandler);
        this.handler = this.upHandler = null;
    },
};
