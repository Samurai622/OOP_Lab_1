// ================== CUSTOM TEXTS (EDIT HERE) ==================

// Для задачі "Додати рядок у лог з кастомним текстом"
const CUSTOM_LOG_TEXT = "Я кастомний текст з коду 😎";

// Для задачі "Змінити заголовок вкладки"
const CUSTOM_TITLE_TEXT = "⏰ Таймер спрацював!";

// Посилання для “нова вкладка”
const NEW_TAB_URL = "https://www.youtube.com/";

// Мотиваційні фрази (з коду)
const MOTIVATION_QUOTES = [
  "Ти сильніший, ніж думаєш.",
  "Надіємось ми отримаємо гарну оцінку за цей тестовий проєкт!",
  "ФОТІУС ПРАЦЮЄ! (сподіваюсь)",
  "Кожна секунда на вагу золота.",
  "Почни зараз. Не в понеділок.",
];

// Favicon “набори” (через emoji SVG data URL)
const FAVICONS = ["⏰", "🎯", "🔥", "⚡", "🎲", "✅"];
let faviconIndex = 0;

// ================== TASKS LIST (USER CHOOSES) ==================
const TASKS = [
  {
    id: "confirm_pause",
    name: "Повідомлення з підтвердженням (пауза до 'Далі')",
    run: async (ctx) => {
      ctx.log("🟦 Confirm: показую модалку і ставлю на паузу");
      await ctx.confirm(`Таймер #${ctx.timerId} спрацював.\nНатисни "Далі", щоб продовжити.`);
      ctx.log("🟦 Confirm: користувач натиснув 'Далі' → продовжуємо");
    }
  },
  {
    id: "dice_roll",
    name: "Кидок кубика (1–6) + показ",
    run: (ctx) => {
      const value = 1 + Math.floor(Math.random() * 6);
      ctx.log(`🎲 Dice: випало ${value}`);
      ctx.screen(`🎲 Кидок кубика\n\nВипало: ${value}`);
    }
  },
  {
    id: "motivation_quote",
    name: "Мотиваційна фраза (рандом)",
    run: (ctx) => {
      const q = MOTIVATION_QUOTES[Math.floor(Math.random() * MOTIVATION_QUOTES.length)] || "Go!";
      ctx.log(`💬 Quote: ${q}`);
      ctx.screen(`💬 Мотивація\n\n${q}`);
    }
  },
  {
    id: "custom_log_line",
    name: "Додати рядок у лог (кастомний текст з коду)",
    run: (ctx) => {
      ctx.log(`📝 Custom log: ${CUSTOM_LOG_TEXT}`);
    }
  },
  {
    id: "set_title",
    name: "Змінити заголовок вкладки (title)",
    run: (ctx) => {
      document.title = CUSTOM_TITLE_TEXT;
      ctx.log(`🟨 Title: встановлено "${CUSTOM_TITLE_TEXT}"`);
    }
  },
  {
    id: "change_favicon",
    name: "Замінити favicon (перемикач)",
    run: (ctx) => {
      faviconIndex = (faviconIndex + 1) % FAVICONS.length;
      const emoji = FAVICONS[faviconIndex];
      setFaviconEmoji(emoji);
      ctx.log(`🟩 Favicon: ${emoji}`);
    }
  },
  {
    id: "play_sound",
    name: "Вивести звук (beep)",
    run: (ctx) => {
      ctx.log("🔊 Beep");
      tryBeep();
    }
  },
  {
    id: "open_new_tab",
    name: "Відкрити нову вкладку (URL з коду)",
    run: (ctx) => {
      ctx.log(`🌐 Open new tab: ${NEW_TAB_URL}`);
      window.open(NEW_TAB_URL, "_blank", "noopener,noreferrer");
    }
  },
];

// ================== APP STATE ==================
let sessionRunning = false;
let sessionPaused = false;

let sessionStartMs = 0;
let pausedTotalMs = 0;
let pauseStartedMs = 0;

let rafId = null;

const timersState = []; // { id, startAtSec, fired, taskId, randomTask, ... }

// ================== HELPERS ==================
const el = (id) => document.getElementById(id);

function nowMs() { return performance.now(); }

function clampInt(n, min, max) {
  if (!Number.isFinite(n)) return min;
  return Math.min(max, Math.max(min, Math.trunc(n)));
}

function pad2(n) { return String(n).padStart(2, "0"); }

function formatTime(ms) {
  const totalSec = ms / 1000;
  const m = Math.floor(totalSec / 60);
  const s = Math.floor(totalSec % 60);
  const tenths = Math.floor((totalSec - Math.floor(totalSec)) * 10);
  return `${pad2(m)}:${pad2(s)}.${tenths}`;
}

