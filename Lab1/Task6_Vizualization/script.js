const languageSelect = document.getElementById("language");
const algorithmSelect = document.getElementById("algorithm");
const codeInput = document.getElementById("codeInput");
const lineNumbers = document.getElementById("lineNumbers");
const resultText = document.getElementById("resultText");
const exampleBtn = document.getElementById("exampleBtn");
const checkBtn = document.getElementById("checkBtn");
const loadFileBtn = document.getElementById("loadFileBtn");
const fileInput = document.getElementById("fileInput");

// Оновлені еталонні коди (повноцінні програми з I/O)
const referenceCodes = {
  "C#": {
    "Selection Sort": `using System;
using System.Linq;

class Program {
    public static void PerformSort(int[] array) {
        int n = array.Length;
        for (int i = 0; i < n - 1; i++) {
            int min_idx = i;
            for (int j = i + 1; j < n; j++) {
                if (array[j] < array[min_idx]) min_idx = j;
            }
            int temp = array[min_idx];
            array[min_idx] = array[i];
            array[i] = temp;
        }
    }

    static void Main() {
        string nStr = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(nStr)) return;
        int n = int.Parse(nStr);
        
        int[] arr = new int[n];
        if (n > 0) {
            string arrStr = Console.ReadLine();
            arr = arrStr.Split(new[] { ' ', '\\t' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        }
        
        PerformSort(arr);
        Console.WriteLine(string.Join(" ", arr));
    }
}`,
    "Shaker Sort": `using System;
using System.Linq;

class Program {
    public static void PerformSort(int[] array) {
        bool swapped = true;
        int start = 0;
        int end = array.Length;
        while (swapped == true) {
            swapped = false;
            for (int i = start; i < end - 1; ++i) {
                if (array[i] > array[i + 1]) {
                    int temp = array[i]; array[i] = array[i + 1]; array[i + 1] = temp;
                    swapped = true;
                }
            }
            if (!swapped) break;
            swapped = false; end--;
            for (int i = end - 1; i >= start; i--) {
                if (array[i] > array[i + 1]) {
                    int temp = array[i]; array[i] = array[i + 1]; array[i + 1] = temp;
                    swapped = true;
                }
            }
            start++;
        }
    }

    static void Main() {
        string nStr = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(nStr)) return;
        int n = int.Parse(nStr);
        
        int[] arr = new int[n];
        if (n > 0) {
            string arrStr = Console.ReadLine();
            arr = arrStr.Split(new[] { ' ', '\\t' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        }
        
        PerformSort(arr);
        Console.WriteLine(string.Join(" ", arr));
    }
}`
  },

  "Python": {
    "Selection Sort": `import sys

def perform_sort(arr):
    n = len(arr)
    for i in range(n - 1):
        min_idx = i
        for j in range(i + 1, n):
            if arr[j] < arr[min_idx]:
                min_idx = j
        temp = arr[min_idx]
        arr[min_idx] = arr[i]
        arr[i] = temp

if __name__ == '__main__':
    data = sys.stdin.read().split()
    if not data: sys.exit()
    n = int(data[0])
    arr = [int(x) for x in data[1:]]
    perform_sort(arr)
    print(' '.join(str(x) for x in arr))`,
    "Shaker Sort": `import sys

def perform_sort(arr):
    n = len(arr)
    swapped = True
    start = 0
    end = n
    while swapped:
        swapped = False
        for i in range(start, end - 1):
            if arr[i] > arr[i + 1]:
                temp = arr[i]
                arr[i] = arr[i + 1]
                arr[i + 1] = temp
                swapped = True
        if not swapped: break
        swapped = False
        end = end - 1
        for i in range(end - 1, start - 1, -1):
            if arr[i] > arr[i + 1]:
                temp = arr[i]
                arr[i] = arr[i + 1]
                arr[i + 1] = temp
                swapped = True
        start = start + 1

if __name__ == '__main__':
    data = sys.stdin.read().split()
    if not data: sys.exit()
    n = int(data[0])
    arr = [int(x) for x in data[1:]]
    perform_sort(arr)
    print(' '.join(str(x) for x in arr))`
  },

  "Java": {
    "Selection Sort": `import java.util.Scanner;

public class Main {
    public static void performSort(int[] array) {
        int n = array.length;
        for (int i = 0; i < n - 1; i++) {
            int min_idx = i;
            for (int j = i + 1; j < n; j++) {
                if (array[j] < array[min_idx]) min_idx = j;
            }
            int temp = array[min_idx];
            array[min_idx] = array[i];
            array[i] = temp;
        }
    }

    public static void main(String[] args) {
        Scanner sc = new Scanner(System.in);
        if (!sc.hasNextInt()) return;
        int n = sc.nextInt();
        int[] arr = new int[n];
        for (int i = 0; i < n; i++) arr[i] = sc.nextInt();
        performSort(arr);
        for(int i = 0; i < n; i++) System.out.print(arr[i] + " ");
    }
}`,
    "Shaker Sort": `import java.util.Scanner;

public class Main {
    public static void performSort(int[] array) {
        boolean swapped = true;
        int start = 0, end = array.length;
        while (swapped) {
            swapped = false;
            for (int i = start; i < end - 1; ++i) {
                if (array[i] > array[i + 1]) {
                    int temp = array[i]; array[i] = array[i + 1]; array[i + 1] = temp;
                    swapped = true;
                }
            }
            if (!swapped) break;
            swapped = false; end--;
            for (int i = end - 1; i >= start; i--) {
                if (array[i] > array[i + 1]) {
                    int temp = array[i]; array[i] = array[i + 1]; array[i + 1] = temp;
                    swapped = true;
                }
            }
            start++;
        }
    }

    public static void main(String[] args) {
        Scanner sc = new Scanner(System.in);
        if (!sc.hasNextInt()) return;
        int n = sc.nextInt();
        int[] arr = new int[n];
        for (int i = 0; i < n; i++) arr[i] = sc.nextInt();
        performSort(arr);
        for(int i = 0; i < n; i++) System.out.print(arr[i] + " ");
    }
}`
  },

  "C": {
    "Selection Sort": `#include <stdio.h>
#include <stdlib.h>

void performSort(int array[], int n) {
    for (int i = 0; i < n - 1; i++) {
        int min_idx = i;
        for (int j = i + 1; j < n; j++) {
            if (array[j] < array[min_idx]) min_idx = j;
        }
        int temp = array[min_idx];
        array[min_idx] = array[i];
        array[i] = temp;
    }
}

int main() {
    int n; 
    if (scanf("%d", &n) != 1) return 0;
    if (n == 0) return 0;
    
    int* arr = (int*)malloc(n * sizeof(int));
    for (int i = 0; i < n; i++) scanf("%d", &arr[i]);
    
    performSort(arr, n);
    for(int i = 0; i < n; i++) printf("%d ", arr[i]);
    
    free(arr); 
    return 0;
}`,
    "Shaker Sort": `#include <stdio.h>
#include <stdlib.h>

void performSort(int array[], int n) {
    int swapped = 1, start = 0, end = n;
    while (swapped == 1) {
        swapped = 0;
        for (int i = start; i < end - 1; ++i) {
            if (array[i] > array[i + 1]) {
                int temp = array[i]; array[i] = array[i + 1]; array[i + 1] = temp;
                swapped = 1;
            }
        }
        if (swapped == 0) break;
        swapped = 0; end--;
        for (int i = end - 1; i >= start; i--) {
            if (array[i] > array[i + 1]) {
                int temp = array[i]; array[i] = array[i + 1]; array[i + 1] = temp;
                swapped = 1;
            }
        }
        start++;
    }
}

int main() {
    int n; 
    if (scanf("%d", &n) != 1) return 0;
    if (n == 0) return 0;
    
    int* arr = (int*)malloc(n * sizeof(int));
    for (int i = 0; i < n; i++) scanf("%d", &arr[i]);
    
    performSort(arr, n);
    for(int i = 0; i < n; i++) printf("%d ", arr[i]);
    
    free(arr); 
    return 0;
}`
  },

  "C++": {
    "Selection Sort": `#include <iostream>
#include <vector>

using namespace std;

void performSort(int array[], int n) {
    for (int i = 0; i < n - 1; i++) {
        int min_idx = i;
        for (int j = i + 1; j < n; j++) {
            if (array[j] < array[min_idx]) min_idx = j;
        }
        int temp = array[min_idx];
        array[min_idx] = array[i];
        array[i] = temp;
    }
}

int main() {
    int n; 
    if (!(cin >> n)) return 0;
    if (n == 0) return 0;
    
    vector<int> arr(n);
    for (int i = 0; i < n; i++) cin >> arr[i];
    
    performSort(arr.data(), n);
    for(int i = 0; i < n; i++) cout << arr[i] << " ";
    return 0;
}`,
    "Shaker Sort": `#include <iostream>
#include <vector>

using namespace std;

void performSort(int array[], int n) {
    bool swapped = true; int start = 0, end = n;
    while (swapped) {
        swapped = false;
        for (int i = start; i < end - 1; ++i) {
            if (array[i] > array[i + 1]) {
                int temp = array[i]; array[i] = array[i + 1]; array[i + 1] = temp;
                swapped = true;
            }
        }
        if (!swapped) break;
        swapped = false; end--;
        for (int i = end - 1; i >= start; i--) {
            if (array[i] > array[i + 1]) {
                int temp = array[i]; array[i] = array[i + 1]; array[i + 1] = temp;
                swapped = true;
            }
        }
        start++;
    }
}

int main() {
    int n; 
    if (!(cin >> n)) return 0;
    if (n == 0) return 0;
    
    vector<int> arr(n);
    for (int i = 0; i < n; i++) cin >> arr[i];
    
    performSort(arr.data(), n);
    for(int i = 0; i < n; i++) cout << arr[i] << " ";
    return 0;
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

function updateLineNumbers() {
  const lines = codeInput.value.split("\n").length || 1;
  lineNumbers.textContent = Array.from({ length: lines }, (_, i) => i + 1).join("\n");
}

function syncScroll() {
  lineNumbers.scrollTop = codeInput.scrollTop;
}

exampleBtn.addEventListener("click", () => {
  const lang = getSelectedLanguage();
  const algo = getSelectedAlgorithm();
  
  if (referenceCodes[lang] && referenceCodes[lang][algo]) {
      codeInput.value = referenceCodes[lang][algo];
      updateLineNumbers();
      setResult("Приклад підставлено. Зверніть увагу: це повноцінна програма.", "neutral");
  } else {
      setResult("Для обраної мови та алгоритму немає прикладу.", "error");
  }
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
    updateLineNumbers();
    setResult(`Файл ${file.name} успішно завантажено.`, "neutral");
  } catch (err) {
    setResult(`Помилка читання файлу: ${err.message}`, "error");
  }
  
  // Скидаємо value, щоб можна було завантажити той самий файл повторно
  fileInput.value = ""; 
});

codeInput.addEventListener("input", updateLineNumbers);
codeInput.addEventListener("scroll", syncScroll);

checkBtn.addEventListener("click", async () => {
    const code = codeInput.value.trim();
    const lang = getSelectedLanguage();
    const algo = getSelectedAlgorithm(); // Без .replace, щоб ключ точно збігався з бекендом

    if (!code) {
        setResult("Помилка: Код порожній. Вставте повний код програми (з main).", "error");
        return;
    }

    setResult(`Аналізую та виконую тести для ${lang}... (це може зайняти до 15 секунд)`, "neutral");
    
    // Блокуємо кнопку на час перевірки
    checkBtn.disabled = true;

    try {
        const response = await fetch("http://localhost:3000/api/check", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ code, lang, algo })
        });

        const result = await response.json();

        if (result.success) {
            setResult(result.message, "success");
        } else {
            setResult(result.message, "error");
        }
    } catch (err) {
        setResult("❌ Сервер недоступний: " + err.message, "error");
    } finally {
        checkBtn.disabled = false;
    }
});

updateLineNumbers();