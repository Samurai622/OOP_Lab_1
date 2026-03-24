const languageSelect = document.getElementById("language");
const algorithmSelect = document.getElementById("algorithm");
const codeInput = document.getElementById("codeInput");
const resultText = document.getElementById("resultText");
const exampleBtn = document.getElementById("exampleBtn");
const checkBtn = document.getElementById("checkBtn");
const loadFileBtn = document.getElementById("loadFileBtn");
const fileInput = document.getElementById("fileInput");

const referenceCodes = {
  "C#": {
    "Selection Sort": `using System;
class ExampleClass {
    public static void SelectionSort(int[] array) {
        int n = array.Length;
        for (int i = 0; i < n - 1; i++) {
            int min_idx = i;
            for (int j = i + 1; j < n; j++) {
                if (array[j] < array[min_idx]) {
                    min_idx = j;
                }
            }
            int temp = array[min_idx];
            array[min_idx] = array[i];
            array[i] = temp;
        }
    }
}`,
    "Shaker Sort": `using System;
class ExampleClass {
    public static void ShakerSort(int[] array) {
        bool swapped = true;
        int start = 0;
        int end = array.Length;
        while (swapped == true) {
            swapped = false;
            for (int i = start; i < end - 1; ++i) {
                if (array[i] > array[i + 1]) {
                    int temp = array[i];
                    array[i] = array[i + 1];
                    array[i + 1] = temp;
                    swapped = true;
                }
            }
            if (swapped == false) break;
            swapped = false;
            end = end - 1;
            for (int i = end - 1; i >= start; i--) {
                if (array[i] > array[i + 1]) {
                    int temp = array[i];
                    array[i] = array[i + 1];
                    array[i + 1] = temp;
                    swapped = true;
                }
            }
            start = start + 1;
        }
    }
}`
  },

  "Python": {
    "Selection Sort": `def selection_sort(arr):
    n = len(arr)
    for i in range(n - 1):
        min_idx = i
        for j in range(i + 1, n):
            if arr[j] < arr[min_idx]:
                min_idx = j
        temp = arr[min_idx]
        arr[min_idx] = arr[i]
        arr[i] = temp`,
    "Shaker Sort": `def shaker_sort(arr):
    n = len(arr)
    swapped = True
    start = 0
    end = n
    while swapped == True:
        swapped = False
        for i in range(start, end - 1):
            if arr[i] > arr[i + 1]:
                temp = arr[i]
                arr[i] = arr[i + 1]
                arr[i + 1] = temp
                swapped = True
        if swapped == False:
            break
        swapped = False
        end = end - 1
        for i in range(end - 1, start - 1, -1):
            if arr[i] > arr[i + 1]:
                temp = arr[i]
                arr[i] = arr[i + 1]
                arr[i + 1] = temp
                swapped = True
        start = start + 1`
  },

  "Java": {
    "Selection Sort": `public static void selectionSort(int[] array) {
    int n = array.length;
    for (int i = 0; i < n - 1; i++) {
        int min_idx = i;
        for (int j = i + 1; j < n; j++) {
            if (array[j] < array[min_idx]) {
                min_idx = j;
            }
        }
        int temp = array[min_idx];
        array[min_idx] = array[i];
        array[i] = temp;
    }
}`,
    "Shaker Sort": `public static void shakerSort(int[] array) {
    boolean swapped = true;
    int start = 0;
    int end = array.length;
    while (swapped == true) {
        swapped = false;
        for (int i = start; i < end - 1; ++i) {
            if (array[i] > array[i + 1]) {
                int temp = array[i];
                array[i] = array[i + 1];
                array[i + 1] = temp;
                swapped = true;
            }
        }
        if (swapped == false) break;
        swapped = false;
        end = end - 1;
        for (int i = end - 1; i >= start; i--) {
            if (array[i] > array[i + 1]) {
                int temp = array[i];
                array[i] = array[i + 1];
                array[i + 1] = temp;
                swapped = true;
            }
        }
        start = start + 1;
    }
}`
  },

  "C": {
    "Selection Sort": `void selectionSort(int array[], int n) {
    for (int i = 0; i < n - 1; i++) {
        int min_idx = i;
        for (int j = i + 1; j < n; j++) {
            if (array[j] < array[min_idx]) {
                min_idx = j;
            }
        }
        int temp = array[min_idx];
        array[min_idx] = array[i];
        array[i] = temp;
    }
}`,
    "Shaker Sort": `void shakerSort(int array[], int n) {
    int swapped = 1;
    int start = 0;
    int end = n;
    while (swapped == 1) {
        swapped = 0;
        for (int i = start; i < end - 1; ++i) {
            if (array[i] > array[i + 1]) {
                int temp = array[i];
                array[i] = array[i + 1];
                array[i + 1] = temp;
                swapped = 1;
            }
        }
        if (swapped == 0) break;
        swapped = 0;
        end = end - 1;
        for (int i = end - 1; i >= start; i--) {
            if (array[i] > array[i + 1]) {
                int temp = array[i];
                array[i] = array[i + 1];
                array[i + 1] = temp;
                swapped = 1;
            }
        }
        start = start + 1;
    }
}`
  },

  "C++": {
    "Selection Sort": `void selectionSort(int array[], int n) {
    for (int i = 0; i < n - 1; i++) {
        int min_idx = i;
        for (int j = i + 1; j < n; j++) {
            if (array[j] < array[min_idx]) {
                min_idx = j;
            }
        }
        int temp = array[min_idx];
        array[min_idx] = array[i];
        array[i] = temp;
    }
}`,
    "Shaker Sort": `void shakerSort(int array[], int n) {
    bool swapped = true;
    int start = 0;
    int end = n;
    while (swapped == true) {
        swapped = false;
        for (int i = start; i < end - 1; ++i) {
            if (array[i] > array[i + 1]) {
                int temp = array[i];
                array[i] = array[i + 1];
                array[i + 1] = temp;
                swapped = true;
            }
        }
        if (swapped == false) break;
        swapped = false;
        end = end - 1;
        for (int i = end - 1; i >= start; i--) {
            if (array[i] > array[i + 1]) {
                int temp = array[i];
                array[i] = array[i + 1];
                array[i + 1] = temp;
                swapped = true;
            }
        }
        start = start + 1;
    }
}`
  }
};