function getElapsedMs() {
  if (!sessionRunning) return 0;
  if (sessionPaused) return Math.max(0, pauseStartedMs - sessionStartMs - pausedTotalMs);
  return Math.max(0, nowMs() - sessionStartMs - pausedTotalMs);
}

function pickRandomTask() {
  if (TASKS.length === 0) return null;
  const idx = Math.floor(Math.random() * TASKS.length);
  return TASKS[idx];
}

function getTaskById(id) {
  return TASKS.find(t => t.id === id) || null;
}

function escapeHtml(s) {
  return String(s ?? "").replace(/[<>&"]/g, c => ({ "<":"&lt;", ">":"&gt;", "&":"&amp;", "\"":"&quot;" }[c]));
}

// ================== LOG + SCREEN ==================
function log(line) {
  const box = el("log");
  const t = sessionRunning ? formatTime(getElapsedMs()) : "—";
  box.textContent += (box.textContent ? "\n" : "") + `[${t}] ${line}`;
  box.scrollTop = box.scrollHeight;
}

function screen(text) {
  el("screenText").textContent = text;
  el("screenStatus").textContent = "Оновлено";
}

// ================== FAVICON ==================
function setFaviconEmoji(emoji) {
  const link = el("favicon");
  const svg = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 64 64">
    <text y="50" x="8" font-size="48">${emoji}</text>
  </svg>`;
  link.setAttribute("href", "data:image/svg+xml," + encodeURIComponent(svg));
}

// ================== SOUND ==================
function tryBeep() {
  try {
    const AudioCtx = window.AudioContext || window.webkitAudioContext;
    if (!AudioCtx) return;

    const ctx = new AudioCtx();
    const o = ctx.createOscillator();
    const g = ctx.createGain();

    o.type = "sine";
    o.frequency.value = 880;
    g.gain.value = 0.06;

    o.connect(g);
    g.connect(ctx.destination);

    o.start();
    setTimeout(() => {
      o.stop();
      ctx.close().catch(() => {});
    }, 140);
  } catch {}
}

// ================== MINIMAL VISUAL FEEDBACK ==================
function flashCard(timerObj) {
  timerObj.card.classList.add("fired");
  timerObj.card.classList.remove("flash");
  void timerObj.card.offsetWidth;
  timerObj.card.classList.add("flash");
}

// ================== CONFIRM MODAL ==================
function showConfirmModal(message) {
  return new Promise((resolve) => {
    const backdrop = el("modalBackdrop");
    const body = el("modalBody");
    const next = el("modalNext");

    body.textContent = message;

    backdrop.classList.remove("hidden");
    backdrop.setAttribute("aria-hidden", "false");

    next.focus();

    const cleanup = () => {
      next.removeEventListener("click", onNext);
      document.removeEventListener("keydown", onKey);
      backdrop.classList.add("hidden");
      backdrop.setAttribute("aria-hidden", "true");
    };

    const onNext = () => {
      cleanup();
      resolve();
    };

    const onKey = (e) => {
      if (e.key === "Enter" || e.key === "Escape") onNext();
    };

    next.addEventListener("click", onNext);
    document.addEventListener("keydown", onKey);
  });
}

// ================== UI STATE ==================
function setButtonsState() {
  const hasTimers = timersState.length > 0;

  el("start").disabled = !hasTimers || sessionRunning;
  el("pause").disabled = !sessionRunning;
  el("stop").disabled = !sessionRunning;
  el("reset").disabled = !hasTimers;

  if (!sessionRunning) {
    el("pause").textContent = "Pause";
    el("pause").classList.remove("toggled");
    return;
  }

  if (sessionPaused) {
    el("pause").textContent = "Resume";
    el("pause").classList.add("toggled");
  } else {
    el("pause").textContent = "Pause";
    el("pause").classList.remove("toggled");
  }
}

function setHint() {
  el("timersHint").textContent =
    timersState.length === 0
      ? "Натисни “Створити таймери” щоб додати їх."
      : `Таймерів: ${timersState.length}. Впиши секунди, вибери завдання (або Random) та тисни Start.`;
}

function buildTaskOptionsHtml(selectedId) {
  return TASKS.map(t => {
    const sel = t.id === selectedId ? "selected" : "";
    return `<option value="${t.id}" ${sel}>${escapeHtml(t.name)}</option>`;
  }).join("");
}

// ================== BUILD TIMERS ==================
function buildTimers() {
  stopSession(true);

  const count = clampInt(parseInt(el("count").value || "1", 10), 1, 50);
  el("count").value = String(count);

  const wrap = el("timers");
  wrap.innerHTML = "";
  timersState.length = 0;

  for (let i = 1; i <= count; i++) {
    const card = document.createElement("div");
    card.className = "timerCard";

    const defaultTaskId = TASKS[0]?.id ?? "";

    card.innerHTML = `
      <div class="timerTop">
        <div class="timerTitle">Таймер #${i}</div>
        <div class="timerMeta">startAt: <span class="startAtLabel">0</span>s</div>
      </div>

      <div class="timerInputs">
        <label>
          Секунда запуску (коли має спрацювати)
          <input class="startAtInput" type="number" min="0" max="9999" value="0">
        </label>

        <label>
          Завдання
          <select class="taskSelect">
            ${buildTaskOptionsHtml(defaultTaskId)}
          </select>
        </label>

        <div class="inlineRow">
          <label style="display:flex; gap:10px; align-items:center;">
            <input class="randomTask" type="checkbox">
            Random
          </label>
          <span class="muted smallTaskHint"></span>
        </div>
      </div>

      <div class="timerStatus">
        <div class="statusLine">
          <div class="statusKey">Статус</div>
          <div class="statusVal statusText">Waiting</div>
        </div>

        <div class="statusLine">
          <div class="statusKey">Залишилось</div>
          <div class="statusVal remainingText">—</div>
        </div>

        <div class="progress"><div class="bar"></div></div>
      </div>
    `;

    wrap.appendChild(card);

    const startAtInput = card.querySelector(".startAtInput");
    const startAtLabel = card.querySelector(".startAtLabel");
    const statusText = card.querySelector(".statusText");
    const remainingText = card.querySelector(".remainingText");
    const bar = card.querySelector(".bar");
    const taskSelect = card.querySelector(".taskSelect");
    const randomTask = card.querySelector(".randomTask");
    const smallTaskHint = card.querySelector(".smallTaskHint");

    const obj = {
      id: i,
      startAtSec: 0,
      fired: false,
      taskId: defaultTaskId,
      randomTask: false,
      card, startAtInput, startAtLabel, statusText, remainingText, bar,
      taskSelect, randomTask, smallTaskHint
    };

    function refreshTaskUI() {
      if (obj.randomTask) {
        obj.taskSelect.disabled = true;
        obj.smallTaskHint.textContent = "🎲 Random";
      } else {
        obj.taskSelect.disabled = false;
        obj.smallTaskHint.textContent = "";
      }
    }

    startAtInput.addEventListener("input", () => {
      obj.startAtSec = clampInt(parseInt(startAtInput.value || "0", 10), 0, 9999);
      startAtLabel.textContent = String(obj.startAtSec);

      if (!sessionRunning) {
        obj.fired = false;
        obj.card.classList.remove("fired", "flash");
        obj.statusText.textContent = "Waiting";
        obj.remainingText.textContent = "—";
        obj.bar.style.width = "0%";
      }

      if (el("autoSort").checked) sortTimersByStartAt();
    });

    taskSelect.addEventListener("change", () => {
      obj.taskId = String(taskSelect.value || "");
      refreshTaskUI();
    });

    randomTask.addEventListener("change", () => {
      obj.randomTask = !!randomTask.checked;
      refreshTaskUI();
    });

    startAtLabel.textContent = "0";
    obj.taskId = String(taskSelect.value || defaultTaskId);
    obj.randomTask = !!randomTask.checked;
    refreshTaskUI();

    timersState.push(obj);
  }

  if (el("autoSort").checked) sortTimersByStartAt();

  el("globalTime").textContent = "00:00.0";
  log(`🧩 Створено таймерів: ${count}`);
  setHint();
  setButtonsState();
}

function sortTimersByStartAt() {
  const wrap = el("timers");
  const sorted = [...timersState].sort((a, b) => a.startAtSec - b.startAtSec);
  for (const t of sorted) wrap.appendChild(t.card);
}

// ================== SESSION CONTROL ==================
function startSession() {
  if (timersState.length === 0) return;

  for (const t of timersState) {
    t.fired = false;
    t.card.classList.remove("fired", "flash");
    t.statusText.textContent = "Waiting";
    t.remainingText.textContent = "—";
    t.bar.style.width = "0%";
  }

  sessionRunning = true;
  sessionPaused = false;

  sessionStartMs = nowMs();
  pausedTotalMs = 0;
  pauseStartedMs = 0;

  log("▶ START session");
  setButtonsState();
  tickLoop();
}

function stopSession(silent = false) {
  sessionRunning = false;
  sessionPaused = false;

  if (rafId !== null) cancelAnimationFrame(rafId);
  rafId = null;

  if (!silent) log("⏹ STOP session");
  setButtonsState();
}

function resetSession() {
  stopSession(true);
  el("globalTime").textContent = "00:00.0";

  for (const t of timersState) {
    t.fired = false;
    t.card.classList.remove("fired", "flash");
    t.statusText.textContent = "Waiting";
    t.remainingText.textContent = "—";
    t.bar.style.width = "0%";
  }

  log("↺ RESET");
  setButtonsState();
}

function pauseResumeSession() {
  if (!sessionRunning) return;

  if (!sessionPaused) {
    sessionPaused = true;
    pauseStartedMs = nowMs();

    if (rafId !== null) cancelAnimationFrame(rafId);
    rafId = null;

    log("⏸ PAUSE");
  } else {
    sessionPaused = false;

    const pausedNow = nowMs() - pauseStartedMs;
    pausedTotalMs += pausedNow;
    pauseStartedMs = 0;

    log("▶ RESUME");
    tickLoop();
  }

  setButtonsState();
}

// Internal pause used by confirm-task
function hardPauseForModal() {
  if (!sessionRunning || sessionPaused) return;
  sessionPaused = true;
  pauseStartedMs = nowMs();

  if (rafId !== null) cancelAnimationFrame(rafId);
  rafId = null;

  setButtonsState();
}

function resumeAfterModal() {
  if (!sessionRunning || !sessionPaused) return;

  const pausedNow = nowMs() - pauseStartedMs;
  pausedTotalMs += pausedNow;
  pauseStartedMs = 0;

  sessionPaused = false;
  setButtonsState();
  tickLoop();
}

// ================== TASK EXECUTION ==================
async function executeTimerTask(timerObj) {
  let task = null;
  if (timerObj.randomTask) task = pickRandomTask();
  else task = getTaskById(timerObj.taskId);

  flashCard(timerObj);

  if (!task) {
    log(`⚠ Таймер #${timerObj.id}: немає задачі для виконання`);
    return;
  }

  const ctx = {
    timerId: timerObj.id,
    startAtSec: timerObj.startAtSec,
    getElapsedMs,
    log,
    screen,
    confirm: async (message) => {
      hardPauseForModal();
      await showConfirmModal(message);
      resumeAfterModal();
    }
  };

  log(`🎯 Таймер #${timerObj.id}: "${task.name}"`);

  try {
    const res = task.run(ctx);
    if (res && typeof res.then === "function") await res;
  } catch (e) {
    log(`❌ Помилка задачі "${task.name}": ${String(e)}`);
  }
}

