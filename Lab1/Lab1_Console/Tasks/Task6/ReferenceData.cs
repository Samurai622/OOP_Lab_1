using System.Collections.Generic;

namespace Lab1_Task6
{
    public static class ReferenceData
    {
        // База еталонних кодів (ПОВНОЦІННІ ПРОГРАМИ з I/O та точками входу)
        // Зверніть увагу: назви методів сортування змінено на PerformSort, 
        // щоб показати, що назва не впливає на перевірку.
        public static readonly Dictionary<string, Dictionary<string, string>> Codes = new()
        {
            ["C#"] = new Dictionary<string, string>
            {
                ["Selection Sort"] = @"using System;
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
            arr = arrStr.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        }
        
        PerformSort(arr);
        Console.WriteLine(string.Join("" "", arr));
    }
}",
                ["Shaker Sort"] = @"using System;
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
            arr = arrStr.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        }
        
        PerformSort(arr);
        Console.WriteLine(string.Join("" "", arr));
    }
}"
            },
            ["Python"] = new Dictionary<string, string>
            {
                ["Selection Sort"] = @"import sys

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
    print(' '.join(str(x) for x in arr))",
                ["Shaker Sort"] = @"import sys

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
    print(' '.join(str(x) for x in arr))"
            },
            ["Java"] = new Dictionary<string, string>
            {
                ["Selection Sort"] = @"import java.util.Scanner;

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
        for(int i = 0; i < n; i++) System.out.print(arr[i] + "" "");
    }
}",
                ["Shaker Sort"] = @"import java.util.Scanner;

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
        for(int i = 0; i < n; i++) System.out.print(arr[i] + "" "");
    }
}"
            },
            ["C"] = new Dictionary<string, string>
            {
                ["Selection Sort"] = @"#include <stdio.h>
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
    if (scanf(""%d"", &n) != 1) return 0;
    if (n == 0) return 0;
    
    int* arr = (int*)malloc(n * sizeof(int));
    for (int i = 0; i < n; i++) scanf(""%d"", &arr[i]);
    
    performSort(arr, n);
    for(int i = 0; i < n; i++) printf(""%d "", arr[i]);
    
    free(arr); 
    return 0;
}",
                ["Shaker Sort"] = @"#include <stdio.h>
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
    if (scanf(""%d"", &n) != 1) return 0;
    if (n == 0) return 0;
    
    int* arr = (int*)malloc(n * sizeof(int));
    for (int i = 0; i < n; i++) scanf(""%d"", &arr[i]);
    
    performSort(arr, n);
    for(int i = 0; i < n; i++) printf(""%d "", arr[i]);
    
    free(arr); 
    return 0;
}"
            },
            ["C++"] = new Dictionary<string, string>
            {
                ["Selection Sort"] = @"#include <iostream>
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
    for(int i = 0; i < n; i++) cout << arr[i] << "" "";
    return 0;
}",
                ["Shaker Sort"] = @"#include <iostream>
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
    for(int i = 0; i < n; i++) cout << arr[i] << "" "";
    return 0;
}"
            }
        };
    }
}