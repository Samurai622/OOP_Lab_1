using System.Collections.Generic;

namespace Lab1_Task6
{
    public static class ReferenceData
    {
        // База еталонних кодів для різних мов
        public static readonly Dictionary<string, Dictionary<string, string>> Codes = new()
        {
            ["C#"] = new Dictionary<string, string>
            {
                ["Selection Sort"] = @"using System;
class ExampleClass {
    public static void SelectionSort(int[] array) {
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
}",
                ["Shaker Sort"] = @"using System;
class ExampleClass {
    public static void ShakerSort(int[] array) {
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
}"
            },
            ["Python"] = new Dictionary<string, string>
            {
                ["Selection Sort"] = @"def selection_sort(arr):
    n = len(arr)
    for i in range(n - 1):
        min_idx = i
        for j in range(i + 1, n):
            if arr[j] < arr[min_idx]:
                min_idx = j
        temp = arr[min_idx]
        arr[min_idx] = arr[i]
        arr[i] = temp",
                ["Shaker Sort"] = @"def shaker_sort(arr):
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
        start = start + 1"
            },
            ["Java"] = new Dictionary<string, string>
            {
                ["Selection Sort"] = @"public static void selectionSort(int[] array) {
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
}",
                ["Shaker Sort"] = @"public static void shakerSort(int[] array) {
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
}"
            },
            ["C"] = new Dictionary<string, string>
            {
                ["Selection Sort"] = @"void selectionSort(int array[], int n) {
    for (int i = 0; i < n - 1; i++) {
        int min_idx = i;
        for (int j = i + 1; j < n; j++) {
            if (array[j] < array[min_idx]) min_idx = j;
        }
        int temp = array[min_idx];
        array[min_idx] = array[i];
        array[i] = temp;
    }
}",
                ["Shaker Sort"] = @"void shakerSort(int array[], int n) {
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
}"
            },
            ["C++"] = new Dictionary<string, string>
            {
                ["Selection Sort"] = @"void selectionSort(int array[], int n) {
    for (int i = 0; i < n - 1; i++) {
        int min_idx = i;
        for (int j = i + 1; j < n; j++) {
            if (array[j] < array[min_idx]) min_idx = j;
        }
        int temp = array[min_idx];
        array[min_idx] = array[i];
        array[i] = temp;
    }
}",
                ["Shaker Sort"] = @"void shakerSort(int array[], int n) {
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
}"
            }
        };

        // Еталонні методи C#, з якими відбувається порівняння часу виконання
        public static class Methods
        {
            public static void SelectionSort(int[] array)
            {
                int n = array.Length;
                for (int i = 0; i < n - 1; i++)
                {
                    int min_idx = i;
                    for (int j = i + 1; j < n; j++) if (array[j] < array[min_idx]) min_idx = j;
                    int temp = array[min_idx]; array[min_idx] = array[i]; array[i] = temp;
                }
            }

            public static void ShakerSort(int[] array)
            {
                bool swapped = true;
                int start = 0, end = array.Length;
                while (swapped)
                {
                    swapped = false;
                    for (int i = start; i < end - 1; ++i)
                    {
                        if (array[i] > array[i + 1])
                        {
                            int temp = array[i]; array[i] = array[i + 1]; array[i + 1] = temp; swapped = true;
                        }
                    }
                    if (!swapped) break;
                    swapped = false; end--;
                    for (int i = end - 1; i >= start; i--)
                    {
                        if (array[i] > array[i + 1])
                        {
                            int temp = array[i]; array[i] = array[i + 1]; array[i + 1] = temp; swapped = true;
                        }
                    }
                    start++;
                }
            }
        }
    }
}