// ================== LOOP ==================
function tickLoop() {
  if (!sessionRunning || sessionPaused) return;

  const elapsedMs = getElapsedMs();
  const elapsedSec = elapsedMs / 1000;

  el("globalTime").textContent = formatTime(elapsedMs);

  for (const t of timersState) {
    const target = t.startAtSec;

    if (target <= 0) t.bar.style.width = "100%";
    else {
      const p = Math.min(1, Math.max(0, elapsedSec / target));
      t.bar.style.width = `${Math.floor(p * 100)}%`;
    }

    if (t.fired) {
      t.statusText.textContent = "Fired";
      t.remainingText.textContent = "0.0s";
      continue;
    }

    const remaining = target - elapsedSec;

    if (remaining <= 0) {
      t.fired = true;
      t.statusText.textContent = "Fired";
      t.remainingText.textContent = "0.0s";
      executeTimerTask(t);
    } else {
      t.statusText.textContent = "Waiting";
      t.remainingText.textContent = `${remaining.toFixed(1)}s`;
    }
  }

  rafId = requestAnimationFrame(tickLoop);
}

// ================== EVENTS ==================
el("build").addEventListener("click", buildTimers);
el("start").addEventListener("click", startSession);
el("pause").addEventListener("click", pauseResumeSession);
el("stop").addEventListener("click", () => stopSession(false));
el("reset").addEventListener("click", resetSession);

el("clearLog").addEventListener("click", () => el("log").textContent = "");
el("clearScreen").addEventListener("click", () => {
  el("screenText").textContent = "Екран очищено.";
  el("screenStatus").textContent = "Поки пусто";
});

el("autoSort").addEventListener("change", () => {
  if (el("autoSort").checked) sortTimersByStartAt();
});

// Start state: no timers
setHint();
setButtonsState();
setFaviconEmoji(FAVICONS[faviconIndex]);
log("Готово. Введи кількість таймерів і натисни “Створити таймери”.");  