function setResult(message, type = "neutral") {
  resultText.textContent = message;
  resultText.className = `result-text ${type}`;
}

function getSelectedLanguage() {
  return languageSelect.value;
}

function getSelectedAlgorithm() {
  return algorithmSelect.value;
}

function normalizeCode(code) {
  return code.replace(/\s+/g, "");
}

function removeComments(code) {
  return code
    .replace(/\/\/.*$/gm, "")
    .replace(/\/\*[\s\S]*?\*\//g, "");
}

function checkHeuristics(code, algo) {
  const cleanCode = removeComments(code);

  if (algo === "Shaker Sort" && !cleanCode.includes("while")) {
    return false;
  }

  if (algo === "Selection Sort" && cleanCode.includes("while")) {
    return false;
  }

  return true;
}

exampleBtn.addEventListener("click", () => {
  const lang = getSelectedLanguage();
  const algo = getSelectedAlgorithm();
  codeInput.value = referenceCodes[lang][algo];
  setResult("Приклад підставлено.", "neutral");
});

loadFileBtn.addEventListener("click", () => {
  fileInput.click();
});

fileInput.addEventListener("change", async (e) => {
  const file = e.target.files[0];
  if (!file) return;

  try {
    const text = await file.text();
    codeInput.value = text;
    setResult(`Файл ${file.name} успішно завантажено.`, "neutral");
  } catch (err) {
    setResult(`Помилка читання файлу: ${err.message}`, "error");
  }
});

checkBtn.addEventListener("click", () => {
  const code = codeInput.value.trim();
  const lang = getSelectedLanguage();
  const algo = getSelectedAlgorithm();

  if (!code) {
    setResult("Помилка: Код порожній.", "error");
    return;
  }

  setResult("Аналізую код...", "neutral");

  setTimeout(() => {
    if (!checkHeuristics(code, algo)) {
      setResult(
        `❌ Помилка: Структура коду не відповідає алгоритму '${algo}'.\n(Наприклад: Shaker Sort має містити цикл 'while', а Selection - ні).`,
        "error"
      );
      return;
    }

    const referenceCode = referenceCodes[lang][algo];
    const normalizedStudent = normalizeCode(code);
    const normalizedReference = normalizeCode(referenceCode);

    if (lang === "C#") {
      const hasSort = /Sort/i.test(code);
      const hasIntArray = /int\s*\[\]/i.test(code);

      if (!hasSort || !hasIntArray) {
        setResult(
          "❌ Для C# у веб-версії очікується метод сортування з параметром int[].",
          "error"
        );
        return;
      }

      if (
        normalizedStudent.includes(normalizedReference) ||
        (algo === "Selection Sort" && /for\s*\(/.test(code)) ||
        (algo === "Shaker Sort" && /while\s*\(/.test(code))
      ) {
        setResult(
          "✅ Успіх! Код пройшов веб-перевірку.\nДля повної компіляції та замірів часу C# потрібен серверний backend.",
          "success"
        );
      } else {
        setResult(
          "❌ Код не пройшов спрощену веб-перевірку для C#.",
          "error"
        );
      }

      return;
    }

    if (
      normalizedStudent.includes(normalizedReference) ||
      normalizedReference.includes(normalizedStudent)
    ) {
      setResult(
        `✅ Успіх! Логіка ${lang} коду збігається з еталонною.`,
        "success"
      );
    } else {
      setResult(
        `❌ Невірно. Структура коду відрізняється від еталонної для ${lang}.`,
        "error"
      );
    }
  }, 250);
});